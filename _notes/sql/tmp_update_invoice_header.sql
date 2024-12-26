
SET @js_query = '[
{
 "id": "abc-def-ghi",  
 "invoiceno": "", 
 "uri": "",
 "linenbr": ""
}]';
drop temporary table if exists tm_invoice_update;
create temporary table tm_invoice_update
SELECT *
  FROM json_table( @js_query, '$[*]' 
  columns( 
	Id VARCHAR(50) PATH '$.id',
	InvoiceNbr VARCHAR(75) PATH '$.invoiceno',
	InvoiceUri VARCHAR(500) PATH '$.uri',
	LineNbr INT PATH '$.linenbr',
	CompleteDate DATETIME PATH '$.completeDt')
) jt
LIMIT 1;

SELECT *
  FROM tm_invoice_update;