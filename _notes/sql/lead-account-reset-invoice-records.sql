SET @leadAcct = 'lead.administrator';
SET @leadId = (
SELECT ld.Id
  FROM LEADUSER ld
  INNER JOIN USERS u ON ld.Id = u.Id
  WHERE ld.UserName = @leadAcct
  LIMIT 1);