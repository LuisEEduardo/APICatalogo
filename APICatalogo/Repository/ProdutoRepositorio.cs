using APICatalogo.Context;
using APICatalogo.Models;
using APICatalogo.Pagination;
using Microsoft.EntityFrameworkCore;

namespace APICatalogo.Repository
{
    public class ProdutoRepositorio : Repository<Produto>, IProdutoRepository
    {
        public ProdutoRepositorio(AppDbContext contexto)
            : base(contexto) { }

        public async Task<PagedList<Produto>> GetProdutos(ProdutosParameters produtosParameters)
        {
            //return Get()
            //    .OrderBy(on => on.Nome)
            //    .Skip((produtosParameters.PageNumber - 1) * produtosParameters.PageSize)
            //    .Take(produtosParameters.PageSize)
            //    .ToList();

            return await PagedList<Produto>.ToPagedList(Get().OrderBy(on => on.CategoriaId), produtosParameters.PageNumber, produtosParameters.PageSize);
        }

        public async Task<IEnumerable<Produto>> GetProdutosPorPreco()
            => await Get().OrderBy(c => c.Preco).ToListAsync();

    }
}
