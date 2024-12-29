drop temporary table if exists tmp_county_prc;

create temporary table tmp_county_prc
SELECT
	cidx.CountyId,
    rr.CountyName,
    TRUE IsActive,
    CAST( 0.40 as DECIMAL(9,2)) PerRecord,
    CAST( NULL as DATETIME ) CompleteDate,
    utc_timestamp() CreateDate
FROM
(
	SELECT
	CountyId, MAX( Id ) Id
    FROM	DBCOUNTYUSAGEREQUEST r
    GROUP BY CountyId
) cidx
LEFT JOIN 	DBCOUNTYUSAGEREQUEST rr
  ON		cidx.Id = rr.Id
LEFT JOIN	DBCOUNTYPRICING prc
  ON		cidx.CountyId = prc.CountyId
WHERE		prc.Id is null
ORDER BY	cidx.CountyId ASC;

insert DBCOUNTYPRICING
(
CountyId, CountyName, IsActive, PerRecord, CompleteDate, CreateDate
)
SELECT CountyId, CountyName, IsActive, PerRecord, CompleteDate, CreateDate
  FROM tmp_county_prc
  ORDER BY CountyId;