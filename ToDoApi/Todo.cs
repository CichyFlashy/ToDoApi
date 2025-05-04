using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace ToDoApi
{
    public class ToDo
    {
        public DateTime Expiry { get; set; }
        [Required]
        public string Title { get; set; }
        public string? Description { get; set; }
        public double CompletePercent { get; set; }
    }
}
