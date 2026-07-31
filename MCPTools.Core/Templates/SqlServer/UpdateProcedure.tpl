CREATE OR ALTER PROCEDURE [dbo].[{{StoredProcedureName}}_Update]
    @{{PrimaryKey}} {{PrimaryKeyType}},
{{ParameterList}}
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE [dbo].[{{TableName}}]
    SET
{{Columns}}
    WHERE [{{PrimaryKey}}] = @{{PrimaryKey}};
END;
GO
