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

            CurveTool tool = new CurveTool(window, 0.01f, new Color(255, 0, 0));

            while (window.IsOpen)
            {
                tool.Update();
                window.Clear();

                window.Draw(tool);

                window.DispatchEvents();
                window.Display();
            }
        }
    }
}