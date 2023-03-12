using SFML.Graphics;
using SFML.System;

namespace CurvyCurves
{
    public class Line : Drawable
    {
        public Vector2f Start { get; set; }
        public Vector2f End { get; set; }

        private Vertex[] line;

        private Color color;

        public Line((float x, float y) start, (float x, float y) end, Color color = default)
        {
            Start = new Vector2f(start.x, start.y);
            End = new Vector2f(end.x, end.y);
            this.color = color == default ? Color.White : color;
            line = new Vertex[2]
            {
                new Vertex(Start) { Color = this.color },
                new Vertex(End) { Color = this.color },
            };
        }

        public Line()
            : this((0, 0), (0, 0))
        {
        }

        public Line(Vector2f start, Vector2f end, Color color = default)
            : this((start.X, start.Y), (end.X, end.Y), color)
        {
        }

        public Line(Line copy)
            : this((copy.Start.X, copy.Start.Y), (copy.End.X, copy.End.Y), copy.color)
        {

        }

        public void Update(Vector2f start, Vector2f end)
        {
            Update((start.X, start.Y), (end.X, end.Y));
        }

        public void Update((float x, float y) start, (float x, float y) end)
        {
            Start = new Vector2f(start.x, start.y);
            End = new Vector2f(end.x, end.y);
            line[0].Position = Start;
            line[1].Position = End;
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            target.Draw(line, PrimitiveType.Lines, states);
        }
    }

}
