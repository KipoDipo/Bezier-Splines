using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace CurvyCurves
{
    public class BezierSpline : Drawable
    {
        private List<CubicBezierAdvanced> curves;
        private Vector2f? startingPos;

        private Color color;

        private float stepSize;

        private bool isConnected;
        public bool IsConnected { get => isConnected; }

        private bool isInterracting;
        private bool hasGrabbedBase;
        private bool hasGrabbedCtrl;

        public bool IsInterracting { get => isInterracting; }
        public bool HasGrabbedBase { get => hasGrabbedBase; }
        public bool HasGrabbedCtrl { get => hasGrabbedCtrl; }


        public List<CubicBezierAdvanced> Curves { set => curves = value; get => curves; }

        public delegate void CubicBezierMouseEventHandler(CubicBezierAdvanced sender, Vector2f position);
        public delegate void BezierSplineEventHandler(BezierSpline sender);

        public event CubicBezierMouseEventHandler? OnGrabbedBase1;
        public event CubicBezierMouseEventHandler? OnGrabbedBase2;
        public event CubicBezierMouseEventHandler? OnGrabbedCtrl1;
        public event CubicBezierMouseEventHandler? OnGrabbedCtrl2;

        public event CubicBezierMouseEventHandler? OnHoverBase1;
        public event CubicBezierMouseEventHandler? OnHoverBase2;
        public event CubicBezierMouseEventHandler? OnHoverCtrl1;
        public event CubicBezierMouseEventHandler? OnHoverCtrl2;

        public event BezierSplineEventHandler? OnUpdate;

        public BezierSpline(float stepSize = 0.01f, Color color = default)
        {
            curves = new List<CubicBezierAdvanced>();
            startingPos = null;
            this.stepSize = stepSize;
            this.color = color;
            isConnected = false;
        }

        public void Add(Vector2f position)
        {
            if (IsConnected)
                return;

            if (curves.Count == 0 && startingPos == null)
            {
                startingPos = position;
                return;
            }
            Line line1, line2;
            Color lineColor = new Color(255, 255, 255, 100);
            int dir;
            if (curves.Count == 0 && startingPos != null)
            {
                line1 = new Line((Vector2f)startingPos, (Vector2f)startingPos + new Vector2f(-50, 0), lineColor);
                dir = (line1.End.X - position.X) > 0 ? 1 : -1;
                line2 = new Line(position + new Vector2f(50, 0) * dir, position, lineColor);
                curves.Add(new CubicBezierAdvanced(line1, line2, stepSize, color));
            }
            else
            {
                line1 = new Line(curves[^1].Line2.End, curves[^1].Line2.End + (curves[^1].Line2.End - curves[^1].Line2.Start), lineColor);
                dir = (line1.End.X - position.X) > 0 ? 1 : -1;
                line2 = new Line(position + new Vector2f(50, 0) * dir, position, lineColor);
                curves.Add(new CubicBezierAdvanced(line1, line2, stepSize, color));
            }

            curves[^1].OnGrabbedBase1 += BezierSpline_OnGrabbedBase1;
            curves[^1].OnGrabbedBase2 += BezierSpline_OnGrabbedBase2;
            curves[^1].OnGrabbedCtrl1 += BezierSpline_OnGrabbedCtrl1;
            curves[^1].OnGrabbedCtrl2 += BezierSpline_OnGrabbedCtrl2;
            curves[^1].OnHoverBase1 += BezierSpline_OnHoverBase1;
            curves[^1].OnHoverBase2 += BezierSpline_OnHoverBase2;
            curves[^1].OnHoverCtrl1 += BezierSpline_OnHoverCtrl1;
            curves[^1].OnHoverCtrl2 += BezierSpline_OnHoverCtrl2;
        }

        private void BezierSpline_OnHoverBase1(CubicBezierAdvanced sender, Vector2f position)
        {
            OnHoverBase1?.Invoke(sender, position);
        }
        private void BezierSpline_OnHoverBase2(CubicBezierAdvanced sender, Vector2f position)
        {
            OnHoverBase2?.Invoke(sender, position);
        }
        private void BezierSpline_OnHoverCtrl1(CubicBezierAdvanced sender, Vector2f position)
        {
            OnHoverCtrl1?.Invoke(sender, position);
        }
        private void BezierSpline_OnHoverCtrl2(CubicBezierAdvanced sender, Vector2f position)
        {
            OnHoverCtrl2?.Invoke(sender, position);
        }

        private void BezierSpline_OnGrabbedBase1(CubicBezierAdvanced sender, Vector2f position)
        {
            OnGrabbedBase1?.Invoke(sender, position);
        }
        private void BezierSpline_OnGrabbedBase2(CubicBezierAdvanced sender, Vector2f position)
        {
            OnGrabbedBase2?.Invoke(sender, position);
        }
        private void BezierSpline_OnGrabbedCtrl1(CubicBezierAdvanced sender, Vector2f position)
        {
            OnGrabbedCtrl1?.Invoke(sender, position);
        }
        private void BezierSpline_OnGrabbedCtrl2(CubicBezierAdvanced sender, Vector2f position)
        {
            OnGrabbedCtrl2?.Invoke(sender, position);
        }

        public void Pop()
        {
            if (curves.Count > 0)
            {
                curves.RemoveAt(curves.Count - 1);
                isConnected = false;
            }
            if (curves.Count == 0)
                startingPos = null;
        }

        public void Connect()
        {
            if (!isConnected)
            {
                Add(curves[0].Line1.Start);
                isConnected = true;
            }
        }

        public void Update()
        {
            OnUpdate?.Invoke(this);
            isInterracting = hasGrabbedCtrl = hasGrabbedBase = false;
            foreach (var c in Curves)
            {
                if (c.HasGrabbed)
                {
                    if (c.HasGrabbedBase)
                        hasGrabbedBase = true;
                    else if (c.HasGrabbedControl)
                        hasGrabbedCtrl = true;
                    isInterracting = true;
                    break;
                }
            }
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            foreach (var c in curves)
                target.Draw(c, states);
        }
    }
}
