using System.Text.Json;
using System.Text.Json.Serialization;
using SMVTelecom.Data;

namespace SMVTelecom.Services;

public class ProdutoService
{
    private readonly string _extraPath;
    private readonly string _removidosPath;
    private List<Produto> _todos;

    private static readonly JsonSerializerOptions _opts = new()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public ProdutoService(IWebHostEnvironment env)
    {
        var data = Path.Combine(env.ContentRootPath, "data");
        _extraPath    = Path.Combine(data, "produtos-extra.json");
        _removidosPath = Path.Combine(data, "produtos-removidos.json");
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

    // Produto adicionado/editado via admin (não é estático original)
    public bool IsExtra(string slug) =>
        LoadExtras().Any(p => p.Slug.Equals(slug, StringComparison.OrdinalIgnoreCase));

    public void Add(Produto produto)
    {
        var extras = LoadExtras();
        extras.Add(produto);
        SaveExtras(extras);
        _todos = Build();
    }

    public bool Update(Produto produto)
    {
        var extras   = LoadExtras();
        var removidos = LoadRemovidos();
        var i = extras.FindIndex(p => p.Slug == produto.Slug);

        if (i >= 0)
        {
            // É um extra — atualiza direto
            extras[i] = produto;
        }
        else if (ProdutoCatalog.Todos.Any(p => p.Slug == produto.Slug))
        {
            // É estático — adiciona override em extras e marca o original como removido
            extras.Add(produto);
            if (!removidos.Contains(produto.Slug))
            {
                removidos.Add(produto.Slug);
                SaveRemovidos(removidos);
            }
        }
        else return false;

        SaveExtras(extras);
        _todos = Build();
        return true;
    }

    public bool Delete(string slug)
    {
        var extras    = LoadExtras();
        var removidos = LoadRemovidos();

        // Tenta remover de extras primeiro
        var ok = extras.RemoveAll(p => p.Slug == slug) > 0;
        if (ok) SaveExtras(extras);

        // Se é estático, adiciona à lista de removidos
        if (ProdutoCatalog.Todos.Any(p => p.Slug == slug) && !removidos.Contains(slug))
        {
            removidos.Add(slug);
            SaveRemovidos(removidos);
            ok = true;
        }

        if (ok) _todos = Build();
        return ok;
    }

    private List<Produto> Build()
    {
        var removidos = LoadRemovidos();
        var lista = ProdutoCatalog.Todos
            .Where(p => !removidos.Contains(p.Slug))
            .ToList();
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

    private List<string> LoadRemovidos()
    {
        if (!File.Exists(_removidosPath)) return new();
        try { return JsonSerializer.Deserialize<List<string>>(File.ReadAllText(_removidosPath)) ?? new(); }
        catch { return new(); }
    }

    private void SaveRemovidos(List<string> removidos)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(_removidosPath)!);
        File.WriteAllText(_removidosPath, JsonSerializer.Serialize(removidos, _opts));
    }
}
