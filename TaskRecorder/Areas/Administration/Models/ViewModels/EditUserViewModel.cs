using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskRecorder.Areas.Administration.Models.ViewModels
{
    public class EditUserViewModel
    {
        public EditUserViewModel()
        {
            Roles = new List<string>();
        }
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public IList<string> Roles { get; set; }
    }
}