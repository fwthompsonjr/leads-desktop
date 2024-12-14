-- delete previous searches for county
SET @county_index = 60;
DROP TEMPORARY TABLE IF EXISTS tmpindexes;
CREATE TEMPORARY TABLE tmpindexes
SELECT Id
  FROM DBSEARCHHISTORY
  WHERE CountyId = @county_index;
  
DELETE 	R
  FROM	DBSEARCHHISTORYRESULT R 
  JOIN	tmpindexes t
    ON	R.SearchHistoryId = t.Id
WHERE 	R.Id != '';

DELETE 	R
  FROM	DBSEARCHHISTORY R 
  JOIN	tmpindexes t
    ON	R.Id = t.Id;