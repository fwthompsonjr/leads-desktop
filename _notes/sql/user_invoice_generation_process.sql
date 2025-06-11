/*
	investigation of invoice creation process.
    for a non-admin user invoice create needs to attach cost
    Start with proc:
    USP_LEADUSER_GENERATE_INVOICE
*/
SET @leadId = '4411c3b7-a44d-11ef-99ce-0af7a01f52e9';
SET @emptId = '00000000-0000-0000-0000-000000000000';
/*
SELECT *
FROM DBCOUNTYINVOICE ii
  WHERE ii.LeadUserId = @leadId
    AND ii.CompleteDate = '2025-05-28 17:02:47'
    AND ii.Total < 0.02;
*/
SET @inv01 = '944b11ff-3be5-11f0-b422-0af36f7c981d';
SET @inv02 = '944cac46-3be5-11f0-b422-0af36f7c981d';

CALL USP_LEADUSER_ROLLBACK_INVOICE ( @inv02 );
/*
SELECT *
  FROM DBCOUNTYINVOICELINE 
  WHERE InvoiceId = @inv02; */