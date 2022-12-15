# FEZ Repacker

## Overview

**FEZ Repacker** is a tool created for unpacking and packing FEZ's `.pak` asset packages. It allows two-way conversion of game assets into easily modifiable file formats, which makes modding of FEZ more accessible. Currently, this tool is in development and supports a full two-way conversion only for a handful of data types.

## Assets conversion description

FEZ stores its game assets in four `.pak` package files: `Essentials.pak`, `Music.pak`, `Other.pak` and `Updates.pak`. These packages can contain three different file types:

- **Ogg** - digital multimedia container. Stores music.
- **Effect files** - *presumably* the `XFO` files used in XNA Game Studio 4.0.
- **XNB** - Microsoft XNA Game Studio 4.0 compiled content containers. Stores textures, sound effects, triles, levels, scripts and other miscellaneous data.

Both Ogg and effect files are directly unpacked from PAK archives by FEZ Repacker. However, it additionally attempts to convert XNB files into a more readable format.

We can distinguish 13 different data types that can be stored in FEZ's XNB files:

- Static images used by sprites and textures
- Animated textures
- 3D model of an art object
- Textures and models of level blocks (triles)
- Language texts
- Bitmap fonts
- Level data
- Map tree data file
- NPC metadata files
- Sky data files
- Tracked songs data files
- XNA shader source code
- Sound effects

Each of these formats are handled differently in order to convert it into a file format that allows easier manipulation of data it's representing (for instance, static images are converted into PNG files), which then Repacker can read and convert back into its original data structure.

If you want to learn more about the process of reading PAK packages and XNB files, read [this documentation file](/Docs/pak.md).

If you're interested in file formats exported by Repacker, read [this](/Docs/formats.md) instead.

## Usage

As of right now, FEZ Repacker is a command line tool. In order to see a list of available commands, use `FEZRepacker.exe help`.

To make the usage of the tool easier, you can put it within `Content` directory of your FEZ installation.

Here's a list of currently defined commands:

- `unpack <source> <destination> [-xnb]` - unpacks PAK file in a location defined by `<source>` parameter and stores its content in a directory defined by `<destination>` parameter. If `-xnb` parameter is used, raw XNB files will be unpacked instead of their converted versions.
- `pack <source> <destionation>` - converts and packs files stored in a directory defined by `<source>` parameter and saves resulting PAK package in a location defined by `<destination>` parameter.
- `add <target> <source> [destination]` - converts and packs files stored in a directory defined by `<source>` parameter and adds them to a PAK package in a location defined by `<target>` parameter. If `[destination]` parameter is defined, original PAK file will not be overwritten, and instead a new one will be located in a location defined by this parameter.
- `remove <target> <name> [destination]` - removes a file defined by a `<name>` parameter from a PAK package in a location defined by `<source>` parameter. If `[destination]` parameter is defined, original PAK file will not be overwritten, and instead a new one will be located in a location defined by this parameter.
- `list <source>` - lists all files packed into a PAK file defined by `<source>` parameter.

Here are some examples how you can use these commands:

- To unpack `Other.pak` into a directory called `Other unpacked`, use:
`FEZRepacker.exe unpack Other.pak "Other unpacked"`
- To repack files contained in `Other unpacked` directory into a package named `Other.pak`, use:
`FEZRepacker.exe pack "Other unpacked" Other.pak`
- To add files contained in `Modded files` directory into the `Updates_old.pak` and save it as a new package called `Updaets.pak`, use:
`FEZRepacker.exe add Updates_old.pak "Modded files" Updates.pak`
- To remove a file called `font\zuish` from package `Updates.pak`, use:
`FEZRepacker.exe remove Updates.pak font\zuish`

If your changes to the original package files didn't affect the game, here's a couple of things to keep in mind:

- directory tree created in unpacking process **does matter** - putting modified files in the main directory after changing them doesn't give them their original location and name in PAK file after repacking, preventing the game from finding the new asset.
- Usually there's no rule in what PAK file files are located, but in some cases it does matter, so while you can get behind packing your textures in, let's say, `Updates.pak`, Ogg music files are required to be located in `Music.pak` to be overwritten. A list of exceptions is larger than that, but I haven't fully figure it out yet.

## Sources

Sources used in a process of writing this tool and documentation:

- [.pak structure reverse-engineering by Mathias Panzenböck (panzi)](http://hackworthy.blogspot.com/2017/08/reverse-engineering-simple-game-archive.html)
- [Official XNB format documentation by Microsoft Corportation](https://docplayer.net/49383763-Microsoft-xna-game-studio-4-0-compiled-xnb-content-format.html)
- [XNB reader code from open source implementation of MonoGame](https://github.com/labnation/MonoGame/blob/d270be3e800a3955886e817cdd06133743a7e043/MonoGame.Framework/Content/ContentManager.cs#L405)
- [xnb_parse repository by fesh0r](https://github.com/fesh0r/xnb_parse/)
- My own decompilation of the latest FEZ release (as of 2022)
