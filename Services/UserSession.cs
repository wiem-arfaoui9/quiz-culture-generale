namespace QuizApp.Services;

public class UserSession
{
    public string Username { get; set; }
    public void Logout() => Username = null;
}