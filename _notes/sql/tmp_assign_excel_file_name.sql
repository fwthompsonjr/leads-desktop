use appdb;
-- check ability to fetch record by id
/*
	CALL USP_LEADUSER_EXL_GET_FILENAME_BY_ID
  cd2f354f-bb59-11ef-99ce-0af7a01f52e9
set @rqid = 'cd2f354f-bb59-11ef-99ce-0af7a01f52e9';
set @fname = 'Bexar_COUNTY_241205_241205_0004.xlsx';
CALL USP_LEADUSER_EXL_GET_FILENAME_BY_ID( @rqid );

SELECT *
  FROM DBCOUNTYINVOICE
  WHERE RequestId = @rqid;
*/
set @rqid = 'cd2f354f-bb59-11ef-99ce-0af7a01f52e9';
  


SELECT r.*, fn.ShortFileName, fn.CompleteDate FileCompletedDate
  FROM DBCOUNTYUSAGEREQUEST r
  LEFT
  JOIN DBCOUNTYFILENAME fn
    ON r.Id = fn.RequestId
  WHERE 1 = 1
  AND   r.Id = @rqid;