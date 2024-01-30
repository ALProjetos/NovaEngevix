using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Moq;
using NovaEngevix.BancoDados;
using NovaEngevix.Controllers;
using NovaEngevix.Models;

namespace NovaEngevix.Tests
{
    [Trait("Unit", "Cliente")]
    public class ClienteControllerUnitTests
    {
        protected Mock<ApplicationDbContext> _mockApplicationDbContext;
        protected readonly DbContextOptions<ApplicationDbContext> _options;

        public ClienteControllerUnitTests()
        {
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "FekaConnectionString")
                .Options;

            _mockApplicationDbContext = new(_options);
        }

        private static Mock<DbSet<T>> MockDbSet<T>(List<T> list) where T : class
        {
            var queryable = list.AsQueryable();
            var mockDbSet = new Mock<DbSet<T>>();

            mockDbSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
            mockDbSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
            mockDbSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            mockDbSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => queryable.GetEnumerator());

            return mockDbSet;
        }

        private IFormFile CriarArquivoIFormFile()
        {
            var fileMock = new Mock<IFormFile>();

            var sourceImg = File.OpenRead(@"Arquivo\pdf_teste.pdf");
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);

            writer.Write(sourceImg);
            writer.Flush();
            ms.Position = 0;

            fileMock.Setup(f => f.FileName).Returns("pdf_teste.pdf").Verifiable();
            fileMock.Setup(f => f.Length).Returns(200).Verifiable();
            fileMock.Setup(_ => _.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                .Returns((Stream stream, CancellationToken token) => ms.CopyToAsync(stream))
                .Verifiable();

            return fileMock.Object;
        }

        private ClienteViewModel CriarModel()
        {
            return new()
            {
                NomeCliente = "Teste",
                Descricao = "Teste",
                Status = "Aprovado",
                Arquivo = CriarArquivoIFormFile()
            };
        }

        [Fact(DisplayName = "Buscar a lista de todos os clientes")]
        public void IndexCliente()
        {
            List<ClienteViewModel> clientes = new()
            {
                new ClienteViewModel()
            };

            _mockApplicationDbContext
                .Setup(s => s.Clientes)
                .Returns(MockDbSet(clientes).Object);

            ClienteController controller = new ClienteController(_mockApplicationDbContext.Object);

            var result = (ViewResult)controller.Index();

            var viewresult = Assert.IsType<ViewResult>(result);
            Assert.NotNull(result.ViewData.Model);
            Assert.IsType<List<ClienteViewModel>>(result.ViewData.Model);
        }

        [Fact(DisplayName = "Criar novo cliente")]
        public void CriarCliente()
        {
            ClienteViewModel cliente = CriarModel();

            _mockApplicationDbContext
                .Setup(s => s.Clientes)
                .Returns(MockDbSet(new List<ClienteViewModel>()).Object);

            _mockApplicationDbContext
                .Setup(s => s.SaveChanges())
                .Returns(1);

            ClienteController controller = new ClienteController(_mockApplicationDbContext.Object);

            var result = controller.Criar(cliente).Result;

            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact(DisplayName = "Editar um cliente por id")]
        public void EditarClientePorId()
        {
            ClienteViewModel cliente = CriarModel();

            _mockApplicationDbContext
                .Setup(s => s.Clientes.Find(It.IsAny<int>()))
                .Returns(cliente);

            ClienteController controller = new ClienteController(_mockApplicationDbContext.Object);

            var result = (ViewResult) controller.Editar(It.IsAny<int>());

            var viewresult = Assert.IsType<ViewResult>(result);
            Assert.NotNull(result.ViewData.Model);
            Assert.IsType<ClienteViewModel>(result.ViewData.Model);
        }

        [Fact(DisplayName = "Editar um cliente", Skip = "Não rodar .Entry não está fazendo mock")]
        public void EditarCliente()
        {
            ClienteViewModel cliente = CriarModel();

            var mockEntity = new Mock<EntityEntry<ClienteViewModel>>();
            mockEntity
                .Setup(s => s.State)
                .Returns(EntityState.Detached);

            _mockApplicationDbContext
                .Setup(s => s.Entry(cliente))
                .Returns(mockEntity.Object);

            _mockApplicationDbContext
                .Setup(s => s.SaveChanges())
                .Returns(1);

            ClienteController controller = new ClienteController(_mockApplicationDbContext.Object);

            var result = controller.Editar(cliente);

            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact(DisplayName = "Detalhes de um cliente")]
        public void DetalhesCliente()
        {
            ClienteViewModel cliente = CriarModel();

            _mockApplicationDbContext
                .Setup(s => s.Clientes.Find(It.IsAny<int>()))
                .Returns(cliente);

            ClienteController controller = new ClienteController(_mockApplicationDbContext.Object);

            var result = (ViewResult) controller.Detalhes(It.IsAny<int>());

            var viewresult = Assert.IsType<ViewResult>(result);
            Assert.NotNull(result.ViewData.Model);
            Assert.IsType<ClienteViewModel>(result.ViewData.Model);
        }

        [Fact(DisplayName = "Deletar um cliente")]
        public void DeleteCliente()
        {
            ClienteViewModel cliente = CriarModel();

            _mockApplicationDbContext
                .Setup(s => s.Clientes.Find(It.IsAny<int>()))
                .Returns(cliente);

            ClienteController controller = new ClienteController(_mockApplicationDbContext.Object);

            var result = (ViewResult) controller.Delete(It.IsAny<int>());

            var viewresult = Assert.IsType<ViewResult>(result);
            Assert.NotNull(result.ViewData.Model);
            Assert.IsType<ClienteViewModel>(result.ViewData.Model);
        }

        [Fact(DisplayName = "Confirma exclusão do cliente")]
        public void ConfirmarExclusaoCliente()
        {
            ClienteViewModel cliente = CriarModel();

            _mockApplicationDbContext
                .Setup(s => s.Clientes.Find(It.IsAny<int>()))
                .Returns(cliente);

            _mockApplicationDbContext
                .Setup(s => s.SaveChanges())
                .Returns(1);

            ClienteController controller = new ClienteController(_mockApplicationDbContext.Object);

            var result = controller.Criar(cliente).Result;

            Assert.IsType<RedirectToActionResult>(result);
        }
    }
}