/*
database record clean update
1. within the search results for dallas county
   the items stored in database are incomplete
   because the search routine had logic errors
   that preventing complete capture of all dates within the search parameters
   
2. remove any duplicate records within the time-span
3. create ui for admin.
	allow view/approve data by date

*/


DROP temporary table if exists tmpHistory;
CREATE temporary table tmpHistory
SELECT Id, SearchDate
  FROM DBSEARCHHISTORY h
  WHERE h.CountyId = 60
  AND CreateDate >= '2024-12-12 20:54:25'
  ORDER BY CreateDate DESC;
  
SELECT hr.*
  FROM DBSEARCHHISTORYRESULT hr 
  JOIN tmpHistory h
    ON hr.SearchHistoryId = h.Id
WHERE hr.Zip = '00000'
ORDER BY hr.DateFiled, hr.Court, hr.`Name`;