using Microsoft.AspNetCore.Identity;

namespace TaskManagerAPI.Models
{
    public class User : IdentityUser
    {
        public ICollection<TaskItem> Tasks { get; set; }
    }
}
