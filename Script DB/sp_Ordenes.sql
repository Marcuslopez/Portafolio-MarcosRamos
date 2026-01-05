CREATE PROCEDURE dbo.sp_Ordenes
(
    @Operacion   VARCHAR(10),
    @IdOrden     INT = NULL,
    @IdCliente   INT = NULL,
    @Total       DECIMAL(18,2) = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY

        -- =========================
        -- OpAdd: INSERT ORDEN
        -- =========================
        IF @Operacion = 'OpAdd'
        BEGIN
            INSERT INTO Ordenes
            (
                IdCliente,
                FechaOrden,
                Total,
                Estado,
                Activo
            )
            VALUES
            (
                @IdCliente,
                GETDATE(),
                @Total,
                'CREADA',
                1
            );

            SELECT SCOPE_IDENTITY() AS IdOrden;
            RETURN;
        END

        -- =========================
        -- OpGet: ORDEN POR ID
        -- =========================
        IF @Operacion = 'OpGet'
        BEGIN
            SELECT
                IdOrden,
                IdCliente,
                FechaOrden,
                Total,
                Estado,
                Activo
            FROM Ordenes
            WHERE IdOrden = @IdOrden;

            RETURN;
        END

        -- =========================
        -- OpList: LISTAR ORDENES
        -- =========================
        IF @Operacion = 'OpList'
        BEGIN
            SELECT
                IdOrden,
                IdCliente,
                FechaOrden,
                Total,
                Estado,
                Activo
            FROM Ordenes
            WHERE Activo = 1
            ORDER BY FechaOrden DESC;

            RETURN;
        END

        RAISERROR('Operacion invalida en sp_Ordenes', 16, 1);

    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();

        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END;
GO