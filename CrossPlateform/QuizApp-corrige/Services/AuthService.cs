using System.Security.Cryptography;
using QuizApp.Models;

namespace QuizApp.Services;

public class AuthService
{
    private readonly DatabaseService _db;
    private readonly UserSession _session;

    // Compte administrateur unique (créé automatiquement s'il n'existe pas)
    public const string AdminUsername = "admin";
    private const string AdminDefaultPassword = "Admin123";   // à changer avant la mise en production
    private bool _adminChecked;

    public AuthService(DatabaseService db, UserSession session)
    {
        _db = db;
        _session = session;
    }

    public static string Hash(string password, byte[] salt) =>
        Convert.ToBase64String(Rfc2898DeriveBytes.Pbkdf2(
            password, salt, 100_000, HashAlgorithmName.SHA256, 32));

    private async Task EnsureAdminAsync()
    {
        if (_adminChecked) return;
        if (await _db.GetUserAsync(AdminUsername) == null)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(16);
            await _db.AddUserAsync(new User
            {
                Username = AdminUsername,
                Salt = Convert.ToBase64String(salt),
                PasswordHash = Hash(AdminDefaultPassword, salt),
                Role = "Admin"
            });
        }
        _adminChecked = true;
    }

    private static bool IsReserved(string username) =>
        string.Equals(username, AdminUsername, StringComparison.OrdinalIgnoreCase) ||
        string.Equals(username, "administrateur", StringComparison.OrdinalIgnoreCase);

    public async Task<(bool ok, string message)> RegisterAsync(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrEmpty(password) || password.Length < 6)
            return (false, "Nom requis et mot de passe d'au moins 6 caractères.");

        await EnsureAdminAsync();

        if (IsReserved(username.Trim()))
            return (false, "Ce nom d'utilisateur est réservé.");

        if (await _db.GetUserAsync(username.Trim()) != null)
            return (false, "Ce nom d'utilisateur existe déjà.");

        byte[] salt = RandomNumberGenerator.GetBytes(16);
        await _db.AddUserAsync(new User
        {
            Username = username.Trim(),
            Salt = Convert.ToBase64String(salt),
            PasswordHash = Hash(password, salt),
            Role = "User"                         // l'inscription crée TOUJOURS un User
        });
        return (true, "Compte créé.");
    }

    public async Task<bool> LoginAsync(string username, string password)
    {
        await EnsureAdminAsync();
        var user = await _db.GetUserAsync(username?.Trim() ?? "");
        if (user == null || string.IsNullOrEmpty(password)) return false;

        bool ok = Hash(password, Convert.FromBase64String(user.Salt)) == user.PasswordHash;
        if (ok)
        {
            _session.Username = user.Username;
            _session.Role = user.Role;            // le rôle vient de la base (Admin / User)
        }
        return ok;
    }
}
