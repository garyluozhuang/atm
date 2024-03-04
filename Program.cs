using System;
using MySql.Data.MySqlClient;
using Ninject;
using Ninject.Modules;

namespace ATMSimulation
{
    class Program
    {
        static void Main(string[] args)
        {
            IKernel kernel = new StandardKernel(new ATMModule());
            var atm = kernel.Get<ATM>();
            atm.Start();
        }
    }

    public class ATMModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IDatabaseConnection>().To<MySQLDatabaseConnection>();
            Bind<IUserInterface>().To<ConsoleUserInterface>();
        }
    }

    public class ATM
    {
        // private readonly IDatabaseConnection _databaseConnection;
        private readonly IUserInterface _userInterface;

        public ATM(IUserInterface userInterface)
        {
            _userInterface = userInterface;
        }

        public void Start()
        {
            _userInterface.DisplayLoginScreen();
        }
    }
    
    public interface IDatabaseConnection
    {
        void OpenConnection();
        void CloseConnection();
        MySqlCommand CreateCommand();
    }
    public class MySQLDatabaseConnection : IDatabaseConnection
    {
        private readonly string _connectionString = "server=localhost;user=root;database=atm;port=3306";
        private MySqlConnection _connection;

        public MySQLDatabaseConnection()
        {
            _connection = new MySqlConnection(_connectionString);
        }

        public void OpenConnection()
        {
            _connection.Open();
        }

        public void CloseConnection()
        {
            _connection?.Close();
            _connection?.Dispose();
        }

        public MySqlCommand CreateCommand()
        {
            if (_connection.State != System.Data.ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection is not open.");
            }

            return _connection.CreateCommand();
        }
    }

    public interface IUserInterface
    {
        void DisplayLoginScreen();
    }
    public class ConsoleUserInterface : IUserInterface
    {
        private readonly IDatabaseConnection _databaseConnection;

        public ConsoleUserInterface(IDatabaseConnection databaseConnection)
        {
            _databaseConnection = databaseConnection;
            _databaseConnection.OpenConnection();
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
            } while (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(pinCode) || pinCode.Length != 5);

            User? user = ValidateLogin(login, pinCode);

            if (user != null)
            {
                DisplayMenu(user);
            }
            else
            {
                Console.WriteLine("Invalid login or PIN code.");
            }
        }

        private User? ValidateLogin(string login, string pinCode)
        {
            try
            {
                var command = _databaseConnection.CreateCommand();
                command.CommandText = "SELECT * FROM customer WHERE login = @login AND pin_code = @pinCode";
                command.Parameters.AddWithValue("@login", login);
                command.Parameters.AddWithValue("@pinCode", pinCode);
                var reader = command.ExecuteReader();

                if (reader.Read())
                {
                    string holderName = reader.GetString("holder_name");
                    decimal balance = reader.GetDecimal("balance");
                    reader.Close();
                    return new Customer(login, pinCode, holderName, balance);
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
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database query error: {ex.Message}");
            }

            return null;
        }

        private void DisplayMenu(User user)
        {
            if (user is Customer customer)
            {
                DisplayCustomerMenu(customer);
            }
            else if (user is Administrator administrator)
            {
                DisplayAdminMenu(administrator);
            }
        }

        private void DisplayCustomerMenu(Customer customer)
        {
            bool exit = false;

            while (!exit)
            {
                Console.WriteLine("Please select an option:");
                Console.WriteLine("1 - Withdraw");
                Console.WriteLine("2 - Deposit");
                Console.WriteLine("3 - Show Balance");
                Console.WriteLine("4 - Exit");

                string? input = Console.ReadLine();

                if (int.TryParse(input, out int option) && option >= 1 && option <= 4)
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
                            customer.WithdrawFunds(_databaseConnection, withdrawAmmount);
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
                            customer.DepositFunds(_databaseConnection, depositAmmount);
                            break;
                        case 3:
                            Console.WriteLine($"Your current balance is: ${customer.Balance}"); ;
                            break;
                        case 4:
                            exit = true;
                            _databaseConnection.CloseConnection();
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid option. Please try again.");
                }
            }
        }

        private void DisplayAdminMenu(Administrator administrator)
        {
            bool exit = false;

            while (!exit)
            {
                Console.WriteLine("Please select an option:");
                Console.WriteLine("1 - Create New Account");
                Console.WriteLine("2 - Delete Existing Account");
                Console.WriteLine("3 - Update Account Information");
                Console.WriteLine("4 - Search Account");
                Console.WriteLine("5 - Exit");

                string? input = Console.ReadLine();

                if (int.TryParse(input, out int option) && option >= 1 && option <= 5)
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
                            } while (string.IsNullOrEmpty(newLogin) || string.IsNullOrEmpty(pinCode) || pinCode.Length != 5 || string.IsNullOrEmpty(holderName));
                            administrator.CreateAccount(_databaseConnection, newLogin, pinCode, holderName);
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
                            } while (string.IsNullOrEmpty(loginToDelete));
                            administrator.DeleteAccount(_databaseConnection, loginToDelete);
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
                            } while (string.IsNullOrEmpty(loginToUpdate));

                            do
                            {
                                Console.Write("Enter new PIN code (5 digits): ");
                                newPinCode = Console.ReadLine();

                                if (string.IsNullOrEmpty(newPinCode) || newPinCode.Length != 5)
                                {
                                    Console.WriteLine("Invalid input. Please try again.");
                                }
                            } while (string.IsNullOrEmpty(newPinCode) || newPinCode.Length != 5);
                            administrator.UpdateAccount(_databaseConnection, loginToUpdate, newPinCode);
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
                            } while (string.IsNullOrEmpty(loginToSearch));
                            administrator.SearchAccount(_databaseConnection, loginToSearch);
                            break;
                        case 5:
                            exit = true;
                            _databaseConnection.CloseConnection();
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

    public abstract class User(string login, string pinCode)
    {
        public string Login { get; } = login;
        public string PinCode { get; } = pinCode;
    }
    public class Customer(string login, string pinCode, string holderName, decimal balance) : User(login, pinCode)
    {
        public string HolderName { get; } = holderName;
        private decimal _balance = balance;

        public decimal Balance
        {
            get
            {
                // 实时从数据库获取余额
                var databaseConnection = new MySQLDatabaseConnection();
                databaseConnection.OpenConnection();

                try
                {
                    var command = databaseConnection.CreateCommand();
                    command.CommandText = "SELECT balance FROM customer WHERE login = @login";
                    command.Parameters.AddWithValue("@login", Login);
                    _balance = command.ExecuteScalar().ToDecimalSafe();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
                finally
                {
                    databaseConnection.CloseConnection();
                }

                return _balance;
            }
        }

        public void WithdrawFunds(IDatabaseConnection databaseConnection, decimal amount)
        {
            try
            {
                var command = databaseConnection.CreateCommand();
                command.CommandText = "UPDATE customer SET balance = balance - @amount WHERE login = @login";
                command.Parameters.AddWithValue("@login", Login);
                command.Parameters.AddWithValue("@amount", amount);
                command.ExecuteNonQuery();
                _balance -= amount;
                Console.WriteLine($"Withdrew ${amount}. New balance: ${_balance}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public void DepositFunds(IDatabaseConnection databaseConnection, decimal amount)
        {
            try
            {
                var command = databaseConnection.CreateCommand();
                command.CommandText = "UPDATE customer SET balance = balance + @amount WHERE login = @login";
                command.Parameters.AddWithValue("@login", Login);
                command.Parameters.AddWithValue("@amount", amount);
                command.ExecuteNonQuery();
                _balance += amount;
                Console.WriteLine($"Deposited ${amount}. New balance: ${_balance}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
    public class Administrator(string login, string pinCode) : User(login, pinCode)
    {
        public void CreateAccount(IDatabaseConnection databaseConnection, string newLogin, string pinCode, string holderName)
        {
            try
            {
                var command = databaseConnection.CreateCommand();
                command.CommandText = "INSERT INTO customer (login, pin_code, holder_name, balance) VALUES (@login, @pin, @holder_name, 0)";
                command.Parameters.AddWithValue("@login", newLogin);
                command.Parameters.AddWithValue("@pin", pinCode);
                command.Parameters.AddWithValue("@holder_name", holderName);

                command.ExecuteNonQuery();
                Console.WriteLine("Account created successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public void DeleteAccount(IDatabaseConnection databaseConnection, string loginToDelete)
        {
            try
            {
                var command = databaseConnection.CreateCommand();
                command.CommandText = "DELETE FROM customer WHERE login = @login";
                command.Parameters.AddWithValue("@login", loginToDelete);
                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    Console.WriteLine("Account deleted successfully.");
                }
                else
                {
                    Console.WriteLine("Account not found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public void UpdateAccount(IDatabaseConnection databaseConnection, string loginToUpdate, string newPinCode)
        {
            try
            {
                var command = databaseConnection.CreateCommand();
                command.CommandText = "UPDATE customer SET pin_code = @newPin WHERE login = @login";
                command.Parameters.AddWithValue("@login", loginToUpdate);
                command.Parameters.AddWithValue("@newPin", newPinCode);
                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    Console.WriteLine("Account updated successfully.");
                }
                else
                {
                    Console.WriteLine("Account not found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public void SearchAccount(IDatabaseConnection databaseConnection, string loginToSearch)
        {
            try
            {
                var command = databaseConnection.CreateCommand();
                command.CommandText = "SELECT login, pin_code, balance FROM customer WHERE login = @login";
                command.Parameters.AddWithValue("@login", loginToSearch);
                var reader = command.ExecuteReader();

                if (reader.Read())
                {
                    string login = reader.GetString(0);
                    string pinCode = reader.GetString(1);
                    decimal balance = reader.GetDecimal(2);
                    Console.WriteLine($"Login: {login}, PIN: {pinCode}, Balance: {balance}");
                }
                else
                {
                    Console.WriteLine("Account not found.");
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }

    public static class Extensions
    {
        public static decimal ToDecimalSafe(this object value)
        {
            try
            {
                return Convert.ToDecimal(value);
            }
            catch (Exception)
            {
                return 0;
            }
        }
    }
}


