using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace CurvyCurves
{
    public class CurveTool : Drawable
    {
        private List<BezierSpline> splines;
        private RenderWindow window;

        private bool isInterracting;

        private Color color;
        private float stepSize;

        public CurveTool(RenderWindow window, float stepSize = 0.01f, Color color = default)
        {

            this.window = window;
            this.color = new Color(color);
            this.stepSize = stepSize;

            isInterracting = false;

            splines = new List<BezierSpline>();

            window.MouseButtonReleased += Window_MouseButtonReleased;
            window.KeyReleased += Window_KeyReleased;
            //splines.Add(new BezierSpline(color: color));
            //splines[^1].Add(new Vector2f(200, 200));
            //splines[^1].Add(new Vector2f(400, 400));
            //splines[^1].Add(new Vector2f(600, 600));
            //splines[^1].OnGrabbedBase1 += Spline_OnGrabbedBase1;
            //splines[^1].OnGrabbedBase2 += Spline_OnGrabbedBase2;
            //splines[^1].OnGrabbedCtrl1 += Spline_OnGrabbedCtrl1;
            //splines[^1].OnGrabbedCtrl2 += Spline_OnGrabbedCtrl2;
            //splines[^1].OnUpdate += Spline_OnUpdate;


            Add(stepSize, color);
        }

        public void Add(float stepSize = 0.01f, Color color = default)
        {
            splines.Add(new BezierSpline(stepSize, color));

            splines[^1].OnGrabbedBase1 += Spline_OnGrabbedBase1;
            splines[^1].OnGrabbedBase2 += Spline_OnGrabbedBase2;
            splines[^1].OnGrabbedCtrl1 += Spline_OnGrabbedCtrl1;
            splines[^1].OnGrabbedCtrl2 += Spline_OnGrabbedCtrl2;
            splines[^1].OnUpdate += Spline_OnUpdate;

        }

        private void Window_KeyReleased(object? sender, KeyEventArgs e)
        {
            if (e.Code == Keyboard.Key.Delete)
            {
                if (splines.Count > 1 && splines[^1].Curves.Count == 0)
                    splines.RemoveAt(splines.Count - 1);
                splines[^1].Pop();
                return;
            }
            if (e.Code == Keyboard.Key.Enter)
            {
                Add(stepSize, color);
            }
        }

        private void Window_MouseButtonReleased(object? sender, MouseButtonEventArgs e)
        {
            if (e.Button == Mouse.Button.Right)
            {
                if (splines[^1].Curves.Count > 0 && splines[^1].Curves[0].IsHoveringBase1(new Vector2f(e.X, e.Y)))
                    splines[^1].Connect();
                else
                    splines[^1].Add((Vector2f)Mouse.GetPosition(window));
            }
        }

        private int updateIndex = -1;
        public void Update()
        {
            if (!isInterracting)
            {
                for (int i = 0; i < splines.Count; i++)
                {
                    splines[i].Update();
                    if (splines[i].IsInterracting)
                    {
                        updateIndex = i;
                        isInterracting = true;
                        break;
                    }
                }
            }
            else
            {
                splines[updateIndex].Update();
                if (!splines[updateIndex].IsInterracting)
                    isInterracting = false;
            }
        }

        private void Spline_OnUpdate(BezierSpline sender)
        {
            for (int i = 0; i < sender.Curves.Count; i++)
            {
                bool canGrab = true;
                for (int j = 0; j < sender.Curves.Count; j++)
                {
                    if (i != j && sender.Curves[j].HasGrabbed)
                    {
                        canGrab = false;
                        break;
                    }
                }
                if (canGrab)
                {
                    sender.Curves[i].Update((Vector2f)Mouse.GetPosition(window), Mouse.IsButtonPressed(Mouse.Button.Left));
                    if (sender.IsConnected)
                    {
                        if (i == 0)
                        {
                            if (sender.Curves[0].HasGrabbedBase1)
                                sender.Curves[^1].SetBase2MoveCtrl2(sender.Curves[0].Line1.Start);
                            else if (sender.Curves[0].HasGrabbedControl1 && Keyboard.IsKeyPressed(Keyboard.Key.LControl))
                                sender.Curves[^1].SetLine2(sender.Curves[^1].Line2.End + (sender.Curves[^1].Line2.End - sender.Curves[0].Line1.End), sender.Curves[^1].Line2.End);
                        }
                        if (i == sender.Curves.Count - 1)
                        {
                            if (sender.Curves[^1].HasGrabbedBase2)
                                sender.Curves[0].SetBase1MoveCtrl1(sender.Curves[^1].Line2.End);
                            else if (sender.Curves[^1].HasGrabbedControl2 && Keyboard.IsKeyPressed(Keyboard.Key.LControl))
                                sender.Curves[0].SetLine1(sender.Curves[0].Line1.Start, sender.Curves[0].Line1.Start + (sender.Curves[0].Line1.Start - sender.Curves[^1].Line2.Start));
                        }
                    }
                    if (i < sender.Curves.Count - 1)
                    {
                        if (sender.Curves[i].HasGrabbedBase2)
                            sender.Curves[i + 1].SetBase1MoveCtrl1(sender.Curves[i].Line2.End);
                        else if (sender.Curves[i].HasGrabbedControl2 && Keyboard.IsKeyPressed(Keyboard.Key.LControl))
                            sender.Curves[i + 1].SetLine1(sender.Curves[i + 1].Line1.Start, sender.Curves[i + 1].Line1.Start + (sender.Curves[i + 1].Line1.Start - sender.Curves[i].Line2.Start));
                    }
                    if (i > 0)
                    {
                        if (sender.Curves[i].HasGrabbedBase1)
                            sender.Curves[i - 1].SetBase2MoveCtrl2(sender.Curves[i].Line1.Start);
                        else if (sender.Curves[i].HasGrabbedControl1 && Keyboard.IsKeyPressed(Keyboard.Key.LControl))
                            sender.Curves[i - 1].SetLine2(sender.Curves[i - 1].Line2.End + (sender.Curves[i - 1].Line2.End - sender.Curves[i].Line1.End), sender.Curves[i - 1].Line2.End);
                    }
                }
            }

        }
        private void Spline_OnGrabbedCtrl2(CubicBezierAdvanced sender, Vector2f position)
        {
            sender.SetLine2(position, sender.Line2.End);
        }
        private void Spline_OnGrabbedCtrl1(CubicBezierAdvanced sender, Vector2f position)
        {
            sender.SetLine1(sender.Line1.Start, position);
        }
        private void Spline_OnGrabbedBase2(CubicBezierAdvanced sender, Vector2f position)
        {
            sender.SetBase2MoveCtrl2(position);
        }
        private void Spline_OnGrabbedBase1(CubicBezierAdvanced sender, Vector2f position)
        {
            sender.SetBase1MoveCtrl1(position);
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            foreach (var s in splines)
                target.Draw(s, states);
        }
    }
}
