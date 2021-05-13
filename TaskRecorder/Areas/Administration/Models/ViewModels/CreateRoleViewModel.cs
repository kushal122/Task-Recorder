using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TaskRecorder.Areas.Administration.Models.ViewModels
{
    public class CreateRoleViewModel
    {
        public int RoleId { get; set; }
        [Required]
        public string RoleName { get; set; }
    }
}
