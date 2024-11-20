# Release | 2.8.6 Setup remote authenication and county management

## Problem:
			
When a user runs application ,
I want to impose security to read operations ,
so that no application functions can be used without rights.,
,
## Resolution:,
- [x] Check user can login,
- [x] Check user county permissions are setup by county,
- [x] Check user can only view counties where they have permission,
- [x] Check user can change password,
- [x] Check user can change county password

### Testing

Execution of backwards compatibility tests, as well as api tests to validate remote access.

### Component Checks
- Check behavior [Dallas]
- Check behavior [Denton]
- Check behavior [Tarrant]
- Check behavior [Travis]
- Check behavior [Bexar]
- Check behavior [Hidalgo]
- Check behavior [ElPaso]
- Check behavior [FortBend]
- Check behavior [Williamson]