using QA_Application.Models;
using System.ComponentModel.DataAnnotations;

namespace QA_Application.ViewModels
{
    public class CommentViewModel
    {
        [Required]
        public int CommentId { get; set; }
        [Required]
        public int IdeaId { get; set; }
        public string Message { get; set; }
    }
}
