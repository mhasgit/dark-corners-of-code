namespace CsvValidator;

public class Program
{
    public static void Main()
    {
        try
        {
            var validator = new CsvValidator();

            Console.WriteLine("=== CSV Import & Validation Tool ===\n");
            Console.Write("Enter CSV file path: ");
            var inputFile = Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(inputFile))
            {
                Console.WriteLine("File path cannot be empty.");
                return;
            }

            // Read CSV
            Console.WriteLine("\nReading CSV file...");
            var people = validator.ReadCsv(inputFile);
            Console.WriteLine($"Read {people.Count} rows from Csv");

            // Validate
            Console.WriteLine("Validating data...");
            var validPeople = new List<Person>();
            var errors = new Dictionary<int, ValidationResult>();

            for (int i = 0; i < people.Count; i++)
            {
                var rowNumber = i + 2;
                var result = validator.Validate(people[i]);

                if (result.isValid)
                {
                    validPeople.Add(people[i]);
                }
                else
                {
                    errors[rowNumber] = result;
                }
            }

            var outputDir = Path.GetDirectoryName(inputFile) ?? Directory.GetCurrentDirectory();
            var validFile = Path.Combine(outputDir, "valid.csv");
            var errorsFile = Path.Combine(outputDir, "errors.csv");

            validator.WriteValidCsv(validFile, validPeople);
            validator.WriteError(errorsFile, errors);

            // Summary
            Console.WriteLine("\n=== Processing Complete ===");
            Console.WriteLine($"Total rows processed: {people.Count}");
            Console.WriteLine($"Valid rows: {validPeople.Count}");
            Console.WriteLine($"Rows with errors: {errors.Count}");
            Console.WriteLine($"\nValid data written to: {validFile}");
            Console.WriteLine($"Errors written to: {errorsFile}");
        }
        catch (FileNotFoundException ex)
        {
            Console.WriteLine($"\n✗ Error: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n✗ Unexpected error: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
        }
    }
}
