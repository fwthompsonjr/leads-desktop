# Release | 2.8.11.0 - Legal Lead - Dallas County - Revisions

## Problem Statement:
1. Dallas JP
When searching JP courts
Court look up is experiencing timeouts
Which causes some courts to be excluded from results.
Added process to ensure proper mapping of judicial officer.
Added loop to iterate all courts until records are ensured.
2. Dallas District
When searching distict courts
Court lookup is excluding cases
Removed court iteration to pull all while still filtering by single date.
3. Dallas Read Details
When reading details page from website
Page timeouts are causing the read to fail
Added retry loop to ensure that all expected cases are fetched

### Component Checks:
- Dallas Justice
Tested ability to fetch all courts
Tested sequence of page read
- Dallas District
Tested ability to fetch all records
Tested sequence of page read
- Dallas County
Tested ability to fetch all courts
Tested sequence of page read