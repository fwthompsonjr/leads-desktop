# Release | 2.8.15.0 - Process Redesign for Harris County

## Problem Statement:
As an application owner
I want the read process for Harris County Civil courts
to follow the same pattern as other common searches
So that Harris County is easy to maintain
And any changes to the target website can be easily addressed.

### Component Checks:
- Wrote new process to interact with Harris Civil read
- Added method to iterate through multiple pages when more than 200 records are available
- Added web page wait into each search component to reduce read error
- Performed backwards compatibility checks for Harris Criminal search