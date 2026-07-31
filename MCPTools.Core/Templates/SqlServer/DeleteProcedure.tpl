CREATE OR ALTER PROCEDURE [dbo].[{{StoredProcedureName}}_Delete]
    @{{PrimaryKey}} {{PrimaryKeyType}}
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM [dbo].[{{TableName}}]
    WHERE [{{PrimaryKey}}] = @{{PrimaryKey}};
END;
GO
