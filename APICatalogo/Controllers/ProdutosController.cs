using APICatalogo.DTOs;
using APICatalogo.Models;
using APICatalogo.Pagination;
using APICatalogo.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace APICatalogo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProdutosController : ControllerBase
    {
        private readonly IUnitOfWork _uof;
        private readonly IMapper _mapper;

        public ProdutosController(IUnitOfWork uof, IMapper mapper)
        {
            _uof = uof;
            _mapper = mapper;
        }

        [HttpGet("menorpreco")]
        public async Task<ActionResult<IEnumerable<ProdutoDTO>>> GetProdutosPrecos()
        {
            var produtos = await _uof.ProdutoRepository.GetProdutosPorPreco();
            var produtosDTO = _mapper.Map<List<ProdutoDTO>>(produtos);
            return produtosDTO;
        }

        [HttpGet()]
        public async Task<ActionResult<IEnumerable<ProdutoDTO>>> Get([FromQuery] ProdutosParameters produtosParameters)
        {
            var produtos = await _uof.ProdutoRepository.GetProdutos(produtosParameters);
            if (produtos is null)
                return NotFound("Produtos não encontrado");

            var metadata = new
            {
                produtos.TotalCount,
                produtos.PageSize,
                produtos.CurrentPage,
                produtos.TotalPages,
                produtos.HasNext,
                produtos.HasPrevious
            };

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(metadata));

            var produtosDTO = _mapper.Map<List<ProdutoDTO>>(produtos);
            return produtosDTO;
        }

        [HttpGet("{id:int:min(1)}", Name = "ObterProduto")]
        public async Task<ActionResult<ProdutoDTO>> Get(int id)
        {
            var produto = await _uof.ProdutoRepository.GetById(p => p.Id == id);

            if (produto is null)
                return NotFound("Produto não encontrado");

            var produtoDTO = _mapper.Map<ProdutoDTO>(produto);
            return produtoDTO;
        }

        [HttpPost]
        public async Task<ActionResult> Post(ProdutoDTO produtoDto)
        {
            if (produtoDto is null)
                return BadRequest();

            var produto = _mapper.Map<Produto>(produtoDto);

            _uof.ProdutoRepository.Add(produto);
            await _uof.Commit();

            var produtoDTO = _mapper.Map<ProdutoDTO>(produto);

            return new CreatedAtRouteResult("ObterProduto", new { id = produto.Id }, produtoDTO);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, ProdutoDTO produtoDto)
        {
            if (id != produtoDto.Id)
                return BadRequest();

            var produto = _mapper.Map<Produto>(produtoDto);

            _uof.ProdutoRepository.Update(produto);
            await _uof.Commit();

            return Ok(produto);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var produto = await _uof.ProdutoRepository.GetById(p => p.Id == id);
            if (produto is null)
                return NotFound("Produto não localizado ...");

            _uof.ProdutoRepository.Delete(produto);
            await _uof.Commit();

            var produtoDTO = _mapper.Map<ProdutoDTO>(produto);

            return Ok(produtoDTO);
        }

    }
}
