using System.Security.Cryptography;
using QuizApp.Models;

namespace QuizApp.Services;

public class AuthService
{
    private readonly DatabaseService _db;
    private readonly UserSession _session;

    public AuthService(DatabaseService db, UserSession session)
    {
        _db = db;
        _session = session;
    }

    private static string Hash(string password, byte[] salt) =>
        Convert.ToBase64String(Rfc2898DeriveBytes.Pbkdf2(
            password, salt, 100_000, HashAlgorithmName.SHA256, 32));

    public async Task<(bool ok, string message)> RegisterAsync(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrEmpty(password) || password.Length < 6)
            return (false, "Nom requis et mot de passe d'au moins 6 caractères.");

        if (await _db.GetUserAsync(username.Trim()) != null)
            return (false, "Ce nom d'utilisateur existe déjà.");

        byte[] salt = RandomNumberGenerator.GetBytes(16);
        await _db.AddUserAsync(new User
        {
            Username = username.Trim(),
            Salt = Convert.ToBase64String(salt),
            PasswordHash = Hash(password, salt)
        });
        return (true, "Compte créé.");
    }

    public async Task<bool> LoginAsync(string username, string password)
    {
        var user = await _db.GetUserAsync(username?.Trim() ?? "");
        if (user == null || string.IsNullOrEmpty(password)) return false;

        bool ok = Hash(password, Convert.FromBase64String(user.Salt)) == user.PasswordHash;
        if (ok) _session.Username = user.Username;      // on garde qui est connecté
        return ok;
    }
}