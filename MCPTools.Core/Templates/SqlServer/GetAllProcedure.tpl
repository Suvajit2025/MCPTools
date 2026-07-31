CREATE OR ALTER PROCEDURE [dbo].[{{StoredProcedureName}}_GetAll]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
{{Columns}}
    FROM [dbo].[{{TableName}}];
END;
GO
