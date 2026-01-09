using ClassApplicationMRL.Interfaces;
using ClassDataMRL.Interfaces;
using ClassDomainMRL.DTOs;
using ClassDomainMRL.DTOs.ClassDomainMRL.DTOs;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace ClassApplicationMRL.Services
{
    public class OrdenService : IOrdenService
    {
        private readonly IOrdenRepository _ordenRepository;
        private readonly IOrdenDetalleRepository _ordenDetalleRepository;
        private readonly IProductoRepository _productoRepository;
        private readonly string _connectionString;

        public OrdenService(
            IOrdenRepository ordenRepository,
            IOrdenDetalleRepository ordenDetalleRepository,
            IProductoRepository productoRepository,
            IConfiguration configuration)
        {
            _ordenRepository = ordenRepository;
            _ordenDetalleRepository = ordenDetalleRepository;
            _productoRepository = productoRepository;

            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException("DefaultConnection no encontrada");
        }

        public int CrearOrden(OrdenCreateDto dto)
        {
            using (SqlConnection cn = new SqlConnection(_connectionString))
            {
                cn.Open();

                using (SqlTransaction tx = cn.BeginTransaction())
                {
                    try
                    {
                        // 1️⃣ Calcular total
                        decimal total = dto.Detalles.Sum(d => d.Cantidad * d.PrecioUnitario);

                        // 2️⃣ Insertar orden
                        int idOrden = _ordenRepository.InsertarOrden(
                            dto.IdCliente,
                            total,
                            cn,
                            tx
                        );

                        // 3️⃣ Insertar detalle + actualizar stock
                        foreach (var item in dto.Detalles)
                        {
                            _ordenDetalleRepository.InsertarDetalle(
                                idOrden,
                                item.IdProducto,
                                item.Cantidad,
                                item.PrecioUnitario,
                                cn,
                                tx
                            );

                            _productoRepository.ActualizarStock(
                                item.IdProducto,
                                item.Cantidad,
                                cn,
                                tx
                            );
                        }

                        // 4️⃣ Commit
                        tx.Commit();
                        return idOrden;
                    }
                    catch
                    {
                        tx.Rollback();
                        throw;
                    }
                }
            }
        }

        public IEnumerable<OrdenResponseDto> ObtenerOrdenes()
        {
            using (var cn = new SqlConnection(_connectionString))
            {
                cn.Open();
                var ordenes = _ordenRepository.ListarOrdenes(cn);

                var resultado = new List<OrdenResponseDto>();

                foreach (var orden in ordenes)
                {
                    var detalles = _ordenDetalleRepository.ListarPorOrden(orden.IdOrden, cn);

                    resultado.Add(new OrdenResponseDto
                    {
                        IdOrden = orden.IdOrden,
                        IdCliente = orden.IdCliente,
                        FechaOrden = orden.FechaOrden,
                        Total = orden.Total,
                        Estado = orden.Estado,
                        Detalles = detalles.Select(d => new OrdenDetalleResponseDto
                        {
                            IdProducto = d.IdProducto,
                            Cantidad = d.Cantidad,
                            PrecioUnitario = d.PrecioUnitario,
                            Subtotal = d.Subtotal
                        }).ToList()
                    });
                }

                return resultado;
            }
        }


        public OrdenResponseDto ObtenerOrdenPorId(int idOrden)
        {
            using (var cn = new SqlConnection(_connectionString))
            {
                cn.Open();
                var orden = _ordenRepository.ObtenerPorId(idOrden, cn);
                if (orden == null) return null;

                var detalles = _ordenDetalleRepository.ListarPorOrden(idOrden, cn);

                return new OrdenResponseDto
                {
                    IdOrden = orden.IdOrden,
                    IdCliente = orden.IdCliente,
                    FechaOrden = orden.FechaOrden,
                    Total = orden.Total,
                    Estado = orden.Estado,
                    Detalles = detalles.Select(d => new OrdenDetalleResponseDto
                    {
                        IdProducto = d.IdProducto,
                        Cantidad = d.Cantidad,
                        PrecioUnitario = d.PrecioUnitario,
                        Subtotal = d.Subtotal
                    }).ToList()
                };
            }
        }

        public void CambiarEstado(int idOrden, string nuevoEstado)
        {
            using (var cn = new SqlConnection(_connectionString))
            {
                cn.Open();
                var orden = _ordenRepository.ObtenerPorId(idOrden,cn);

                if (orden == null)
                    throw new Exception("Orden no encontrada");

                if (orden.Estado == "ANULADA")
                    throw new Exception("No se puede modificar una orden anulada");

                if (orden.Estado == "PAGADA" && nuevoEstado == "ANULADA")
                    throw new Exception("No se puede anular una orden pagada");

                _ordenRepository.CambiarEstado(idOrden, nuevoEstado,cn);
            } 
        }



    }
}
