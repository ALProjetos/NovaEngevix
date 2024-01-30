using Microsoft.EntityFrameworkCore;
using NovaEngevix.Models;

namespace NovaEngevix.BancoDados
{
    public class ApplicationDbContext : DbContext
    {
        public virtual DbSet<ClienteViewModel> Clientes { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}
