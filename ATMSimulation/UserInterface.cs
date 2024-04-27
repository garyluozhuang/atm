// <copyright file="UserInterface.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ATMSimulation.UI;
using ATMSimulation.DatabaseConnection;
using ATMSimulation.Entities;

public interface IUserInterface
{
    void DisplayLoginScreen();
}

public class ConsoleUserInterface : IUserInterface
{
    private readonly IDatabaseConnection databaseConnection;

    public ConsoleUserInterface(IDatabaseConnection databaseConnection)
    {
        this.databaseConnection = databaseConnection;
        this.databaseConnection.OpenConnection();
    }

    public void DisplayLoginScreen()
    {
        string? login;
        string? pinCode;

        do
        {
            Console.Write("Please enter your login: ");
            login = Console.ReadLine();

            Console.Write("Please enter your 5-digit PIN code: ");
            pinCode = Console.ReadLine();

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(pinCode) || pinCode.Length != 5)
            {
                Console.WriteLine("Invalid input. Please try again.");
            }
        }
        while (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(pinCode) || pinCode.Length != 5);

        var user = this.ValidateLogin(login, pinCode);

        if (user != null)
        {
            this.DisplayMenu(user);
        }
        else
        {
            Console.WriteLine("Invalid login or PIN code.");
        }
    }

    private User? ValidateLogin(string login, string pinCode)
    {
        var command = this.databaseConnection.CreateCommand();
        command.Parameters.AddWithValue("@login", login);
        command.Parameters.AddWithValue("@pinCode", pinCode);

        command.CommandText = "SELECT * FROM customer WHERE login = @login AND pin_code = @pinCode";
        var reader = command.ExecuteReader();
        if (reader.Read())
        {
            var balance = reader.GetDecimal("balance");
            reader.Close();
            return new Customer(login, pinCode, balance);
        }

        reader.Close();

        command.CommandText = "SELECT * FROM administrator WHERE login = @login AND pin_code = @pinCode";
        reader = command.ExecuteReader();
        if (reader.Read())
        {
            reader.Close();
            return new Administrator(login, pinCode);
        }

        reader.Close();

        return null;
    }

    private void DisplayMenu(User user)
    {
        if (user is Customer customer)
        {
            this.DisplayCustomerMenu(customer);
        }
        else if (user is Administrator)
        {
            this.DisplayAdminMenu();
        }
    }

    private void DisplayCustomerMenu(Customer customer)
    {
        var exit = false;

        while (!exit)
        {
            Console.WriteLine("Please select an option:");
            Console.WriteLine("1 - Withdraw");
            Console.WriteLine("2 - Deposit");
            Console.WriteLine("3 - Show Balance");
            Console.WriteLine("4 - Exit");

            var input = Console.ReadLine();

            if (int.TryParse(input, out var option) && option >= 1 && option <= 4)
            {
                switch (option)
                {
                    case 1:
                        decimal withdrawAmmount;
                        while (true)
                        {
                            Console.Write("Enter amount to withdraw: ");
                            if (decimal.TryParse(Console.ReadLine(), out withdrawAmmount) && withdrawAmmount > 0)
                            {
                                break;
                            }

                            Console.WriteLine("Invalid amount. Please try again.");
                        }

                        customer.WithdrawFunds(this.databaseConnection, withdrawAmmount);
                        break;
                    case 2:
                        decimal depositAmmount;
                        while (true)
                        {
                            Console.Write("Enter amount to deposit: ");
                            if (decimal.TryParse(Console.ReadLine(), out depositAmmount) && depositAmmount > 0)
                            {
                                break;
                            }

                            Console.WriteLine("Invalid amount. Please try again.");
                        }

                        customer.DepositFunds(this.databaseConnection, depositAmmount);
                        break;
                    case 3:
                        Console.WriteLine($"Your current balance is: ${customer.Balance}");
                        break;
                    case 4:
                        exit = true;
                        this.databaseConnection.CloseConnection();
                        break;
                    default:
                        break;
                }
            }
            else
            {
                Console.WriteLine("Invalid option. Please try again.");
            }
        }
    }

    private void DisplayAdminMenu()
    {
        var exit = false;

        while (!exit)
        {
            Console.WriteLine("Please select an option:");
            Console.WriteLine("1 - Create New Account");
            Console.WriteLine("2 - Delete Existing Account");
            Console.WriteLine("3 - Update Account Information");
            Console.WriteLine("4 - Search Account");
            Console.WriteLine("5 - Exit");

            var input = Console.ReadLine();

            if (int.TryParse(input, out var option) && option >= 1 && option <= 5)
            {
                switch (option)
                {
                    case 1:
                        string? newLogin;
                        string? pinCode;
                        string? holderName;
                        do
                        {
                            Console.Write("Enter new customer login: ");
                            newLogin = Console.ReadLine();

                            Console.Write("Enter new PIN code (5 digits): ");
                            pinCode = Console.ReadLine();

                            Console.Write("Enter new holder name: ");
                            holderName = Console.ReadLine();

                            if (string.IsNullOrEmpty(newLogin) || string.IsNullOrEmpty(pinCode) || pinCode.Length != 5 || string.IsNullOrEmpty(holderName))
                            {
                                Console.WriteLine("Invalid input. Please try again.");
                            }
                        }
                        while (string.IsNullOrEmpty(newLogin) || string.IsNullOrEmpty(pinCode) || pinCode.Length != 5 || string.IsNullOrEmpty(holderName));
                        Administrator.CreateAccount(this.databaseConnection, newLogin, pinCode, holderName);
                        break;
                    case 2:
                        string? loginToDelete;
                        do
                        {
                            Console.Write("Enter customer login to delete: ");
                            loginToDelete = Console.ReadLine();

                            if (string.IsNullOrEmpty(loginToDelete))
                            {
                                Console.WriteLine("Invalid input. Please try again.");
                            }
                        }
                        while (string.IsNullOrEmpty(loginToDelete));
                        Administrator.DeleteAccount(this.databaseConnection, loginToDelete);
                        break;
                    case 3:
                        string? loginToUpdate;
                        string? newPinCode;
                        do
                        {
                            Console.Write("Enter customer login to update: ");
                            loginToUpdate = Console.ReadLine();

                            if (string.IsNullOrEmpty(loginToUpdate))
                            {
                                Console.WriteLine("Invalid input. Please try again.");
                            }
                        }
                        while (string.IsNullOrEmpty(loginToUpdate));

                        do
                        {
                            Console.Write("Enter new PIN code (5 digits): ");
                            newPinCode = Console.ReadLine();

                            if (string.IsNullOrEmpty(newPinCode) || newPinCode.Length != 5)
                            {
                                Console.WriteLine("Invalid input. Please try again.");
                            }
                        }
                        while (string.IsNullOrEmpty(newPinCode) || newPinCode.Length != 5);
                        Administrator.UpdateAccount(this.databaseConnection, loginToUpdate, newPinCode);
                        break;
                    case 4:
                        string? loginToSearch;

                        do
                        {
                            Console.Write("Enter customer login to search: ");
                            loginToSearch = Console.ReadLine();

                            if (string.IsNullOrEmpty(loginToSearch))
                            {
                                Console.WriteLine("Invalid input. Please try again.");
                            }
                        }
                        while (string.IsNullOrEmpty(loginToSearch));
                        Administrator.SearchAccount(this.databaseConnection, loginToSearch);
                        break;
                    case 5:
                        exit = true;
                        this.databaseConnection.CloseConnection();
                        break;
                    default:
                        break;
                }
            }
            else
            {
                Console.WriteLine("Invalid option. Please try again.");
            }
        }
    }
}
