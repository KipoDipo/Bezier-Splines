using SFML.Graphics;

namespace CurvyCurves
{
    public class QuadraticBezier : Drawable
    {
        private List<Line> lines = new List<Line>();
        private float stepSize;
        private Color color;

        public QuadraticBezier(Line line1, Line line2, float stepSize, Color color = default)
        {
            this.stepSize = stepSize;
            this.color = color;
            Update(line1, line2);
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            foreach (Line l in lines)
                target.Draw(l, states);
        }

        public void Update(Line line1, Line line2)
        {
            lines.Clear();

            Line line3 = new Line(line1.End, line2.Start);

            Tracer t1 = new Tracer();
            Tracer t2 = new Tracer();
            Tracer t3 = new Tracer();
            
            for (float i = 0; i < 1; i += stepSize)
            {
                t1.Update(line1, stepSize);
                t2.Update(line2, stepSize);

                line3.Update((t1.Position.X, t1.Position.Y), (t2.Position.X, t2.Position.Y));
                t3.Update(line3, stepSize);

                if (i != 0)
                    lines.Add(new Line(lines[^1].End, t3.Position, color));
                else
                    lines.Add(new Line(line1.Start, t3.Position, color));
                i = MathF.Round(i, 2);
            }
            lines.Add(new Line(lines[^1].End, line2.End, color));
        }
    }
}
