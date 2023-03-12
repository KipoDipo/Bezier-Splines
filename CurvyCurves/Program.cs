using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace CurvyCurves
{
    public class Program
    {
        const int WIDTH = 1280, HEIGHT = 720;
        static RenderWindow window = new RenderWindow(new VideoMode(WIDTH, HEIGHT), "Bézier Curves");

        static void Main(string[] args)
        {
            window.SetVerticalSyncEnabled(true);
            window.Closed += Window_Closed;

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

        private static void Window_Closed(object? sender, EventArgs e)
        {
            window.Close();
        }
    }
}