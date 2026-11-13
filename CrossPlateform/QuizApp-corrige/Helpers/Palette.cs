using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace QuizApp.Helpers;

/// <summary>Dégradés pastel utilisés pour colorer les tuiles de catégories.</summary>
public static class Palette
{
    private static readonly (string from, string to)[] Tones =
    {
        ("#7C5CFC", "#B58CFF"),   // violet
        ("#38BDF8", "#7DD3FC"),   // bleu ciel
        ("#FF7EB6", "#FFA8CF"),   // rose
        ("#FFC933", "#FFA94D"),   // soleil
        ("#2FCB98", "#6EE7B7"),   // menthe
    };

    public static Brush Banner(int index)
    {
        var (from, to) = Tones[Math.Abs(index) % Tones.Length];
        return new LinearGradientBrush(
            new GradientStopCollection
            {
                new GradientStop(Color.FromArgb(from), 0f),
                new GradientStop(Color.FromArgb(to), 1f)
            },
            new Point(0, 0), new Point(1, 1));
    }
}
