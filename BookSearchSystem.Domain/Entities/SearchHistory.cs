using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookSearchSystem.Domain.Entities;

[Table("search_history", Schema = "BookSearch")]
public class SearchHistory
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(255)]
    public string AuthorSearched { get; set; } = string.Empty;
    
    [Required]
    public DateTime SearchDate { get; set; }
    
    [Required]
    public DateTime CreatedAt { get; set; }
    
    // Constructor
    public SearchHistory()
    {
        SearchDate = DateTime.UtcNow;
        CreatedAt = DateTime.UtcNow;
    }
    
    public SearchHistory(string authorSearched) : this()
    {
        AuthorSearched = authorSearched;
    }
}
