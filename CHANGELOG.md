# Release | 2.8.12.0 - Lead UAT Process Reviews

## Problem Statement:
## User Acceptance Feedback Items
1. When attempting to execute tarrant county search
The process freezes with error `Value cannot be null. (Parameter 'source')`
2. Collin county search is disabled.
3. Denton pulls records but no open document button.
4. Harris does not pull civil or JP.

### Component Checks:
## Testing and Corrections
- **Tarrant County**: Changed process to use `Invoke` to allow UI thread updates
Removed interaction with legacy caseList.xml file which is deprecated.
- **Collin county**: Issue was specific to user account and a database update unlocked account.
- **Denton county**: The user has 3 alternate methods to open Excel files:
  - From Top Left Menu (Open). New Form to list and open files
  - From Bottom Right Menu (Recent Files List)
  - From Top Left Menu (File -> View -> Recent)
  - Denton is special case where the [Settings] button is needed
- **Harris County**: Unable to reproduce issue checked bot civil and JP
There is a backlog item to change the output file name for Harris Criminal
So that the filenames are different and easy to understand without opening the file.