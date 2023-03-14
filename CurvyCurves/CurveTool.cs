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

        private float stepSize;

        private Sprite cursor;

        static readonly Texture CURSOR_DEFAULT = new Texture(Resources.cursor_default);
        static readonly Texture CURSOR_ADD = new Texture(Resources.cursor_add);
        static readonly Texture CURSOR_KEEP = new Texture(Resources.cursor_keep);

        private static Color inactiveColor = new Color(150, 150, 150);
        private Color activeColor;

        private int activeIndex = 0;

        public CurveTool(RenderWindow window, float stepSize = 0.01f, Color color = default)
        {
            cursor = new Sprite(CURSOR_DEFAULT);
            this.splines = new List<BezierSpline>();
            this.window = window;
            this.isInterracting = false;

            this.activeColor = new Color(color);

            this.stepSize = stepSize;

            window.MouseButtonReleased += Window_MouseButtonReleased;
            window.KeyReleased += Window_KeyReleased;
            window.SetMouseCursorVisible(false);

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
        public void Update()
        {
            Texture updateCursorTexture;
            int lastActiveIndex = activeIndex;

            if (!isInterracting)
            {
                splines[activeIndex].Update();
                if (splines[activeIndex].IsInterracting)
                {
                    isInterracting = true;
                }
                else
                {
                    for (int i = 0; i < splines.Count; i++)
                    {
                        splines[i].Update();
                        if (i != activeIndex && splines[i].IsInterracting)
                        {
                            activeIndex = i;
                            isInterracting = true;
                            break;
                        }
                    }
                }
            }
            else
            {
                splines[activeIndex].Update();
                if (!splines[activeIndex].IsInterracting)
                    isInterracting = false;
            }

            if (lastActiveIndex != activeIndex)
            {
                splines.RemoveAll(x => x.Curves.Count == 0);
                for (int i = 0; i < splines.Count; i++)
                {
                    if (i == activeIndex)
                        splines[i].Color = activeColor;
                    else
                        splines[i].Color = inactiveColor;
                }
            }

            if (Keyboard.IsKeyPressed(Keyboard.Key.LControl))
                updateCursorTexture = CURSOR_KEEP;
            else if (Keyboard.IsKeyPressed(Keyboard.Key.LShift))
                updateCursorTexture = CURSOR_ADD;
            else
                updateCursorTexture = CURSOR_DEFAULT;

            if (updateCursorTexture != cursor.Texture)
            {
                cursor.Texture = updateCursorTexture;
            }
            cursor.Position = window.MapPixelToCoords(Mouse.GetPosition(window));
        }

        private void Window_KeyReleased(object? sender, KeyEventArgs e)
        {
            if (e.Code == Keyboard.Key.Delete)
            {
                if (splines.Count > 1 && splines[activeIndex].Curves.Count <= 1)
                {
                    splines.RemoveAt(splines.Count - 1);
                    splines[^1].Color = activeColor;
                    activeIndex -= activeIndex == 0 ? 0 : 1;
                    return;
                }
                splines[activeIndex].Pop();
                return;
            }
            if (e.Code == Keyboard.Key.Enter && splines[activeIndex].Curves.Count > 0)
            {
                splines[activeIndex].Color = inactiveColor;
                activeIndex = splines.Count;
                Add(stepSize, activeColor);
            }
        }
        private void Window_MouseButtonReleased(object? sender, MouseButtonEventArgs e)
        {
            if (e.Button == Mouse.Button.Left && Keyboard.IsKeyPressed(Keyboard.Key.LShift))
            {
                if (splines[activeIndex].Curves.Count > 0 && splines[activeIndex].Curves[0].IsHoveringBase1(new Vector2f(e.X, e.Y)))
                {
                    splines[activeIndex].Connect();
                    splines[activeIndex].Curves[^1].SetLine2(splines[activeIndex].Curves[^1].Line2.End + (splines[activeIndex].Curves[^1].Line2.End - splines[activeIndex].Curves[0].Line1.End), splines[activeIndex].Curves[^1].Line2.End);
                }
                else
                {
                    splines[activeIndex].Add((Vector2f)Mouse.GetPosition(window));
                }
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
                    sender.Curves[i].DispatchEvents((Vector2f)Mouse.GetPosition(window), Mouse.IsButtonPressed(Mouse.Button.Left));
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
            if (!Keyboard.IsKeyPressed(Keyboard.Key.LShift))
                sender.SetLine2(position, sender.Line2.End);
        }
        private void Spline_OnGrabbedCtrl1(CubicBezierAdvanced sender, Vector2f position)
        {
            if (!Keyboard.IsKeyPressed(Keyboard.Key.LShift))
                sender.SetLine1(sender.Line1.Start, position);
        }
        private void Spline_OnGrabbedBase2(CubicBezierAdvanced sender, Vector2f position)
        {
            if (!Keyboard.IsKeyPressed(Keyboard.Key.LShift))
                sender.SetBase2MoveCtrl2(position);
        }
        private void Spline_OnGrabbedBase1(CubicBezierAdvanced sender, Vector2f position)
        {
            if (!Keyboard.IsKeyPressed(Keyboard.Key.LShift))
                sender.SetBase1MoveCtrl1(position);
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            foreach (var s in splines)
                target.Draw(s, states);
            target.Draw(cursor);
        }
    }
}
