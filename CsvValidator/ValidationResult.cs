namespace CsvValidator;

public class ValidationResult
{
    public bool isValid { get; set; } = true;
    public List<string> Errors { get; set; } = new();
}
