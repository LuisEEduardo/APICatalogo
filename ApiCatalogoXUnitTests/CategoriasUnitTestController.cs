using APICatalogo.Context;
using APICatalogo.Controllers;
using APICatalogo.DTOs;
using APICatalogo.DTOs.Mappings;
using APICatalogo.Pagination;
using APICatalogo.Repository;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiCatalogoXUnitTests
{
    public class CategoriasUnitTestController
    {
        private IMapper mapper;
        private IUnitOfWork repository;
        private static DbContextOptions<AppDbContext> dbContextOptions { get; }
        public static string connectionString = "server=localhost;userid=root;password=luis93;database=CatalogoDb";

        public CategoriasController _controller { get; set; }
        public CategoriasParameters _parameters { get; set; }

        static CategoriasUnitTestController()
        {
            dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
                .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
                .Options;
        }

        public CategoriasUnitTestController()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfile());
            });

            mapper = config.CreateMapper();

            var context = new AppDbContext(dbContextOptions);

            //DBUnitTestsMockInitializer db = new DBUnitTestsMockInitializer();
            //db.Seed(context);

            repository = new UnitOfWork(context);

            _controller = new CategoriasController(repository, mapper);
            _parameters = new CategoriasParameters();
        }

        [Fact]
        public void GetCategorias_Return_OkResult()
        {
            // Arrange 
            var controller = new CategoriasController(repository, mapper);
            var parameters = new CategoriasParameters();


            // Act 
            var data = controller.Get(parameters);

            // Assert
            Assert.IsType<List<CategoriaDTO>>(data.Result.Value);
        }

        [Fact]
        public void GetCategorias_Return_BadRequest()
        {
            var data = _controller.Get(_parameters);
            Assert.IsType<BadRequestResult>(data.Result.Result);
        }

        [Fact]
        public void GetCategorias_MacthResult()
        {
            var data = _controller.Get(_parameters);

            Assert.IsType<List<CategoriaDTO>>(data.Result.Value);
            var cat = data.Result.Value.Should().BeAssignableTo<List<CategoriaDTO>>().Subject;

            Assert.Equal("Bebidas", cat[0].Nome);
            Assert.Equal("bebidas.jpg", cat[0].ImagemUrl);

            Assert.Equal("Lanches", cat[1].Nome);
            Assert.Equal("lanches.jpg", cat[1].ImagemUrl);
        }

        [Fact]
        public void GetCategoriasById_Return_OkResult()
        {
            var catagoriaId = 2;
            var data = _controller.Get(catagoriaId);

            Assert.IsType<CategoriaDTO>(data.Result.Value);
        }

        [Fact]
        public void GetCategoriaById_Return_NotFoundObjectResult()
        {
            var catId = 9999;
            var data = _controller.Get(catId);

            Assert.IsType<NotFoundObjectResult>(data.Result.Result);
        }

        [Fact]
        public void Post_Categoria_AddValidData_Return_CreatedAtRouteResult()
        {
            var cat = new CategoriaDTO()
            {
                Nome = "Teste Unitario 1 Inclusao",
                ImagemUrl = "testecatInclusao.jpg"
            };

            var data = _controller.Post(cat);

            Assert.IsType<CreatedAtRouteResult>(data.Result);
        }

        [Fact]
        public void Put_Categoria_Update_ValidData_Return_OkObjectResult()
        {
            var catId = 2;

            var existingPost = _controller.Get(catId);
            var result = existingPost.Result.Value.Should().BeAssignableTo<CategoriaDTO>().Subject;

            var catDto = new CategoriaDTO();
            catDto.Id = catId;
            catDto.Nome = "categoria atualizada";
            catDto.ImagemUrl = result.ImagemUrl;

            var updatedData = _controller.Put(catId, catDto);

            Assert.IsType<OkObjectResult>(updatedData.Result);
        }

        [Fact]
        public void Delete_Categoria_Return_OkResult()
        {
            var catId = 2;
            var data = _controller.Delete(catId);

            Assert.IsType<OkObjectResult>(data.Result);
        }

    }
}
