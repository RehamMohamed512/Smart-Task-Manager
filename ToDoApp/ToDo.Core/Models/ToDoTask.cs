using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ToDo.Core.Models
{
    public class ToDoTask
    {
        //Data Annotations

        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage ="Title is Required")]
        [MaxLength(100, ErrorMessage ="Title can't be longer than 100 Character")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is Required")]
        [MaxLength(500, ErrorMessage = "Description can't be longer than 500 Character")]
        public string Description { get; set; }= string.Empty;
        public bool IsCompleted { get; set; }= false;
        public DateTime CreatedAt { get; set; }= DateTime.Now;
        public DateTime? DueDate { get; set; }

        //relation

        public int UserId { get; set; }
        [JsonIgnore]
        public User User { get; set; }
    }
}
