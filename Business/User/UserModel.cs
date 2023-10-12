using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.User
{
    public class UserModel
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
    }

    public class UserCreateModel
    {
        public Guid? Id { get; set; } = Guid.NewGuid();

        public string? UserName { get; set; }

        public string? Address { get; set; }

        public string? Email { get; set; }

        public string? Phone { get; set; }

        public string? Avatar { get; set; }

        public string? Birthday { get; set; }

        public string? Password { get; set; }

        public bool Sate { get; set; } = true;

        public int CountError { get; set; }

        public DateTime? TimeLock { get; set; }

        public bool IsLock { get; set; }

        public string? TokenChangePassword { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }


        /// <summary>
        /// khóa ngoại tham chiếu đến bảng Roles
        /// </summary>
        public int? RoleId { get; set; }
    }
}
