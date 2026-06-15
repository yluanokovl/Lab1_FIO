
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Security.Cryptography;

Console.Write("Логин: ");
string login = Console.ReadLine();

Console.Write("Пароль: ");
string password = Console.ReadLine();

Console.Write("Подтверждение пароля: ");
string confirmPassword = Console.ReadLine();

var result = ValidateRegistration(
    login,
    password,
    confirmPassword);

Console.WriteLine();
Console.WriteLine($"Результат: {result.Success}");
Console.WriteLine($"Сообщение: {result.Message}");


RegistrationResult ValidateRegistration(
    string login,
    string password,
    string confirmPassword)
{
    try
    {
        string[] existingUsers =
        {
            "admin",
            "root",
            "user123",
            "test_user",
            "manager"
        };

        if (string.IsNullOrWhiteSpace(login))
            return Error("Логин не может быть пустым");

        foreach (var user in existingUsers)
        {
            if (login.Equals(
                user,
                StringComparison.OrdinalIgnoreCase))
            {
                return Error("Логин уже существует");
            }
        }

        bool isPhone =
            Regex.IsMatch(
                login,
                @"^\+\d-\d{3}-\d{3}-\d{4}$");

        bool isEmail =
            Regex.IsMatch(
                login,
                @"^[^@\s]+@[^@\s]+\.[^@\s]+$");

        if (login.Contains("@"))
        {
            if (!isEmail)
                return Error("Некорректный формат email");
        }
        else if (login.StartsWith("+"))
        {
            if (!isPhone)
                return Error("Некорректный формат телефона");
        }
        else
        {
            if (login.Length < 5)
                return Error("Логин должен содержать минимум 5 символов");

            if (!Regex.IsMatch(
                login,
                @"^[A-Za-z0-9_]+$"))
            {
                return Error(
                    "Логин может содержать только латиницу, цифры и _");
            }
        }

        if (password.Length < 7)
            return Error(
                "Пароль должен содержать минимум 7 символов");

        if (password != confirmPassword)
            return Error(
                "Пароль и подтверждение не совпадают");

        if (!Regex.IsMatch(password, @"[А-ЯЁ]"))
            return Error(
                "Пароль должен содержать заглавную кириллическую букву");

        if (!Regex.IsMatch(password, @"[а-яё]"))
            return Error(
                "Пароль должен содержать строчную кириллическую букву");

        if (!Regex.IsMatch(password, @"\d"))
            return Error(
                "Пароль должен содержать цифру");

        if (!Regex.IsMatch(password, @"[!@#$%^&*()_+\-=\[\]{};':"",./<>?\\|]"))
            return Error(
                "Пароль должен содержать спецсимвол");

        if (Regex.IsMatch(password, @"[A-Za-z]"))
            return Error(
                "Пароль может содержать только кириллицу, цифры и спецсимволы");

        string maskedPassword = MaskPassword(password);
        string maskedConfirm = MaskPassword(confirmPassword);

        WriteLog(
            "INFO",
            $"Логин={login}, Пароль={maskedPassword}, Подтверждение={maskedConfirm}, Успешная регистрация");

        return new RegistrationResult
        {
            Success = true,
            Message = ""
        };
    }
    catch (Exception ex)
    {
        WriteLog("ERROR", ex.ToString());

        return new RegistrationResult
        {
            Success = false,
            Message = "Внутренняя ошибка"
        };
    }
}

RegistrationResult Error(string message)
{
    WriteLog(
        "ERROR",
        message);

    return new RegistrationResult
    {
        Success = false,
        Message = message
    };
}

string MaskPassword(string password)
{
    using SHA256 sha = SHA256.Create();

    byte[] hash =
        sha.ComputeHash(
            Encoding.UTF8.GetBytes(password));

    return Convert.ToHexString(hash);
}

void WriteLog(
    string level,
    string message)
{
    string logMessage =
        $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{level}] {message}";

    Console.WriteLine(logMessage);

    File.AppendAllText(
        "registration.log",
        logMessage + Environment.NewLine);
}

class RegistrationResult
{
    public bool Success { get; set; }

    public string Message { get; set; }
}