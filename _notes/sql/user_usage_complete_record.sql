-- user_usage_complete_record
SET @js = '{ 
"idx": "123-456-789",
"rc": 0 }';
DROP TEMPORARY TABLE IF EXISTS tmp_update_usage;
CREATE TEMPORARY TABLE tmp_update_usage
SELECT
	JSON_VALUE( @js, '$.idx') Id
	, CONVERT( JSON_VALUE( @js, '$.rc'), SIGNED )RecordCount;
    
-- perform update
UPDATE DBCOUNTYUSAGEREQUEST D
JOIN tmp_update_usage T ON D.Id = T.Id
  SET
  RecordCount = T.RecordCount,
  CompleteDate = utc_timestamp()
WHERE 
D.Id = T.Id
AND D.CompleteDate IS NULL;
DROP TEMPORARY TABLE IF EXISTS tmp_update_usage;
