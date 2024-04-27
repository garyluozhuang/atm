// <copyright file="Customer.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ATMSimulation.Entities;
using System.Globalization;
using ATMSimulation.DatabaseConnection;


public class Customer(string login, string pinCode, decimal balance) : User(login, pinCode)
{
    private decimal balance = balance;

    public decimal Balance
    {
        get
        {
            var databaseConnection = new MySQLDatabaseConnection();
            databaseConnection.OpenConnection();

            var command = databaseConnection.CreateCommand();
            command.CommandText = "SELECT balance FROM customer WHERE login = @login";
            command.Parameters.AddWithValue("@login", this.Login);
            this.balance = Convert.ToDecimal(command.ExecuteScalar(), CultureInfo.InvariantCulture);

            databaseConnection.CloseConnection();

            return this.balance;
        }
    }

    public void WithdrawFunds(IDatabaseConnection databaseConnection, decimal amount)
    {
        var command = databaseConnection.CreateCommand();
        if (this.Balance < amount)
        {
            Console.WriteLine("Insufficient funds. Please try again.");
        }
        else
        {
            command.CommandText = "UPDATE customer SET balance = balance - @amount WHERE login = @login";
            command.Parameters.AddWithValue("@login", this.Login);
            command.Parameters.AddWithValue("@amount", amount);
            command.ExecuteNonQuery();
            this.balance -= amount;
            Console.WriteLine($"Withdrew ${amount}. New balance: ${this.balance}");
        }

    }

    public void DepositFunds(IDatabaseConnection databaseConnection, decimal amount)
    {
        var command = databaseConnection.CreateCommand();
        command.CommandText = "UPDATE customer SET balance = balance + @amount WHERE login = @login";
        command.Parameters.AddWithValue("@login", this.Login);
        command.Parameters.AddWithValue("@amount", amount);
        command.ExecuteNonQuery();
        this.balance += amount;
        Console.WriteLine($"Deposited ${amount}. New balance: ${this.balance}");
    }
}
