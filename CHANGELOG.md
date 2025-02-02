# Release | 2.8.10.0 - Dallas county max record search revision

## Problem Statement:
As a user,
I want to retrieve all available records for a given court
So that I have the most complete set of records for marketing

### Component Checks:
1. Dallas County: Alter method to set max rows
1. Dallas County: Add method to wait for table populated before fetching case detail
1. Dallas County: Add setting to exclude weekends from search
1. Dallas County: Add retry logic with pause when fetching address details