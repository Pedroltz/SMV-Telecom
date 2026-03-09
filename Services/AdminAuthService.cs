using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace SMVTelecom.Services;

public class AdminUser
{
    public string Email        { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public string Salt         { get; set; } = "";
}

public class AdminAuthService
{
    private readonly string _path;
    private List<AdminUser> _users;

    public AdminAuthService(IWebHostEnvironment env)
    {
        _path = Path.Combine(env.ContentRootPath, "data", "users.json");
        _users = Load();
        Bootstrap();
    }

    private void Bootstrap()
    {
        if (_users.Count == 0)
        {
            var salt = Guid.NewGuid().ToString("N");
            _users.Add(new AdminUser
            {
                Email        = "contato@smvtelecom.com",
                Salt         = salt,
                PasswordHash = Hash("Igaracu@2026", salt)
            });
            Save();
        }
    }

    public static string Hash(string password, string salt)
    {
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(salt + password));
        return Convert.ToHexString(bytes);
    }

    public bool Authenticate(string email, string password)
    {
        var u = _users.FirstOrDefault(x => x.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        return u is not null && u.PasswordHash == Hash(password, u.Salt);
    }

    public List<string> GetEmails() => _users.Select(u => u.Email).ToList();

    public bool AddUser(string email, string password)
    {
        if (_users.Any(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase))) return false;
        var salt = Guid.NewGuid().ToString("N");
        _users.Add(new AdminUser { Email = email, Salt = salt, PasswordHash = Hash(password, salt) });
        Save();
        return true;
    }

    public bool ChangePassword(string email, string newPassword)
    {
        var u = _users.FirstOrDefault(x => x.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        if (u is null) return false;
        u.Salt = Guid.NewGuid().ToString("N");
        u.PasswordHash = Hash(newPassword, u.Salt);
        Save();
        return true;
    }

    public bool DeleteUser(string email)
    {
        var removed = _users.RemoveAll(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase)) > 0;
        if (removed) Save();
        return removed;
    }

    private List<AdminUser> Load()
    {
        if (!File.Exists(_path)) return new();
        try { return JsonSerializer.Deserialize<List<AdminUser>>(File.ReadAllText(_path)) ?? new(); }
        catch { return new(); }
    }

    private void Save()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(_path)!);
        File.WriteAllText(_path, JsonSerializer.Serialize(_users, new JsonSerializerOptions { WriteIndented = true }));
    }
}
