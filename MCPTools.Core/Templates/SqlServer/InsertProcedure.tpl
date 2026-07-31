CREATE OR ALTER PROCEDURE [dbo].[{{StoredProcedureName}}_Insert]
{{ParameterList}}
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO [dbo].[{{TableName}}]
    (
{{Columns}}
    )
    VALUES
    (
{{ParameterList}}
    );
END;
GO
