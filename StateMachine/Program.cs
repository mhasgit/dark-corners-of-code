var order = new Order()
{
    Id = 1,
    CustomerName = "Mark"
};

Console.WriteLine("SateMachine");
Console.WriteLine($"Order: {order}");

while (true)
{
    Console.WriteLine($"Current state: {order.State}");
    var transistion = order.GetAvailableTransition();

    if (transistion.Any())
    {
        Console.WriteLine($"Available Transitions: {string.Join(", ", transistion)}");
    }
    else
    {
        Console.WriteLine("No Available transition (final state)");
    }

    Console.WriteLine("\nOptions");
    Console.WriteLine("1. Mark as paid");
    Console.WriteLine("2. Mark as shipped");
    Console.WriteLine("3. Mark as Cancelled");
    Console.WriteLine("4. Exit");
    Console.WriteLine("\nChoose");

    var choice = Console.ReadLine()?.Trim();

    try
    {
        switch (choice)
        {
            case "1":
                order.MarkAsPaid();
                Console.WriteLine($"Order state is Paid");
                break;
            case "2":
                order.MarkAsShipped();
                Console.WriteLine("Order is Shipped");
                break;
            case "3":
                order.Cancel();
                Console.WriteLine("Order Cancelled");
                break;
            case "4":
                Console.WriteLine("\nGoodbye!");
                return;
            default:
                Console.WriteLine("Invalid option, choose between 1-4");
                break;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"\n {ex.Message}");
    }
    Console.WriteLine();
}
enum OrderState
{
    Created,
    Paid,
    Shipped,
    Cancelled
}

class Order
{
    public int Id { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public OrderState State { get; set; } = OrderState.Created;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime PaidAt { get; set; }
    public DateTime ShippedAt { get; set; }
    public DateTime CanceledAt { get; set; }


    public void MarkAsPaid()
    {
        if (State != OrderState.Created)
        {
            throw new InvalidOperationException("Order needs to be created");
        }

        State = OrderState.Paid;
        PaidAt = DateTime.Now;
    }

    public void MarkAsShipped()
    {
        if (State != OrderState.Paid)
        {
            throw new InvalidOperationException("Pay for the order first");
        }

        State = OrderState.Shipped;
        ShippedAt = DateTime.Now;
    }

    public void Cancel()
    {
        if (State == OrderState.Shipped)
        {
            throw new InvalidOperationException("Order can not be cancelled, becaue it is Shipped");
        }

        if (State == OrderState.Cancelled)
        {
            throw new InvalidOperationException("Order can not be cancel, because it is already Canceled");
        }

        State = OrderState.Cancelled;
        CanceledAt = DateTime.Now;
    }

    public List<OrderState> GetAvailableTransition()
    {
        // return State switch
        // {
        //     OrderState.Created => new List<OrderState> { OrderState.Paid, OrderState.Cancelled },
        //     OrderState.Paid => new List<OrderState> { OrderState.Shipped, OrderState.Cancelled },
        //     OrderState.Shipped => new List<OrderState> { },
        //     OrderState.Cancelled => new List<OrderState> { },
        //     _ => new List<OrderState> { }
        // };


        if (State == OrderState.Created)
        {
            return new List<OrderState> { OrderState.Paid, OrderState.Cancelled };
        }
        else if (State == OrderState.Paid)
        {
            return new List<OrderState> { OrderState.Shipped, OrderState.Cancelled };
        }
        if (State == OrderState.Shipped)
        {
            return new List<OrderState> { };
        }
        if (State == OrderState.Cancelled)
        {
            return new List<OrderState> { };
        }
        return new List<OrderState>();
    }

    public override string ToString()
    {
        return $"Order {Id} - {CustomerName} - {State}";
    }
}
