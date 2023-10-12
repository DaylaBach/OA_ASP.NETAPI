using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Business.Product
{
    public class ProductModel
    {
        public Guid? ProductId { get; set; } = Guid.NewGuid();

        public string? ProductName { get; set; }

        public string? Address { get; set; }

        public float? SalePrice { get; set; }

        public string? Image { get; set; }

        /*public string? Description { get; set; }*/
    }

    public class ProductCreateModel
    {
        public Guid? ProductId { get; set; } = Guid.NewGuid();

        public string? ProductName { get; set; }

        public int? Quantity { get; set; }

        public float? Price { get; set; }

        public string? Address { get; set; }

        public float? SalePrice { get; set; }

        public bool Status { get; set; } = true;

        public string? Description { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public string? Image { get; set; }

        public IFormFile? FileImage { get; set; }


        public Guid? CategoryId { get; set; }

        public virtual Data.DataModel.Category? Category { get; set; }
    }
}
