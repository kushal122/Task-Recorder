using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TaskRecorder_DataModels.Models
{
    [Table("TaskRequest")]
    public  class TaskRequest
    {
        [Key]
        public int RequestId { get; set; }
        public string TaskId { get; set; }
        public DateTime RequestDate { get; set; }
        public DateTime DeadLine { get; set; }
        public string Status { get; set; }
        public string UserId { get; set; }
        public string Description { get; set; }
        public bool RequestConfirmed { get; set; }
        public bool AdminResponse { get; set; }
    }
}
