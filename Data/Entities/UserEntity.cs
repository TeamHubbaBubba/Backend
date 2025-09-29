
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities
{
    [Table("Users")]
    public class UserEntity : IdentityUser<Guid>
    {
        [ProtectedPersonalData]
        public string FirstName { get; set; } = null!;
        [ProtectedPersonalData]
        public string LastName { get; set; } = null!;
    }
}
