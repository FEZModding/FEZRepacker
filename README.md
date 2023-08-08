# FEZ Repacker

## Overview

**FEZ Repacker** is a tool created for unpacking and packing FEZ's `.pak` asset packages. It allows two-way conversion of game assets into easily modifiable file formats, which makes modding of FEZ more accessible. Currently, this tool is in development and supports a full two-way conversion only for a handful of data types.

## Assets conversion description

FEZ stores its game assets in four `.pak` package files: `Essentials.pak`, `Music.pak`, `Other.pak` and `Updates.pak`. These packages can contain three different file types:

- **Ogg** - digital multimedia container. Stores music.
- **Effect files** - `FXC` effect files containing compiled XNA effect shaders.
- **XNB** - Microsoft XNA Game Studio 4.0 compiled content containers. Stores textures, sound effects, triles, levels, scripts and other miscellaneous data.

In addition to directly unpacking Ogg and effect files, Repacker is capable of converting specific XNB assets types into other intermediate formats and vice-versa, allowing much easier data manipulation.

Here's a list of all 13 asset types handled by FEZ and file types Repacker converts them to:

|XNB content type name|Main purpose|Conversion format|
|-|-|-|
|Texture2D|Sprites and textures|PNG images|
|AnimatedTexture|Animated textures|WebP animation|
|ArtObject|3D models of art objects|Custom `.fezao` file bundle|
|TrileSet|Texture and models of level blocks (triles)|Custom `.fezts` file bundle|
|SpriteFont|Bitmap font|Custon `.fezfont` file bundle|
|Dictionary[String,Dictionary[String,String]]|Language texts|`.fezdata.json` JSON file|
|Level|Level data|`.fezlvl.json` JSON file|
|MapTree|World map data|`.fezmap.json` JSON file|
|NpcMetadata|NPC behaviour information|`.feznpc.json` JSON file|
|Sky|Skybox structure|`.fezsky.json` JSON file|
|TrackedSong|Song information|`.fezsong.json` JSON file|
|Effect|Binary XNA effect file container|Binary FNA effect file|
|SoundEffect|WAV sound effect container|WAV sound file|

You can read about conversion formats in detail [here](https://github.com/Krzyhau/FEZRepacker/wiki/Converted-content-formats).

If you want to learn more about the technical process of reading PAK packages and XNB files, read [this page on the wiki.](https://github.com/Krzyhau/FEZRepacker/wiki/FEZ-assets-data-structure).

## Usage

As of right now, FEZ Repacker is a command line tool. In order to see a list of available commands, use `FEZRepacker.exe --help`.

To make the usage of the tool easier, you can put it within `Content` directory of your FEZ installation.

Here's a list of currently defined commands (differs from the newest pre-release):

- `[--unpack, -u] [pak-path] [destination-folder]`
Unpacks entire .PAK package into specified directory (creates one if doesn't exist) and attempts to convert XNB assets into their corresponding format in the process.

- `--unpack-raw [pak-path] [destination-folder]`
Unpacks entire .PAK package into specified directory (creates one if doesn't exist) leaving XNB assets in their original form.

- `--unpack-decompressed [pak-path] [destination-folder]`
Unpacks entire .PAK package into specified directory (creates one if doesn't exist).and attempts to decompress all XNB assets, but does not convert them.

- `[--unpack-fez-content, -g] [fez-content-directory] [destination-folder]`
Unpacks and converts all game assets into specified directory (creates one if doesn't exist).

- `[--pack, -p] [input-directory-path] [destination-pak-path] <include-pak-path>`
Loads files from given input directory path, tries to deconvert them and pack into a destination .PAK file with given path. If include .PAK path is provided, it'll add its content into the new .PAK package.

- `[--list, -l] [pak-path]`
Lists all files contained withing given .PAK package.

- `[--convert-from-xnb, -x] [xnb-input] [file-output]`
Attempts to convert given XNB input (this can be a path to a single asset or an entire directory) and save it at given output (if input is a directory, converts all files within it recursively and dumps all converted files in specified path).

- `[--convert-to-xnb, -X] [file-input] [xnb-output]`
Attempts to convert given input (this can be a path to a single file or an entire directory) into XNB file(s) and save it at given output (if input is a directory, converts all files within it recursively and dumps all converted files in specified path).

Here are some examples how you can use these commands:

- To unpack `Other.pak` into a directory called `Other unpacked`, use:
`FEZRepacker.exe --unpack Other.pak "Other unpacked"`
- To repack files contained in `Other unpacked` directory into a package named `Other.pak` while also including the contents of `Other_old.pak`, use:
`FEZRepacker.exe --pack "Other unpacked" Other.pak Other_old.pak`

It is recommended to use mod loader to swap original assets, but if you're trying to do it manually by recompiling them into one of the archives and your changes didn't affect the game, here's a couple of things to keep in mind:

- directory tree created in unpacking process **does matter** - putting modified files in the main directory after changing them doesn't give them their original location and name in PAK file after repacking, preventing the game from finding the new asset.
- Packages are loaded in this order: `Essentials.pak`, `Updates.pak` and `Other.pak`. If you're trying to override an existing file, it has to be in the same package or in a package that's loaded sooner.
- Music is handled separately and has to be packed in `Music.pak`.

## Sources

Sources used in a process of writing this tool and documentation:

- [.pak structure reverse-engineering by Mathias Panzenböck (panzi)](http://hackworthy.blogspot.com/2017/08/reverse-engineering-simple-game-archive.html)
- [Official XNB format documentation by Microsoft Corportation](https://docplayer.net/49383763-Microsoft-xna-game-studio-4-0-compiled-xnb-content-format.html)
- [XNB reader code from open source implementation of MonoGame](https://github.com/labnation/MonoGame/blob/d270be3e800a3955886e817cdd06133743a7e043/MonoGame.Framework/Content/ContentManager.cs#L405)
- [xnb_parse repository by fesh0r](https://github.com/fesh0r/xnb_parse/)
- My own decompilation of the latest FEZ release (as of 2022)
