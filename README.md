# ItemPath Plugin for Rainmeter

The **ItemPath Plugin** is a versatile Rainmeter plugin that allows users to select files or folders through interactive dialogs and dynamically write the results to a specified `.ini` file. This plugin is useful for creating dynamic and customizable Rainmeter skins.

---

## Features

- **ChooseFile**: Select a file with a specific extension filter (e.g., `.txt`, `.png`) and copy it to a specified location.
- **ChooseFolder**: Select a folder and write its name and path to a `.ini` file.
- Write selected file or folder information (name and path) to customizable variables in a `.ini` file.
- Support for dynamic folder creation and file copying.

---

## Commands

### Choose a File
```ini
[!CommandMeasure MeasurePath "MeasureType=ChooseFile|FileType=.txt|VarName=FileName|VarPath=FilePath|IniPath=#@#Vars.nek|CopyPathVar=CopyPath|CopyPath=#@#CopyFolder"]
```

- **MeasureType**: `ChooseFile` specifies the action to select a file.
- **FileType**: Specify file extensions (e.g., `.txt,.jpg,.png`).
- **VarName**: Variable name to store the file name.
- **VarPath**: Variable name to store the file path.
- **IniPath**: Path to the `.ini` file where variables will be written.
- **CopyPathVar**: Variable name to store the path of the copied file (optional).
- **CopyPath**: Folder where the file will be copied (optional).

---

### Choose a Folder
```ini
[!CommandMeasure MeasurePath "MeasureType=ChooseFolder|VarName=FolderName|VarPath=FolderPath|IniPath=#@#Vars.nek"]
```

- **MeasureType**: `ChooseFolder` specifies the action to select a folder.
- **VarName**: Variable name to store the folder name.
- **VarPath**: Variable name to store the folder path.
- **IniPath**: Path to the `.ini` file where variables will be written.

---

## Example Rainmeter Skin

```ini
[Rainmeter]
Update=1000

[MeasurePath]
Measure=Plugin
Plugin=ItemPath
OnCompleteAction=[!Log "[MeasurePath]"]
DynamicVariables=1

[Button]
Meter=String
Text=Choose a File 
W=400
H=40
SolidColor=47,47,47
FontSize=18
AntiAlias=1
DynamicVariables=1
LeftMouseUpAction=[!CommandMeasure MeasurePath "MeasureType=ChooseFile|FileType=.txt,.png|VarName=FileName|VarPath=FilePath|IniPath=#@#Vars.nek|CopyPathVar=CopyFilePath|CopyPath=#@#CopyFolder"]

[Button1]
Meter=String
Text=Choose a Folder
W=400
H=40
Y=5R
FontSize=18
SolidColor=47,47,47
AntiAlias=1
DynamicVariables=1
LeftMouseUpAction=[!CommandMeasure MeasurePath "MeasureType=ChooseFolder|VarName=FolderName|VarPath=FolderPath|IniPath=#@#Vars.nek"]

[Result]
Meter=String
Text=Result: [MeasurePath]
W=400
H=40
Y=5R
FontSize=18
SolidColor=47,47,47
AntiAlias=1
DynamicVariables=1

```

---

## Requirements

- **Rainmeter**: Latest version.
- **Platform**: Windows.

---

## Installation

1. Download the compiled `ItemPath.dll` from the [Releases](https://github.com/NSTechBytes/ItemPath/releases) page.
2. Place the `ItemPath.dll` in your Rainmeter `Plugins` folder.
3. Create or update your Rainmeter skin to include the plugin commands.

---

## License

This project is licensed under the Apache License. See the `LICENSE` file for details.

