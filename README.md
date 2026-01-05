ERP LITE – API REST (.NET)

Sistema ERP Lite desarrollado como proyecto de portafolio, enfocado en demostrar buenas prácticas de arquitectura por capas, seguridad, transacciones y acceso a datos con SQL Server.

Nombre del Proyecto: Master Portafolio
El proyecto implementa un flujo real de negocio:
Productos → Clientes → Órdenes con Detalle,
incluyendo validación de stock y rollback automático ante errores.


============================================================
TECNOLOGÍAS UTILIZADAS
============================================================

- ASP.NET Core 8 (Web API)
- C#
- SQL Server
- ADO.NET (Microsoft.Data.SqlClient)
- Stored Procedures
- JWT (JSON Web Tokens)
- Swagger / OpenAPI
- Inyección de Dependencias
- Middleware global de errores


============================================================
ARQUITECTURA DEL PROYECTO
============================================================

Arquitectura por capas, con separación clara de responsabilidades:

WebAPIMRL
  - Controllers (API / HTTP)

ClassApplicationMRL
  - Services (Lógica de negocio y transacciones)

ClassDataMRL
  - Interfaces (Contratos de acceso a datos)
  - Repositories (ADO.NET + Stored Procedures)

ClassDomainMRL
  - Entities (Modelos de dominio)
  - DTOs (Objetos de transferencia)


Principios aplicados:
- Controllers sin lógica de negocio
- Services controlan transacciones
- Repositories encapsulan acceso a datos
- Dominio desacoplado de infraestructura


============================================================
SEGURIDAD
============================================================

- Autenticación mediante JWT
- Autorización por Roles y Policies
- Endpoints protegidos con [Authorize]
- Tokens firmados con clave simétrica

Ejemplo de roles:
- Administrador
- Vendedor
- Usuario


============================================================
FUNCIONALIDADES IMPLEMENTADAS
============================================================

PRODUCTOS
- CRUD completo
- Manejo de stock
- Actualización segura desde órdenes

CLIENTES
- CRUD completo
- Validación básica de datos

ÓRDENES (MÓDULO PRINCIPAL)
- Crear orden con múltiples productos
- Detalle de orden
- Validación de stock en base de datos
- Transacciones completas (Commit / Rollback)
- Consulta de órdenes:
  - Listado general
  - Orden con detalle


============================================================
MANEJO DE TRANSACCIONES
============================================================

Las transacciones se controlan en la capa Service, no en los Stored Procedures.

Flujo:
Crear Orden
 → Insertar Orden
 → Insertar Detalles
 → Actualizar Stock
 → Commit / Rollback

Si el stock es insuficiente:
- SQL lanza excepción
- El Service hace rollback
- No se inserta la orden
- No se descuenta stock


============================================================
MIDDLEWARE GLOBAL DE ERRORES
============================================================

Se implementa un middleware global para:

- Capturar excepciones
- Retornar respuestas JSON consistentes
- Diferenciar errores SQL (400)
- Manejar errores internos (500)
- Evitar duplicar try/catch en controllers

Ejemplo de respuesta:

{
  "status": 400,
  "message": "Ocurrió un error durante el procesamiento de la solicitud.",
  "detail": "Stock insuficiente para el producto"
}


============================================================
ENDPOINTS PRINCIPALES
============================================================

AUTENTICACIÓN
- POST /api/auth/login

PRODUCTOS
- GET    /api/products
- POST   /api/products
- PUT    /api/products
- DELETE /api/products/{id}

CLIENTES
- GET  /api/clients
- POST /api/clients

ÓRDENES
- POST /api/orders
- GET  /api/orders
- GET  /api/orders/{id}


============================================================
BASE DE DATOS
============================================================

- SQL Server
- Acceso mediante Stored Procedures
- Un Stored Procedure por entidad
- Operaciones controladas por flags:

  OpAdd
  OpMod
  OpDel
  OpGet
  OpList
  OpUpdateStock

Ejemplo:

EXEC sp_Productos
    @Operacion = 'OpUpdateStock',
    @IdProducto = 1,
    @Cantidad = 10;


============================================================
CÓMO EJECUTAR EL PROYECTO
============================================================

1. Clonar el repositorio
2. Ejecutar los scripts SQL (tablas y stored procedures)
3. Configurar la cadena de conexión en appsettings.json
4. Ejecutar la API desde Visual Studio
5. Acceder a Swagger desde el navegador

Ejemplo:
https://localhost:{puerto}/swagger


============================================================
OBJETIVO DEL PROYECTO
============================================================

Este proyecto fue desarrollado con fines de portafolio profesional, demostrando:

- Buen diseño de arquitectura
- Dominio de ASP.NET Web API
- Uso correcto de SQL Server y transacciones
- Seguridad con JWT
- Código mantenible y escalable


============================================================
AUTOR
============================================================

Desarrollado por: Marcos Rodolfo Ramos Lopez
Perfil enfocado en Backend .NET / APIs / SQL Server
