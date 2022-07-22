using APICatalogo.Context;
using APICatalogo.Models;
using APICatalogo.Pagination;

namespace APICatalogo.Repository
{
    public class ProdutoRepositorio : Repository<Produto>, IProdutoRepository
    {
        public ProdutoRepositorio(AppDbContext contexto)
            : base(contexto) { }

        public PagedList<Produto> GetProdutos(ProdutosParameters produtosParameters)
        {
            //return Get()
            //    .OrderBy(on => on.Nome)
            //    .Skip((produtosParameters.PageNumber - 1) * produtosParameters.PageSize)
            //    .Take(produtosParameters.PageSize)
            //    .ToList();

            return PagedList<Produto>.ToPagedList(Get().OrderBy(on => on.CategoriaId), produtosParameters.PageNumber, produtosParameters.PageSize);
        }

        public IEnumerable<Produto> GetProdutosPorPreco()
            => Get().OrderBy(c => c.Preco).ToList();

    }
}
