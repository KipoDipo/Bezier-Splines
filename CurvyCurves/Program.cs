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

            //Line line1 = new Line((WIDTH * 0.3f, HEIGHT * 0.6f), (WIDTH * 0.45f, HEIGHT * 0.3f), new Color(255, 255, 255, 100));
            //Line line2 = new Line((WIDTH * 0.65f, HEIGHT * 0.3f), (WIDTH * 0.8f, HEIGHT * 0.6f), new Color(255, 255, 255, 100));

            BezierSpline spline = new BezierSpline(0.01f, new Color(0, 255, 255));

            window.MouseButtonReleased += (object? sender, MouseButtonEventArgs e) =>
            {
                if (e.Button == Mouse.Button.Right)
                {
                    if (spline.Curves.Count > 0 && spline.Curves[0].IsHoveringBase1(new Vector2f(e.X, e.Y)))
                        spline.Connect();
                    else
                        spline.Add((Vector2f)Mouse.GetPosition(window));
                }
            };
            window.KeyReleased += (object? sender, KeyEventArgs e) =>
            {
                if (e.Code == Keyboard.Key.Delete)
                {
                    spline.Pop();
                }
            };

            //spline.Add(new Vector2f(300, 300));
            //spline.Add(new Vector2f(400, 700));
            //spline.Add(new Vector2f(700, 500));

            spline.OnGrabbedBase1 += Spline_OnGrabbedBase1;
            spline.OnGrabbedBase2 += Spline_OnGrabbedBase2;
            spline.OnGrabbedCtrl1 += Spline_OnGrabbedCtrl1;
            spline.OnGrabbedCtrl2 += Spline_OnGrabbedCtrl2;
            spline.OnUpdate += Spline_OnUpdate;

            while (window.IsOpen)
            {
                spline.Update();

                window.Clear();

                window.Draw(spline);

                window.DispatchEvents();
                window.Display();
            }

        }

        private static void Spline_OnUpdate(BezierSpline sender)
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
        private static void Spline_OnGrabbedCtrl2(CubicBezierAdvanced sender, Vector2f position)
        {
            sender.SetLine2(position, sender.Line2.End);
        }
        private static void Spline_OnGrabbedCtrl1(CubicBezierAdvanced sender, Vector2f position)
        {
            sender.SetLine1(sender.Line1.Start, position);
        }
        private static void Spline_OnGrabbedBase2(CubicBezierAdvanced sender, Vector2f position)
        {
            sender.SetBase2MoveCtrl2(position);
        }
        private static void Spline_OnGrabbedBase1(CubicBezierAdvanced sender, Vector2f position)
        {
            sender.SetBase1MoveCtrl1(position);
        }

        private static void Window_Closed(object? sender, EventArgs e)
        {
            window.Close();
        }
    }
}