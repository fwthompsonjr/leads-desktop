<#
	rollback any pending changes in repository
#>

git reset
git checkout .
git clean -fdx