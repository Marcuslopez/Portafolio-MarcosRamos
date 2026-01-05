

Use DBPortafolio

CREATE TABLE Roles (
    IdRol INT IDENTITY(1,1) PRIMARY KEY,
    Nombre VARCHAR(50) NOT NULL,
    Activo BIT NOT NULL DEFAULT 1
);


CREATE TABLE Usuarios (
    IdUsuario INT IDENTITY(1,1) PRIMARY KEY,
    Nombre VARCHAR(100) NOT NULL,
    Email VARCHAR(100) NOT NULL UNIQUE,
    PasswordHash VARCHAR(255) NOT NULL,
    IdRol INT NOT NULL,
    Activo BIT NOT NULL DEFAULT 1,
    FechaCreacion DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Usuarios_Roles
        FOREIGN KEY (IdRol) REFERENCES Roles(IdRol)
);


CREATE TABLE Clientes (
    IdCliente INT IDENTITY(1,1) PRIMARY KEY,
    Nombre VARCHAR(100) NOT NULL,
    Email VARCHAR(100),
    Telefono VARCHAR(20),
    Activo BIT NOT NULL DEFAULT 1,
    FechaCreacion DATETIME NOT NULL DEFAULT GETDATE()
);


CREATE TABLE Caterias (
    IdCateria INT IDENTITY(1,1) PRIMARY KEY,
    Nombre VARCHAR(100) NOT NULL,
    Activo BIT NOT NULL DEFAULT 1
);


CREATE TABLE Productos (
    IdProducto INT IDENTITY(1,1) PRIMARY KEY,
    Nombre VARCHAR(150) NOT NULL,
    Precio DECIMAL(10,2) NOT NULL,
    Stock INT NOT NULL,
    IdCateria INT NOT NULL,
    Activo BIT NOT NULL DEFAULT 1,
    CONSTRAINT FK_Productos_Caterias
        FOREIGN KEY (IdCateria) REFERENCES Caterias(IdCateria)
);


CREATE TABLE Ordenes (
    IdOrden INT IDENTITY(1,1) PRIMARY KEY,
    IdCliente INT NOT NULL,
    FechaOrden DATETIME NOT NULL DEFAULT GETDATE(),
    Total DECIMAL(10,2) NOT NULL,
    Estado VARCHAR(50) NOT NULL,
    CONSTRAINT FK_Ordenes_Clientes
        FOREIGN KEY (IdCliente) REFERENCES Clientes(IdCliente)
);



 CREATE TABLE OrdenesDetalle (
    IdDetalle INT IDENTITY(1,1) PRIMARY KEY,
    IdOrden INT NOT NULL,
    IdProducto INT NOT NULL,
    Cantidad INT NOT NULL,
    PrecioUnitario DECIMAL(10,2) NOT NULL,
    Subtotal DECIMAL(10,2) NOT NULL,
    CONSTRAINT FK_OrdenesDetalle_Ordenes
        FOREIGN KEY (IdOrden) REFERENCES Ordenes(IdOrden),
    CONSTRAINT FK_OrdenesDetalle_Productos
        FOREIGN KEY (IdProducto) REFERENCES Productos(IdProducto)
);


CREATE TABLE Facturas (
    IdFactura INT IDENTITY(1,1) PRIMARY KEY,
    IdOrden INT NOT NULL,
    FechaFactura DATETIME NOT NULL DEFAULT GETDATE(),
    Total DECIMAL(10,2) NOT NULL,
    CONSTRAINT FK_Facturas_Ordenes
        FOREIGN KEY (IdOrden) REFERENCES Ordenes(IdOrden)
);


CREATE TABLE LogsSistema (
    IdLog INT IDENTITY(1,1) PRIMARY KEY,
    Modulo VARCHAR(50) NOT NULL,
    Mensaje VARCHAR(255) NOT NULL,
    Fecha DATETIME NOT NULL DEFAULT GETDATE()
);
GO