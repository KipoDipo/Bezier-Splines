using SFML.Graphics;
using SFML.Window;
using SFML.System;

namespace Test
{
    public class Line : Drawable
    {
        public Vertex[] ver;

        public Line(Vector2f start, Vector2f end)
        {
            ver = new Vertex[2]
            {
                    new Vertex(start),
                    new Vertex(end),
            };
        }

        public void Update(Vector2f start, Vector2f end)
        {
            ver[0].Position = start;
            ver[1].Position = end;
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            target.Draw(ver, PrimitiveType.Lines);
        }
    }
    public partial class Program
    {
        static int WIDTH = 770, HEIGHT = 770;
        static RenderWindow window = new RenderWindow(new VideoMode((uint)WIDTH, (uint)HEIGHT), "Testy");


        static void Main(string[] args)
        {
            List<Line> lines = new List<Line>();

            (int x, int y)
                start = (0, HEIGHT),
                end = (0, 0);

            int stepSize = 30;
            for (int i = 0; i < WIDTH / stepSize; i++)
            {
                lines.Add(new Line(new Vector2f(start.x, start.y), new Vector2f(end.x, end.y)));
                start.y -= stepSize;
                end.x += stepSize;
            }
            start = (0, 0);
            end = (WIDTH, 0);
            for (int i = 0; i < WIDTH / stepSize; i++)
            {
                lines.Add(new Line(new Vector2f(start.x, start.y), new Vector2f(end.x, end.y)));
                start.x += stepSize;
                end.y += stepSize;
            }
            start = (WIDTH, 0);
            end = (WIDTH, HEIGHT);
            for (int i = 0; i < WIDTH / stepSize; i++)
            {
                lines.Add(new Line(new Vector2f(start.x, start.y), new Vector2f(end.x, end.y)));
                start.y += stepSize;
                end.x -= stepSize;
            }
            start = (WIDTH, HEIGHT);
            end = (0, HEIGHT);
            for (int i = 0; i < WIDTH / stepSize; i++)
            {
                lines.Add(new Line(new Vector2f(start.x, start.y), new Vector2f(end.x, end.y)));
                start.x -= stepSize;
                end.y -= stepSize;
            }



            while (window.IsOpen)
            {
                window.Clear();
                foreach (var l in lines)
                    window.Draw(l);
                //ver[0] = new Vertex(new Vector2f(ver[0].Position.X + 10, ver[0].Position.Y));
                //ver[1] = new Vertex(new Vector2f(ver[0].Position.X, ver[0].Position.Y + 10));
                //
                //window.Draw(ver, PrimitiveType.Lines);

                window.DispatchEvents();
                window.Display();
            }
        }
    }
}