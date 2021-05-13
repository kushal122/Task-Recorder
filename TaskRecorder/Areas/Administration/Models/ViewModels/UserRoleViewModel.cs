using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskRecorder.Areas.Administration.Models.ViewModels
{
    public class UserRoleViewModel
    {
        public string UserId { get; set; }
        public int RoleId { get; set; }
        public string Email { get; set; }
        public string RoleName { get; set; }
        public bool IsSelected { get; set; }
    }
}
