using System.Text.Json;

namespace SMVTelecom.Services;

public class SiteConfig
{
    public string Telefone  { get; set; } = "+55 11 98515-2865";
    public string Whatsapp  { get; set; } = "5511985152865";
    public string Email     { get; set; } = "contato@smvtelecom.com";
}

public class SiteConfigService
{
    private readonly string _path;
    private SiteConfig _config;

    public SiteConfigService(IWebHostEnvironment env)
    {
        _path = Path.Combine(env.ContentRootPath, "data", "site-config.json");
        _config = Load();
    }

    public SiteConfig Config => _config;

    private SiteConfig Load()
    {
        if (!File.Exists(_path)) return new SiteConfig();
        try { return JsonSerializer.Deserialize<SiteConfig>(File.ReadAllText(_path)) ?? new(); }
        catch { return new SiteConfig(); }
    }

    public void Save(SiteConfig config)
    {
        _config = config;
        Directory.CreateDirectory(Path.GetDirectoryName(_path)!);
        File.WriteAllText(_path, JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true }));
    }
}
