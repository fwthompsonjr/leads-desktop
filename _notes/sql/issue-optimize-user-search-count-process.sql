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
    - [x] user_usage_get_detail_mtd (resultset, DbCountyUsageRequestDto)
    - [x] user_usage_get_detail_ytd (resultset, DbCountyUsageRequestDto)
    - [x] user_usage_get_summary_mtd (resultset, DbUsageSummaryDto)
    - [x] user_usage_get_summary_ytd (resultset, DbUsageSummaryDto)
    
    - [x] user_usage_append_record (single, returns DbCountyAppendLimitDto)
    - [x] user_usage_complete_record (non-query, returns true)
    
    - [x] user_usage_get_monthly_limit (single, returns DbCountyUsageLimitDto)
    - [x] user_usage_set_monthly_limit (single, returns DbCountyUsageLimitDto)
	
	DTOs:
	- 	DbCountyAppendLimitDto
		Id
	- 	DbCountyUsageLimitDto
		Id, LeadUserId, CountyId, IsActive, MaxRecords, CompleteDate, CreateDate
	- 	DbCountyUsageRequestDto
		Id, LeadUserId, CountyId, CountyName, StartDate, EndDate, DateRange, RecordCount, CompleteDate, CreateDate
	- 	DbUsageSummaryDto
		Id, UserName, LeadUserId, SearchYear, SearchMonth, 
		LastSearchDate, CountyId, CountyName, MTD, MonthlyLimit
		
	Sproc:
	-	USP_USER_USAGE_SET_MONTHLY_LIMIT ( lead_index, county_id, month_limit ) returns DbCountyUsagelimitDto
	-	USP_USER_USAGE_GET_MONTHLY_LIMIT ( lead_index, county_id ) returns DbCountyUsagelimitDto
	-	USP_USER_USAGE_GET_MONTHLY_LIMIT_ALL ( lead_index ) returns List<DbCountyUsagelimitDto>
	-	USP_USER_USAGE_APPEND_RECORD ( js_parameter ) returns DbCountyAppendlimitDto
	-	USP_USER_USAGE_COMPLETE_RECORD ( js_parameter ) non-query
	-	USP_USER_USAGE_GET_SUMMARY_YTD ( lead_index, search_date ) returns List<DbUsageSummaryDto>
	-	USP_USER_USAGE_GET_SUMMARY_MTD ( lead_index, search_date ) returns List<DbUsageSummaryDto>
	-	USP_USER_USAGE_GET_DETAIL_YTD ( lead_index, search_date ) returns List<DbCountyUsageRequestDto>
	-	USP_USER_USAGE_GET_DETAIL_MTD ( lead_index, search_date ) returns List<DbCountyUsageRequestDto>
    
*/
SELECT *
  FROM LEADUSERCOUNTYUSAGE;