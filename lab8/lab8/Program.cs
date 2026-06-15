using System;
using System.Collections.Generic;
using System.IO;

Console.Write("Введите сторону A: ");
string sideA = Console.ReadLine();

Console.Write("Введите сторону B: ");
string sideB = Console.ReadLine();

Console.Write("Введите сторону C: ");
string sideC = Console.ReadLine();

var result = CalculateTriangle(sideA, sideB, sideC);

Console.WriteLine();
Console.WriteLine($"Тип треугольника: {result.TriangleType}");
Console.WriteLine($"Вершина A: {result.VertexA}");
Console.WriteLine($"Вершина B: {result.VertexB}");
Console.WriteLine($"Вершина C: {result.VertexC}");


// ======================================
// Метод вычисления треугольника
// ======================================

TriangleResult CalculateTriangle(
    string sideA,
    string sideB,
    string sideC)
{
    try
    {
        if (!float.TryParse(sideA, out float a) ||
            !float.TryParse(sideB, out float b) ||
            !float.TryParse(sideC, out float c) ||
            a <= 0 || b <= 0 || c <= 0)
        {
            WriteLog(
                "ERROR",
                $"Невалидные данные: A={sideA}, B={sideB}, C={sideC}");

            return new TriangleResult
            {
                TriangleType = "",
                VertexA = (-2, -2),
                VertexB = (-2, -2),
                VertexC = (-2, -2)
            };
        }

        if (!(a + b > c &&
              a + c > b &&
              b + c > a))
        {
            WriteLog(
                "WARNING",
                $"Не треугольник: A={a}, B={b}, C={c}");

            return new TriangleResult
            {
                TriangleType = "не треугольник",
                VertexA = (-1, -1),
                VertexB = (-1, -1),
                VertexC = (-1, -1)
            };
        }

        string triangleType;

        const float epsilon = 0.0001f;

        bool ab = Math.Abs(a - b) < epsilon;
        bool bc = Math.Abs(b - c) < epsilon;
        bool ac = Math.Abs(a - c) < epsilon;

        if (ab && bc)
            triangleType = "равносторонний";
        else if (ab || bc || ac)
            triangleType = "равнобедренный";
        else
            triangleType = "разносторонний";

        double ax = 0;
        double ay = 0;

        double bx = c;
        double by = 0;

        double x = (b * b + c * c - a * a) / (2 * c);
        double y = Math.Sqrt(Math.Max(0, b * b - x * x));

        var points = new List<(double X, double Y)>
        {
            (ax, ay),
            (bx, by),
            (x, y)
        };

        var coords = ScaleTo100(points);

        var result = new TriangleResult
        {
            TriangleType = triangleType,
            VertexA = coords[0],
            VertexB = coords[1],
            VertexC = coords[2]
        };

        WriteLog(
            "INFO",
            $"Успешный запрос: A={a}, B={b}, C={c}, Тип={triangleType}, Координаты=({result.VertexA}), ({result.VertexB}), ({result.VertexC})");

        return result;
    }
    catch (Exception ex)
    {
        WriteLog(
            "ERROR",
            ex.ToString());

        return new TriangleResult
        {
            TriangleType = "",
            VertexA = (-2, -2),
            VertexB = (-2, -2),
            VertexC = (-2, -2)
        };
    }
}


// ======================================
// Масштабирование в область 100x100
// ======================================

List<(int, int)> ScaleTo100(
    List<(double X, double Y)> points)
{
    double minX = double.MaxValue;
    double maxX = double.MinValue;
    double minY = double.MaxValue;
    double maxY = double.MinValue;

    foreach (var p in points)
    {
        minX = Math.Min(minX, p.X);
        maxX = Math.Max(maxX, p.X);

        minY = Math.Min(minY, p.Y);
        maxY = Math.Max(maxY, p.Y);
    }

    double width = Math.Max(maxX - minX, 1);
    double height = Math.Max(maxY - minY, 1);

    double scale = Math.Min(
        100.0 / width,
        100.0 / height);

    var result = new List<(int, int)>();

    foreach (var p in points)
    {
        int sx = (int)Math.Round(
            (p.X - minX) * scale);

        int sy = (int)Math.Round(
            (p.Y - minY) * scale);

        result.Add((sx, sy));
    }

    return result;
}


// ======================================
// Логирование
// ======================================

void WriteLog(string level, string message)
{
    string logMessage =
        $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{level}] {message}";

    Console.WriteLine(logMessage);

    File.AppendAllText(
        "triangle.log",
        logMessage + Environment.NewLine);
}


// ======================================
// Класс результата
// ======================================

class TriangleResult
{
    public string TriangleType { get; set; }

    public (int X, int Y) VertexA { get; set; }

    public (int X, int Y) VertexB { get; set; }

    public (int X, int Y) VertexC { get; set; }
}