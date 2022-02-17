using Xamarin.Forms;
using Xamarin.Forms.Shapes;

namespace TalkiPlay
{
    public static class Geometries
    {
        private const int ClipSquareSize = 200;
        private const int HalfClipSquareSize = 100;
        
        public static RectangleGeometry OneQuarterClipGeometry = new RectangleGeometry
        {
            Rect = new Xamarin.Forms.Rectangle(HalfClipSquareSize, 0, HalfClipSquareSize, HalfClipSquareSize)
        };

        public static RectangleGeometry HalfClipGeometry = new RectangleGeometry
        {
            Rect = new Xamarin.Forms.Rectangle(HalfClipSquareSize, 0, HalfClipSquareSize, ClipSquareSize)
        };

        public static PathGeometry ThreeQuartersClipGeometry = new PathGeometry()
        {
            Figures =
            {
                new PathFigure()
                {
                    StartPoint = new Point(HalfClipSquareSize, 0),
                    IsClosed = true,
                    Segments =
                    {
                        new LineSegment(new Point(ClipSquareSize,0)),
                        new LineSegment(new Point(ClipSquareSize,ClipSquareSize)),
                        new LineSegment(new Point(0,ClipSquareSize)),
                        new LineSegment(new Point(0,HalfClipSquareSize)),
                        new LineSegment(new Point(HalfClipSquareSize,HalfClipSquareSize)),
                        new LineSegment(new Point(HalfClipSquareSize,0)),
                    }

                }
            }
        };
    }
}