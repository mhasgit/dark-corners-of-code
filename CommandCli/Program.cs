namespace CommandCli;

public class Program
{
    public static void Main()
    {
        var list = new ItemList();
        var commandManager = new CommandManager();

        Console.WriteLine("=== Command Pattern CLI ===\n");

        while (true)
        {
            Console.WriteLine($"Items ({list.Count}):");
            var items = list.GetAll();
            if (items.Count == 0)
            {
                Console.WriteLine("  (empty)");
            }
            else
            {
                for (int i = 0; i < items.Count; i++)
                {
                    Console.WriteLine($"  {i}. {items[i]}");
                }
            }

            Console.WriteLine($"\nHistory: {commandManager.UndoCount} undo, {commandManager.RedoCount} redo");
            Console.WriteLine("\nOptions:");
            Console.WriteLine("1. Add item");
            Console.WriteLine("2. Remove item");
            Console.WriteLine($"3. Undo{(commandManager.CanUndo ? " ✓" : "")}");
            Console.WriteLine($"4. Redo{(commandManager.CanRedo ? " ✓" : "")}");
            Console.WriteLine("5. Exit");
            Console.Write("\nChoose: ");

            var choice = Console.ReadLine()?.Trim();

            try
            {
                switch (choice)
                {
                    case "1":
                        Console.Write("Enter item text: ");
                        var itemText = Console.ReadLine();
                        if (!string.IsNullOrWhiteSpace(itemText))
                        {
                            var addCmd = new AddCommand(list, itemText);
                            commandManager.ExecuteCommand(addCmd);
                            Console.WriteLine($"✓ {addCmd.Description}");
                        }
                        else
                        {
                            Console.WriteLine("Item text cannot be empty.");
                        }
                        break;
                    case "2":
                        Console.Write("Enter index to remove: ");
                        if (int.TryParse(Console.ReadLine(), out int index))
                        {
                            if (index >= 0 && index < list.Count)
                            {
                                var removeCmd = new RemoveCommand(list, index);
                                commandManager.ExecuteCommand(removeCmd);
                                Console.WriteLine($"✓ {removeCmd.Description}");
                            }
                            else
                            {
                                Console.WriteLine($"Invalid index. Must be between 0 and {list.Count - 1}");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Invalid index. Please enter a number.");
                        }
                        break;
                    case "3":
                        if (commandManager.CanUndo)
                        {
                            commandManager.Undo();
                            Console.WriteLine("✓ Undone");
                        }
                        else
                        {
                            Console.WriteLine("Nothing to undo.");
                        }
                        break;
                    case "4":
                        if (commandManager.CanRedo)
                        {
                            commandManager.Redo();
                            Console.WriteLine("✓ Redone");
                        }
                        else
                        {
                            Console.WriteLine("Nothing to redo.");
                        }
                        break;
                    case "5":
                        Console.WriteLine("Goodbye!");
                        return;
                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            Console.WriteLine();
        }
    }
}

public class ItemList
{
    private readonly List<string> _items = new();

    public void Add(string item)
    {
        if (string.IsNullOrWhiteSpace(item))
        {
            throw new ArgumentNullException(nameof(item));
        }
        _items.Add(item);
    }

    public void Insert(int index, string item)
    {
        if (index < 0 || index >= _items.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        _items.Insert(index, item);
    }

    public void RemoveAt(int index)
    {
        if (index < 0 || index >= _items.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }
        _items.RemoveAt(index);
    }

    public string? GetItem(int index)
    {
        if (index >= 0 && index < _items.Count)
        {
            return _items[index];
        }
        return null;
    }

    public List<string> GetAll()
    {
        return new List<string>(_items);
    }

    public int Count => _items.Count;
}

public interface ICommand
{
    void Execute();
    void Undo();
    string Description { get; }
}

public class AddCommand : ICommand
{
    private readonly ItemList _list;
    private readonly string _item;

    public AddCommand(ItemList list, string item)
    {
        _list = list ?? throw new ArgumentNullException(nameof(list));
        _item = item ?? throw new ArgumentNullException(nameof(item));
    }

    public string Description => $"Add '{_item}'";
    public void Execute()
    {
        _list.Add(_item);
    }

    public void Undo()
    {
        if (_list.Count > 0)
        {
            _list.RemoveAt(_list.Count - 1);
        }
    }
}

public class RemoveCommand : ICommand
{
    private readonly ItemList _list;
    private readonly int _index;
    private string? _removedItem;

    public RemoveCommand(ItemList list, int index)
    {
        _list = list ?? throw new ArgumentNullException(nameof(list));
        _index = index;
    }

    public string Description => $"Remove item at {_index}";

    public void Execute()
    {
        _removedItem = _list.GetItem(_index);

        if (_removedItem != null)
        {
            _list.RemoveAt(_index);
        }
    }

    public void Undo()
    {
        if (_removedItem != null)
        {
            if (_index >= _list.Count)
            {
                _list.Add(_removedItem);
            }
            else
            {
                _list.Insert(_index, _removedItem);
            }
        }
    }
}

public class CommandManager
{
    private readonly Stack<ICommand> _undoCommand = new();
    private readonly Stack<ICommand> _redoCommand = new();

    public void ExecuteCommand(ICommand command)
    {
        if (command == null)
        {
            throw new ArgumentNullException(nameof(command));
        }

        command.Execute();
        _undoCommand.Push(command);
        _redoCommand.Clear();
    }

    public void Undo()
    {
        if (!CanUndo)
        {
            throw new InvalidOperationException("Nothing to undo");
        }

        var command = _undoCommand.Pop();
        command.Undo();
        _redoCommand.Push(command);
    }

    public void Redo()
    {
        if (!CanRedo)
        {
            throw new InvalidOperationException("Nothing to redo");
        }

        var command = _redoCommand.Pop();
        command.Execute();
        _undoCommand.Push(command);
    }

    public bool CanUndo => _undoCommand.Count > 0;
    public bool CanRedo => _redoCommand.Count > 0;

    public int UndoCount => _undoCommand.Count;
    public int RedoCount => _redoCommand.Count;
}
