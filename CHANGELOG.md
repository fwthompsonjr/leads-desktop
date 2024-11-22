# Release | 2.8.7 Setup user friendly context menus

## Problem:
			
When a user runs application,
I want to allow user to view and edit user specific content,
so that application functions are clearly visible to consumer.

## Search Process
- Feature: add driver hide to county searches
- BugFix: refactor driver read headless property
- BugFix: correcting console out to remove full exception detail

## User Settings
- Feature: add menu options to tool strip button
- Feature: created service to read user settings
- Feature: bind user settings
- BugFix: ui enable/disable settings per account
- BugFix: integrate user session settings to search processes

## Code Management
- BugFix: code refactor
- BugFix: code analysis remove warnings
- BugFix: code cleanup

## Deployment Management
- Feature: change build pipeline
- Feature: modification to release script
- Feature: process improvement for release creation
- Test: release generation process

### Testing

Execution of backwards compatibility tests, as well as api tests to validate remote access.

### Component Checks
- Check behavior [Dallas]
- Check behavior [Bexar]