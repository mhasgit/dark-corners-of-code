using System.Security.Cryptography.X509Certificates;

namespace TodoCli;

partial class Program
{
    static void Main(string[] args)
    {
        Menu();
    }

    public static void Menu()
    {
        var service = new TodoService(@"D:\DemoMisc\todos.json");
        while (true)
        {
            Console.WriteLine("\n=== Todo CLI ===");
            Console.WriteLine("1. Add a todo");
            Console.WriteLine("2. List todos");
            Console.WriteLine("3. Complete a todo");
            Console.WriteLine("4. Delete a todo");
            Console.WriteLine("5. Exit");
            Console.Write("\nChoose an option: ");

            var choice = Console.ReadLine()?.Trim();

            try
            {
                switch (choice)
                {
                    case "1":
                        Console.Write("Enter todo title: ");
                        var title = Console.ReadLine();
                        if (!string.IsNullOrEmpty(title))
                        {
                            service.Add(title);
                        }
                        else
                        {
                            Console.WriteLine("Title cannot be empty");
                        }
                        break;
                    case "2":
                        service.List();
                        break;
                    case "3":
                        Console.Write("Enter todo id to complete: ");
                        if (int.TryParse(Console.ReadLine(), out int completeId))
                        {
                            service.Complete(completeId);
                        }
                        else
                        {
                            Console.WriteLine("Invalid ID, please enter a number");
                        }
                        break;
                    case "4":
                        Console.Write("Enter todo id to delete: ");
                        if (int.TryParse(Console.ReadLine(), out int deleteId))
                        {
                            service.Delete(deleteId);
                        }
                        else
                        {
                            Console.WriteLine("Invalid ID, please enter a number");
                        }
                        break;
                    case "5":
                        Console.WriteLine("Goodbye!");
                        return;
                    default:
                        Console.WriteLine("Invalid option, please choose 1-5");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error {ex.Message}");
            }

        }
    }
}
