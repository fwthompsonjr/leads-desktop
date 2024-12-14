-- user_usage_append_record js_parameter
-- UserUsageAppendRecordModel
SET @js = '{ 
"lidx": "123-456-789",
"cid": 15,
"ctname": "Somewhere", 
"sdte": "2024-5-5", 
"edte": "2024-5-15", 
"rc": 0 }';
DROP TEMPORARY TABLE IF EXISTS tmp_insert_usage;
CREATE TEMPORARY TABLE tmp_insert_usage
SELECT
	JSON_VALUE( @js, '$.lidx') LeadUserId
	, CONVERT( JSON_VALUE( @js, '$.cid'), SIGNED ) CountyId
	, JSON_VALUE( @js, '$.ctname') CountyName
	, CAST( JSON_VALUE( @js, '$.sdte') AS DATE ) StartDate
	, CAST( JSON_VALUE( @js, '$.edte') AS DATE ) EndDate
	, JSON_VALUE( @js, '$.drange') DateRange
	, CONVERT( JSON_VALUE( @js, '$.rc'), SIGNED ) RecordCount;
   
SET @id = (SELECT Id FROM LEADUSER WHERE Id = (SELECT LeadUserId FROM tmp_insert_usage));   
-- perform insert
INSERT DBCOUNTYUSAGEREQUEST
(
	LeadUserId, CountyId, CountyName, StartDate, EndDate, DateRange, RecordCount
)
SELECT
LeadUserId, CountyId, CountyName, StartDate, EndDate, DateRange, RecordCount
FROM tmp_insert_usage
WHERE @id is not null;

-- return record index
SELECT Id
  FROM DBCOUNTYUSAGEREQUEST r
  JOIN tmp_insert_usage t
    ON r.LeadUserId = t.LeadUserId
  WHERE r.CompleteDate IS NULL
  ORDER BY r.CreateDate DESC
  LIMIT 1;
  