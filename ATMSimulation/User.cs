// <copyright file="User.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ATMSimulation.Entities;

public abstract class User(string login, string pinCode)
{
    public string Login { get; } = login;

    public string PinCode { get; } = pinCode;
}
