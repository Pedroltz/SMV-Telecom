using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SMVTelecom.Services;

namespace SMVTelecom.Pages.Produto;

public class IndexModel : PageModel
{
    private readonly ProdutoService _prod;

    public SMVTelecom.Data.Produto Item { get; private set; } = null!;
    public List<SMVTelecom.Data.Produto> MaisItens { get; private set; } = [];

    public IndexModel(ProdutoService prod) => _prod = prod;

    public IActionResult OnGet(string slug)
    {
        var produto = _prod.FindBySlug(slug);
        if (produto is null) return NotFound();

        Item = produto;
        MaisItens = _prod.GetByCategoria(produto.Categoria)
            .Where(p => p.Slug != produto.Slug)
            .ToList();
        return Page();
    }
}
