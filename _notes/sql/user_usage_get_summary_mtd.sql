-- user_usage_get_summary_mtd
-- inbound parameters
SET @lead_index = '';
SET @search_date = CAST('2024-12-12' AS DATE);
-- local parameters
SET @search_month = MONTH( @search_date );
SET @search_year  = YEAR( @search_date );
-- query
SELECT 
r.*
FROM VWUSAGEHISTORY r
WHERE LeadUserId = @lead_index
AND SearchMonth = @search_month
AND SearchYear = @search_year
ORDER BY
r.LastSearchDate;
