CREATE OR ALTER PROCEDURE [dbo].[{{StoredProcedureName}}_GetById]
(
{{PrimaryKeySqlParameters}}
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
{{SelectColumns}}
    FROM [dbo].[{{TableName}}]
    WHERE
{{PrimaryKeyWhere}};
END;
GO
