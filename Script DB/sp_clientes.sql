
Use DbPortafolio;
Go
--===========================================================
-- Store Procedure CRUD para la entidad Clientes
-- Autor: Marcos Rodolfo Ramos.
-- Frecha Creacion dd/mm/yyyy: 02/01/2026.
--===========================================================


CREATE OR ALTER PROCEDURE dbo.sp_Clientes
(
    @Operacion   VARCHAR(10),
    @IdCliente   INT = NULL,
    @Nombre      VARCHAR(100) = NULL,
    @Email       VARCHAR(100) = NULL,
    @Telefono    VARCHAR(20) = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY

        -- =========================
        -- OpAdd: INSERT
        -- =========================
        IF @Operacion = 'OpAdd'
        BEGIN
            INSERT INTO dbo.Clientes (Nombre, Email, Telefono, Activo, FechaCreacion)
            VALUES (@Nombre, @Email, @Telefono, 1, GETDATE());

            SELECT SCOPE_IDENTITY() AS IdCliente;
            RETURN;
        END

        -- =========================
        -- OpMod: UPDATE
        -- =========================
        IF @Operacion = 'OpMod'
        BEGIN
            UPDATE dbo.Clientes
            SET Nombre   = @Nombre,
                Email    = @Email,
                Telefono = @Telefono
            WHERE IdCliente = @IdCliente;

            RETURN;
        END

        -- =========================
        -- OpDel: DELETE lógico
        -- =========================
        IF @Operacion = 'OpDel'
        BEGIN
            UPDATE dbo.Clientes
            SET Activo = 0
            WHERE IdCliente = @IdCliente;

            RETURN;
        END

        -- =========================
        -- OpGet: GET BY ID
        -- =========================
        IF @Operacion = 'OpGet'
        BEGIN
            SELECT
                IdCliente,
                Nombre,
                Email,
                Telefono,
                Activo,
                FechaCreacion
            FROM dbo.Clientes
            WHERE IdCliente = @IdCliente;

            RETURN;
        END

        -- =========================
        -- OpList: LIST
        -- =========================
        IF @Operacion = 'OpList'
        BEGIN
            SELECT
                IdCliente,
                Nombre,
                Email,
                Telefono,
                Activo,
                FechaCreacion
            FROM dbo.Clientes
            WHERE Activo = 1
            ORDER BY Nombre;

            RETURN;
        END

        -- =========================
        -- Operación inválida
        -- =========================
        RAISERROR('Operacion invalida. Use: OpAdd, OpMod, OpDel, OpGet, OpList', 16, 1);

    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();

        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END;
GO