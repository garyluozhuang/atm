// <copyright file="Administrator.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ATMSimulation.Entities;
using ATMSimulation.DatabaseConnection;

public class Administrator(string login, string pinCode) : User(login, pinCode)
{
    public static void CreateAccount(IDatabaseConnection databaseConnection, string newLogin, string pinCode, string holderName)
    {
        var command = databaseConnection.CreateCommand();
        command.CommandText = "SELECT COUNT(*) FROM customer WHERE login = @login";
        command.Parameters.AddWithValue("@login", newLogin);

        var count = (long)command.ExecuteScalar();

        if (count > 0)
        {
            Console.WriteLine("Account with this login already exists.");
        }
        else
        {
            command.CommandText = "INSERT INTO customer (login, pin_code, holder_name, balance) VALUES (@login, @pin, @holder_name, 0)";
            command.Parameters.AddWithValue("@pin", pinCode);
            command.Parameters.AddWithValue("@holder_name", holderName);

            command.ExecuteNonQuery();
            Console.WriteLine("Account created successfully.");
        }
    }

    public static void DeleteAccount(IDatabaseConnection databaseConnection, string loginToDelete)
    {
        var command = databaseConnection.CreateCommand();
        command.CommandText = "DELETE FROM customer WHERE login = @login";
        command.Parameters.AddWithValue("@login", loginToDelete);
        var rowsAffected = command.ExecuteNonQuery();

        if (rowsAffected > 0)
        {
            Console.WriteLine("Account deleted successfully.");
        }
        else
        {
            Console.WriteLine("Account not found.");
        }
    }

    public static void UpdateAccount(IDatabaseConnection databaseConnection, string loginToUpdate, string newPinCode)
    {
        var command = databaseConnection.CreateCommand();
        command.CommandText = "UPDATE customer SET pin_code = @newPin WHERE login = @login";
        command.Parameters.AddWithValue("@login", loginToUpdate);
        command.Parameters.AddWithValue("@newPin", newPinCode);
        var rowsAffected = command.ExecuteNonQuery();

        if (rowsAffected > 0)
        {
            Console.WriteLine("Account updated successfully.");
        }
        else
        {
            Console.WriteLine("Account not found.");
        }
    }

    public static void SearchAccount(IDatabaseConnection databaseConnection, string loginToSearch)
    {
        var command = databaseConnection.CreateCommand();
        command.CommandText = "SELECT login, pin_code, balance FROM customer WHERE login = @login";
        command.Parameters.AddWithValue("@login", loginToSearch);
        var reader = command.ExecuteReader();

        if (reader.Read())
        {
            var login = reader.GetString(0);
            var pinCode = reader.GetString(1);
            var balance = reader.GetDecimal(2);
            Console.WriteLine($"Login: {login}, PIN: {pinCode}, Balance: {balance}");
        }
        else
        {
            Console.WriteLine("Account not found.");
        }

        reader.Close();
    }
}
