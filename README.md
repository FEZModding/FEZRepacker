# FEZ Repacker

## Overview

**FEZ Repacker** is a tool created for unpacking and packing FEZ's `.pak` asset packages. It allows two-way conversion of game assets into easily modifiable file formats, which makes modding of FEZ more accessible. Currently, this tool is in development and supports a full two-way conversion only for a handful of data types.

## Assets conversion description

FEZ stores its game assets in four `.pak` package files: `Essentials.pak`, `Music.pak`, `Other.pak` and `Updates.pak`. These packages can contain three different file types:

- **Ogg** - digital multimedia container. Stores music.
- **Effect files** - `FXC` effect files containing compiled XNA effect shaders.
- **XNB** - Microsoft XNA Game Studio 4.0 compiled content containers. Stores textures, sound effects, triles, levels, scripts and other miscellaneous data.

Both Ogg and effect files are directly unpacked from PAK archives by FEZ Repacker. However, it additionally attempts to convert XNB files into a more readable format.

We can distinguish 13 different data types that can be stored in FEZ's XNB files:

- Static images used by sprites and textures (`Texture2D`)
- Animated textures (`AnimatedTexture`)
- 3D model of an art object (`ArtObject`)
- Textures and models of level blocks/triles (`TrileSet`)
- Language texts (`Dictionary[String,Dictionary[String,String]]`)
- Bitmap fonts (`SpriteFont`)
- Level data (`Level`)
- World map data file (`MapTree`)
- NPC metadata files (`NpcMetadata`)
- Sky data files (`Sky`)
- Song information files (`TrackedSong`)
- binary XNA effect files (`Effect`)
- SFX (`SoundEffect`)

Each of these formats are handled differently in order to convert it into a file format that allows easier manipulation of data it's representing (for instance, static images are converted into PNG files), which then Repacker can read and convert back into its original data structure.

If you want to learn more about the process of reading PAK packages and XNB files, read [this page on the wiki.](/wiki/FEZ-assets-data-structure).

If you're interested in file formats exported by Repacker, read [this](/wiki/Converted-content-formats) instead.

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
Attempts to convert given XNB input (this can be a path to a single asset or an entire directory) and save it at given output (if input is a directory, dumps all converted files in specified path). Directories are treated recursively.

- `[--convert-to-xnb, -X] [file-input] [xnb-output]`
Attempts to convert given input (this can be a path to a single file or an entire directory) into XNB file(s) and save it at given output (if input is a directory, dumps all converted files in specified path). Directories are treated recursively.

Here are some examples how you can use these commands:

- To unpack `Other.pak` into a directory called `Other unpacked`, use:
`FEZRepacker.exe --unpack Other.pak "Other unpacked"`
- To repack files contained in `Other unpacked` directory into a package named `Other.pak` while also including the contents of `Other_old.pak`, use:
`FEZRepacker.exe --pack "Other unpacked" Other.pak Other_old.pak`

If you're trying to swap original assets by recompiling them into one of the archives and your changes didn't affect the game, here's a couple of things to keep in mind:

- directory tree created in unpacking process **does matter** - putting modified files in the main directory after changing them doesn't give them their original location and name in PAK file after repacking, preventing the game from finding the new asset.
- Usually there's no rule in what PAK package your files are located, but in some cases it does matter, so while you can get behind packing your textures in, let's say, `Updates.pak`, music Ogg files are required to be located in `Music.pak` to be overwritten. A list of exceptions is larger than that, but I haven't fully figured it out yet.

## Sources

Sources used in a process of writing this tool and documentation:

- [.pak structure reverse-engineering by Mathias Panzenböck (panzi)](http://hackworthy.blogspot.com/2017/08/reverse-engineering-simple-game-archive.html)
- [Official XNB format documentation by Microsoft Corportation](https://docplayer.net/49383763-Microsoft-xna-game-studio-4-0-compiled-xnb-content-format.html)
- [XNB reader code from open source implementation of MonoGame](https://github.com/labnation/MonoGame/blob/d270be3e800a3955886e817cdd06133743a7e043/MonoGame.Framework/Content/ContentManager.cs#L405)
- [xnb_parse repository by fesh0r](https://github.com/fesh0r/xnb_parse/)
- My own decompilation of the latest FEZ release (as of 2022)
