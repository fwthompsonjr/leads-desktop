SET @user_name = 'app.testing.account';
SET @user_id = (SELECT Id FROM LEADUSER WHERE UserName = @user_name LIMIT 1);
SELECT *
FROM LEADUSERCOUNTY
-- UPDATE LEADUSERCOUNTY
-- SET 
-- MonthlyUsage = 10000
  WHERE LeadUserId = @user_id
    AND CountyName = 'Harris';