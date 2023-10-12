using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.AddressAccount
{
    public class AddressAccountModel
    {
        public Guid? AddressId { get; set; } = Guid.NewGuid();

        public string? Address { get; set; }

        public string? Image { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        /// <summary>
        /// khóa ngoại tham chiếu đến bảng Account
        /// </summary>
        public Guid? AccountId { get; set; }

        public virtual Data.DataModel.Account? Account { get; set; }
    }
}
