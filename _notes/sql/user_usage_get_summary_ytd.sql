-- user_usage_get_summary_ytd
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
AND SearchYear = @search_year
ORDER BY
r.LastSearch;
