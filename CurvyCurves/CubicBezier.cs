using SFML.Graphics;
using SFML.System;

namespace CurvyCurves
{
    public class CubicBezier : Drawable
    {
        private List<Line> lines = new List<Line>();
        private float stepSize;
        private Color color;

        public CubicBezier(Line line1, Line line2, float stepSize, Color color = default)
        {
            this.stepSize = stepSize;
            this.color = color;
            Update(line1, line2);
        }

        public Vector2f Start { get => lines[0].Start; }
        public Vector2f End   { get => lines[^1].End; }

        public void Update(Line line1, Line line2)
        {
            lines.Clear();

            Line line3 = new Line(line1.End, line2.Start);

            Tracer t1 = new Tracer();
            Tracer t2 = new Tracer();
            Tracer t3 = new Tracer();

            Tracer t4 = new Tracer();
            Tracer t5 = new Tracer();

            Tracer t6 = new Tracer();

            for (float i = 0; i < 1; i += stepSize)
            {
                t1.Update(line1, stepSize);
                t2.Update(line2, stepSize);
                t3.Update(line3, stepSize);

                Line lineLeft = new Line(t1.Position, t3.Position);
                Line lineRight = new Line(t3.Position, t2.Position);

                t4.Update(lineLeft, stepSize);
                t5.Update(lineRight, stepSize);

                Line lastLine = new Line(t4.Position, t5.Position);

                t6.Update(lastLine, stepSize);

                if (i == 0)
                    lines.Add(new Line(line1.Start, t6.Position, color));
                else
                    lines.Add(new Line(lines[^1].End, t6.Position, color));

                i = MathF.Round(i, 2);
            }
            lines.Add(new Line(lines[^1].End, line2.End, color));
        }


        public void Draw(RenderTarget target, RenderStates states)
        {
            foreach (Line l in lines)
                target.Draw(l, states);
        }
    }
}
