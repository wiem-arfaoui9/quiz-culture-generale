using SQLite;

namespace QuizApp.Models;

[Table("Users")]
public class User
{
    [PrimaryKey, AutoIncrement] public int Id { get; set; }
    [Unique] public string Username { get; set; }
    public string PasswordHash { get; set; }
    public string Salt { get; set; }
    public string Role { get; set; } = "User";     // "Admin" ou "User"
}