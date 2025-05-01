
/*
	i want to ensure that offline request data is uploaded to table
		DBSEARCHHISTORY
		DBSEARCHHISTORYRESULT	
        
Update API
 - add ability to call procedures:
	CALL USP_OFFLINE_SET_SEARCH_TYPE_INTERNAL(); 
		-- this is needed before get by user-id query
		-- this is needed before calling sync history prc below
	CALL USP_OFFLINESEARCH_SYNC_HISTORY();
		-- this is needed after a search is flagged as completed

*/

CALL USP_OFFLINE_SET_SEARCH_TYPE_INTERNAL();

DROP TEMPORARY TABLE IF EXISTS tm_work;
CREATE TEMPORARY TABLE tm_work
SELECT 
	Row_Number() OVER (ORDER BY oo.CreateDate) RwId,
    oo.*, 
    js.Message, 
    CASE 
    WHEN ct.Message = 'COUNTY' THEN 0
    WHEN ct.Message = 'DISTRICT' THEN 1
    WHEN ct.Message = 'JUSTICE' THEN 2
    ELSE 0 END CaseTypeId
  FROM OFFLINESEARCH oo
  JOIN ( SELECT OfflineId, Message FROM OFFLINESEARCHWORKLOAD WHERE LineId = 1) js
    ON oo.Id = js.OfflineId
  LEFT
  JOIN ( SELECT OfflineId, Message FROM OFFLINESEARCHWORKLOAD WHERE LineId = 3) ct
    ON oo.Id = ct.OfflineId
  LEFT
  JOIN ( SELECT OfflineId, Message FROM OFFLINESEARCHWORKLOAD WHERE LineId = 4) rr
    ON oo.Id = rr.OfflineId
 WHERE rr.Id IS NULL
   AND oo.IsCompleted = TRUE
   AND oo.ExpectedRows > 0
 ORDER BY oo.CreateDate;

SET @rwId = 1;
SET @js = ( SELECT Message FROM tm_work WHERE RwId = @rwId);
SET @pipe = '|';
SET @spc = ' ';
SET @zero = '00000';

DROP TEMPORARY TABLE IF EXISTS tm_case_header;
DROP TEMPORARY TABLE IF EXISTS tm_case_data;
DROP TEMPORARY TABLE IF EXISTS tm_case_filing_dates;
DROP TEMPORARY TABLE IF EXISTS tm_search_history_insert; -- DBSEARCHHISTORY

CREATE TEMPORARY TABLE tm_case_data
SELECT
q.`Name`,
q.Zip, 
q.Address1, 
CASE
	WHEN q.Zip = @zero THEN Address2
	WHEN LENGTH(q.Address3) - LENGTH(REPLACE(q.Address3, @spc, '')) > 4
    THEN 
    TRIM(REPLACE( q.Address3, CONCAT_WS(@spc, SUBSTRING_INDEX(q.Address3, @spc, -3)), ''))
    ELSE Address2 END Address2, 
CASE 
	WHEN q.Zip = @zero THEN Address3
	WHEN LENGTH(q.Address3) - LENGTH(REPLACE(q.Address3, @spc, '')) > 4
	THEN TRIM(CONCAT_WS(@spc, SUBSTRING_INDEX(q.Address3, @spc, -3)))
	ELSE Address3 END Address3, 
CaseNumber, 
DateFiled, 
Court, 
CaseType, 
CaseStyle, 
Plaintiff
FROM
(
SELECT 
jt.`Name`, 
jt.Address,
CASE 
	WHEN jt.Address Is Null OR LENGTH(jt.Address) = 0 THEN @zero 
    ELSE SUBSTRING_INDEX( SUBSTRING_INDEX(jt.Address, @pipe, -1), @spc, -1 )
    END Zip, 
CASE 
	WHEN jt.Address Is Null OR LENGTH(jt.Address) = 0 THEN ''
    WHEN INSTR( jt.Address, @pipe) < 0 THEN ''
    ELSE SUBSTRING_INDEX(jt.Address, @pipe, 1) END Address1, 
'' Address2,
	CASE
	WHEN jt.Address Is Null OR LENGTH(jt.Address) = 0 THEN ''
    WHEN INSTR( jt.Address, @pipe) < 0 THEN ''
    ELSE TRIM(REPLACE(REPLACE( jt.Address, SUBSTRING_INDEX(jt.Address, @pipe, 1), ''), @pipe, ' '))  END Address3, 
CaseNumber, 
DATE_FORMAT( STR_TO_DATE(FiledDate, '%m/%d/%Y'), '%m/%d/%Y') DateFiled, 
Court, 
CaseType, 
CaseStyle, 
Plaintiff
  FROM JSON_table (@js, "$[*]" COLUMNS(
	`Name` varchar(200) PATH '$.partyname',
	Zip varchar(20) PATH '$.courtDate',
	Address varchar(500) PATH '$.Address',
	CaseNumber varchar(50) PATH '$.caseNumber',
	FiledDate varchar(25) PATH '$.filedate',
	Court varchar(25) PATH '$.location',
	CaseType varchar(50) PATH '$.casetype',
	CaseStyle varchar(500) PATH '$.caseStyle',
	Plaintiff varchar(200) PATH '$.Plaintiff'
  ))jt
  ) q;
 

CREATE TEMPORARY TABLE tm_case_filing_dates
	SELECT 
		row_number() OVER (ORDER BY STR_TO_DATE(DateFiled, '%m/%d/%Y')) RwId,
        DateFiled
	FROM ( SELECT DateFiled FROM tm_case_data ) tm;
SET @dateId = 1;
SET @searchId = (SELECT 
	RequestId
FROM tm_work WHERE RwId = @rwId LIMIT 1);

SET @searchTypeIndex = (SELECT CaseTypeId
FROM tm_work WHERE RwId = @rwId LIMIT 1);
SET @searchDt = (SELECT STR_TO_DATE(DateFiled, '%m/%d/%Y') DateFiled FROM tm_case_filing_dates WHERE RwId = @dateId);
SET @db_searchhistory_id = (
SELECT h.Id
FROM
DBSEARCHHISTORY h
JOIN (SELECT Id, CountyId FROM DBCOUNTYUSAGEREQUEST WHERE Id = @searchId) a
  WHERE a.CountyId = h.CountyId
  AND @searchDt = h.SearchDate
  AND @searchTypeIndex = h.SearchTypeId
  AND 0 = h.CaseTypeId
  AND 0 = h.DistrictCourtId
  AND 0 = h.DistrictSearchTypeId
ORDER BY CreateDate DESC
LIMIT 1);

SET @db_searchhistory_id = CASE WHEN @db_searchhistory_id IS NULL THEN UUID() ELSE @db_searchhistory_id END;

INSERT DBSEARCHHISTORY
(
	Id, CountyId, SearchDate, SearchTypeId, CaseTypeId, DistrictCourtId, DistrictSearchTypeId, CreateDate, CompleteDate
)
SELECT 
  @db_searchhistory_id Id,
  a.CountyId, 
  @searchDt SearchDate, 
  @searchTypeIndex SearchTypeId, 
  0 CaseTypeId, 
  0 DistrictCourtId, 
  0 DistrictSearchTypeId, 
  a.CreateDate, 
  a.CompleteDate	
FROM DBCOUNTYUSAGEREQUEST a
LEFT
JOIN DBSEARCHHISTORY h
  ON @db_searchhistory_id = h.Id
  WHERE a.Id = @searchId
    AND h.Id IS NULL
LIMIT 1; 

CREATE TEMPORARY TABLE tm_search_history_insert
SELECT 
dd.SearchHistoryId, 
dd.`Name`, 
dd.Zip, 
dd.Address1, 
dd.Address2, 
dd.Address3, 
dd.CaseNumber, 
dd.DateFiled, 
dd.Court, 
dd.CaseType, 
dd.CaseStyle, 
dd.Plaintiff
FROM (
	SELECT 
	@db_searchhistory_id SearchHistoryId,
    `Name`, Zip, Address1, Address2, Address3, CaseNumber, DateFiled, Court, CaseType, CaseStyle, Plaintiff
    FROM tm_case_data d
    WHERE DateFiled = @searchDt ) dd
LEFT JOIN DBSEARCHHISTORYRESULT rr
  ON dd.CaseNumber = rr.CaseNumber
  AND dd.DateFiled = rr.DateFiled
WHERE rr.Id IS NULL
ORDER BY CASE WHEN dd.Zip = @zero THEN 'zzzzz' ELSE dd.CaseNumber END, dd.CaseNumber;

UPDATE DBSEARCHHISTORYRESULT rr
JOIN tm_search_history_insert dd
  ON dd.CaseNumber = rr.CaseNumber
  AND dd.DateFiled = rr.DateFiled
  AND dd.SearchHistoryId = rr.SearchHistoryId
  SET 
  rr.`Name` = dd.`Name`, 
  rr.Zip = dd.Zip, 
  rr.Address1 = dd.Address1, 
  rr.Address2 = dd.Address2, 
  rr.Address3 = dd.Address3,
  rr.Court = dd.Court,
  rr.CaseType = dd.CaseType,
  rr.CaseStyle = dd.CaseStyle,
  rr.Plaintiff = dd.Plaintiff
  WHERE 
  rr.Zip = @zero
  AND dd.Zip != @zero
  AND rr.Id != '';
  
  INSERT DBSEARCHHISTORYRESULT
  (
	SearchHistoryId, `Name`, Zip, Address1, Address2, Address3, 
    CaseNumber, DateFiled, Court, CaseType, CaseStyle, Plaintiff
  )
  SELECT
	dd.SearchHistoryId, dd.`Name`, dd.Zip, dd.Address1, dd.Address2, dd.Address3, 
    dd.CaseNumber, dd.DateFiled, dd.Court, dd.CaseType, dd.CaseStyle, dd.Plaintiff
  FROM tm_search_history_insert dd
LEFT JOIN DBSEARCHHISTORYRESULT rr
  ON dd.CaseNumber = rr.CaseNumber
  AND dd.DateFiled = rr.DateFiled
  AND dd.SearchHistoryId = rr.SearchHistoryId
WHERE rr.Id IS NULL;