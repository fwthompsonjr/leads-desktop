# Release | 2.8.10.0 - Dallas county max record search revision

## Problem Statement:
As a user,  
I want to retrieve all available records for a given court  
So that I have the most complete set of records for marketing

### New Behaviors:
1. *Dallas County*: Alter method to set max rows
1. *Dallas County*: Add method to wait for table populated before fetching case detail
1. *Dallas County*: Add setting to exclude weekends from search
1. *Dallas County*: Add retry logic with pause when fetching address details
1. *Interface*: Added top level menu navigation
1. *Interface*: Removed pop-up window when search completes
1. *Interface*: Added Button to Open Excel after search completes

### Component Checks:
1. Test search dallas county - county courts - single date
1. Test search dallas county - county courts - multiple dates
1. Test search dallas county - county courts - include weekends
1. Test search dallas county - district courts - single date
1. Test search dallas county - district courts - multiple dates
1. Test search dallas county - district courts - include weekends
1. Test search dallas county - justice courts - single date
1. Test search dallas county - justice courts - multiple dates
1. Test search dallas county - justice courts - include weekends