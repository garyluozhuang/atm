// <copyright file="DatabaseConnection.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ATMSimulation.DatabaseConnection;
using MySql.Data.MySqlClient;

public interface IDatabaseConnection : IDisposable
{
    void OpenConnection();

    void CloseConnection();

    MySqlCommand CreateCommand();
}

public class MySQLDatabaseConnection : IDatabaseConnection
{
    private readonly string connectionString = "server=localhost;user=root;database=atm;port=3306";
    private readonly MySqlConnection connection;

    public MySQLDatabaseConnection() => this.connection = new MySqlConnection(this.connectionString);

    public void OpenConnection() => this.connection.Open();

    public void CloseConnection()
    {
        this.connection.Close();
        this.connection.Dispose();
    }

    public MySqlCommand CreateCommand()
    {
        if (this.connection.State != System.Data.ConnectionState.Open)
        {
            throw new InvalidOperationException("Connection is not open.");
        }

        return this.connection.CreateCommand();
    }

    public void Dispose()
    {
        this.connection.Dispose();
        GC.SuppressFinalize(this);
    }
}
