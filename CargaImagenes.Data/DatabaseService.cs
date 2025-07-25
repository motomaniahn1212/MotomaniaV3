﻿using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace CargaImagenes.Data
{
    public class DatabaseService : IDatabaseService
    {
        private readonly string _connectionString;
        private readonly int _commandTimeout;

        public DatabaseService(ConnectionConfig config)
        {
            if (string.IsNullOrWhiteSpace(config.ConnectionString))
                throw new ArgumentException("La cadena de conexión no puede estar vacía.", nameof(config));
            _connectionString = config.ConnectionString;
            _commandTimeout = config.CommandTimeout;
        }

        private static void AddParameters(SqlCommand command, IDictionary<string, object> parameters)
        {
            if (parameters.ContainsKey("CategoryID") && parameters["CategoryID"] == null)
                throw new ArgumentException("CategoryID no puede ser nulo.", nameof(parameters));

            foreach (var (key, value) in parameters)
                command.Parameters.AddWithValue(key, value ?? DBNull.Value);
        }

        public DataTable ExecuteQuery(string query)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection)
            {
                CommandTimeout = _commandTimeout
            };
            using var adapter = new SqlDataAdapter(command);
            var dt = new DataTable();

            connection.Open();
            adapter.Fill(dt);
            return dt;
        }

        public DataTable ExecuteQueryWithParameters(string query, Dictionary<string, object>? parameters = null)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection)
            {
                CommandTimeout = _commandTimeout
            };

            if (parameters != null)
            {
                AddParameters(command, parameters);
            }

            using var adapter = new SqlDataAdapter(command);
            var dt = new DataTable();

            connection.Open();
            adapter.Fill(dt);
            return dt;
        }

        public int ExecuteNonQuery(string query, Dictionary<string, object>? parameters = null)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection)
            {
                CommandTimeout = _commandTimeout
            };

            if (parameters != null)
            {
                AddParameters(command, parameters);
            }

            connection.Open();
            return command.ExecuteNonQuery();
        }

        public object? ExecuteScalar(string query, Dictionary<string, object>? parameters = null)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection)
            {
                CommandTimeout = _commandTimeout
            };

            if (parameters != null)
            {
                AddParameters(command, parameters);
            }

            connection.Open();
            return command.ExecuteScalar();
        }

        public async Task<DataTable> ExecuteQueryAsync(string query)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            var dt = new DataTable();
            dt.Load(reader);
            return dt;
        }

        public async Task<DataTable> ExecuteQueryWithParametersAsync(string query, Dictionary<string, object>? parameters = null)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);

            if (parameters != null)
            {
                if (parameters.ContainsKey("CategoryID") && parameters["CategoryID"] == null)
                    throw new ArgumentException("CategoryID no puede ser nulo.", nameof(parameters));

                foreach (var (key, value) in parameters)
                    command.Parameters.AddWithValue(key, value ?? DBNull.Value);
            }

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            var dt = new DataTable();
            dt.Load(reader);
            return dt;
        }

        public async Task<int> ExecuteNonQueryAsync(string query, Dictionary<string, object>? parameters = null)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);

            if (parameters != null)
            {
                if (parameters.ContainsKey("CategoryID") && parameters["CategoryID"] == null)
                    throw new ArgumentException("CategoryID no puede ser nulo.", nameof(parameters));

                foreach (var (key, value) in parameters)
                    command.Parameters.AddWithValue(key, value ?? DBNull.Value);
            }

            await connection.OpenAsync();
            return await command.ExecuteNonQueryAsync();
        }

        public async Task<object?> ExecuteScalarAsync(string query, Dictionary<string, object>? parameters = null)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);

            if (parameters != null)
            {
                if (parameters.ContainsKey("CategoryID") && parameters["CategoryID"] == null)
                    throw new ArgumentException("CategoryID no puede ser nulo.", nameof(parameters));

                foreach (var (key, value) in parameters)
                    command.Parameters.AddWithValue(key, value ?? DBNull.Value);
            }

            await connection.OpenAsync();
            return await command.ExecuteScalarAsync();
        }
    }
}
