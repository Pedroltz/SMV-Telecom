using System.Text.Json;
using System.Text.Json.Serialization;
using SMVTelecom.Data;

namespace SMVTelecom.Services;

public class ProdutoService
{
    private readonly string _extraPath;
    private List<Produto> _todos;

    private static readonly JsonSerializerOptions _opts = new()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public ProdutoService(IWebHostEnvironment env)
    {
        _extraPath = Path.Combine(env.ContentRootPath, "data", "produtos-extra.json");
        _todos = Build();
    }

    public List<Produto> Todos => _todos;

    public Produto? FindBySlug(string slug) =>
        _todos.FirstOrDefault(p => p.Slug.Equals(slug, StringComparison.OrdinalIgnoreCase));

    public List<Produto> GetByCategoria(string categoria) =>
        _todos.Where(p => p.Categoria == categoria).ToList();

    public List<Produto> GetRelacionados(Produto produto) =>
        produto.Relacionados
            .Select(FindBySlug)
            .Where(p => p is not null)
            .Cast<Produto>()
            .ToList();

    public bool IsExtra(string slug) =>
        LoadExtras().Any(p => p.Slug.Equals(slug, StringComparison.OrdinalIgnoreCase));

    public void Add(Produto produto)
    {
        var extras = LoadExtras();
        extras.Add(produto);
        SaveExtras(extras);
        _todos = Build();
    }

    public void Update(Produto produto)
    {
        var extras = LoadExtras();
        var i = extras.FindIndex(p => p.Slug == produto.Slug);
        if (i >= 0) extras[i] = produto;
        SaveExtras(extras);
        _todos = Build();
    }

    public bool Delete(string slug)
    {
        var extras = LoadExtras();
        var ok = extras.RemoveAll(p => p.Slug == slug) > 0;
        if (ok) { SaveExtras(extras); _todos = Build(); }
        return ok;
    }

    private List<Produto> Build()
    {
        var lista = new List<Produto>(ProdutoCatalog.Todos);
        lista.AddRange(LoadExtras());
        return lista;
    }

    private List<Produto> LoadExtras()
    {
        if (!File.Exists(_extraPath)) return new();
        try { return JsonSerializer.Deserialize<List<Produto>>(File.ReadAllText(_extraPath)) ?? new(); }
        catch { return new(); }
    }

    private void SaveExtras(List<Produto> extras)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(_extraPath)!);
        File.WriteAllText(_extraPath, JsonSerializer.Serialize(extras, _opts));
    }
}
