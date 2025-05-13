/*
Issue:
 Need to correct process that loads data into history
 Target proc = USP_OFFLINESEARCH_SYNC_HISTORY
*/
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
 WHERE 1 = 1
   AND oo.IsCompleted = TRUE
   AND oo.ExpectedRows > 0
   AND oo.PercentComplete > 95
 ORDER BY oo.CreateDate;