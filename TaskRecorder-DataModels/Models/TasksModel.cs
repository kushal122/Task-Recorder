using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TaskRecorder_DataModels.Models
{
    [Table("Tasks")]
    public class TasksModel
    {
        [Key]
        public string TaskId { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        public DateTime UploadedDate { get; set; } = DateTime.Now;
        public string AssignedTo { get; set; }
        [Required]
        public DateTime DeadLine { get; set; }
        public bool IsConfirmed { get; set; }
        public string Status { get; set; }
        public string UserId { get; set; }
    }
}
