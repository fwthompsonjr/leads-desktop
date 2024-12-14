-- user_usage_get_monthly_limit
-- inbound parameters
SET @lead_index = '';
SET @county_id = 5;
-- local parameters

SET @month_limit	= (
	SELECT MaxRecords
    FROM DBCOUNTYUSAGELIMIT
    WHERE LeadUserId = @lead_index
      AND CountyId = @county_id 
      AND IsActive = TRUE
      AND CompleteDate IS NOT NULL
	ORDER BY CreateDate DESC
    LIMIT 1);
SET @month_limit = CASE WHEN @month_limit IS NULL THEN 15000 ELSE @month_limit END;
SELECT 
	case when dst.Id 			IS NULL THEN '' ELSE dst.Id END Id, 
    case when dst.LeadUserId 	IS NULL THEN src.LeadUserId ELSE dst.LeadUserId END LeadUserId, 
    case when dst.CountyId 		IS NULL THEN src.CountyId ELSE dst.CountyId END CountyId, 
    case when dst.IsActive 		IS NULL THEN src.IsActive ELSE dst.IsActive END IsActive, 
    case when dst.MaxRecords 	IS NULL THEN src.MaxRecords ELSE dst.MaxRecords END MaxRecords, 
    dst.CompleteDate, 
    dst.CreateDate
FROM
(
	SELECT @lead_index LeadUserId, @county_id CountyId, @month_limit MaxRecords, 0 IsActive
) src
LEFT JOIN
( SELECT *
  FROM DBCOUNTYUSAGELIMIT 
    WHERE LeadUserId = @lead_index
      AND CountyId = @county_id 
      AND IsActive = TRUE
      AND CompleteDate IS NOT NULL
	ORDER BY CreateDate DESC
    LIMIT 1
) dst
ON src.LeadUserId = dst.LeadUserId;