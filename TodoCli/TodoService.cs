using System.Data;
using Newtonsoft.Json;

namespace TodoCli;

partial class Program
{
    public class TodoService
    {
        private readonly string _filePath;
        private List<TodoItem> _todos = new();
        private int _nextId = 1;

        public TodoService(string filepath)
        {
            _filePath = filepath ?? throw new ArgumentNullException(nameof(filepath));
            LoadFromFile();
        }
        public void Add(string title)
        {
            if (String.IsNullOrEmpty(title))
            {
                throw new ArgumentNullException("Title cannot be empty", nameof(title));
            }

            var todo = new TodoItem
            {
                Id = _nextId++,
                Title = title,
                isCompleted = false,
                CreatedAt = DateTime.UtcNow
            };

            _todos.Add(todo);
            SaveToFile();
            Console.WriteLine(new string('-', 50));
            Console.WriteLine(new string('-', 50));
            Console.WriteLine($"Added todo #{todo.Id}: {todo.Title}");
        }

        public void List()
        {
            if (!_todos.Any())
            {
                Console.WriteLine("No todos yet, Add one to get started");
            }

            Console.WriteLine("\nYour todos");
            Console.WriteLine(new string('-', 50));
            Console.WriteLine(new string('-', 50));

            foreach (var todo in _todos.OrderBy(t => t.CreatedAt))
            {
                var status = todo.isCompleted ? "[✓]" : "[ ]";
                var date = todo.CreatedAt.ToString("MMM dd, yyyy");
                Console.WriteLine($"{status}, #{todo.Id}, {todo.Title}, (Created: {date})");
            }
        }

        public void Complete(int id)
        {
            var todo = _todos.FirstOrDefault(t => t.Id == id);
            if (todo == null)
            {
                Console.WriteLine($"Todo #{id} not found");
            }
            if (todo.isCompleted)
            {
                Console.WriteLine($"Todo #{id} is already completed");
            }

            todo.isCompleted = true;
            SaveToFile();
            Console.WriteLine(new string('-', 50));
            Console.WriteLine(new string('-', 50));
            Console.WriteLine($"Completed: {todo.Title}");
        }

        public void Delete(int id)
        {
            var todo = _todos.FirstOrDefault(t => t.Id == id);

            if (todo == null)
            {
                Console.WriteLine($"Todo #{id} not found");
            }
            _todos.Remove(todo);
            SaveToFile();

            Console.WriteLine(new string('-', 50));
            Console.WriteLine(new string('-', 50));
            Console.WriteLine($"Deleted {todo.Title}");
        }

        private void LoadFromFile()
        {
            try
            {
                if (File.Exists(_filePath))
                {
                    var json = File.ReadAllText(_filePath);
                    _todos = JsonConvert.DeserializeObject<List<TodoItem>>(json) ?? new List<TodoItem>();

                    if (_todos.Any())
                    {
                        _nextId = _todos.Max(t => t.Id) + 1;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading todos: {ex.Message}");
                _todos = new List<TodoItem>();
            }
        }

        private void SaveToFile()
        {
            try
            {
                if (File.Exists(_filePath))
                {
                    var json = JsonConvert.SerializeObject(_todos, Formatting.Indented);
                    File.WriteAllText(_filePath, json);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving todos: {ex.Message}");
            }
        }
    }
}
