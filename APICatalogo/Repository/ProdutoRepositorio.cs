using APICatalogo.Context;
using APICatalogo.Models;

namespace APICatalogo.Repository
{
    public class ProdutoRepositorio : Repository<Produto>, IProdutoRepository
    {
        public ProdutoRepositorio(AppDbContext contexto)
            : base(contexto) { }

        public IEnumerable<Produto> GetProdutosPorPreco()
            => Get().OrderBy(c => c.Preco).ToList();

    }
}
