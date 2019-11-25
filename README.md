# GameDataManagementTools
An editor tool converts ScriptableObject to Excel and Json.
Supports:
- Import: Load the ScriptableObject from Excel and Json. Also support loading from online google sheet.
- Export: Save the ScriptableObject to Excel and Json. **DO NOT** suppport saving to online google sheet.

## How to use
- Import the package `GameDataManagementTools.package`
- Install the excel library `EPPlus` in your project
- Try it!
  - Change the `GoogleSheetId` in `GameConstant`
    - It is in the web address `"https://docs.google.com/spreadsheets/d/[THE-SHEET-ID]/edit"`
  - Try the example data classes `ExampleDataTable` and `ExampleData`
    - The example asset is in `/GameDataManagementTools/Assets/`
    - The example excel and json files are in `/GameDataManagementTools/GameData/`
  - Derive your own game data classes from the base classes: `GameDataBase`, `GameDataTableBase`, `GameDataTableBaseEditor`
