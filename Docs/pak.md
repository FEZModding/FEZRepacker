# FEZ `.pak` archive file description

## Overview

FEZ stores its game assets in four `.pak` files: `Essentials.pak`, `Music.pak`, `Other.pak` and `Updates.pak`. This documentation will attempt to give a rought idea on how FEZ Repacker reads them.

Both PAK  and XNB files are binary files. In nearly all cases, all of the fields in them are little-endian unless specified otherwise. Additionally, all string fields are prefixed with their length as a seven-bit encoded integer.

## `.pak` archive format

The file starts with a small header of 32-bit integer indicating a number of files in this archive, followed by an array of files.
|Field Type|Description|
|-|-|
|`uint32`|File count defining the size of following array.|
|File Array|Array of variable-sized files, placed one after another.|

Every file in the array is represented as follows:
|Field Type|Description|
|-|-|
|`string`|Path and name of the file without its extension.|
|`uint32`|Size of the file data in bytes.|
|`byte[]`|File data.|

Types of files stored in the archive can vary. Because file extensions are not included in their paths, the only indication of a file type is its own header and path/name. In FEZ, there seem to be three file types:

- **XNB** - Microsoft XNA Game Studio 4.0 compiled content containers. Stores textures, sound effects, triles, levels, scripts and other miscellaneous data.
- **Ogg** - digital multimedia container. Stores music.
- **Effect files** - files within the `effects` directory, having no recognisable identifier. Presumably they're the `XFO` files used in XNA Game Studio 4.0.

Two latter formats are trivial, as Repacker will directly unpack them without converting them in any way, so the focus will now be put on the first one, which contains majority of the game's data.

## `XNB` file format

Header of each XNB file has following structure:

|Field Type|Description|
|-|-|
|`char[3]`|Format identifier. Should be equal to `['X', 'N', 'B']`.|
|`char`|Target platform. Possible values are `w`, `m` and `x` for MS Windows, Windows Phone 7 and Xbox 360 respectively.|
|`byte`|XNB format version. FEZ uses `5` - XNA Game Studio 4.0.|
|`byte`|Flag bits. `0x01` - content is for HiDef profile (used for FEZ assets), `0x40` - data is compressed (LZ4, unused for FEZ), `0x80` - data is compressed (LZX, used for all FEZ assets).|
|`uint32`|Compressed file size, including header.|

If compression flag is set, the rest of the file is compressed and prefixed with `uint32` representing the size of this data after decompression. In case of FEZ, slightly modified LZX algorithm is used for compression. For details, visit [XNB reader code from open source implementation of MonoGame](https://github.com/labnation/MonoGame/blob/d270be3e800a3955886e817cdd06133743a7e043/MonoGame.Framework/Content/ContentManager.cs#L405). Repacker decompresses all of the compressed files using this algorithm and then keeps them that way, even when packing it back to PAK packages.

Decompressed/uncompressed file content after header should look like this:
|Field Type|Description|
|-|-|
|`7BitEncodedInt`|Type reader count, defining the number of elements in following array.|
|Type Reader Info Array| An array of type reader information blocks.|
|`7BitEncodedInt`|Shared resource count, defining the number of additional resources in array later on. Always 0 in FEZ.|
|Object| The primary resource of the XNB file.|
|Object Array| Shared resources array. In FEZ, none of the original XNB files contain additional resources.

Eath `Type Reader Info Array` entry stores information about the `ContentTypeReader<T>` subclass that was used to read the informations within this file:
|Field Type|Description|
|-|-|
|`string`|Type reader name - .NET assembly qualified name of a subclass.|
|`int32`|Version number, usually zero.|

In original XNB files, type reader name includes assembly name specification (which contains assembly identifier, version, culture and public key token). In most cases, they're not required by the game to read the asset file.
However, sometimes it is necessary to include it, especially for readers and types in FezEngine namespace. The way it's solved in FEZ Repacker is by including simplified assembly name specification to every data structure that can be stored in XNB files.

Each Object is a reference to a type reader, followed by a raw data, like so:
|Field Type|Description|
|-|-|
|`7BitEncodedInt`|Object type. If non-zero, then `(type - 1)`th entry of Type Reader Info Array is used.|
|`byte[]`|Raw object data. Empty if object type is zero.|

The idea is that XNB reader can use the object type to determine what reader should be used to read the raw object data. However, since FEZ's XNB files contain only one main resource and its structure is statically typed, FEZ Repacker uses object type only to determine whether there's any data to read, and assumes read type from structure definiton in other cases. That's not what the game is doing though, so Repacker is still including correctly assigned object types and type reader info in the converted XNB file.

## Reader types and behaviour

Based on primary reader type of every XNB file contained within FEZ's packages, we can distinguish 13 unique data types.

|XNB content type name|Main purpose|
|-|-|-|
|Texture2D|Sprites and textures|
|AnimatedTexture|Animated textures|
|ArtObject|3D models of art objects|
|TrileSet|Texture and models of level blocks (triles)|
|Dictionary[String,Dictionary[String,String]]|Language texts|
|SpriteFont|Bitmap font|
|Level|Level data|
|MapTree|???|
|NpcMetadata|???|
|Sky|???|
|TrackedSong|???|
|Effect|XNA shader source code|
|SoundEffect|???|

Each primary type has its own reader, which defines how raw object data should be read. These readers can use other readers - if an object contains a property which itself has its own reader, it's stored in XNB file as a 7-bit encoded integer resembling object type, followed by an actual object data which is then read by it's own reader, similarly to how main object types are handled. Primitive types like `int`, `bool`, `float` etc. are usually stored directly with no prefix.

In both cases, exception may occur, which is determined by how type reader is working. For instance, properties with types like `string` or `TimeSpan` are sometimes read as primitives (in which case, only their values are stored in the file), and sometimes using their own respective readers (in which case, their value is preceded by an identifier of the reader).

Additionally, if a property has been defined as nullable, it's preceded by a boolean. If it's false, entire object data is empty, including object reader type if it's present.

At some point, when I'm done with creating conversion for all types, I'm going to document all of the structures, along with how each property is stored with previously mentioned factors in mind.
