// IDatabaseService.cs

using System.Data;
using System.Threading.Tasks;

namespace CargaImagenes.Data
{
    public interface IDatabaseService
    {
        DataTable ExecuteQuery(string query);
        DataTable ExecuteQueryWithParameters(string query, Dictionary<string, object>? parameters = null);
        int ExecuteNonQuery(string query, Dictionary<string, object>? parameters = null);
        object? ExecuteScalar(string query, Dictionary<string, object>? parameters = null);

        Task<DataTable> ExecuteQueryAsync(string query);
        Task<DataTable> ExecuteQueryWithParametersAsync(string query, Dictionary<string, object>? parameters = null);
        Task<int> ExecuteNonQueryAsync(string query, Dictionary<string, object>? parameters = null);
        Task<object?> ExecuteScalarAsync(string query, Dictionary<string, object>? parameters = null);
    }
}
