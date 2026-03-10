using System.Net.Http.Headers;
using System.Text.RegularExpressions;

namespace CsvValidator;

public class CsvValidator
{
    private static readonly Regex EmailRegex = new(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);


    // public ValidationResult Validate(Person person)
    // {
    //     var result = new ValidationResult();

    //     if (string.IsNullOrWhiteSpace(person.Name))
    //     {
    //         result.Errors.Add("Name is required");
    //         result.isValid = false;
    //     }

    //     if (string.IsNullOrWhiteSpace(person.Email))
    //     {
    //         result.Errors.Add("Email is required");
    //         result.isValid = false;
    //     }
    //     else if (!EmailRegex.IsMatch(person.Email))
    //     {
    //         result.Errors.Add("Email format is invalid");
    //         result.isValid = false;
    //     }

    //     if (person.Age < 18 || person.Age > 120)
    //     {
    //         result.Errors.Add($"Age must be between 18 and 120 (got {person.Age})");
    //         result.isValid = false;
    //     }

    //     return result;
    // }


    // public List<Person> ReadCsv(string filePath)
    // {
    //     if (!File.Exists(filePath))
    //     {
    //         throw new FileNotFoundException($"CSV file not found: {filePath}");
    //     }

    //     var lines = File.ReadAllLines(filePath);
    //     var people = new List<Person>();

    //     if (lines.Length < 2)
    //     {
    //         Console.WriteLine("CSV file must have at least a header and one data row");
    //         return people;
    //     }

    //     for (int i = 1; i < lines.Length; i++)
    //     {
    //         try
    //         {
    //             var parts = lines[i].Split(',');

    //             if (parts.Length < 3)
    //             {
    //                 Console.WriteLine($"Warning: Row {i + 1} has insufficient columns, skipping.");
    //                 continue;
    //             }

    //             var person = new Person
    //             {
    //                 Name = parts[0].Trim(),
    //                 Email = parts[1].Trim(),
    //                 Age = int.TryParse(parts[2].Trim(), out var age) ? age : 0
    //             };

    //             people.Add(person);
    //         }
    //         catch (Exception ex)
    //         {
    //             Console.WriteLine($"Warning: Error parsing row {i + 1}: {ex.Message}");
    //         }
    //     }

    //     return people;
    // }

    // public void WriteValidCsv(string filePath, List<Person> validPeople)
    // {
    //     var lines = new List<string> { "Name,Email,Age" };

    //     foreach (var person in validPeople)
    //     {
    //         lines.Add($"{person.Name},{person.Email},{person.Age}");
    //     }

    //     File.WriteAllLines(filePath, lines);
    // }

    // public void WriteError(string filePath, Dictionary<int, ValidationResult> errors)
    // {
    //     var errorLines = new List<string>();

    //     foreach (var kvp in errors.OrderBy(e => e.Key))
    //     {
    //         var rowNumber = kvp.Key;
    //         var result = kvp.Value;

    //         foreach (var error in result.Errors)
    //         {
    //             errorLines.Add($"Row {rowNumber}: {error}");
    //         }
    //     }

    //     if (errorLines.Count == 0)
    //     {
    //         errorLines.Add("No errors found!");
    //     }

    //     File.WriteAllLines(filePath, errorLines);
    // }

}
