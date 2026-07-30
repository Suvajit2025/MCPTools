CREATE OR ALTER PROCEDURE [dbo].[{{StoredProcedure}}_GetAll]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
{{Columns}}
    FROM [dbo].[{{TableName}}];
END;
GO

CREATE OR ALTER PROCEDURE [dbo].[{{StoredProcedure}}_GetById]
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

CREATE OR ALTER PROCEDURE [dbo].[{{StoredProcedure}}_Insert]
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

CREATE OR ALTER PROCEDURE [dbo].[{{StoredProcedure}}_Update]
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

CREATE OR ALTER PROCEDURE [dbo].[{{StoredProcedure}}_Delete]
    @{{PrimaryKey}} {{PrimaryKeyType}}
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM [dbo].[{{TableName}}]
    WHERE [{{PrimaryKey}}] = @{{PrimaryKey}};
END;
GO
