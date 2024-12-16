/*
	convert existing records
*/

SET @lead_index = '4411c3b7-a44d-11ef-99ce-0af7a01f52e9';
SET @date_none = '1900-01-01';
SET @allow_insert = 0;
DROP TEMPORARY TABLE IF EXISTS tmp_usr_searches;
DROP TEMPORARY TABLE IF EXISTS tmp_usr_searches_upload;
CREATE TEMPORARY TABLE tmp_usr_searches
SELECT 
	q.*,
    CASE 
    WHEN CountyName = 'Denton'		THEN 1
    WHEN CountyName = 'Tarrant'		THEN 10
    WHEN CountyName = 'Collin'		THEN 20
    WHEN CountyName = 'Harris'		THEN 30
    WHEN CountyName = 'Dallas'		THEN 60
    WHEN CountyName = 'Travis'		THEN 70
    WHEN CountyName = 'Bexar'		THEN 80
    WHEN CountyName = 'Hidalgo' 	THEN 90
    WHEN CountyName IN ('ElPaso', 'El Paso' )		THEN 100
    WHEN CountyName IN ('FortBend', 'Fort Bend' )	THEN 110
    WHEN CountyName = 'Williamson'	THEN 120
    WHEN CountyName = 'Grayson'		THEN 130
    ELSE 0 END CountyId,
    CASE 
    WHEN CountyName = 'Denton'		THEN 'Denton'
    WHEN CountyName = 'Tarrant'		THEN 'Tarrant'
    WHEN CountyName = 'Collin'		THEN 'Collin'
    WHEN CountyName = 'Harris'		THEN 'Harris'
    WHEN CountyName = 'Dallas'		THEN 'Dallas'
    WHEN CountyName = 'Travis'		THEN 'Travis'
    WHEN CountyName = 'Bexar'		THEN 'Bexar'
    WHEN CountyName = 'Hidalgo' 	THEN 'Hidalgo'
    WHEN CountyName IN ('ElPaso', 'El Paso' )		THEN 'El Paso'
    WHEN CountyName IN ('FortBend', 'Fort Bend' )	THEN 'Fort Bend'
    WHEN CountyName = 'Williamson'	THEN 'Williamson'
    WHEN CountyName = 'Grayson'		THEN 'Grayson'
    ELSE CountyName END MappedName

FROM (
SELECT LeadUserId, CountyName, LeadUserCountyId, COUNT(1) Searches
  FROM LEADUSERCOUNTYUSAGE
  WHERE LeadUserId = @lead_index
  GROUP BY LeadUserId, CountyName, LeadUserCountyId
  ) q
order by CountyName;

CREATE TEMPORARY TABLE tmp_usr_searches_upload
SELECT 
	sq.LeadUserId,
    sq.MappedName CountyName,
    sq.CountyId,
    sq.MonthlyUsage RecordCount,
    sq.DateRange,
    CASE WHEN HasQuestions != 0 THEN @date_none
    WHEN HasAnswer = 0 THEN @date_none
    ELSE STR_TO_DATE( TRIM(LEFT(DateRange, HasAnswer)) , "%m/%d/%y") END StartDate,
    CASE WHEN HasQuestions != 0 THEN @date_none
    WHEN HasAnswer = 0 THEN @date_none
    ELSE STR_TO_DATE( REPLACE(TRIM((right(DateRange, 10))), 'o', '') , "%m/%d/%y") END EndDate,
    sq.CompleteDate,
    sq.CreateDate
FROM (
SELECT 
	t.LeadUserId,
    t.MappedName,
    t.CountyId,
    u.MonthlyUsage,
    u.DateRange,
    u.CreateDate,
    ADDDATE( u.CreateDate, INTERVAL 10 MINUTE) CompleteDate,
    (SELECT POSITION("?" IN u.DateRange)) HasQuestions,
    (SELECT POSITION(" to " IN u.DateRange)) HasAnswer
  FROM tmp_usr_searches t 
  INNER JOIN LEADUSERCOUNTYUSAGE u 
  ON 	t.LeadUserId = u.LeadUserId
  AND 	t.CountyName = u.CountyName ) sq;
SET SQL_SAFE_UPDATES = 0;
UPDATE tmp_usr_searches_upload
SET DateRange = '? to ?'
WHERE StartDate = @date_none
AND LeadUserId != '';
SET SQL_SAFE_UPDATES = 1;

SELECT *
FROM tmp_usr_searches_upload
WHERE LeadUserId != '';

INSERT DBCOUNTYUSAGEREQUEST
(
	LeadUserId, CountyId, CountyName, StartDate, EndDate, DateRange, RecordCount, CompleteDate, CreateDate
)
SELECT LeadUserId, CountyId, CountyName, StartDate, EndDate, DateRange, RecordCount, CompleteDate, CreateDate
  FROM tmp_usr_searches_upload
  WHERE @allow_insert = 1;