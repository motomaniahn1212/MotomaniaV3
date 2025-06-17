using CargaImagenes.Core;
using Microsoft.Data.SqlClient;

// Aquí viven Proveedor, Departamento, Categoria

namespace CargaImagenes.Data
{
    public class RepositorioDatos
    {
        private readonly string _conexion;

        public RepositorioDatos(string conexion)
        {
            _conexion = conexion;
        }

        public List<Proveedor> ObtenerProveedores()
        {
            var lista = new List<Proveedor>();
            using var con = new SqlConnection(_conexion);
            con.Open();
            using var cmd = new SqlCommand(
                "SELECT ID, Name FROM Supplier ORDER BY Name", con);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new Proveedor
                {
                    Id = reader.GetInt32(0),
                    Nombre = reader.GetString(1)
                });
            }
            return lista;
        }

        public List<Departamento> ObtenerDepartamentos()
        {
            var lista = new List<Departamento>();
            using var con = new SqlConnection(_conexion);
            con.Open();
            using var cmd = new SqlCommand(
                "SELECT ID, Name FROM Department ORDER BY Name", con);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new Departamento
                {
                    Id = reader.GetInt32(0),
                    Nombre = reader.GetString(1)
                });
            }
            return lista;
        }

        public List<Categoria> ObtenerCategorias()
        {
            var lista = new List<Categoria>();
            using var con = new SqlConnection(_conexion);
            con.Open();
            using var cmd = new SqlCommand(
                "SELECT ID, Name, DepartmentID FROM Category ORDER BY Name", con);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new Categoria
                {
                    Id = reader.GetInt32(0),
                    Nombre = reader.GetString(1),
                    DepartamentoId = reader.GetInt32(2)
                });
            }
            return lista;
        }
    }
}
