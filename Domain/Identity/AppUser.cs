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
        [StringLength(100)]
        public string Name { get; set; }          
        [StringLength(100)]
        public string Password { get; set; }
        [StringLength(100)]
        public string ConfirmPassword { get; set; }
        [StringLength(100)]
        public string Photo { get; set; }
        public virtual ICollection<AppUserRole> UserRoles { get; set; }
    }
}
