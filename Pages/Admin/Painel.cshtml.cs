using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SMVTelecom.Data;
using SMVTelecom.Services;

namespace SMVTelecom.Pages.Admin;

public class PainelModel : PageModel
{
    private readonly SiteConfigService _cfg;
    private readonly AdminAuthService  _auth;
    private readonly ProdutoService    _prod;

    public SiteConfig Config    { get; private set; } = new();
    public List<string> Users   { get; private set; } = new();
    public List<SMVTelecom.Data.Produto> Todos  { get; private set; } = new();
    public bool IsExtra(string slug) => _prod.IsExtra(slug);
    public string? Mensagem     { get; set; }
    public string? MensagemTipo { get; set; } // "ok" | "erro"

    public PainelModel(SiteConfigService cfg, AdminAuthService auth, ProdutoService prod)
    {
        _cfg  = cfg;
        _auth = auth;
        _prod = prod;
    }

    private IActionResult CheckAuth()
    {
        if (HttpContext.Session.GetString("admin") is null)
            return RedirectToPage("/Admin/Index");
        return null!;
    }

    private void Load()
    {
        Config = _cfg.Config;
        Users  = _auth.GetEmails();
        Todos  = _prod.Todos.ToList();
    }

    public IActionResult OnGet()
    {
        var r = CheckAuth(); if (r is not null) return r;
        Load();
        return Page();
    }

    // ── Configurações ─────────────────────────────────────────────
    public IActionResult OnPostSalvarConfig(string telefone, string whatsapp, string email)
    {
        var r = CheckAuth(); if (r is not null) return r;
        _cfg.Save(new SiteConfig { Telefone = telefone, Whatsapp = whatsapp, Email = email });
        Mensagem = "Configurações salvas com sucesso."; MensagemTipo = "ok";
        Load(); return Page();
    }

    // ── Usuários ──────────────────────────────────────────────────
    public IActionResult OnPostAdicionarUsuario(string novoEmail, string novaSenha)
    {
        var r = CheckAuth(); if (r is not null) return r;
        if (_auth.AddUser(novoEmail, novaSenha))
        { Mensagem = $"Usuário {novoEmail} criado."; MensagemTipo = "ok"; }
        else
        { Mensagem = "E-mail já cadastrado."; MensagemTipo = "erro"; }
        Load(); return Page();
    }

    public IActionResult OnPostAlterarSenha(string emailAlterarSenha, string novaSenhaAlterada)
    {
        var r = CheckAuth(); if (r is not null) return r;
        _auth.ChangePassword(emailAlterarSenha, novaSenhaAlterada);
        Mensagem = "Senha alterada."; MensagemTipo = "ok";
        Load(); return Page();
    }

    public IActionResult OnPostRemoverUsuario(string emailRemover)
    {
        var r = CheckAuth(); if (r is not null) return r;
        var atual = HttpContext.Session.GetString("admin");
        if (atual?.Equals(emailRemover, StringComparison.OrdinalIgnoreCase) == true)
        { Mensagem = "Não é possível remover o usuário logado."; MensagemTipo = "erro"; }
        else
        { _auth.DeleteUser(emailRemover); Mensagem = $"Usuário {emailRemover} removido."; MensagemTipo = "ok"; }
        Load(); return Page();
    }

    // ── Produtos ──────────────────────────────────────────────────
    public IActionResult OnPostAdicionarProduto(
        string slug, string nome, string marca, string categoria,
        string img, string descricao, string descricaoLonga,
        string features, string relacionados)
    {
        var r = CheckAuth(); if (r is not null) return r;
        var produto = new SMVTelecom.Data.Produto
        {
            Slug          = slug.Trim().ToLower(),
            Nome          = nome.Trim(),
            Marca         = marca.Trim(),
            Categoria     = categoria,
            Img           = img.Trim(),
            Descricao     = descricao.Trim(),
            DescricaoLonga = descricaoLonga.Trim(),
            Features      = features.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries),
            Relacionados  = relacionados.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries),
        };
        _prod.Add(produto);
        Mensagem = $"Produto \"{nome}\" adicionado."; MensagemTipo = "ok";
        Load(); return Page();
    }

    public IActionResult OnPostRemoverProduto(string slugRemover)
    {
        var r = CheckAuth(); if (r is not null) return r;
        if (_prod.Delete(slugRemover))
        { Mensagem = "Produto removido."; MensagemTipo = "ok"; }
        else
        { Mensagem = "Produto não encontrado ou é estático."; MensagemTipo = "erro"; }
        Load(); return Page();
    }

    // ── Logout ────────────────────────────────────────────────────
    public IActionResult OnPostLogout()
    {
        HttpContext.Session.Clear();
        return RedirectToPage("/Admin/Index");
    }
}
