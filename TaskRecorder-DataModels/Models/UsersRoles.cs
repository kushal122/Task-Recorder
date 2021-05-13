using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TaskRecorder_DataModels.Models
{
    [Table("UserRoles")]
    public class UsersRoles
    {
        [Key]
        public int Id { get; set; }
        public int RoleId { get; set; }
        public string UserId { get; set; }
        public virtual Users Users { get; set; }
        public virtual Roles Roles { get; set; }
    }
}
