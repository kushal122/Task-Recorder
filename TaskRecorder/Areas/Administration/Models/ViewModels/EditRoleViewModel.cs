using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskRecorder.Areas.Administration.Models.ViewModels
{
    public class EditRoleViewModel
    {
        public EditRoleViewModel()
        {
            Users = new List<string>();
        }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public IList<string> Users { get; set; }
    }
}
