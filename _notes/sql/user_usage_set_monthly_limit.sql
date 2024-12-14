-- user_usage_set_monthly_limit

-- inbound parameters
SET @lead_index = '';
SET @county_id = 5;
SET @month_limit = 5000;

SET @id = (SELECT Id FROM LeadUser WHERE Id = @lead_index);
UPDATE DBCOUNTYUSAGELIMIT L 
  SET
  CompleteDate = utc_timestamp(),
  IsActive = 0
  WHERE 
	@id IS NOT NULL
	AND LeadUserId = @lead_index
    AND (
		CompleteDate IS NULL
        OR IsActive = 1
    );

    
INSERT DBCOUNTYUSAGELIMIT
(
	LeadUserId, CountyId, IsActive, MaxRecords
)
SELECT
	@lead_index, @county_id, 1, @month_limit
    WHERE @id IS NOT NULL;
-- Id, LeadUserId, CountyId, IsActive, MaxRecords, CompleteDate, CreateDate
SELECT *
FROM DBCOUNTYUSAGELIMIT
WHERE LeadUserId 	= 	@lead_index
  AND CountyId		=	@county_id
ORDER BY CreateDesc DESC LIMIT 1;