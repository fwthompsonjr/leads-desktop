/*
SELECT *
  FROM DBCOUNTYUSAGELIMIT
  ORDER BY CreateDate DESC;
*/
SET @mxRecords = -1;

DROP TEMPORARY TABLE IF EXISTS tmp_lead_list;
DROP TEMPORARY TABLE IF EXISTS tmp_lead_mxlimit;
DROP TEMPORARY TABLE IF EXISTS tmp_county_list;
CREATE TEMPORARY TABLE tmp_county_list
( Id int not null primary key,
  CountyName varchar(50) );
  
CREATE TEMPORARY TABLE tmp_lead_list
SELECT Id FROM LEADUSER;

insert tmp_county_list ( Id, CountyName )
values
	( 1, 'Denton' )
    , ( 10, 'Tarrant' )
    , ( 20, 'Collin' )
    , ( 30, 'Harris' )
    , ( 60, 'Dallas' )
    , ( 70, 'Travis' )
    , ( 80, 'Bexar' )
    , ( 90, 'Hidalgo' )
    , ( 100, 'El Paso' )
    , ( 110, 'Fort Bend' )
    , ( 120, 'Williamson' )
    , ( 130, 'Grayson' )
    , ( 0, 'Other' );
CREATE TEMPORARY TABLE tmp_lead_mxlimit
SELECT a.*
FROM
(
	SELECT	
	ll.Id LeadUserId, 
    cc.Id CountyId,
    TRUE IsActive,
    @mxRecords MaxRecords
	FROM tmp_lead_list ll
    CROSS JOIN ( SELECT * FROM tmp_county_list WHERE Id != 0 ) cc
) a
LEFT
JOIN 	DBCOUNTYUSAGELIMIT dst
ON 		a.LeadUserId = dst.LeadUserId
AND		a.CountyId = dst.CountyId
WHERE 	dst.Id IS NULL;

INSERT	DBCOUNTYUSAGELIMIT
(
	LeadUserId, CountyId, IsActive, MaxRecords
)    
SELECT 
	LeadUserId, CountyId, IsActive, MaxRecords
  FROM tmp_lead_mxlimit src
  ORDER BY LeadUserId, CountyId;