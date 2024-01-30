using System.ComponentModel.DataAnnotations.Schema;

namespace NovaEngevix.Models
{
    [Table("Clientes")]
    public class ClienteViewModel
    {
        public int Id { get; set; }
        public string NomeCliente { get; set; }
        public string NomeArquivo { get; set; }
        public string Descricao { get; set; }
        public string Status { get; set; }
        [NotMapped]
        public IFormFile Arquivo { get; set; }
    }
}
