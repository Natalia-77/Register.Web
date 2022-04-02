using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Identity
{
    public class AppUser : IdentityUser<long>
    {        
        [StringLength(500)]
        public string Photo { get; set; }
        public virtual ICollection<AppUserRole> UserRoles { get; set; }
    }
}
