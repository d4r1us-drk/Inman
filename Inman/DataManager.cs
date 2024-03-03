using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using Inman.Models;

namespace Inman;

public class DataManager
{
    private static readonly LogHandler<DataManager> LogHandler = new();
    private static string ConnectionString = string.Empty;

    public DataManager(string connectionString) {
        ConnectionString = connectionString;
    }

    private MySqlConnection GetConnection()
    {
        return new MySqlConnection(ConnectionString);
    }

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
