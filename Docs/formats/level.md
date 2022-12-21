# `.fezlvl` file format

## Overview

`.fezlvl` files contain level data stored in JSON format. This document presents a structure and purpose of each property in this file format.

It's incomplete in some cases, and it might be incorrect due to lack of proper testing in the game itself, but that'll improve over time.

Additionally. Keep in mind this format differs from how the level data is arranged originally, in order to improve readability and eliminate redundant and unnecessary data arrangement. However, that is still in progress, so what's present here is still subject to change.

## Level

Top-level object stored in `.fezlvl` JSON file.

|Property name|Type|Description|
|-|-|-|
|Name|String|Name of the level used internally by the game.|
|NodeType|[LevelNodeType](#levelnodetype)|Type of node used to represent level on the map.|
|Size|[Vector3](#vector3)|Size of the map.|
|StartingPosition|[TrileFace](#trileface)|Default location and orientation where player should start the level.|
|Flat|Boolean|If true, blocks player's ability to shift in this level.|
|Quantum|Boolean|If true, blocks are visually randomized.|
|Descending|Boolean|Behaviour currently unknown. Used only in ZU_CODE_LOOP.|
|Loops|Boolean|If true, level loops vertically.|
|Rainy|Boolean|Determines whether it should rain in the level.|
|BaseDiffuse|Float|Scale of a diffuse lighting.|
|BaseAmbient|Float|Scale of an ambient lighting.|
|SkyName|String|Name of a sky asset to use in this level.|
|SkipPostProcess|Boolean|If true, level is rendered without post-processing effects.|
|GomezHaloName|String|A name of a background plane texture used for halo effect around Gomez.|
|HaloFiltering|Boolean|Behaviour currently unknown.|
|BlinkingAlpha|Boolean|Behaviour currently unknown.|
|WaterHeight|Float|Height of water in this level.|
|WaterType|[LiquidType](#liquidtype)|Type of liquid to use for water in this level.|
|SongName|String|Name of the song to play.|
|MutedLoops|String[]|List of names of loops to mute in played song.|
|AmbienceTracks|[AmbienceTrack](#ambiencetrack)|List of ambience tracks to play.|
|SequenceSamplesPath|string|Behaviour currently unknown.|
|LowPass|Boolean|If true, music is put through low pass filter.|
|TrileSetName|String|A name of a Trile Set to use in this level.|
|Triles|[Trile](#trile)[]|A list of triles in this level.|
|Groups|[TrileGroup](#trilegroup)[]|A list of trile groups.|
|Volumes|[Volume](#volume)[]|A list of volumes in this level.|
|Scripts|[Script](#script)[]|A list of scripts in this level.|
|ArtObjects|[ArtObjectInstance](#artobjectinstance)[]|A list of art object instances in this level.|
|BackgroundPlanes|[BackgroundPlane](#backgroundplane)[]|A list of background planes in this level.|
|Paths|[MovementPath](#movementpath)[]|A list of paths in this level.|
|NonPlayerCharacters|[NpcInstance](#npcinstance)[]|A list of NPCs in this level.|

## Enum

All enums are stored as PascalCamelCase string parameters.

## LevelNodeType

[Enum](#enum) specifying how level should appear in the map. It can take one of three values:

- **Node**
- **Hub**
- **Lesser**

## Vector3

Geometry structure containing X, Y and Z coordinates. It's stored as an array of three floating point numbers, simirarly to how GeoJSON does it.

## TrileFace

Structure identifying location and orientation of a trile face in a level.

|Property name|Type|Description|
|-|-|-|
|Id|[TrileEmplacement](#trileemplacement)|Location of the starting position.|
|Face|[FaceOrientation](#faceorientation)|Orientation of the starting position.|

## TrileEmplacement

Geometry structure containing X, Y, and Z coordinates of a trile in the map. It's stored as an array of three integer numbers.

## FaceOrientation

[Enum](#enum) specifying one of six possible trile face orientations - **Left**, **Down**, **Back**, **Right**, **Top** or **Front**.

## LiquidType

[Enum](#enum) specifying one possible water types used in levels - **None**, **Water**, **Blood**, **Lava**, **Sewer**, **Purple** or **Green**.

## AmbienceTrack

Structure specifying the name of ambience track to play and when it's played.

|Property name|Type|Description|
|-|-|-|
|Name|String|Name of an ambience track to play.|
|Day|Boolean|If true, ambience track will play during the day.|
|Dusk|Boolean|If true, ambience track will play during the dusk.|
|Night|Boolean|If true, ambience track will play during the nigty.|
|Dawn|Boolean|If true, ambience track will play during the dawn.|

## Trile

Structure containing information about a single trile - a level tile in 3D space. Multiple triles may be present in the same position.

|Property name|Type|Description|
|-|-|-|
|Position|[TrileEmplacement](#trileemplacement)|Position of this trile.|
|Orientation|[FaceOrientation](#faceorientation)|Orientation of this trile. Can only face horizontal directions (**Top** and **Down** won't work).|
|Id|Integer|ID of a trile in Trile Set used by this level.|
|Settings|[TrileSettings](#trilesettings)|Additional parameters of this trile. Can be null.|

## TrileSettings

Structure containing additional information of a trile.

|Property name|Type|Description|
|-|-|-|
|Offset|[Vector3](#vector3)|Position offset of this trile.|
|SignText|String|Language identifier of a text which appears when interacting with this trile.|
|Sequence|String|Behaviour currently unknown. Presumably related to blinking blocks.|
|SequenceSampleName|String|Behaviour currently unknown. Same as above.|
|SequenceAlternateSampleName|String|Behaviour currently unknown. Same as above.|
|HostVolume|Integer|ID of host [Volume](#volume). Purpose unknown.|

## TrileGroup

Structure containing a list of triles grouped together and parameters of this group.

|Property name|Type|Description|
|-|-|-|
|Triles|[TrileEmplacement](#trileemplacement)[]|List of trile coordinates used to determine which triles belong to this group.|
|Path|[MovementPath](#movementpath)|Path this group moves along|
|Heavy|Boolean|Behaviour currently unknown.|
|ActorType|[ActorType](#actortype)|Type of actor this group should be treated as.|
|GeyserOffset|Float|Behaviour currently unknown.|
|GeyserPauseFor|Float|Behaviour currently unknown.|
|GeyserLiftFor|Float|Behaviour currently unknown.|
|GeyserApexHeight|Float|Behaviour currently unknown.|
|SpinCenter|[Vector3](#vector3)|Behaviour currently unknown.|
|SpinClockwise|Boolean|Behaviour currently unknown.|
|SpinFrequency|Float|Behaviour currently unknown.|
|SpinNeedsTriggering|Boolean|Behaviour currently unknown.|
|Spin180Degrees|Boolean|Behaviour currently unknown.|
|FallOnRotate|Boolean|Behaviour currently unknown.|
|SpinOffset|Float|Behaviour currently unknown.|
|AssociatedSound|String|Behaviour currently unknown.|

## MovementPath

Structure defining the path of movement and its properties. It's used by [Trile groups](#trilegroup) and occasionally by functions available through [Scripts](#script).

|Property name|Type|Description|
|-|-|-|
|Segments|[PathSegment](#pathsegment)[]|A list of path segments defining the path.|
|NeedsTrigger|Boolean|If true, the path needs to be triggered to function.|
|EndBehavior|[PathEndBehaviour](#pathendbehaviour)|Defines how path should behave when end is reached.|
|SoundName|String|A name of sound to use when path is in use.|
|IsSpline|Bool|If true, path uses spline interpolation.|
|OffsetSeconds|Float|Determines how long to wait until movement starts.|
|SaveTrigger|Boolean|Behaviour currently unknown.|

## PathSegment

Structure defining properties of a path segment.

|Property name|Type|Description|
|-|-|-|
|Destination|[Vector3](#vector3)|Destination of this path segment.|
|Duration|Float|Time it takes to traverse the segment, in seconds.|
|WaitTimeOnStart|Float|Time to wait on start of the segments, in seconds.|
|WaitTimeOnFinish|Float|Time to wait on end of the segment, in seconds.|
|Acceleration|Float|Acceleration factor. Exact behaviour currently unknown.|
|Deceleration|Float|Deceleration factor. Exact behaviour currently unknown.|
|JitterFactor|Float|Jittering factor. Exact behaviour currently unknown.|
|Orientation|[Quaternion](#quaternion)|Behaviour currently unknown.|
|CustomData|[CameraNodeData](#cameranodedata)|Contains additional camera data. Can be null.|

## Quaternion

Geometry structure containing X, Y, Z and W coordinates. It's stored as an array of four floating point numbers, simirarly to how GeoJSON does it.

## CameraNodeData

Structure containing custom information about camera movement along path.

|Property name|Type|Description|
|-|-|-|
|Perspective|Boolean|Behaviour currently unknown.|
|PixelsPerTrixel|Integer|Behaviour currently unknown.|
|SoundName|String|Behaviour currently unknown.|

## PathEndBehaviour

[Enum](#enum) specifying how path segment should end - **Bounce**, **Loop** or **Stop**.

## ActorType

[Enum](#enum) specifying a type of actor used by TrileGroup, Art Object or Background Plane. Depending on a type of actor, specified object can be treated differently. A list of all actor types along with their behaviours will be provided in the future.

## Volume

Structure defining a cuboid zone with custom properties. These zones can also be used by [Scripts](#script) for custom conditions or triggers.

|Property name|Type|Description|
|-|-|-|
|Orientations|[FaceOrientation](#faceorientation)|Orientations towards which perspective can be shifted for this Volume to work.|
|From|[Vector3](#vector3)|Coordinates of one of the corners of this volume zone.|
|To|[Vector3](#vector3)|Coordinates of another of the corners of this volume zone.|
|DotDialogue|[DotDialogueLine](#dotdialogueline)[]|List of Dot dialogues used by this volume.|
|CodePattern|[CodeInput](#codeinput)[]|Input combo used by this volume.|
|IsBlackHole|Boolean|Exact behaviour currently unknown.|
|NeedsTrigger|Boolean|Behaviour currently unknown.|
|IsSecretPassage|Boolean|Behaviour currently unknown.|
|WaterLocked|Boolean|Behaviour currently unknown.|
|IsPointOfInterest|Boolean|Behaviour currently unknown.|
|FarawayPlaneOffset|[Vector2](#vector2)|Behaviour currently unknown.|

## DotDialogueLine

Structure defining Dot dialogue.

|Property name|Type|Description|
|-|-|-|
|ResourceText|String|Behaviour currently unknown.|
|Grouped|Boolean|Behaviour currently unknown.|

## CodeInput

[Enum](#enum) specifying in-game input - **None**, **Up**, **Down**, **Left**, **Right**, **SpinLeft**, **SpinRight** or **Jump**.

## Vector2

Geometry structure containing X and Y coordinates. It's stored as an array of two floating point numbers, simirarly to how GeoJSON does it.

## Script

Structure defining custom behaviour which can await certain triggers, react to specified conditions and execute defined actions with given parameters.

|Property name|Type|Description|
|-|-|-|
|Name|String|Name of the script. Presumably unused.|
|Timeout|Boolean|A time after which the script should be terminated, in seconds.|
|Triggers|[ScriptTrigger](#scripttrigger)[]|A list of triggers this script can react to.|
|Conditions|[ScriptCondition](#scriptcondition)[]|A list of conditions that has to be met for this script to be executed.|
|Actions|[ScriptAction](#scriptaction)[]|A list of actions this script will execute.|
|OneTime|Boolean|If set, this script will execute only once.|
|Trigerless|Boolean|Behaviour currently unknown.|
|IgnoreEndTriggers|Boolean|Behaviour currently unknown.|
|LevelWideOneTime|Boolean|Behaviour currently unknown.|
|Disabled|Boolean|If set, this script will be disabled until enabled by another script.|
|IsWinCondition|Boolean|Behaviour currently unknown.|

## Script Operation

[ScriptTrigger](#scripttrigger), [ScriptCondition](#scriptcondition) and [ScriptAction](#scriptaction) are using string-encoded operations. Their syntax may vary, but they will always start with entity identifier followed by a dot. Entity identifier is simply it's class name. If an entity is not static (like Level or Camera), it's followed by a square brackets [] operator containing an ID of an object (like Volume or ArtObject).

A full list of entities and their exposed triggers, properties and actions will be provided in the future.

## ScriptTrigger

[String-encoded operation](#script-operation) starting with entity identifier, followed by the name of the trigger, like so:

```js
[EntityIdentifier].[TriggerName]
```

As an example:

```js
Level.Start
```

## ScriptCondition

[String-encoded operation](#script-operation) starting with entity identifier, followed by the name of the property, one of the valid comparison operator (`==`, `>=`, `<=`, `>`, `<` or `!=`) and value literal, like so:

```js
[EntityIdentifier].[Property] [ComparisonOperator] [ValueLiteral]
```

As an example:

```js
Gomez.CollectedCubes >= 1
```

```js
Game.GetLevelState == INTRO_COMPLETE
```

## ScriptAction

[String-encoded operation](#script-operation) starting with entity identifier, followed by the name of the action and a list of parameters enclosed by parentheses and separated by commas. Additionally, operation string can be preceded by two control characters:

- `#` - blocks execution of following actions until this one is completed.
- `!` - terminates the entire script once the action is executed.

Script action has following syntax:

```js
[ControlCharacters][EntityIdentifier].[ActionName]([Property1], [Property2], ...)
```

As an example:

```js
Game.Wait(1)
```

```js
#Dot.Say(DOT_CUBES_GET_A, False, False)
```

## ArtObjectInstance

Structure defining art object instance, its location and properties.

|Property name|Type|Description|
|-|-|-|
|Name|String|Name of the art object used by this instance.|
|Position|Vector3|Position of this instance.|
|Rotation|Quaternion|Rotation of this instance.|
|Scale|Vector3|Scale of this instance.|
|Inactive|Boolean|If set, this Art Object will be inactive by default.|
|AttachedGroup|Integer|Identifier of a [Trile Group](#trilegroup) this art object belongs to. Can be null.|
|RotationCenter|Vector3|Rotation center of this art object.|
|SpinView|[Viewport](#viewpoint)|Behaviour currently unknown.|
|SpinEvery|Float|Behaviour currently unknown.|
|SpinOffset|Float|Behaviour currently unknown.|
|OffCenter|Boolean|Behaviour currently unknown.|
|VibrationPattern|[VibrationMotor](#vibrationmotor)[]|Behaviour currently unknown.|
|CodePattern|[CodeInput](#codeinput)[]|Behaviour currently unknown.|
|Segment|[PathSegment](#pathsegment)|Behaviour currently unknown.|
|NextNode|Integer|Behaviour currently unknown. Can be null.|
|DestinationLevel|String|Behaviour currently unknown.|
|TreasureMapName|String|Behaviour currently unknown.|
|InvisibleSides|[FaceOrientation](#faceorientation)[]|Behaviour currently unknown.|
|TimeswitchWindBackSpeed|Float|Behaviour currently unknown.|
|ContainedTrile|[ActorType](#actortype)|Behaviour currently unknown.|

## VibrationMotor

[Enum](#enum) specifying controller vibration - **None**, **LeftLow** or **RightHigh**.

## Viewpoint

[Enum](#enum) specifying viewport - **None**, **Front**, **Right**, **Back**, **Left**, **Up**, **Down** or **Perspective**.

## BackgroundPlane

Structure defining background plane, its location and properties.

|Property name|Type|Description|
|-|-|-|
|Position|[Vector3](#vector3)|Position of a background plane.|
|Rotation|[Quaternion](#quaternion)|Rotation of a background plane.|
|Scale|[Vector3](#vector3)|Scale of a background plane.|
|Size|[Vector3](#vector3)|Behaviour currently unknown.|
|TextureName|String|Name of a texture used by this background plane.|
|LightMap|Boolean|Behaviour currently unknown.|
|AllowOverbrightness|Boolean|Behaviour currently unknown.|
|Filter|[Color](#color)|Behaviour currently unknown.|
|Animated|Boolean|Behaviour currently unknown.|
|Doublesided|Boolean|Behaviour currently unknown.|
|Opacity|Float|Behaviour currently unknown.|
|AttachedGroup|Integer|A [Trile Group](#trilegroup) this background plane belongs to. Can be null.|
|Billboard|Boolean|Behaviour currently unknown.|
|SyncWithSamples|Boolean|Behaviour currently unknown.|
|Crosshatch|Boolean|Behaviour currently unknown.|
|UnusedFlag|Boolean|Behaviour currently unknown.|
|AlwaysOnTop|Boolean|Behaviour currently unknown.|
|Fullbright|Boolean|Behaviour currently unknown.|
|PixelatedLightmap|Boolean|Behaviour currently unknown.|
|XTextureRepeat|Boolean|Behaviour currently unknown.|
|YTextureRepeat|Boolean|Behaviour currently unknown.|
|ClampTexture|Boolean|Behaviour currently unknown.|
|ActorType|[ActorType](#actortype)|Behaviour currently unknown.|
|AttachedPlane|Integer|Behaviour currently unknown. Can be null.|
|ParallaxFactor|Float|Behaviour currently unknown.|

## Color

Structure containing information about R, G, B and A components of the color. It's stored as a HTML color code (`#rrggbbaa`) in a string.

## NpcInstance

Structure defining an instance of a non-playable character in the level.

|Property name|Type|Description|
|-|-|-|
|Name|String|Name of an NPC to use for this instance.|
|Position|[Vector3](#vector3)|Initial position of this NPC instance.|
|DestinationOffset|[Vector3](#vector3)|Behaviour currently unknown.|
|WalkSpeed|Float|Behaviour currently unknown.|
|RandomizeSpeech|Boolean|Behaviour currently unknown.|
|SayFirstSpeechLineOnce|Boolean|Behaviour currently unknown.|
|AvoidsGomez|Boolean|Behaviour currently unknown.|
|ActorType|[ActorType](#actortype)|Behaviour currently unknown.|
|Speech|[SpeechLine](#speechline)[]|Behaviour currently unknown.|
|Speech|[NpcAction Dictionary](#npcaction-dictionary)|Behaviour currently unknown.|

## SpeechLine

Structure defining a speech line and its additional attributes.

|Property name|Type|Description|
|-|-|-|
|Text|String|Language identifier of a text which is used.|
|OverrideContent|[NpcActionContent](#npcactioncontent)|Behaviour currently unknown.|

## NpcActionContent

|Property name|Type|Description|
|-|-|-|
|AnimationName|String|Behaviour currently unknown.|
|SoundName|String|Behaviour currently unknown.|

## NpcAction Dictionary

A dictionary of [NpcActionContents](#npcactioncontent) assigned to specific [NpcAction](#npcaction) as a key.

## NpcAction

[Enum](#enum) specifying NPC's action - **None**, **Idle**, **Idle2**, **Idle3**, **Walk**, **Turn**, **Talk**, **Burrow**, **Hide**, **ComeOut**, **TakeOff**, **Fly** or **Land**.
