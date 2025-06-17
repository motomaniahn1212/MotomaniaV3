// IDatabaseService.cs

using System.Data;

namespace CargaImagenes.Data
{
    public interface IDatabaseService
    {
        DataTable ExecuteQuery(string query);
        DataTable ExecuteQueryWithParameters(string query, Dictionary<string, object>? parameters = null);
        int ExecuteNonQuery(string query, Dictionary<string, object>? parameters = null);
        object? ExecuteScalar(string query, Dictionary<string, object>? parameters = null);
    }
}
