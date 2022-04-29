# Unofficial FEZ `.pak` archive file documentation

## Overview

FEZ stores its game assets in four `.pak` files: `Essentials.pak`, `Music.pak`, `Other.pak` and `Updates.pak`. This documentation will attempt to give a rought idea on how to read them.

Notes:

- All of the fields are little-endian unless specified otherwise.
- Strings are prefixed with their length as a seven-bit encoded integer.

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

Types of files stored in the archive can vary. Because file extensions are not included, the only indication of a file type is its own header and path/name. In FEZ, there seem to be three file types:

- **XNB** - Microsoft XNA Game Studio 4.0 compiled content containers. Stores textures, sound effects, triles, levels, scripts and other miscellaneous data.
- **Ogg** - digital multimedia container. Stores music.
- **Effect files** - files within the `effects` directory, having no recognisable identifier. Presumably they're the `XFO` files used in XNA Game Studio 4.0.

## `XNB` file format

Header of each XNB file has following structure:

|Field Type|Description|
|-|-|
|`char[3]`|Format identifier. Should be equal to `['X', 'N', 'B']`.|
|`char`|Target platform. Possible values are `w`, `m` and `x` for MS Windows, Windows Phone 7 and Xbox 360 respectively.|
|`byte`|XNB format version. FEZ uses `5` - XNA Game Studio 4.0.|
|`byte`|Flag bits. `0x01` - content is for HiDef profile (used for FEZ assets), `0x40` - data is compressed (LZ4, unused for FEZ), `0x80` - data is compressed (LZX, used for all FEZ assets).|
|`uint32`|Compressed file size, including header.|
|`uing32`|Optional; is present only when compression flag is set. Represents the size of the file after decompression without this header.|

If compression flag is set, the rest of the file is compressed. In case of FEZ, slightly modified LZX algorithm is used. For details, visit [XNB reader code from open source implementation of MonoGame](https://github.com/labnation/MonoGame/blob/d270be3e800a3955886e817cdd06133743a7e043/MonoGame.Framework/Content/ContentManager.cs#L405).

Decompressed/uncompressed file content after header should look like this:
|Field Type|Description|
|-|-|
|`7BitEncodedInt`|Type reader count, defining the number of elements in following array.|
|Type Reader Info Array| An array of type reader information blocks.|
|`7BitEncodedInt`|Shared resource count, defining the number of additional resources in array later on. Always 0 in FEZ.|
|Object| The primary resource of the XNB file.|
|Object Array| Shared resources array. In FEZ, none of the original XNB files contain additional resources.

Eath Type Reader Info Array entry stores information about the `ContentTypeReader<T>` subclass that was used to read the informations within this file.
|Field Type|Description|
|-|-|
|`string`|Type reader name - .NET assembly qualified name of a subclass.|
|`int32`|Version number, usually zero.|

Each Object is a reference to a type reader, followed by a raw data.
|Field Type|Description|
|-|-|
|`7BitEncodedInt`|Object type. If non-zero, then `(type - 1)`th entry of Type Reader Info Array is used.|
|`byte[]`|Raw object data. Empty if object type is zero.|

## `XNB` resource types.
FEZ contains 13 main resource types (that is, resource which is primary for given XNB file):

- AnimatedTexture (FezEngine)
- ArtObject (FezEngine)
- Level (FezEngine)
- MapTree (FezEngine)
- NpcMetadata (FezEngine)
- Sky (FezEngine)
- TrackedSong (FezEngine)
- TrileSet (FezEngine)
- Dictionary (Microsoft.XNA.Framework)
- Effect (Microsoft.XNA.Framework)
- SoundEffect (Microsoft.XNA.Framework)
- SpriteFont (Microsoft.XNA.Framework)
- Texture2D (Microsoft.XNA.Framework)

### **[to be continued. I'll fill this in as I go.]**

## Sources
Sources used in a process of writing this documentation:

- [.pak structure reverse-engineering by Mathias Panzenböck (panzi)](http://hackworthy.blogspot.com/2017/08/reverse-engineering-simple-game-archive.html)
- [Official XNB format documentation by Microsoft Corportation](https://docplayer.net/49383763-Microsoft-xna-game-studio-4-0-compiled-xnb-content-format.html)
- [XNB reader code from open source implementation of MonoGame](https://github.com/labnation/MonoGame/blob/d270be3e800a3955886e817cdd06133743a7e043/MonoGame.Framework/Content/ContentManager.cs#L405)
- [xnb_parse repository by fesh0r](https://github.com/fesh0r/xnb_parse/)
- [Decompilation of FEZ 1.0.6 by dptug](https://github.com/dptug/FEZ)
