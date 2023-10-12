using Data.DataModel;
using Microsoft.EntityFrameworkCore;

namespace Data
{
    public class MyDB_Context : DbContext
    {
       
        public MyDB_Context(DbContextOptions<MyDB_Context> options) : base(options) { }
        
        public DbSet<Users> User { get; set; }
        public DbSet<Roles> Role { get; set; }
        public DbSet<Account> Account { get; set; }
        public DbSet<AddressAccount> AddressAccount { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<Comment> Comment { get; set; }
        public DbSet<Order> Order { get; set; }
        public DbSet<OrderDetail> OrderDetail { get; set; }
        public DbSet<Payments> Payments { get; set; }
        public DbSet<Product> Product { get; set; }
    }
}
