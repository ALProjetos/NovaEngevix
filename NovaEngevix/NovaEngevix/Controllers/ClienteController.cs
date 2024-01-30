using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NovaEngevix.BancoDados;
using NovaEngevix.Models;

namespace NovaEngevix.Controllers
{
    public class ClienteController : Controller
    {
        private readonly string DEFAULT_PATH = "wwwroot/uploads";
        private readonly ApplicationDbContext _context;

        public ClienteController(ApplicationDbContext context)
            => _context = context;

        public IActionResult Index()
            => View(_context.Clientes.ToList());

        public IActionResult Criar()
            => View();

        [HttpPost]
        public async Task<IActionResult> Criar(ClienteViewModel cliente)
        {
            if (ModelState.IsValid)
            {
                cliente.NomeArquivo = cliente.Arquivo.FileName;

                _context.Clientes.Add(cliente);
                bool salvou = _context.SaveChanges() > 0;

                // Só deverá salvar o arquivo local se salvou no banco de dados
                if (salvou && (cliente?.Arquivo?.Length ?? 0) > 0)
                {
                    var uploadDir = Path.Combine(Directory.GetCurrentDirectory(), DEFAULT_PATH);
                    var filePath = Path.Combine(uploadDir, $"{cliente.NomeCliente}_{cliente.Arquivo.FileName}");

                    if (!Directory.Exists(uploadDir))
                        Directory.CreateDirectory(uploadDir);

                    using var stream = new FileStream(filePath, FileMode.Create);
                    await cliente.Arquivo.CopyToAsync(stream);
                }

                return RedirectToAction("Index");
            }
            return View(cliente);
        }

        public IActionResult Download(int id)
        {
            ClienteViewModel cliente = _context.Clientes.Find(id);

            if (null == cliente)
                return NotFound();

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), DEFAULT_PATH, $"{cliente.NomeCliente}_{cliente.NomeArquivo}");

            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);

            // Define o tipo MIME do arquivo (por exemplo, "application/pdf" para PDF)
            var mimeType = "application/octet-stream";

            // Retorna o arquivo para download
            return File(fileBytes, mimeType, cliente.NomeArquivo);
        }

        public IActionResult Editar(int id)
            => View(_context.Clientes.Find(id));

        [HttpPost]
        public IActionResult Editar(ClienteViewModel cliente)
        {
            if (ModelState.IsValid)
            {
                _context.Entry(cliente).State = EntityState.Modified;
                _context.SaveChanges();

                return RedirectToAction("Index");
            }
            return View(cliente);
        }

        public IActionResult Detalhes(int id)
            => View(_context.Clientes.Find(id));

        public IActionResult Delete(int id)
            => View(_context.Clientes.Find(id));

        [HttpPost, ActionName("Delete")]
        public IActionResult ConfirmarExclusao(int id)
        {
            ClienteViewModel cliente = _context.Clientes.Find(id);
            if (null != cliente)
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), DEFAULT_PATH, $"{cliente.NomeCliente}_{cliente.NomeArquivo}");

                if(System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);

                _context.Clientes.Remove(cliente);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }
    }
}
