-- user_usage_get_detail_mtd
-- inbound parameters
SET @lead_index = '';
SET @search_date = CAST( '2024-12-12' AS DATE);
-- local parameters
SET @search_month = MONTH( @search_date );
SET @search_year  = YEAR( @search_date );

DROP TEMPORARY TABLE IF EXISTS tmp_mtd_limit;
CREATE TEMPORARY TABLE tmp_mtd_limit
SELECT 		CountyId, MIN( MaxRecords ) MonthlyLimit
    FROM 	DBCOUNTYUSAGELIMIT
    WHERE 	LeadUserId = @lead_index
      AND 	IsActive = TRUE
      AND 	CompleteDate IS NOT NULL
	GROUP BY CountyId;

SET @user_name = (SELECT UserName FROM LEADUSER WHERE Id = @lead_index);
SET @user_name = CASE WHEN @user_name IS NULL THEN 'N/A' ELSE @user_name END;
-- query 
SELECT 
@user_name AS `UserName`,
@search_year AS `SearchYear`,
@search_month AS `SearchMonth`,
CASE 
	WHEN mx.MonthlyLimit IS NULL THEN 15000
    ELSE mx.MonthlyLimit END MonthlyLimit,
r.*
FROM DBCOUNTYUSAGEREQUEST r
LEFT JOIN tmp_mtd_limit mx
 ON r.CountyId = mx.CountyId
WHERE LeadUserId = @lead_index
AND MONTH( CreateDate ) = @search_month
AND YEAR( CreateDate ) = @search_year
ORDER BY
r.CreateDate;
