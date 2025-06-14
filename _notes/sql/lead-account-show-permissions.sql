/*
	user permissions
	1. make certain that each lead account has permissions established
    2. add a user permission for billing method ( TEST, PROD )
*/
SET @iOrderId = (SELECT MAX(OrderId) FROM PERMISSIONMAP ) + 1;
SET @uBillingMethod = 'User.Billing.Type';
SET @testUid = '4411c3b7-a44d-11ef-99ce-0af7a01f52e9';
SET @uidBilling = CONVERT( UUID() , CHAR(36));
SET @bHasRecord = EXISTS( SELECT 1 FROM PERMISSIONMAP WHERE KeyName = @uBillingMethod );

SELECT 
	v.id Id, 
    v.orderid OrderId,
    v.userid UserId, 
    v.permissionmapid PermissionMapId, 
    v.keyname KeyName, 
    v.keyvalue KeyValue
  FROM VWUSERPERMISSION v
  WHERE v.userid = @testUid
    AND v.keyname = @uBillingMethod;


/*
INSERT PERMISSIONMAP ( Id, OrderId, KeyName )
SELECT 
	@uidBilling Id,
    @iOrderId OrderId, 
    @uBillingMethod BillingMethod
FROM DUAL
WHERE @bHasRecord = FALSE;

DROP TEMPORARY TABLE IF EXISTS tmp_lead_permission;


CREATE TEMPORARY TABLE tmp_lead_permission
SELECT q.UserId, q.PermissionMapId, q.KeyName, q.KeyValue
FROM
(
SELECT  t.UserId, pm.Id PermissionMapId, pm.KeyName, 
CASE
WHEN pm.KeyName = 'Account.IsPrimary' THEN 'True'
WHEN pm.KeyName = 'Account.Permission.Level' THEN 'Guest'
WHEN pm.KeyName = 'Account.Linked.Accounts' THEN ''
WHEN pm.KeyName = 'Setting.MaxRecords.Per.Year' THEN '50'
WHEN pm.KeyName = 'Setting.MaxRecords.Per.Month' THEN '15'
WHEN pm.KeyName = 'Setting.MaxRecords.Per.Request' THEN '5'
WHEN pm.KeyName = 'Setting.Pricing.Name' THEN 'Guest'
WHEN pm.KeyName = 'Setting.Pricing.Per.Year' THEN '0'
WHEN pm.KeyName = 'Setting.Pricing.Per.Month' THEN '0'
WHEN pm.KeyName = 'Setting.Pricing.Per.Request' THEN '0'
WHEN pm.KeyName = 'Setting.State.Subscriptions' THEN ''
WHEN pm.KeyName = 'Setting.State.County.Subscriptions' THEN ''
WHEN pm.KeyName = 'Setting.State.Subscriptions.Active' THEN ''
WHEN pm.KeyName = 'Setting.State.County.Subscriptions.Active' THEN ''
WHEN pm.KeyName = 'User.State.Discount' THEN ''
WHEN pm.KeyName = 'User.State.County.Discount' THEN ''
WHEN pm.KeyName = 'User.Billing.Type' THEN 'Test'
ELSE '' END KeyValue, 
pm.OrderId
FROM PERMISSIONMAP pm
CROSS JOIN ( SELECT Id UserId FROM tmp_lead_accts ) t
) q
LEFT JOIN USERPERMISSION upm
ON q.UserId = upm.UserId
AND q.PermissionMapId = upm.PermissionMapId
WHERE upm.Id IS NULL
  AND q.UserId = @testUid
ORDER BY q.UserId, q.OrderId;

SELECT *
  FROM tmp_lead_permission;

INSERT USERPERMISSION
( Id, UserId, PermissionMapId, KeyValue)
SELECT
 CONVERT( UUID(), CHAR(36) ) Id,
 UserId, PermissionMapId, KeyValue
  FROM tmp_lead_permission;

*/