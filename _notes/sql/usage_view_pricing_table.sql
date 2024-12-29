/*

SET @countid = 7;
SET @rsp = CAST( 0.00 as DECIMAL(9,2));
CALL USP_LEADUSER_GET_PRICE_PER_RECORD ( @countid,  @rsp );

SELECT @rsp

*/



SELECT
 *
 FROM DBCOUNTYPRICING
 WHERE IsActive = TRUE ;