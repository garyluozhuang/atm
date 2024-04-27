namespace ATMSimulationTest;
using System.Text;
using ATMSimulation;
using ATMSimulation.DatabaseConnection;
using ATMSimulation.UI;
using Moq;



public class ATMSimulationTest
{


    [Fact]
    public void ATMTest()
    {
        var mockUserInterface = new Mock<IUserInterface>();
        var atm = new ATMhhh(mockUserInterface.Object);
        atm.Start();
        mockUserInterface.Verify(m => m.DisplayLoginScreen(), Times.Once);
    }

    [Fact]
    public void DisplayLoginScreenValidLoginAndPINShouldDisplayCustomerMenu()
    {
        var input = new StringReader("john_doe\n12345\n4");
        Console.SetIn(input);
        var output = new StringWriter();
        Console.SetOut(output);
        var consoleUI = new ConsoleUserInterface(new MySQLDatabaseConnection());

        consoleUI.DisplayLoginScreen();
        var actualOutput = output.ToString();

        var expectedOutput = "Please enter your login: Please enter your 5-digit PIN code: Please select an option:\n1 - Withdraw\n2 - Deposit\n3 - Show Balance\n4 - Exit\n";
        // Assert
        Assert.Equal(expectedOutput, actualOutput);
    }

    [Fact]
    public void DisplayLoginScreenValidLoginAndPINShouldDisplayAdminMen()
    {
        var input = new StringReader("admin1\n11111\n5");
        Console.SetIn(input);
        var output = new StringWriter();
        Console.SetOut(output);
        var consoleUI = new ConsoleUserInterface(new MySQLDatabaseConnection());

        consoleUI.DisplayLoginScreen();
        var actualOutput = output.ToString();

        var expectedOutput = "Please enter your login: Please enter your 5-digit PIN code: Please select an option:\n1 - Create New Account\n2 - Delete Existing Account\n3 - Update Account Information\n4 - Search Account\n5 - Exit\n";
        // Assert
        Assert.Equal(expectedOutput, actualOutput);
    }

    [Fact]
    public void DisplayLoginScreenInvalidPINShouldDisplayErrorMessage()
    {
        var input = new StringReader("admin\n1111\nadmin1\n11111\n5");
        Console.SetIn(input);
        var output = new StringWriter();
        Console.SetOut(output);
        var consoleUI = new ConsoleUserInterface(new MySQLDatabaseConnection());

        consoleUI.DisplayLoginScreen();
        var actualOutput = output.ToString();

        var expectedOutput = "Please enter your login: Please enter your 5-digit PIN code: Invalid input. Please try again.\nPlease enter your login: Please enter your 5-digit PIN code: Please select an option:\n1 - Create New Account\n2 - Delete Existing Account\n3 - Update Account Information\n4 - Search Account\n5 - Exit\n";
        // Assert
        Assert.Equal(expectedOutput, actualOutput);
    }

    [Fact]
    public void DisplayLoginScreenInvalidLoginShouldDisplayErrorMessage()
    {
        var input = new StringReader("aaaaa\n11111\n");
        Console.SetIn(input);
        var output = new StringWriter();
        Console.SetOut(output);
        var consoleUI = new ConsoleUserInterface(new MySQLDatabaseConnection());

        consoleUI.DisplayLoginScreen();
        var actualOutput = output.ToString();

        var expectedOutput = "Please enter your login: Please enter your 5-digit PIN code: Invalid login or PIN code.\n";
        // Assert
        Assert.Equal(expectedOutput, actualOutput);
    }

    [Fact]
    public void DisplayLoginScreenCustomerWithdrawTest()
    {
        var input = new StringReader("john_doe\n12345\n1\n100\n4");
        Console.SetIn(input);
        var output = new StringWriter();
        Console.SetOut(output);
        var consoleUI = new ConsoleUserInterface(new MySQLDatabaseConnection());

        consoleUI.DisplayLoginScreen();
        var actualOutput = output.ToString();

        var expectedOutput = "Please enter your login: Please enter your 5-digit PIN code: Please select an option:\n1 - Withdraw\n2 - Deposit\n3 - Show Balance\n4 - Exit\nEnter amount to withdraw: Withdrew $100. New balance:";
        // Assert
        Assert.Contains(expectedOutput, actualOutput);
    }

    [Fact]
    public void DisplayLoginScreenCustomerWithdrawInvalidNumberTest()
    {
        var input = new StringReader("john_doe\n12345\n1\n-1\n100\n4");
        Console.SetIn(input);
        var output = new StringWriter();
        Console.SetOut(output);
        var consoleUI = new ConsoleUserInterface(new MySQLDatabaseConnection());

        consoleUI.DisplayLoginScreen();
        var actualOutput = output.ToString();

        var expectedOutput = "Please enter your login: Please enter your 5-digit PIN code: Please select an option:\n1 - Withdraw\n2 - Deposit\n3 - Show Balance\n4 - Exit\nEnter amount to withdraw: Invalid amount. Please try again.";
        // Assert
        Assert.Contains(expectedOutput, actualOutput);
    }

    [Fact]
    public void DisplayLoginScreenCustomerWithdrawInsufficientFundsTest()
    {
        var input = new StringReader("john_doe\n12345\n1\n100000\n100\n4");
        Console.SetIn(input);
        var output = new StringWriter();
        Console.SetOut(output);
        var consoleUI = new ConsoleUserInterface(new MySQLDatabaseConnection());

        consoleUI.DisplayLoginScreen();
        var actualOutput = output.ToString();

        var expectedOutput = "Please enter your login: Please enter your 5-digit PIN code: Please select an option:\n1 - Withdraw\n2 - Deposit\n3 - Show Balance\n4 - Exit\nEnter amount to withdraw: Insufficient funds. Please try again.";
        // Assert
        Assert.Contains(expectedOutput, actualOutput);
    }

    [Fact]
    public void DisplayLoginScreenCustomerDepositTest()
    {
        var input = new StringReader("john_doe\n12345\n2\n100\n4");
        Console.SetIn(input);
        var output = new StringWriter();
        Console.SetOut(output);
        var consoleUI = new ConsoleUserInterface(new MySQLDatabaseConnection());

        consoleUI.DisplayLoginScreen();
        var actualOutput = output.ToString();

        var expectedOutput = "Please enter your login: Please enter your 5-digit PIN code: Please select an option:\n1 - Withdraw\n2 - Deposit\n3 - Show Balance\n4 - Exit\nEnter amount to deposit: Deposited $100. New balance:";
        // Assert
        Assert.Contains(expectedOutput, actualOutput);
    }

    [Fact]
    public void DisplayLoginScreenCustomerDepositInvalidNumberTest()
    {
        var input = new StringReader("john_doe\n12345\n2\n-1\n100\n4");
        Console.SetIn(input);
        var output = new StringWriter();
        Console.SetOut(output);
        var consoleUI = new ConsoleUserInterface(new MySQLDatabaseConnection());

        consoleUI.DisplayLoginScreen();
        var actualOutput = output.ToString();

        var expectedOutput = "Please enter your login: Please enter your 5-digit PIN code: Please select an option:\n1 - Withdraw\n2 - Deposit\n3 - Show Balance\n4 - Exit\nEnter amount to deposit: Invalid amount. Please try again.";
        // Assert
        Assert.Contains(expectedOutput, actualOutput);
    }

    [Fact]
    public void DisplayLoginScreenCustomerShowBalanceTest()
    {
        var input = new StringReader("john_doe\n12345\n3\n4");
        Console.SetIn(input);
        var output = new StringWriter();
        Console.SetOut(output);
        var consoleUI = new ConsoleUserInterface(new MySQLDatabaseConnection());

        consoleUI.DisplayLoginScreen();
        var actualOutput = output.ToString();

        var expectedOutput = "Please enter your login: Please enter your 5-digit PIN code: Please select an option:\n1 - Withdraw\n2 - Deposit\n3 - Show Balance\n4 - Exit\nYour current balance is:";
        // Assert
        Assert.Contains(expectedOutput, actualOutput);
    }

    [Fact]
    public void DisplayLoginScreenAdminCreateAccountTest()
    {
        var login = GenerateRandomString(8);
        var holderName = GenerateRandomString(12);
        var input = new StringReader("admin1\n11111\n1\n" + login + "\n12345\n" + holderName + "\n5");
        Console.SetIn(input);
        var output = new StringWriter();
        Console.SetOut(output);
        var consoleUI = new ConsoleUserInterface(new MySQLDatabaseConnection());

        consoleUI.DisplayLoginScreen();
        var actualOutput = output.ToString();

        var expectedOutput = "Please enter your login: Please enter your 5-digit PIN code: Please select an option:\n1 - Create New Account\n2 - Delete Existing Account\n3 - Update Account Information\n4 - Search Account\n5 - Exit\nEnter new customer login: Enter new PIN code (5 digits): Enter new holder name: Account created successfully.";
        // Assert
        Assert.Contains(expectedOutput, actualOutput);
    }

    [Fact]
    public void DisplayLoginScreenAdminCreateExistingAccountTest()
    {
        var login = "john_doe";
        var holderName = "John Doe";
        var input = new StringReader("admin1\n11111\n1\n" + login + "\n12345\n" + holderName + "\n5");
        Console.SetIn(input);
        var output = new StringWriter();
        Console.SetOut(output);
        var consoleUI = new ConsoleUserInterface(new MySQLDatabaseConnection());

        consoleUI.DisplayLoginScreen();
        var actualOutput = output.ToString();

        var expectedOutput = "Please enter your login: Please enter your 5-digit PIN code: Please select an option:\n1 - Create New Account\n2 - Delete Existing Account\n3 - Update Account Information\n4 - Search Account\n5 - Exit\nEnter new customer login: Enter new PIN code (5 digits): Enter new holder name: Account with this login already exists.";
        // Assert
        Assert.Contains(expectedOutput, actualOutput);
    }

    [Fact]
    public void DisplayLoginScreenAdminCreateAccountInvalidTest()
    {
        var login = GenerateRandomString(8);
        var holderName = GenerateRandomString(12);
        var input = new StringReader("admin1\n11111\n1\n\n12345\n\n" + login + "\n12345\n" + holderName + "\n5");
        Console.SetIn(input);
        var output = new StringWriter();
        Console.SetOut(output);
        var consoleUI = new ConsoleUserInterface(new MySQLDatabaseConnection());

        consoleUI.DisplayLoginScreen();
        var actualOutput = output.ToString();

        var expectedOutput = "Please enter your login: Please enter your 5-digit PIN code: Please select an option:\n1 - Create New Account\n2 - Delete Existing Account\n3 - Update Account Information\n4 - Search Account\n5 - Exit\nEnter new customer login: Enter new PIN code (5 digits): Enter new holder name: Invalid input. Please try again.";
        // Assert
        Assert.Contains(expectedOutput, actualOutput);
    }

    [Fact]
    public void DisplayLoginScreenAdminDeleteAccountTest()
    {
        var login = GenerateRandomString(8);
        var holderName = GenerateRandomString(12);
        var input = new StringReader("admin1\n11111\n1\n" + login + "\n12345\n" + holderName + "\n2\n\n" + login + "\n5");
        Console.SetIn(input);
        var output = new StringWriter();
        Console.SetOut(output);
        var consoleUI = new ConsoleUserInterface(new MySQLDatabaseConnection());

        consoleUI.DisplayLoginScreen();
        var actualOutput = output.ToString();

        var expectedOutput = "Please enter your login: Please enter your 5-digit PIN code: Please select an option:\n1 - Create New Account\n2 - Delete Existing Account\n3 - Update Account Information\n4 - Search Account\n5 - Exit\nEnter new customer login: Enter new PIN code (5 digits): Enter new holder name: Account created successfully.\nPlease select an option:\n1 - Create New Account\n2 - Delete Existing Account\n3 - Update Account Information\n4 - Search Account\n5 - Exit\nEnter customer login to delete: Invalid input. Please try again.\nEnter customer login to delete: Account deleted successfully.";
        // Assert
        Assert.Contains(expectedOutput, actualOutput);
    }

    [Fact]
    public void DisplayLoginScreenAdminDeleteNonExistingAccountTest()
    {
        var login = GenerateRandomString(8);
        var holderName = GenerateRandomString(12);
        var input = new StringReader("admin1\n11111\n2\n" + login + "\n5");
        Console.SetIn(input);
        var output = new StringWriter();
        Console.SetOut(output);
        var consoleUI = new ConsoleUserInterface(new MySQLDatabaseConnection());

        consoleUI.DisplayLoginScreen();
        var actualOutput = output.ToString();

        var expectedOutput = "Please enter your login: Please enter your 5-digit PIN code: Please select an option:\n1 - Create New Account\n2 - Delete Existing Account\n3 - Update Account Information\n4 - Search Account\n5 - Exit\nEnter customer login to delete: Account not found.";
        // Assert
        Assert.Contains(expectedOutput, actualOutput);
    }

    [Fact]
    public void DisplayLoginScreenAdminUpdateAccountTest()
    {
        var login = GenerateRandomString(8);
        var holderName = GenerateRandomString(12);
        var input = new StringReader("admin1\n11111\n1\n" + login + "\n12345\n" + holderName + "\n3\n\n" + login + "\n54321\n5");
        Console.SetIn(input);
        var output = new StringWriter();
        Console.SetOut(output);
        var consoleUI = new ConsoleUserInterface(new MySQLDatabaseConnection());

        consoleUI.DisplayLoginScreen();
        var actualOutput = output.ToString();

        var expectedOutput = "Please enter your login: Please enter your 5-digit PIN code: Please select an option:\n1 - Create New Account\n2 - Delete Existing Account\n3 - Update Account Information\n4 - Search Account\n5 - Exit\nEnter new customer login: Enter new PIN code (5 digits): Enter new holder name: Account created successfully.\nPlease select an option:\n1 - Create New Account\n2 - Delete Existing Account\n3 - Update Account Information\n4 - Search Account\n5 - Exit\nEnter customer login to update: Invalid input. Please try again.\nEnter customer login to update: Enter new PIN code (5 digits): Account updated successfully.";
        // Assert
        Assert.Contains(expectedOutput, actualOutput);
    }

    [Fact]
    public void DisplayLoginScreenAdminUpdateNonExistingAccountTest()
    {
        var login = GenerateRandomString(8);
        var holderName = GenerateRandomString(12);
        var input = new StringReader("admin1\n11111\n3\n" + login + "\n54321\n5");
        Console.SetIn(input);
        var output = new StringWriter();
        Console.SetOut(output);
        var consoleUI = new ConsoleUserInterface(new MySQLDatabaseConnection());

        consoleUI.DisplayLoginScreen();
        var actualOutput = output.ToString();

        var expectedOutput = "Please enter your login: Please enter your 5-digit PIN code: Please select an option:\n1 - Create New Account\n2 - Delete Existing Account\n3 - Update Account Information\n4 - Search Account\n5 - Exit\nEnter customer login to update: Enter new PIN code (5 digits): Account not found.";
        // Assert
        Assert.Contains(expectedOutput, actualOutput);
    }

    [Fact]
    public void DisplayLoginScreenAdminSearchAccountTest()
    {
        var login = GenerateRandomString(8);
        var holderName = GenerateRandomString(12);
        var input = new StringReader("admin1\n11111\n1\n" + login + "\n12345\n" + holderName + "\n4\n\n" + login + "\n5");
        Console.SetIn(input);
        var output = new StringWriter();
        Console.SetOut(output);
        var consoleUI = new ConsoleUserInterface(new MySQLDatabaseConnection());

        consoleUI.DisplayLoginScreen();
        var actualOutput = output.ToString();

        var expectedOutput = "Please enter your login: Please enter your 5-digit PIN code: Please select an option:\n1 - Create New Account\n2 - Delete Existing Account\n3 - Update Account Information\n4 - Search Account\n5 - Exit\nEnter new customer login: Enter new PIN code (5 digits): Enter new holder name: Account created successfully.\nPlease select an option:\n1 - Create New Account\n2 - Delete Existing Account\n3 - Update Account Information\n4 - Search Account\n5 - Exit\nEnter customer login to search: Invalid input. Please try again.\nEnter customer login to search: Login:";
        // Assert
        Assert.Contains(expectedOutput, actualOutput);
    }

    [Fact]
    public void DisplayLoginScreenAdminSearchNonExistingAccountTest()
    {
        var login = GenerateRandomString(8);
        var holderName = GenerateRandomString(12);
        var input = new StringReader("admin1\n11111\n4\n" + login + "\n5");
        Console.SetIn(input);
        var output = new StringWriter();
        Console.SetOut(output);
        var consoleUI = new ConsoleUserInterface(new MySQLDatabaseConnection());

        consoleUI.DisplayLoginScreen();
        var actualOutput = output.ToString();

        var expectedOutput = "Please enter your login: Please enter your 5-digit PIN code: Please select an option:\n1 - Create New Account\n2 - Delete Existing Account\n3 - Update Account Information\n4 - Search Account\n5 - Exit\nEnter customer login to search: Account not found.";
        // Assert
        Assert.Contains(expectedOutput, actualOutput);
    }

    private static string GenerateRandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var builder = new StringBuilder();
        var random = new Random();

        for (var i = 0; i < length; i++)
        {
            builder.Append(chars[random.Next(chars.Length)]);
        }

        return builder.ToString();
    }

}


