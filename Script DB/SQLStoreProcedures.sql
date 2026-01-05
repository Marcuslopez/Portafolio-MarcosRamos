
-- Scrips de Store Procedure Sistema Portafolio Marcos Ramos 28/12/2025---

Use DBPortafolio
GO

CREATE PROCEDURE sp_Usuarios
(
    @Operacion VARCHAR(10),
    @IdUsuario INT = NULL,
    @Nombre VARCHAR(100) = NULL,
    @Email VARCHAR(100) = NULL,
    @PasswordHash VARCHAR(255) = NULL,
    @IdRol INT = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    IF @Operacion = 'OpAdd'
    BEGIN
        INSERT INTO Usuarios (Nombre, Email, PasswordHash, IdRol, Activo)
        VALUES (@Nombre, @Email, @PasswordHash, @IdRol, 1);
    END

    ELSE IF @Operacion = 'OpMod'
    BEGIN
        UPDATE Usuarios
        SET Nombre = @Nombre,
            Email = @Email,
            IdRol = @IdRol
        WHERE IdUsuario = @IdUsuario;
    END

    ELSE IF @Operacion = 'OpDel'
    BEGIN
        UPDATE Usuarios
        SET Activo = 0
        WHERE IdUsuario = @IdUsuario;
    END

    ELSE IF @Operacion = 'OpGet'
    BEGIN
        SELECT IdUsuario, Nombre, Email, IdRol, Activo, FechaCreacion
        FROM Usuarios
        WHERE IdUsuario = @IdUsuario;
    END

    ELSE IF @Operacion = 'OpList'
    BEGIN
        SELECT IdUsuario, Nombre, Email, IdRol, Activo, FechaCreacion
        FROM Usuarios
        WHERE Activo = 1;
    END
END;
GO

CREATE PROCEDURE sp_Clientes
(
    @Operacion VARCHAR(10),
    @IdCliente INT = NULL,
    @Nombre VARCHAR(100) = NULL,
    @Email VARCHAR(100) = NULL,
    @Telefono VARCHAR(20) = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    IF @Operacion = 'OpAdd'
    BEGIN
        INSERT INTO Clientes (Nombre, Email, Telefono, Activo)
        VALUES (@Nombre, @Email, @Telefono, 1);
    END

    ELSE IF @Operacion = 'OpMod'
    BEGIN
        UPDATE Clientes
        SET Nombre = @Nombre,
            Email = @Email,
            Telefono = @Telefono
        WHERE IdCliente = @IdCliente;
    END

    ELSE IF @Operacion = 'OpDel'
    BEGIN
        UPDATE Clientes
        SET Activo = 0
        WHERE IdCliente = @IdCliente;
    END

    ELSE IF @Operacion = 'OpGet'
    BEGIN
        SELECT * FROM Clientes WHERE IdCliente = @IdCliente;
    END

    ELSE IF @Operacion = 'OpList'
    BEGIN
        SELECT * FROM Clientes WHERE Activo = 1;
    END
END;
GO

USE [DBPortafolio]
GO
/****** Object:  StoredProcedure [dbo].[sp_Productos]    Script Date: 01/03/2026 11:05:05 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

USE [DBPortafolio]
GO
/****** Object:  StoredProcedure [dbo].[sp_Productos]    Script Date: 01/03/2026 11:05:05 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[sp_Productos]
(
    @Operacion VARCHAR(20),
    @IdProducto INT = NULL,
    @Nombre VARCHAR(150) = NULL,
    @Descripcion VARCHAR(150) = NULL,
    @Precio DECIMAL(10,2) = NULL,
    @Stock INT = NULL,
    @IdCategoria INT = NULL,
    @Cantidad INT = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    IF @Operacion = 'OpAdd'
    BEGIN
        INSERT INTO Productos (Nombre,descripcion,Precio, Stock, IdCategoria, Activo)
        VALUES (@Nombre,@Descripcion,@Precio, @Stock, @IdCategoria, 1);
    END

    ELSE IF @Operacion = 'OpMod'
    BEGIN
        UPDATE Productos
        SET Nombre = @Nombre,
            Precio = @Precio,
            Stock = @Stock,
            IdCategoria = @IdCategoria
        WHERE IdProducto = @IdProducto;
    END

    ELSE IF @Operacion = 'OpDel'
    BEGIN
        UPDATE Productos
        SET Activo = 0
        WHERE IdProducto = @IdProducto;
    END

    ELSE IF @Operacion = 'OpGet'
    BEGIN
        SELECT * FROM Productos WHERE IdProducto = @IdProducto;
    END

    ELSE IF @Operacion = 'OpList'
    BEGIN
        SELECT * FROM Productos WHERE Activo = 1;
    END
    
    IF @Operacion = 'OpUpdateStock'
    BEGIN
    UPDATE Productos
    SET Stock = Stock - @Cantidad
    WHERE IdProducto = @IdProducto
      AND Stock >= @Cantidad;
    IF @@ROWCOUNT = 0  
        RAISERROR('Stock insuficiente, consultar cantidad existente del producto.', 16, 1);
        RETURN;
    END

END;


CREATE PROCEDURE sp_Login
(
    @Email VARCHAR(100),
    @PasswordHash VARCHAR(255)
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        U.IdUsuario,
        U.Nombre,
        U.Email,
        U.IdRol,
        R.Nombre AS Rol
    FROM Usuarios U
    INNER JOIN Roles R ON U.IdRol = R.IdRol
    WHERE U.Email = @Email
      AND U.PasswordHash = @PasswordHash
      AND U.Activo = 1;
END;
GO

CREATE PROCEDURE sp_CambiarPassword
(
    @IdUsuario INT,
    @NuevoPasswordHash VARCHAR(255)
)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Usuarios
    SET PasswordHash = @NuevoPasswordHash
    WHERE IdUsuario = @IdUsuario
      AND Activo = 1;
END;
GO

CREATE TYPE DetalleOrdenType AS TABLE
(
    IdProducto INT,
    Cantidad INT,
    PrecioUnitario DECIMAL(10,2)
);
GO

CREATE PROCEDURE sp_Ordenes_InsertConDetalle
(
    @IdCliente INT,
    @Estado VARCHAR(50),
    @DetalleItems DetalleOrdenType READONLY,
    @IdOrdenGenerada INT OUTPUT
)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        -- 1️⃣ Validar stock
        IF EXISTS (
            SELECT 1
            FROM @DetalleItems D
            INNER JOIN Productos P ON D.IdProducto = P.IdProducto
            WHERE P.Stock < D.Cantidad
        )
        BEGIN
            RAISERROR('Stock insuficiente para uno o más productos.', 16, 1);
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- 2️⃣ Insertar orden
        INSERT INTO Ordenes (IdCliente, FechaOrden, Total, Estado)
        VALUES (@IdCliente, GETDATE(), 0, @Estado);

        SET @IdOrdenGenerada = SCOPE_IDENTITY();

        -- 3️⃣ Insertar detalle
        INSERT INTO DetalleOrden
        (
            IdOrden,
            IdProducto,
            Cantidad,
            PrecioUnitario,
            Subtotal
        )
        SELECT
            @IdOrdenGenerada,
            IdProducto,
            Cantidad,
            PrecioUnitario,
            Cantidad * PrecioUnitario
        FROM @DetalleItems;

        -- 4️⃣ Actualizar stock
        UPDATE P
        SET P.Stock = P.Stock - D.Cantidad
        FROM Productos P
        INNER JOIN @DetalleItems D ON P.IdProducto = D.IdProducto;

        -- 5️⃣ Actualizar total
        UPDATE Ordenes
        SET Total = (
            SELECT SUM(Subtotal)
            FROM DetalleOrden
            WHERE IdOrden = @IdOrdenGenerada
        )
        WHERE IdOrden = @IdOrdenGenerada;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;

        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@ErrorMessage, 16, 1);
    END CATCH
END;
GO

CREATE PROCEDURE sp_Productos_UpdateStock
(
    @IdProducto INT,
    @Cantidad INT
)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Productos
    SET Stock = Stock + @Cantidad
    WHERE IdProducto = @IdProducto
      AND Activo = 1;
END;
GO

CREATE PROCEDURE sp_Ordenes_CambiarEstado
(
    @IdOrden INT,
    @Estado VARCHAR(50)
)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Ordenes
    SET Estado = @Estado
    WHERE IdOrden = @IdOrden;
END;
GO

CREATE PROCEDURE sp_Reportes_VentasPorFecha
(
    @FechaInicio DATE,
    @FechaFin DATE
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        CAST(O.FechaOrden AS DATE) AS Fecha,
        SUM(O.Total) AS TotalVentas
    FROM Ordenes O
    WHERE O.FechaOrden BETWEEN @FechaInicio AND @FechaFin
      AND O.Estado = 'Pagada'
    GROUP BY CAST(O.FechaOrden AS DATE)
    ORDER BY Fecha;
END;
GO

CREATE PROCEDURE sp_Reportes_VentasPorCliente
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        C.Nombre,
        SUM(O.Total) AS TotalComprado
    FROM Clientes C
    INNER JOIN Ordenes O ON C.IdCliente = O.IdCliente
    WHERE O.Estado = 'Pagada'
    GROUP BY C.Nombre
    ORDER BY TotalComprado DESC;
END;
GO

CREATE PROCEDURE sp_Reportes_ProductosMasVendidos
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        P.Nombre,
        SUM(D.Cantidad) AS CantidadVendida
    FROM Productos P
    INNER JOIN DetalleOrden D ON P.IdProducto = D.IdProducto
    GROUP BY P.Nombre
    ORDER BY CantidadVendida DESC;
END;
GO

CREATE PROCEDURE sp_Log_Insert
(
    @Modulo VARCHAR(50),
    @Mensaje VARCHAR(255)
)
AS
BEGIN
    INSERT INTO LogsSistema (Modulo, Mensaje)
    VALUES (@Modulo, @Mensaje);
END;
GO

CREATE PROCEDURE sp_Log_List
AS
BEGIN
    SELECT * FROM LogsSistema
    ORDER BY Fecha DESC;
END;
GO