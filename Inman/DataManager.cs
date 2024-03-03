using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using Inman.Models;

namespace Inman;

/// <summary>
/// Esta clase gestiona operaciones de datos como la inserción, actualización y eliminación
/// de registros en la base de datos utilizando procedimientos almacenados. También proporciona
/// métodos para recuperar listas de clientes, facturas, tipos de producto, productos y productos
/// de factura de la base de datos.
/// </summary>
public class DataManager
{
    // Manejador de registro de eventos estático utilizado por la clase DataManager para registrar eventos.
    private static readonly LogHandler<DataManager> LogHandler = new();
    
    // Cadena de conexión utilizada por el DataManager para conectarse a la base de datos.
    private static string ConnectionString = string.Empty;
    
    /// <summary>
    /// Constructor de la clase DataManager. Establece la cadena de conexión a la base de datos.
    /// </summary>
    /// <param name="connectionString">Cadena de conexión a la base de datos.</param>
    public DataManager(string connectionString)
    {
        ConnectionString = connectionString;
    }
    
    /// <summary>
    /// Obtiene una nueva conexión a la base de datos utilizando la cadena de conexión almacenada.
    /// </summary>
    /// <returns>Objeto MySqlConnection que representa la conexión a la base de datos.</returns>
    private MySqlConnection GetConnection()
    {
        return new MySqlConnection(ConnectionString);
    }

    /// <summary>
    /// Inserta un nuevo cliente en la base de datos utilizando un procedimiento almacenado. La información
    /// del cliente, incluyendo nombre, apellido, género, número de teléfono y dirección de correo electrónico
    /// se proporcionan como parámetros. El método maneja transacciones de base de datos y registra mensajes
    /// de éxito o fracaso utilizando un LogHandler.
    /// </summary>
    /// <param name="nombre">El nombre del cliente.</param>
    /// <param name="apellido">El apellido del cliente.</param>
    /// <param name="sexo">El género del cliente.</param>
    /// <param name="telefono">El número de teléfono del cliente.</param>
    /// <param name="correo">La dirección de correo electrónico del cliente.</param>
    public void InsertarCliente(string nombre, string apellido, string sexo, string telefono,
        string correo)
    {
        using MySqlConnection connection = GetConnection();
        try
        {
            connection.Open();

            using MySqlTransaction transaction = connection.BeginTransaction();
            try
            {
                using MySqlCommand command = new MySqlCommand("InsertarCliente", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("p_nombre", nombre);
                command.Parameters.AddWithValue("p_apellido", apellido);
                command.Parameters.AddWithValue("p_sexo", sexo);
                command.Parameters.AddWithValue("p_telefono", telefono);
                command.Parameters.AddWithValue("p_correo", correo);
                command.ExecuteNonQuery();

                transaction.Commit();
                LogHandler.LogInfo("Cliente insertado exitosamente en la base de datos.");
            }
            catch (Exception ex)
            {
                LogHandler.LogError($"Error al intentar insertar un cliente.", ex);
                transaction.Rollback();
            }
        }
        catch (Exception ex)
        {
            LogHandler.LogError($"Error en la conexion.", ex);
        }
    }

    /// <summary>
    /// Actualiza un cliente existente en la base de datos utilizando un procedimiento almacenado.
    /// La información del cliente, incluyendo nombre, apellido, género, número de teléfono y dirección
    /// de correo electrónico se proporcionan como parámetros. El método maneja transacciones de base de datos
    /// y registra mensajes de éxito o fracaso utilizando un LogHandler.
    /// </summary>
    /// <param name="id">El ID del cliente que se actualizará.</param>
    /// <param name="newNombre">El nuevo nombre del cliente.</param>
    /// <param name="newApellido">El nuevo apellido del cliente.</param>
    /// <param name="newSexo">El nuevo género del cliente.</param>
    /// <param name="newTelefono">El nuevo número de teléfono del cliente.</param>
    /// <param name="newCorreo">La nueva dirección de correo electrónico del cliente.</param>
    public void ActualizarCliente(int id, string newNombre, string newApellido, string newSexo, string newTelefono,
        string newCorreo)
    {
        using MySqlConnection connection = GetConnection();
        try
        {
            connection.Open();

            using MySqlTransaction transaction = connection.BeginTransaction();
            try
            {
                using MySqlCommand command = new MySqlCommand("ActualizarCliente", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("p_id", id);
                command.Parameters.AddWithValue("p_nombre", newNombre);
                command.Parameters.AddWithValue("p_apellido", newApellido);
                command.Parameters.AddWithValue("p_sexo", newSexo);
                command.Parameters.AddWithValue("p_telefono", newTelefono);
                command.Parameters.AddWithValue("p_correo", newCorreo);
                command.ExecuteNonQuery();
                
                transaction.Commit();
                LogHandler.LogInfo("Cliente actualizado exitosamente en la base de datos.");
            }
            catch (Exception ex)
            {
                LogHandler.LogError($"Error al intentar actualizar un cliente.", ex);
                transaction.Rollback();
            }
        }
        catch (Exception ex)
        {
            LogHandler.LogError($"Error en la conexion.", ex);
        }
    }

    /// <summary>
    /// Inserta una nueva factura en la base de datos utilizando un procedimiento almacenado.
    /// El código de factura, el ID del cliente y el porcentaje de impuesto se proporcionan
    /// como parámetros. El método maneja transacciones de base de datos y registra mensajes
    /// de éxito o fracaso utilizando un LogHandler.
    /// </summary>
    /// <param name="codigoFactura">El código de la factura.</param>
    /// <param name="idCliente">El ID del cliente asociado a la factura.</param>
    /// <param name="porcentajeImpuesto">El porcentaje de impuesto de la factura.</param>
    public void InsertarFactura(string codigoFactura, int idCliente, int porcentajeImpuesto)
    {
        using MySqlConnection connection = GetConnection();
        try
        {
            connection.Open();
            using MySqlTransaction transaction = connection.BeginTransaction();
            try
            {
                using MySqlCommand command = new MySqlCommand("InsertarFactura", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("p_codigoFactura", codigoFactura);
                command.Parameters.AddWithValue("p_idCliente", idCliente);
                command.Parameters.AddWithValue("p_porcentajeImpuesto", porcentajeImpuesto);
                command.ExecuteNonQuery();

                transaction.Commit();
                LogHandler.LogInfo("Factura insertada exitosamente en la base de datos.");
            }
            catch (Exception ex)
            {
                LogHandler.LogError($"Error en la transacción.", ex);
                transaction.Rollback();
            }
        }
        catch (Exception ex)
        {
            LogHandler.LogError($"Error al intentar insertar una factura.", ex);
        }
    }

    /// <summary>
    /// Inserta un nuevo tipo de producto en la base de datos utilizando un procedimiento almacenado.
    /// El nombre del tipo de producto se proporciona como parámetro. El método maneja transacciones
    /// de base de datos y registra mensajes de éxito o fracaso utilizando un LogHandler.
    /// </summary>
    /// <param name="nombreTipoProducto">El nombre del tipo de producto.</param>
    public void InsertarTipoProducto(string nombreTipoProducto)
    {
        using MySqlConnection connection = GetConnection();
        try
        {
            connection.Open();
            using MySqlTransaction transaction = connection.BeginTransaction();
            try
            {
                using MySqlCommand command = new MySqlCommand("InsertarTipoProducto", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("p_nombreTipoProducto", nombreTipoProducto);
                command.ExecuteNonQuery();

                transaction.Commit();
                LogHandler.LogInfo("Tipo de producto insertado exitosamente en la base de datos.");
                connection.Close();
            }
            catch (Exception ex)
            {
                LogHandler.LogError($"Error en la transacción.", ex);
                transaction.Rollback();
                connection.Close();
            }
        }
        catch (Exception ex)
        {
            LogHandler.LogError($"Error al intentar insertar un tipo de producto.", ex);
        }
    }

    /// <summary>
    /// Inserta un nuevo producto en la base de datos utilizando un procedimiento almacenado.
    /// El código del producto, el ID del tipo de producto, el nombre del producto, el precio y
    /// el porcentaje de descuento se proporcionan como parámetros. El método maneja transacciones
    /// de base de datos y registra mensajes de éxito o fracaso utilizando un LogHandler.
    /// </summary>
    /// <param name="codigoProducto">El código del producto.</param>
    /// <param name="idTipoProducto">El ID del tipo de producto.</param>
    /// <param name="nombreProducto">El nombre del producto.</param>
    /// <param name="precio">El precio del producto.</param>
    /// <param name="porcentajeDescuento">El porcentaje de descuento del producto.</param>
    public void InsertarProducto(string codigoProducto, int idTipoProducto, string nombreProducto, decimal precio,
        int porcentajeDescuento)
    {
        using MySqlConnection connection = GetConnection();
        try
        {
            connection.Open();
            using MySqlTransaction transaction = connection.BeginTransaction();
            try
            {
                using MySqlCommand command = new MySqlCommand("InsertarProducto", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("p_codigoProducto", codigoProducto);
                command.Parameters.AddWithValue("p_idTipoProducto", idTipoProducto);
                command.Parameters.AddWithValue("p_nombreProducto", nombreProducto);
                command.Parameters.AddWithValue("p_precio", precio);
                command.Parameters.AddWithValue("p_porcentajeDescuento", porcentajeDescuento);
                command.ExecuteNonQuery();

                transaction.Commit();
                LogHandler.LogInfo("Producto insertado exitosamente en la base de datos.");
            }
            catch (Exception ex)
            {
                LogHandler.LogError($"Error en la transacción.", ex);
                transaction.Rollback();
            }
        }
        catch (Exception ex)
        {
            LogHandler.LogError($"Error al intentar insertar un producto.", ex);
        }
    }

    /// <summary>
    /// Relaciona un producto con una factura en la base de datos utilizando un procedimiento almacenado.
    /// El código de factura, el código de producto y el precio se proporcionan como parámetros. El método maneja
    /// transacciones de base de datos y registra mensajes de éxito o fracaso utilizando un LogHandler.
    /// </summary>
    /// <param name="codigoFactura">El código de la factura.</param>
    /// <param name="codigoProducto">El código del producto.</param>
    /// <param name="precio">El precio del producto relacionado con la factura.</param>
    public void InsertarFacturaProducto(string codigoFactura, string codigoProducto, decimal precio)
    {
        using MySqlConnection connection = GetConnection();
        try
        {
            connection.Open();
            using MySqlTransaction transaction = connection.BeginTransaction();
            try
            {
                using MySqlCommand command = new MySqlCommand("InsertarFacturaProducto", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("p_codigoFactura", codigoFactura);
                command.Parameters.AddWithValue("p_codigoProducto", codigoProducto);
                command.Parameters.AddWithValue("p_precio", precio);
                command.ExecuteNonQuery();

                transaction.Commit();
                LogHandler.LogInfo("Producto relacionado exitosamente con su factura en la base de datos.");
            }
            catch (Exception ex)
            {
                LogHandler.LogError($"Error en la transacción.", ex);
                transaction.Rollback();
            }
        }
        catch (Exception ex)
        {
            LogHandler.LogError($"Error al intentar relacionar un producto con su factura.", ex);
        }
    }

    /// <summary>
    /// Elimina un cliente de la base de datos utilizando un procedimiento almacenado.
    /// El ID del cliente se proporciona como parámetro. El método maneja transacciones
    /// de base de datos y registra mensajes de éxito o fracaso utilizando un LogHandler.
    /// </summary>
    /// <param name="id">El ID del cliente que se eliminará.</param>
    public void BorrarCliente(int id)
    {
        using MySqlConnection connection = GetConnection();
        try
        {
            connection.Open();
            using MySqlTransaction transaction = connection.BeginTransaction();
            try
            {
                using MySqlCommand command = new MySqlCommand("BorrarCliente", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("p_id", id);
                command.ExecuteNonQuery();

                transaction.Commit();
                LogHandler.LogInfo("Cliente eliminado exitosamente de la base de datos.");
            }
            catch (Exception ex)
            {
                LogHandler.LogError($"Error en la transacción.", ex);
                transaction.Rollback();
            }
        }
        catch (Exception ex)
        {
            LogHandler.LogError($"Error al intentar eliminar un cliente.", ex);
        }
    }

    /// <summary>
    /// Elimina una factura de la base de datos utilizando un procedimiento almacenado.
    /// El código de factura se proporciona como parámetro. El método maneja transacciones
    /// de base de datos y registra mensajes de éxito o fracaso utilizando un LogHandler.
    /// </summary>
    /// <param name="codigoFactura">El código de la factura que se eliminará.</param>
    public void BorrarFactura(string codigoFactura)
    {
        using MySqlConnection connection = GetConnection();
        try
        {
            connection.Open();
            using MySqlTransaction transaction = connection.BeginTransaction();
            try
            {
                using MySqlCommand command = new MySqlCommand("BorrarFactura", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("p_codigoFactura", codigoFactura);
                command.ExecuteNonQuery();

                transaction.Commit();
                LogHandler.LogInfo("Factura eliminada exitosamente de la base de datos.");
            }
            catch (Exception ex)
            {
                LogHandler.LogError($"Error en la transacción.", ex);
                transaction.Rollback();
            }
        }
        catch (Exception ex)
        {
            LogHandler.LogError($"Error al intentar eliminar una factura.", ex);
        }
    }

    /// <summary>
    /// Elimina un tipo de producto de la base de datos utilizando un procedimiento almacenado.
    /// El ID del tipo de producto se proporciona como parámetro. El método maneja transacciones
    /// de base de datos y registra mensajes de éxito o fracaso utilizando un LogHandler.
    /// </summary>
    /// <param name="id">El ID del tipo de producto que se eliminará.</param>
    public void BorrarTipoProducto(int id)
    {
        using MySqlConnection connection = GetConnection();
        try
        {
            connection.Open();
            using MySqlTransaction transaction = connection.BeginTransaction();
            try
            {
                using MySqlCommand command = new MySqlCommand("BorrarTipoProducto", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("p_id", id);
                command.ExecuteNonQuery();

                transaction.Commit();
                LogHandler.LogInfo("Tipo de producto eliminado exitosamente de la base de datos.");
            }
            catch (Exception ex)
            {
                LogHandler.LogError($"Error en la transacción.", ex);
                transaction.Rollback();
            }
        }
        catch (Exception ex)
        {
            LogHandler.LogError($"Error al intentar eliminar un tipo de producto.", ex);
        }
    }

    /// <summary>
    /// Elimina un producto de la base de datos utilizando un procedimiento almacenado.
    /// El código del producto se proporciona como parámetro. El método maneja transacciones
    /// de base de datos y registra mensajes de éxito o fracaso utilizando un LogHandler.
    /// </summary>
    /// <param name="codigoProducto">El código del producto que se eliminará.</param>
    public void BorrarProducto(string codigoProducto)
    {
        using MySqlConnection connection = GetConnection();
        try
        {
            connection.Open();
            using MySqlTransaction transaction = connection.BeginTransaction();
            try
            {
                using MySqlCommand command = new MySqlCommand("BorrarProducto", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("p_codigoProducto", codigoProducto);
                command.ExecuteNonQuery();

                transaction.Commit();
                LogHandler.LogInfo("Producto eliminado exitosamente de la base de datos.");
            }
            catch (Exception ex)
            {
                LogHandler.LogError($"Error en la transacción.", ex);
                transaction.Rollback();
            }
        }
        catch (Exception ex)
        {
            LogHandler.LogError($"Error al intentar eliminar un producto.", ex);
        }
    }

    /// <summary>
    /// Elimina la relación entre un producto y una factura de la base de datos utilizando un procedimiento almacenado.
    /// El ID de la relación se proporciona como parámetro. El método maneja transacciones de base de datos y registra
    /// mensajes de éxito o fracaso utilizando un LogHandler.
    /// </summary>
    /// <param name="id">El ID de la relación entre producto y factura que se eliminará.</param>
    public void BorrarFacturaProducto(int id)
    {
        using MySqlConnection connection = GetConnection();
        try
        {
            connection.Open();
            using MySqlTransaction transaction = connection.BeginTransaction();
            try
            {
                using MySqlCommand command = new MySqlCommand("BorrarFacturaProducto", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("p_id", id);
                command.ExecuteNonQuery();

                transaction.Commit();
                LogHandler.LogInfo("Relacion entre producto y factura eliminada exitosamente de la base de datos.");
            }
            catch (Exception ex)
            {
                LogHandler.LogError($"Error en la transacción.", ex);
                transaction.Rollback();
            }
        }
        catch (Exception ex)
        {
            LogHandler.LogError($"Error al intentar eliminar una relacion entre producto y factura.", ex);
        }
    }

    /// <summary>
    /// Obtiene una lista de clientes desde la base de datos utilizando un procedimiento almacenado.
    /// El método maneja la conexión a la base de datos, ejecuta la consulta y mapea los resultados
    /// a objetos de tipo Cliente. En caso de error, registra el problema utilizando un LogHandler.
    /// </summary>
    /// <returns>Una lista de objetos de tipo Cliente.</returns>
    public List<Cliente> ObtenerClientes()
    {
        List<Cliente> clientes = new();
        using MySqlConnection connection = GetConnection();
        try
        {
            connection.Open();
            using MySqlCommand command = new MySqlCommand("ObtenerClientes", connection);
            command.CommandType = CommandType.StoredProcedure;
            using MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                Cliente cliente = new()
                {
                    Id = Convert.ToInt32(reader["id"]),
                    Nombre = reader["nombre"].ToString(),
                    Apellido = reader["apellido"].ToString(),
                    Sexo = reader["sexo"].ToString(),
                    Telefono = reader["telefono"].ToString(),
                    Correo = reader["correo"].ToString(),
                };
                clientes.Add(cliente);
            }

            connection.Close();
        }
        catch (Exception ex)
        {
            LogHandler.LogError($"Error al obtener clientes.", ex);
        }

        return clientes;
    }

    /// <summary>
    /// Obtiene una lista de facturas desde la base de datos utilizando un procedimiento almacenado.
    /// El método maneja la conexión a la base de datos, ejecuta la consulta y mapea los resultados
    /// a objetos de tipo Factura. En caso de error, registra el problema utilizando un LogHandler.
    /// </summary>
    /// <returns>Una lista de objetos de tipo Factura.</returns>
    public List<Factura> ObtenerFacturas()
    {
        List<Factura> facturas = new();
        using MySqlConnection connection = GetConnection();
        try
        {
            connection.Open();
            using MySqlCommand command = new MySqlCommand("ObtenerFacturas", connection);
            command.CommandType = CommandType.StoredProcedure;
            using MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                Factura factura = new()
                {
                    CodigoFactura = reader["codigoFactura"].ToString(),
                    IdCliente = Convert.ToInt32(reader["idCliente"]),
                    PorcentajeImpuesto = Convert.ToInt32(reader["porcentajeImpuesto"]),
                    Subtotal = Convert.ToDecimal(reader["subtotal"]),
                    Total = Convert.ToDecimal(reader["total"]),
                    DescuentoTotal = Convert.ToDecimal(reader["descuentoTotal"]),
                    ImpuestoTotal = Convert.ToDecimal(reader["impuestoTotal"])
                };
                facturas.Add(factura);
            }

            connection.Close();
        }
        catch (Exception ex)
        {
            LogHandler.LogError($"Error al obtener facturas.", ex);
        }

        return facturas;
    }

    /// <summary>
    /// Obtiene una lista de tipos de producto desde la base de datos utilizando un procedimiento almacenado.
    /// El método maneja la conexión a la base de datos, ejecuta la consulta y mapea los resultados
    /// a objetos de tipo TipoProducto. En caso de error, registra el problema utilizando un LogHandler.
    /// </summary>
    /// <returns>Una lista de objetos de tipo TipoProducto.</returns>
    public List<TipoProducto> ObtenerTiposProducto()
    {
        List<TipoProducto> tiposProducto = new();
        using MySqlConnection connection = GetConnection();
        try
        {
            connection.Open();
            using MySqlCommand command = new MySqlCommand("ObtenerTiposProducto", connection);
            command.CommandType = CommandType.StoredProcedure;
            using MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                TipoProducto tipoProducto = new()
                {
                    Id = Convert.ToInt32(reader["id"]),
                    NombreTipo = reader["nombreTipo"].ToString()
                };
                tiposProducto.Add(tipoProducto);
            }

            connection.Close();
        }
        catch (Exception ex)
        {
            LogHandler.LogError($"Error al obtener tipos de producto.", ex);
        }

        return tiposProducto;
    }

    /// <summary>
    /// Obtiene una lista de productos desde la base de datos utilizando un procedimiento almacenado.
    /// El método maneja la conexión a la base de datos, ejecuta la consulta y mapea los resultados
    /// a objetos de tipo Producto. En caso de error, registra el problema utilizando un LogHandler.
    /// </summary>
    /// <returns>Una lista de objetos de tipo Producto.</returns>
    public List<Producto> ObtenerProductos()
    {
        List<Producto> productos = new();
        using MySqlConnection connection = GetConnection();
        try
        {
            connection.Open();
            using MySqlCommand command = new MySqlCommand("ObtenerProductos", connection);
            command.CommandType = CommandType.StoredProcedure;
            using MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                Producto producto = new()
                {
                    CodigoProducto = reader["codigoProducto"].ToString(),
                    IdTipoProducto = Convert.ToInt32(reader["idTipoProducto"]),
                    Nombre = reader["nombre"].ToString(),
                    Precio = Convert.ToDecimal(reader["precio"]),
                    PorcentajeDescuento = Convert.ToInt32(reader["porcentajeDescuento"])
                };
                productos.Add(producto);
            }

            connection.Close();
        }
        catch (Exception ex)
        {
            LogHandler.LogError($"Error al obtener productos.", ex);
        }

        return productos;
    }

    /// <summary>
    /// Obtiene una lista de relaciones entre facturas y productos desde la base de datos
    /// utilizando un procedimiento almacenado. El método maneja la conexión a la base de datos,
    /// ejecuta la consulta y mapea los resultados a objetos de tipo FacturaProducto. En caso
    /// de error, registra el problema utilizando un LogHandler.
    /// </summary>
    /// <returns>Una lista de objetos de tipo FacturaProducto.</returns>
    public List<FacturaProducto> ObtenerFacturasProductos()
    {
        List<FacturaProducto> facturasProductos = new();
        using MySqlConnection connection = GetConnection();
        try
        {
            connection.Open();
            using MySqlCommand command = new MySqlCommand("ObtenerFacturasProductos", connection);
            command.CommandType = CommandType.StoredProcedure;
            using MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                FacturaProducto facturaProducto = new()
                {
                    Id = Convert.ToInt32(reader["id"]),
                    CodigoFactura = reader["codigoFactura"].ToString(),
                    CodigoProducto = reader["codigoProducto"].ToString(),
                    Precio = Convert.ToDecimal(reader["precio"]),
                    Descuento = Convert.ToDecimal(reader["descuento"])
                };
                facturasProductos.Add(facturaProducto);
            }

            connection.Close();
        }
        catch (Exception ex)
        {
            LogHandler.LogError($"Error en la conexión.", ex);
        }

        return facturasProductos;
    }
}
