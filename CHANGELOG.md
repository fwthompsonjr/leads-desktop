# Release | 2.8.8.1 - Dallas case type sort + User interaction to track searches

## Problem Statement:

### Objective 1 - Dallas Case Type Filter
As an application
I want to filter case types in Dallas county
So that search finds the maximum amount of eligible cases.

### Objective 2 - User interaction to track searches
As an application
I want to track user search behavior
So that :
- I have a count of the number of searches performed
- I can enforce limits on monthly searches
- I can bill for records downloaded accurately

### Component Checks:

1. Dallas county end-to-end testing
2. Dallas county add filter for OPEN cases
3. Dallas county add logic for no-records found condition
4. Dallas county add logic to include weekends in searches
5. Search tracking - update behavior of search history screen
6. Search tracking - allow user to filter result by county
7. Search tracking - set fetch records to 1 year
8. Search limit - apply limit to record return result
9. Search limit - post records downloaded to max returned after filter is applied