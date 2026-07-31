CREATE OR ALTER PROCEDURE [dbo].[{{StoredProcedureName}}_GetById]
    @{{PrimaryKey}} {{PrimaryKeyType}}
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
{{Columns}}
    FROM [dbo].[{{TableName}}]
    WHERE [{{PrimaryKey}}] = @{{PrimaryKey}};
END;
GO
