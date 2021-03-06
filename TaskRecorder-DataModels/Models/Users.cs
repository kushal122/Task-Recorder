using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TaskRecorder_DataModels.Models
{
    [Table("Users")]
    public class Users
    {
        public Users()
        {
            UsersRoles = new HashSet<UsersRoles>();
        }
        [Key]
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string Password { get; set; }
        public virtual ICollection<UsersRoles> UsersRoles { get; set; }
    }
}
