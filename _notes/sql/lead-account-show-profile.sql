SET @leadIndex = 'fef29532-a487-11ef-99ce-0af7a01f52e9';
SET @user_name = (SELECT UserName FROM LEADUSER WHERE Id = @leadIndex); -- 'lead.administrator';
SET @user_clone = concat(@user_name, '-clone');

SET @lead_user_id = (SELECT Id FROM LEADUSER WHERE UserName = @user_name LIMIT 1);
SET @user_id = (SELECT Id FROM USERS WHERE Id = @lead_user_id OR UserName = @user_clone LIMIT 1);
DROP temporary table if exists tm_user_profile;
CREATE temporary table tm_user_profile
SELECT 
	m.Id, 
	m.OrderId, 
    CASE 
    WHEN @user_id is null THEN @lead_user_id
    WHEN @user_id is not null AND @user_id = @lead_user_id THEN @lead_user_id
    ELSE @user_id END UserId,
    p.Id ProfileId,
    CASE 
		WHEN m.KeyName LIKE '% Name' THEN 'Name'
		WHEN m.KeyName LIKE 'Phone%' THEN 'Phone'
		WHEN m.KeyName LIKE 'Email%' THEN 'Email'
		WHEN m.KeyName LIKE 'Address 1%' THEN 'Address (Primary)'
		WHEN m.KeyName LIKE 'Address 2%' THEN 'Address (Secondary)'
        ELSE 'Other' END ProfileGroup,
	REPLACE (
	REPLACE (
	REPLACE (
	REPLACE (
    REPLACE (m.KeyName, 
		'Address 1 - Address', 'Mailing Address:'),
        'Address 2 - Address', 'Business Address:'),
        'Email 1', 'Primary Email:'),
        'Email 2', 'Alternate Email 1:'),
        'Email 3', 'Alternate Email 2:') KeyName,
	p.KeyValue
  FROM PROFILEMAP m
  LEFT JOIN USERPROFILE p
  ON m.Id = p.ProfileMapId
  AND p.UserId = @user_id
ORDER BY m.OrderId;
SET @idx = 0;


SELECT * 
FROM tm_user_profile
WHERE OrderId NOT IN (12, 16);


SELECT 
	JSON_ARRAYAGG(JSON_OBJECT(
    'id', Id,
    'orderId', OrderId,
    'userId', UserId,
    'profileId', ProfileId, 
    'profileGroup', ProfileGroup, 
    'keyName', KeyName, 
    'keyValue', KeyValue)) js
FROM tm_user_profile;
