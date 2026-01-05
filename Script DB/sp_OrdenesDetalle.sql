USE DBPortafolio
GO

CREATE OR ALTER PROCEDURE dbo.sp_OrdenesDetalle
(
    @Operacion        VARCHAR(10),
    @IdDetalle        INT = NULL,
    @IdOrden          INT = NULL,
    @IdProducto       INT = NULL,
    @Cantidad         INT = NULL,
    @PrecioUnitario   DECIMAL(18,2) = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY

        -- =========================
        -- OpAdd: INSERT DETALLE
        -- =========================
        IF @Operacion = 'OpAdd'
        BEGIN
            INSERT INTO OrdenesDetalle
            (
                IdOrden,
                IdProducto,
                Cantidad,
                PrecioUnitario,
                Subtotal
            )
            VALUES
            (
                @IdOrden,
                @IdProducto,
                @Cantidad,
                @PrecioUnitario,
                @Cantidad * @PrecioUnitario
            );

            RETURN;
        END

        -- =========================
        -- OpList: DETALLES POR ORDEN
        -- =========================
        IF @Operacion = 'OpList'
        BEGIN
            SELECT
                IdDetalle,
                IdOrden,
                IdProducto,
                Cantidad,
                PrecioUnitario,
                Subtotal
            FROM OrdenesDetalle
            WHERE IdOrden = @IdOrden;

            RETURN;
        END

        RAISERROR('Operacion invalida en sp_OrdenesDetalle', 16, 1);

    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();

        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END;
GO