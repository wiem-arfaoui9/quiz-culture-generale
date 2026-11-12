using SQLite;
using QuizApp.Models;

namespace QuizApp.Services;

public class DatabaseService
{
    private SQLiteAsyncConnection _db;
    private readonly SemaphoreSlim _lock = new(1, 1);

    private async Task InitAsync()
    {
        if (_db != null) return;
        await _lock.WaitAsync();
        try
        {
            if (_db != null) return;

            var path = Path.Combine(FileSystem.AppDataDirectory, "quiz2.db3");
            var conn = new SQLiteAsyncConnection(path);
            await conn.CreateTableAsync<User>();
            await conn.CreateTableAsync<Category>();
            await conn.CreateTableAsync<Question>();
            await conn.CreateTableAsync<ScoreRecord>();
            await SeedAsync(conn);
            _db = conn;
        }
        finally { _lock.Release(); }
    }

    // Données de démonstration : seulement au tout premier lancement
    private static async Task SeedAsync(SQLiteAsyncConnection conn)
    {
        if (await conn.Table<Category>().CountAsync() > 0) return;

        var cat = new Category { Name = "Culture générale", Icon = "🌍" };
        await conn.InsertAsync(cat);

        Question Q(int lvl, string t, string a, string b, string c, string d, int ok, string exp) => new()
        {
            CategoryId = cat.Id,
            Level = lvl,
            Text = t,
            Option1 = a,
            Option2 = b,
            Option3 = c,
            Option4 = d,
            CorrectIndex = ok,
            Explanation = exp
        };

        await conn.InsertAllAsync(new List<Question>
        {
            Q(1, "Capitale de la Tunisie ?", "Sfax", "Tunis", "Sousse", "Bizerte", 1, "Tunis est la capitale du pays."),
            Q(1, "Capitale de la France ?", "Lyon", "Paris", "Nice", "Lille", 1, "Paris est la capitale et la plus grande ville de France."),
            Q(1, "Combien y a-t-il de continents ?", "5", "6", "7", "8", 2, "On compte 7 continents dans le modèle le plus courant."),
            Q(2, "Quelle est la plus grande planète du système solaire ?", "Mars", "Jupiter", "Saturne", "Terre", 1, "Jupiter est une géante gazeuse, la plus grosse planète."),
            Q(2, "En quelle année a débuté la Révolution française ?", "1689", "1789", "1889", "1989", 1, "La prise de la Bastille a eu lieu en 1789."),
            Q(2, "Quel est le plus long fleuve d'Afrique ?", "Le Congo", "Le Niger", "Le Nil", "Le Zambèze", 2, "Le Nil mesure environ 6 650 km."),
        });
    }

    // ---------- USERS ----------
    public async Task<User> GetUserAsync(string username)
    {
        await InitAsync();
        return await _db.Table<User>()
            .Where(u => u.Username == username).FirstOrDefaultAsync();
    }

    public async Task AddUserAsync(User u)
    {
        await InitAsync();
        await _db.InsertAsync(u);
    }

    // ---------- CATEGORIES ----------
    public async Task<List<Category>> GetCategoriesAsync()
    {
        await InitAsync();
        return await _db.Table<Category>().ToListAsync();
    }

    public async Task SaveCategoryAsync(Category c)       // ajouter OU modifier
    {
        await InitAsync();
        if (c.Id == 0) await _db.InsertAsync(c);
        else await _db.UpdateAsync(c);
    }

    public async Task DeleteCategoryAsync(Category c)
    {
        await InitAsync();
        await _db.ExecuteAsync("DELETE FROM Questions WHERE CategoryId = ?", c.Id);
        await _db.ExecuteAsync("DELETE FROM Scores WHERE CategoryId = ?", c.Id);
        await _db.DeleteAsync(c);
    }

    // ---------- QUESTIONS ----------
    public async Task<List<Question>> GetQuestionsAsync(int categoryId)            // pour l'admin
    {
        await InitAsync();
        return await _db.Table<Question>()
            .Where(q => q.CategoryId == categoryId).ToListAsync();
    }

    public async Task<List<Question>> GetQuestionsAsync(int categoryId, int level) // pour le jeu
    {
        await InitAsync();
        var list = await _db.Table<Question>()
            .Where(q => q.CategoryId == categoryId && q.Level == level).ToListAsync();
        return list.OrderBy(_ => Random.Shared.Next()).ToList();                   // mélange
    }

    public async Task<Question> GetQuestionAsync(int id)
    {
        await InitAsync();
        return await _db.Table<Question>()
            .Where(q => q.Id == id).FirstOrDefaultAsync();
    }

    public async Task SaveQuestionAsync(Question q)       // ajouter OU modifier
    {
        await InitAsync();
        if (q.Id == 0) await _db.InsertAsync(q);
        else await _db.UpdateAsync(q);
    }

    public async Task DeleteQuestionAsync(Question q)
    {
        await InitAsync();
        await _db.DeleteAsync(q);
    }

    // ---------- SCORES ----------
    public async Task SaveScoreAsync(ScoreRecord s)
    {
        await InitAsync();
        await _db.InsertAsync(s);
    }

    public async Task<int> GetBestScoreAsync(string username, int categoryId)
    {
        await InitAsync();
        var list = await _db.Table<ScoreRecord>()
            .Where(s => s.Username == username && s.CategoryId == categoryId).ToListAsync();
        return list.Count == 0 ? 0 : list.Max(s => s.Points);
    }
}