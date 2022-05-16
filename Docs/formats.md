# Exported content formats

XNB content files contained within FEZ's `.pak` archives contain only one resource, which is read by a specific data type reader. In order to make game assets easily editable by modders after unpacking, they're converted into more popular file formats. These file formats are also used to determine what content type should be used when converting a file back into the XNB content file.

Here's a list of all content formats and paired file types.

|XNB content type name|Main purpose|Converted file format|
|-|-|-|
|Texture2D|Sprites and textures|PNG image
|AnimatedTexture|Animated textures|Animated WebP image|
|ArtObject|3D models|Custom `.fezao` format|
|TrileSet|Texture and models of level blocks (triles)|Custom `.fezts` format|
|Dictionary[String,Dictionary[String,String]]|Language texts|YAML data file
|SpriteFont|Bitmap font|???|
|Level|Level data|???|
|MapTree|???|???|
|NpcMetadata|???|???|
|Sky|???|???|
|TrackedSong|???|???|
|Effect|XNA shader source code|???|
|SoundEffect|???|???|

## `.fezao` file type

Art objects are exported as a ZIP archive with `.fezao` extensions, which contains three files:

- `texture.png` - texture of the art object.
- `model.obj` - model of the art object.
- `data.json` - additional data, if contains any.
