/*
	DBSEARCHHISTORY
    Id, CountyId
    CREATE TEMPORARY TABLE tmp_invoice_needed (
    TempId int NOT NULL AUTO_INCREMENT,
    SearchId varchar(50) NOT NULL,
    CountyId int,
    PRIMARY KEY (TempId)
);

*/

SELECT
sss.Id,
sss.LeadUserId,
sss.CountyId,
CASE WHEN sss.MaxRecords = -1 THEN TRUE ELSE FALSE END IsAdmin,
(SELECT RecordCount FROM DBCOUNTYUSAGEREQUEST WHERE Id = sss.Id) RecordCount,
sss.CreateDate
FROM
(

SELECT 
ssq.Id,
ssq.CountyId,
ssq.LeadUserId,
( 	SELECT MaxRecords 
	FROM DBCOUNTYUSAGELIMIT 
    WHERE IsActive = TRUE 
    AND LeadUserId = ssq.LeadUserId
    AND CountyId = ssq.CountyId
    ORDER BY CreateDate DESC LIMIT 1) MaxRecords,
CreateDate
FROM
(
SELECT 
Id, 
CountyId, 
MAX( LeadUserId ) LeadUserId,
MAX( CreateDate ) CreateDate
FROM (
	SELECT s.Id, s.CountyId, s.LeadUserId, s.CreateDate
	FROM DBCOUNTYUSAGEREQUEST s
	LEFT JOIN DBCOUNTYINVOICE ci
	ON	s.Id = ci.RequestId
	AND s.LeadUserId = ci.LeadUserId
	WHERE ci.Id is null
  AND s.CompleteDate is not null ) sq
GROUP BY Id, CountyId
) ssq
) sss
ORDER BY CreateDate ASC;