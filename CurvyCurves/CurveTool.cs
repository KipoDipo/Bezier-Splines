using SFML.Graphics;


namespace CurvyCurves
{
    public class CurveTool
    {
        private BezierSpline spline;

        public CurveTool()
        {
            spline = new BezierSpline(0.01f, new Color(0, 255, 255));
        }
    }
}
