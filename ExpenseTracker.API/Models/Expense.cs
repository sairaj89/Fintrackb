using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.API.Models
{
    public class Expense
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, ErrorMessage = "Title length can't be more than 100 characters")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Amount is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero")]
        [Precision(18, 2)]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Category is required")]
        [StringLength(50, ErrorMessage = "Category length can't be more than 50 characters")]
        public string Category { get; set; }

        // Foreign key - still required
        [Required]
        [ForeignKey("User")]
        public int UserId { get; set; }

        // Navigation property to User - made nullable
        public User? User { get; set; }
    }
}
