namespace QuizApp.Services;

public class UserSession
{
    public string Username { get; set; }
    public bool IsAdmin { get; set; }

    public void Logout()
    {
        Username = null;
        IsAdmin = false;
    }
}