SELECT
    r.id AS Id:GUID:KEY,
    r.usage_time AS UsageTime:DATETIME,
    r.expected_pension AS ExpectedPension:DOUBLE,
    r.sex AS Sex:enumeration:Sex,
    r.salary_amount AS SalaryAmount:DOUBLE,
    r.considered_sick_leave AS ConsideredSickLeave:BOOLEAN,
    r.sub_account_balance AS SubAccountBalance:DOUBLE,
    r.account_balance AS AccountBalance:DOUBLE,
    r.pension AS Pension:DOUBLE,
    r.real_pension AS RealPension:DOUBLE,
    pc.code AS PostalCode:STRING,
    p.name AS ProvinceName:STRING
FROM report r
LEFT JOIN post_code pc ON r.postal_code_id = pc.id
LEFT JOIN province p ON pc.province_id = p.id
WHERE pc.province_id = @ProvinceId:GUID

 
