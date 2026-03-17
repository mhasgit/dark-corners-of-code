namespace ExpressionParser;

public class Program
{
    static void Main()
    {
        var evaluator = new ExpressionEvaluator();

        Console.WriteLine("=== Expression Parser / Calculator ===\n");
        Console.WriteLine("Supports: +, -, *, /, parentheses");
        Console.WriteLine("Example: (2 + 3) * 4\n");

        while (true)
        {
            Console.Write("Enter expression (or 'exit'): ");
            string input = Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(input) || input.ToLower() == "exit")
            {
                break;
            }

            try
            {
                var result = evaluator.Evaluate(input);
                Console.WriteLine($"Result: {result}\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}\n");
            }
        }

        Console.WriteLine("Goodbye!");
    }
}

public class ExpressionEvaluator
{
    public double Evaluate(string expression)
    {
        if (string.IsNullOrWhiteSpace(expression))
        {
            throw new ArgumentException("Expression cannot be empty", nameof(expression));
        }

        expression = expression.Replace(" ", "");

        var index = 0;
        return EvaluateExpression(expression, ref index);
    }

    private double EvaluateExpression(string expr, ref int index)
    {
        // Handle addition and subtraction (lowest precedence)
        double result = EvaluateTerm(expr, ref index);

        while (index < expr.Length)
        {
            char op = expr[index];
            if (op == '+' || op == '-')
            {
                index++;
                double right = EvaluateTerm(expr, ref index);
                if (op == '+')
                {
                    result += right;
                }
                else
                {
                    result -= right;
                }
            }
            else
            {
                break;
            }
        }
        return result;
    }

    private double EvaluateTerm(string expr, ref int index)
    {
        // Handle multiplication and division (higher precedence)
        double result = EvaluateFactor(expr, ref index);

        while (index < expr.Length)
        {
            char op = expr[index];
            if (op == '*' || op == '/')
            {
                index++;
                double right = EvaluateFactor(expr, ref index);

                if (op == '*')
                {
                    result *= right;
                }
                else
                {
                    if (right == 0)
                    {
                        throw new DivideByZeroException("Division by zero");
                    }
                    result /= right;
                }
            }
            else
            {
                break;
            }
        }

        return result;
    }

    private double EvaluateFactor(string expr, ref int index)
    {
        // Handle parentheses and numbers (highest precedence)
        if (index >= expr.Length)
        {
            throw new ArgumentException("Unexpected end of expression");
        }

        if (expr[index] == '(')
        {
            index++;
            double result = EvaluateExpression(expr, ref index);

            if (index >= expr.Length || expr[index] != ')')
            {
                throw new ArgumentException("Missing parenthesis");
            }
            index++;
            return result;

        }
        else if (expr[index] == '-' || expr[index] == '+')
        {
            bool negative = expr[index] == '-';
            index++;
            double result = EvaluateFactor(expr, ref index);
            return negative ? -result : result;
        }
        else
        {
            return ParseNumber(expr, ref index);
        }
    }

    private double ParseNumber(string expr, ref int index)
    {
        int startIndex = index;
        bool hasDecimal = false;

        if (index < expr.Length && expr[index] == '-')
        {
            index++;
        }

        while (index < expr.Length)
        {
            char c = expr[index];
            if (char.IsDigit(c))
            {
                index++;
            }
            else if (c == '.' && !hasDecimal)
            {
                hasDecimal = true;
                index++;
            }
            else
            {
                break;
            }
        }

        if (index < startIndex || index == startIndex + 1 && expr[startIndex] == '-')
        {
            throw new ArgumentException($"Invalid number at position {startIndex}");
        }

        string numberStr = expr.Substring(startIndex, index - startIndex);
        if (double.TryParse(numberStr, out double result))
        {
            return result;
        }

        throw new ArgumentException($"Invalid number: {numberStr}");
    }
}
