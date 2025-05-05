using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace ToDoApi
{
    public class ToDo
    {
        public int Id { get; set; }

        // Expiry is required
        [Required(ErrorMessage = "Expiry date is required")]
        public DateTime Expiry { get; set; }

        // Title is required and should not exceed 100 characters
        [Required(ErrorMessage = "Title is required")]
        [MaxLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
        public required string Title { get; set; }

        // Description can be up to 500 characters
        [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }

        // CompletePercent should be between 0 and 100
        [Range(0, 100, ErrorMessage = "Complete percent must be between 0 and 100")]
        public double CompletePercent { get; set; }
        public bool IsDone { get; set; } = false;
    }
}
