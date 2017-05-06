using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ItsyBits.Models;

namespace ItsyBits.Areas.Admin.Models {
    public class ApplicationUserVm : ApplicationUser {

        public virtual string NewPassword { get; set; }

        public virtual bool IsAdmin { get; set; }

    }
}
