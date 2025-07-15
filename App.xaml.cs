using System.Globalization;
using System.Windows;
using System.Windows.Forms; 

namespace ReRoundify;

public partial class App : System.Windows.Application
{
    private const double DefaultCornerRadius = 16;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Start with the default radius.
        double cornerRadius = DefaultCornerRadius;

        // Check if command-line arguments were provided.
        // We look for the pattern: --radius [number]
        if (e.Args.Length > 0)
        {
            for (int i = 0; i < e.Args.Length; i++)
            {
                // Find the "--radius" flag.
                if (e.Args[i].ToLower() == "--radius" && i + 1 < e.Args.Length)
                {
                    // Try to parse the *next* argument as a number.
                    // This is safe and won't crash if the user types something invalid.
                    if (double.TryParse(e.Args[i + 1], CultureInfo.InvariantCulture, out double parsedRadius))
                    {
                        cornerRadius = parsedRadius;
                        break; // We found our flag, no need to look further.
                    }
                }
            }
        }
        
        // Loop through each monitor connected to the system.
        // Each window will now be created with the determined cornerRadius.
        foreach (var screen in Screen.AllScreens)
        {
            var bounds = screen.Bounds;
            
            var window = new CornerWindow(cornerRadius)
            {
                Left = bounds.Left,
                Top = bounds.Top,
                Width = bounds.Width,
                Height = bounds.Height
            };
        
            window.Show();
        }
    }
}