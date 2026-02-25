namespace TodoCli;

partial class Program
{
    public class TodoItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public bool isCompleted { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
