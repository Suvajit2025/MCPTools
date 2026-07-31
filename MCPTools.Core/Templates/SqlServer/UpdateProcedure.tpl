CREATE OR ALTER PROCEDURE [dbo].[{{StoredProcedureName}}_Update]
(
{{UpdateSqlParameters}}
)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE [dbo].[{{TableName}}]
    SET
{{UpdateSetClause}}
    WHERE
{{PrimaryKeyWhere}};
END;
GO
