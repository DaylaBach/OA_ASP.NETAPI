using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Business.Category
{
    public class CategoryModel
    {
        public Guid? CategoryId { get; set; }

        public string? CategoryName { get; set; }

        public string? Image { get; set; }
    }

    public class CategoryCreateModel
    {
        public Guid? CategoryId { get; set; } = Guid.NewGuid();

        public string? CategoryName { get; set; }

        public bool Status { get; set; } = true;

        public string? Image { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public IFormFile? FileImage { get; set; }

    }
}
