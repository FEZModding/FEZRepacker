# Converted content formats

XNB content files contained within FEZ's `.pak` archives contain only one resource, which is read by a specific data type reader. In order to make game assets easily editable by modders after unpacking, they're converted into file formats which allow easier manipulation of data they're representing. These file formats are also used to determine what content type should be used when converting a file back into the XNB content file.

Here's a list of all content formats and paired file types.

## Texture2D type

XNB files containing primary type of `Texture2D` are converted into loselessly compressed PNG image files.

## AnimatedTexture type

XNB files containing primary type of `AnimatedTexture` are converted into an animated WebP image by splicing the animation atlas texture into individual loselessly compressed frames with alpha channel and giving each of them their defined timing. This process is then reversed when converting it back into XNB file by parsing frame timing and arranging frames into an atlas texture.

## ArtObject type

Although currently not implmenented, XNB files containing primary type of `ArtObject` will be processed into a ZIP archive with `.fezao` extensions, which will contain three files:

- `texture.png` - texture of the art object, saved similarly as `Texture2D` type.
- `model.obj` - model of the art object, saved as an OBJ file.
- `data.json` - additional data, if contains any, stored in a JSON file.

## TrileSet type

A way of converting `TrileSet` type is currently not defined.

## `Dictionary[String,Dictionary[String,String]]` type

This type is primarily used by a language file, which defines dialogues for every language in the game. It's converted into a JSON file, with each key of dictionaries being a JSON property.

## SpriteFont type

A way of converting `SpriteFont` type is currently not defined.

## Level type

XNB files containing primary type of `Level` will be converted into a JSON file with an altered structure. The structure itself isn't yet defined, but it'll aim for optimizing file size and readability. Once it's ready, a detailed documentation will be linked here. As of right now, the structure is similar to the data structure in XNB file.

## MapTree type

A way of converting `MapTree` type is currently not defined.

## NpcMetadata type

A way of converting `NpcMetadata` type is currently not defined.

## Sky type

A way of converting `Sky` type is currently not defined.

## TrackedSong type

A way of converting `TrackedSong` type is currently not defined.

## Effect type

A way of converting `Effect` type is currently not defined.

## SoundEffect type

A way of converting `SoundEffect` type is currently not defined.
