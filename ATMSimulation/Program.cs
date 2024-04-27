
using ATMSimulation;
using Ninject;

try
{
    IKernel kernel = new StandardKernel(new ATMModule());
    var atm = kernel.Get<ATM>();
    atm.Start();
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}
// <copyright file="Program.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ATMSimulation
{
    using ATMSimulation.DatabaseConnection;
    using ATMSimulation.UI;
    using Ninject.Modules;

    public class ATMModule : NinjectModule
    {
        public override void Load()
        {
            this.Bind<IDatabaseConnection>().To<MySQLDatabaseConnection>();
            this.Bind<IUserInterface>().To<ConsoleUserInterface>();
        }
    }

    public class ATM(IUserInterface userInterface)
    {
        private readonly IUserInterface userInterface = userInterface;

        public void Start() => this.userInterface.DisplayLoginScreen();
    }
}
