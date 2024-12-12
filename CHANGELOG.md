# Release | 2.8.7.x Dallas county justice officer search

## Problem:
			
When performing search in Dallas county, 
loop through each court judicial officer on per date basis 
so that maximum record limit of 200 is avoided  
in the search process.

## Search Process
- admin has control of user settings
- database storage of previous searches has been integrated
- process should report percent completion during execution
- additional progress messages added to search process

### Testing

Execution of backwards compatibility tests, 
as well as api tests to validate remote access.
1. _2.8.7.4_ - Dallas county process optimization

### Component Checks
- Check behavior [Bexar]
- Check behavior [Collin]
- Check behavior [Dallas]
- Check behavior [Denton]
- Check behavior [ElPaso]
- Check behavior [FortBend]
- Check behavior [Harris]
- Check behavior [Harris - Criminal]
- Check behavior [Tarrant]