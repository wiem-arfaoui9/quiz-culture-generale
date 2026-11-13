namespace QuizApp.Services;

public class UserSession
{
    public string Username { get; set; }
    public string Role { get; set; }

    public bool IsAdmin => Role == "Admin";

    public void Logout()
    {
        Username = null;
        Role = null;
    }
}
