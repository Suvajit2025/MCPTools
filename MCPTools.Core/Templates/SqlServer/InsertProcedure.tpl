CREATE OR ALTER PROCEDURE [dbo].[{{StoredProcedureName}}_Insert]
(
{{InsertSqlParameters}}
)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO [dbo].[{{TableName}}]
    (
{{InsertColumns}}
    )
    VALUES
    (
{{InsertValues}}
    );
END;
GO
