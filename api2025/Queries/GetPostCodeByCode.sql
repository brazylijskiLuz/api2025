SELECT 
    c.id AS Id:GUID:KEY,
    c.code AS Code:STRING,
    p.name AS Name:STRING
FROM post_code c
JOIN province p ON c.province_id = p.id
WHERE c.code = @Code:STRING;