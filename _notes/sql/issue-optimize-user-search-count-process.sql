/*
	Issue:
    When attempting to save user search session,
    There is no save of search record into database.
    
    Acceptance:
    - When a user completes a search, search details are saved to database
    - When user fetches history, results are returned to UI
    - Method to retrieve results should be optimized for speed
    - Optimize count of records by user-id per month
    - Optimize count of records by user-id per year
    Behaviors:
    - [x] user_usage_get_detail_mtd
    - [x] user_usage_get_detail_ytd
    - [x] user_usage_get_summary_mtd (resultset, DbUsageSummaryDto)
    - [x] user_usage_get_summary_ytd (resultset, DbUsageSummaryDto)
    
    - [x] user_usage_append_record (single, returns DbCountyAppendlimitDto)
    - [x] user_usage_complete_record (non-query, returns true)
    
    - [x] user_usage_get_monthly_limit (single, returns DbCountyUsagelimitDto)
    - [x] user_usage_set_monthly_limit (single, returns DbCountyUsagelimitDto)
	
	DTOs:
	- 	DbCountyAppendlimitDto
		Id
	- 	DbCountyUsagelimitDto
		Id, LeadUserId, CountyId, IsActive, MaxRecords, CompleteDate, CreateDate
	- 	DbCountyUsageRequestDto
		Id, LeadUserId, CountyId, CountyName, StartDate, EndDate, DateRange, RecordCount, CompleteDate, CreateDate
	- 	DbUsageSummaryDto
		Id, UserName, LeadUserId, SearchYear, SearchMonth, 
		LastSearchDate, CountyId, CountyName, MTD, MonthlyLimit
		
	Sproc:
	-	usp_user_usage_set_monthly_limit ( lead_index, county_id, month_limit ) returns DbCountyUsagelimitDto
	-	usp_user_usage_get_monthly_limit ( lead_index, county_id ) returns DbCountyUsagelimitDto
	-	usp_user_usage_append_record ( js_parameter ) returns DbCountyAppendlimitDto
	-	usp_user_usage_complete_record ( js_parameter ) non-query
	-	usp_user_usage_get_summary_ytd
    
*/
SELECT *
  FROM LEADUSERCOUNTYUSAGE;