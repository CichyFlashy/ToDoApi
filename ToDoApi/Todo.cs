using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace ToDoApi
{
    public class ToDo
    {
        public int Id { get; set; }
        public DateTime Expiry { get; set; }
        public required string Title { get; set; }
        public string? Description { get; set; }
        public double CompletePercent { get; set; }
    }
}
