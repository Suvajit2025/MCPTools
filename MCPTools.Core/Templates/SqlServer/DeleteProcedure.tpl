CREATE OR ALTER PROCEDURE [dbo].[{{StoredProcedureName}}_Delete]
(
{{PrimaryKeySqlParameters}}
)
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM [dbo].[{{TableName}}]
    WHERE
{{PrimaryKeyWhere}};
END;
GO
