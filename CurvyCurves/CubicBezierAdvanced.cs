using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace CurvyCurves
{
    public class CubicBezierAdvanced : Drawable
    {
        private CircleShape ctrl1, ctrl2;
        private RectangleShape base1, base2;

        private Line l1, l2, l3, l4, l5, l6;
        private Tracer t1, t2, t3, t4, t5, t6;

        private bool isHoldingCtrl1 = false, isHoldingCtrl2 = false;
        private bool isHoldingBase1 = false, isHoldingBase2 = false;
        private bool hasGrabbed = false;

        private CubicBezier bezier;

        public bool HasGrabbed { get => hasGrabbed; }

        public bool HasGrabbedControl { get => isHoldingCtrl1 || isHoldingCtrl2; }
        public bool HasGrabbedControl1 { get => isHoldingCtrl1; }
        public bool HasGrabbedControl2 { get => isHoldingCtrl2; }

        public bool HasGrabbedBase { get => isHoldingBase1 || isHoldingBase2; }
        public bool HasGrabbedBase1 { get => isHoldingBase1; }
        public bool HasGrabbedBase2 { get => isHoldingBase2; }

        public CubicBezier Bezier { get => bezier; }
        public Line Line1 { get => l1; }
        public Line Line2 { get => l2; }

        public delegate void CubicBezierMouseEventHandler(CubicBezierAdvanced sender, Vector2f position);

        public event CubicBezierMouseEventHandler? OnGrabbedBase1;
        public event CubicBezierMouseEventHandler? OnGrabbedBase2;
        public event CubicBezierMouseEventHandler? OnGrabbedCtrl1;
        public event CubicBezierMouseEventHandler? OnGrabbedCtrl2;

        public event CubicBezierMouseEventHandler? OnHoverBase1;
        public event CubicBezierMouseEventHandler? OnHoverBase2;
        public event CubicBezierMouseEventHandler? OnHoverCtrl1;
        public event CubicBezierMouseEventHandler? OnHoverCtrl2;

        public CubicBezierAdvanced(Line line1, Line line2, float stepSize, Color color = default)
        {
            ctrl1 = new CircleShape(10)
            {
                FillColor = new Color(255, 255, 255, 100),
                OutlineColor = new Color(255, 255, 255, 180),
                OutlineThickness = 1,
                Origin = new Vector2f(10, 10)
            };
            ctrl2 = new CircleShape(ctrl1);
            base1 = new RectangleShape(new Vector2f(20, 20))
            {
                FillColor = new Color(255, 255, 255, 100),
                OutlineColor = new Color(255, 255, 255, 180),
                OutlineThickness = 1,
                Origin = new Vector2f(20, 20) / 2
            };
            base2 = new RectangleShape(base1);

            l1 = new Line(line1);
            l2 = new Line(line2);
            l3 = new Line((0, 0), (0, 0), new Color(255, 255, 255, 50));
            l4 = new Line((0, 0), (0, 0), new Color(255, 255, 255, 50));
            l5 = new Line((0, 0), (0, 0), new Color(255, 255, 255, 50));
            l6 = new Line((0, 0), (0, 0), new Color(255, 255, 255, 50));

            t1 = new Tracer(new Color(255, 255, 255, 50));
            t2 = new Tracer(new Color(255, 255, 255, 50));
            t3 = new Tracer(new Color(255, 255, 255, 50));
            t4 = new Tracer(new Color(255, 255, 255, 50));
            t5 = new Tracer(new Color(255, 255, 255, 50));
            t6 = new Tracer(new Color(100, 255, 255, 100));

            bezier = new CubicBezier(line1, line2, stepSize, color);
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            target.Draw(bezier, states);

            if (!Keyboard.IsKeyPressed(Keyboard.Key.Escape))
            {
                target.Draw(l1, states);
                target.Draw(l2, states);
                target.Draw(ctrl1, states);
                target.Draw(ctrl2, states);
                target.Draw(base1, states);
                target.Draw(base2, states);
            }

            if (Keyboard.IsKeyPressed(Keyboard.Key.Space))
            {
                t1.Update(l1);
                t2.Update(l2);
                l3.Update(l1.End, l2.Start);
                t3.Update(l3);
                l4.Update(t1.Position, t3.Position);
                l5.Update(t3.Position, t2.Position);
                t4.Update(l4);
                t5.Update(l5);
                l6.Update(t4.Position, t5.Position);
                t6.Update(l6);
                target.Draw(t1, states);
                target.Draw(t2, states);
                target.Draw(t3, states);
                target.Draw(t4, states);
                target.Draw(t5, states);
                target.Draw(t6, states);
                target.Draw(l3, states);
                target.Draw(l4, states);
                target.Draw(l5, states);
                target.Draw(l6, states);
                if (t1.Progress == 1)
                    t1.Progress = t2.Progress = t3.Progress = t4.Progress = t5.Progress = t6.Progress = 0;
            }
            else
            {
                t1.Progress = t2.Progress = t3.Progress = t4.Progress = t5.Progress = t6.Progress = 0;
            }
        }

        public bool IsHoveringControl1(Vector2f position)
        {
            return MathF.Abs(position.X - ctrl1.Position.X) <= ctrl1.Radius &&
                MathF.Abs(position.Y - ctrl1.Position.Y) <= ctrl1.Radius;
        }
        public bool IsHoveringControl2(Vector2f position)
        {
            return MathF.Abs(position.X - ctrl2.Position.X) <= ctrl2.Radius &&
                MathF.Abs(position.Y - ctrl2.Position.Y) <= ctrl2.Radius;
        }
        public bool IsHoveringBase1(Vector2f position)
        {
            return MathF.Abs(position.X - base1.Position.X) <= base1.Size.X / 2 &&
                MathF.Abs(position.Y - base1.Position.Y) <= base1.Size.Y / 2;
        }
        public bool IsHoveringBase2(Vector2f position)
        {
            return MathF.Abs(position.X - base2.Position.X) <= base2.Size.X / 2 &&
                MathF.Abs(position.Y - base2.Position.Y) <= base2.Size.Y / 2;
        }

        public void SetBase1MoveCtrl1(Vector2f BasePosition)
        {
            Vector2f diff1 = l1.End - BasePosition;
            Vector2f diff2 = BasePosition - l1.Start;
            l1.Update(BasePosition, BasePosition + diff1 + diff2);
            Update();
        }
        public void SetBase2MoveCtrl2(Vector2f BasePosition)
        {
            Vector2f diff1 = l2.Start - BasePosition;
            Vector2f diff2 = BasePosition - l2.End;
            l2.Update(BasePosition + diff1 + diff2, BasePosition);
            Update();
        }
        public void SetLine1(Vector2f Start, Vector2f End)
        {
            l1.Update(Start, End);
            Update();
        }
        public void SetLine2(Vector2f Start, Vector2f End)
        {
            l2.Update(Start, End);
            Update();
        }
        
        private void Update()
        {
            ctrl1.Position = Tracer.Lerp(l1, 1);
            ctrl2.Position = Tracer.Lerp(l2, 0);
            base1.Position = Tracer.Lerp(l1, 0);
            base2.Position = Tracer.Lerp(l2, 1);

            bezier.Update(l1, l2);
        }

        public void DispatchEvents(Vector2f position, bool isInterracting)
        {
            bool isHoveringCtrl1 = IsHoveringControl1(position);
            bool isHoveringCtrl2 = IsHoveringControl2(position);
            bool isHoveringBase1 = IsHoveringBase1(position);
            bool isHoveringBase2 = IsHoveringBase2(position);

            if (isHoveringCtrl1) OnHoverCtrl1?.Invoke(this, position);
            if (isHoveringCtrl2) OnHoverCtrl2?.Invoke(this, position);
            if (isHoveringBase1) OnHoverBase1?.Invoke(this, position);
            if (isHoveringBase2) OnHoverBase2?.Invoke(this, position);

            if (isInterracting)
            {
                if (!hasGrabbed)
                {
                    isHoldingCtrl1 = isHoveringCtrl1;
                    isHoldingCtrl2 = isHoveringCtrl2;
                    isHoldingBase1 = isHoveringBase1;
                    isHoldingBase2 = isHoveringBase2;
                }

                if (isHoldingBase1 || isHoldingBase2 || isHoldingCtrl1 || isHoldingCtrl2)
                    hasGrabbed = true;

                if (isHoldingCtrl1)
                    OnGrabbedCtrl1?.Invoke(this, position);

                else if (isHoldingCtrl2)
                    OnGrabbedCtrl2?.Invoke(this, position);

                else if (isHoldingBase1)
                    OnGrabbedBase1?.Invoke(this, position);

                else if (isHoldingBase2)
                    OnGrabbedBase2?.Invoke(this, position);
            }
            else
            {
                ClearFlags();
                Update();
            }
        }
        private void ClearFlags()
        {
            isHoldingCtrl1 = isHoldingCtrl2 = isHoldingBase1 = isHoldingBase2 = hasGrabbed = false;
        }

    }
}
