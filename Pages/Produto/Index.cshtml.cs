using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SMVTelecom.Pages.Produto;

public class IndexModel : PageModel
{
    public SMVTelecom.Data.Produto Item { get; private set; } = null!;
    public List<SMVTelecom.Data.Produto> MaisItens { get; private set; } = [];

    public IActionResult OnGet(string slug)
    {
        var produto = SMVTelecom.Data.ProdutoCatalog.FindBySlug(slug);
        if (produto is null) return NotFound();

        Item = produto;
        MaisItens = SMVTelecom.Data.ProdutoCatalog.Todos
            .Where(p => p.Categoria == produto.Categoria && p.Slug != produto.Slug)
            .ToList();
        return Page();
    }
}
