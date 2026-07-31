CREATE OR ALTER PROCEDURE [dbo].[{{StoredProcedureName}}_GetAll]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
{{SelectColumns}}
    FROM [dbo].[{{TableName}}];
END;
GO
