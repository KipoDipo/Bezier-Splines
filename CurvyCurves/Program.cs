using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace CurvyCurves
{
    public class Program
    {
        static RenderWindow window = new RenderWindow(new VideoMode(1280, 720), "Bézier Curves");

        static void Main(string[] args)
        {
            window.SetVerticalSyncEnabled(true);
            window.Closed += (object? sender, EventArgs e) => window.Close();

            Color theme = new Color(0, 255, 150);

            CurveTool tool = new CurveTool(window, 0.01f, theme);
            Color background = new Color((byte)(theme.R/10),(byte)(theme.G/10),(byte)(theme.B/10));

            while (window.IsOpen)
            {
                tool.Update();

                window.Clear(background);
                window.Draw(tool);
                window.DispatchEvents();
                window.Display();
            }
        }
    }
}