using SFML.Graphics;
using SFML.System;

namespace CurvyCurves
{
    public class Tracer : Drawable
    {
        private CircleShape tracer { get; set; }

        private float progress;
        public float Progress 
        { 
            set => progress = value < 0 || value > 1 ? 0 : value; 
            get => progress; 
        }

        public Vector2f Position { get => tracer.Position; }

        public Tracer(Color color = default)
        {
            progress = 0;

            tracer = new CircleShape(10)
            {
                Origin = new Vector2f(10, 10),
                FillColor = color == default ? Color.White : color
            };
        }

        public void Update(Line line, float stepSize = 0.01f)
        {
            tracer.Position = Lerp(line, progress);
            progress += stepSize;
            if (progress + stepSize > 1)
                progress = 1;
        }

        public static Vector2f Lerp(Line line, float t)
        {
            return (1 - t) * line.Start + t * line.End;
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            target.Draw(tracer, states);
        }
    }

}
