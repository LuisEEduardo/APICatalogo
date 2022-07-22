using APICatalogo.DTOs;
using APICatalogo.Models;
using APICatalogo.Pagination;
using APICatalogo.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace APICatalogo.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CategoriasController : ControllerBase
    {
        private readonly IUnitOfWork _uof;
        private readonly IMapper _mapper;

        public CategoriasController(IUnitOfWork uof, IMapper mapper)
        {
            _uof = uof;
            _mapper = mapper;
        }

        [HttpGet("produtos")]
        public async Task<ActionResult<IEnumerable<CategoriaDTO>>> GetCategoriasProdutos()
        {
            var produtos = await _uof.CategoriaRepository.GetCategoriasProdutos();
            var produtosDTO = _mapper.Map<List<CategoriaDTO>>(produtos);
            return produtosDTO;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoriaDTO>>> Get([FromQuery] CategoriasParameters categoriasParameters)
        {
            var categorias = await _uof.CategoriaRepository.GetCategorias(categoriasParameters);

            var metadata = new
            {
                categorias.TotalCount,
                categorias.PageSize,
                categorias.CurrentPage,
                categorias.TotalPages,
                categorias.HasNext,
                categorias.HasPrevious
            };

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(metadata));

            var categoriasDTO = _mapper.Map<List<CategoriaDTO>>(categorias);
            return categoriasDTO;
        }

        [HttpGet("{id:int}", Name = "ObterCategoria")]
        public async Task<ActionResult<CategoriaDTO>> Get(int id)
        {
            try
            {
                var categoria = await _uof.CategoriaRepository.GetById(c => c.Id == id);

                if (categoria is null)
                    return NotFound("Categoria não foi encontrada ...");

                var categoriaDTO = _mapper.Map<CategoriaDTO>(categoria);

                return Ok(categoriaDTO);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um problema ao tratar a solicitação");
            }
        }

        [HttpPost]
        public async Task<ActionResult> Post(CategoriaDTO categoriaDto)
        {
            if (categoriaDto is null)
                return BadRequest();

            var categoria = _mapper.Map<Categoria>(categoriaDto);

            _uof.CategoriaRepository.Add(categoria);
            await _uof.Commit();

            var categoriaDTO = _mapper.Map<CategoriaDTO>(categoria);

            return new CreatedAtRouteResult("ObterCategoria", new { id = categoria.Id }, categoriaDTO);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, CategoriaDTO categoriaDto)
        {
            if (id != categoriaDto.Id)
                return BadRequest();

            var categoria = _mapper.Map<Categoria>(categoriaDto);

            _uof.CategoriaRepository.Update(categoria);
            await _uof.Commit();

            return Ok(categoriaDto);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var categoria = await _uof.CategoriaRepository.GetById(ca => ca.Id == id);

            if (categoria is null)
                return NotFound("Categoria não encontrada ...");

            _uof.CategoriaRepository.Delete(categoria);
            await _uof.Commit();

            var categoriaDTO = _mapper.Map<CategoriaDTO>(categoria);

            return Ok(categoriaDTO);
        }

    }
}
