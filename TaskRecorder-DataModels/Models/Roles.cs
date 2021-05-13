using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TaskRecorder_DataModels.Models
{
    [Table("Roles")]
    public class Roles
    {
        public Roles()
        {
            UsersRoles = new HashSet<UsersRoles>();
        }
        [Key]
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public virtual ICollection<UsersRoles> UsersRoles { get; set; }
    }
}
