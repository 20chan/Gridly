using Microsoft.Xna.Framework;

namespace Gridly
{
    public static class Geometry
    {
        public static bool IsTwoSegmentsInstersect(Vector2 p1, Vector2 p2, Vector2 q1, Vector2 q2)
        {
            var seg1Grad = (p2.Y - p1.Y) / (p2.X - p1.X);
            var seg2Grad = (q2.Y - q1.Y) / (q2.X - q1.X);
            var seg1Yint = p1.Y - seg1Grad * p1.X;
            var seg2Yint = q1.Y - seg2Grad * q1.X;

            if (seg1Grad == seg2Grad)
            {
                if (seg1Yint == seg2Yint)
                    return true;
                else return false;
            }

            var intersectx = (seg2Yint - seg1Yint) / (seg1Grad - seg2Grad);
            var intersect = new Vector2(intersectx, intersectx * seg1Grad + seg1Yint);
            if (Vector2.Distance(intersect, p1) < Vector2.Distance(p1, p2)
                && Vector2.Distance(intersect, p2) < Vector2.Distance(p1, p2)
                && Vector2.Distance(intersect, q1) < Vector2.Distance(q1, q2)
                && Vector2.Distance(intersect, q2) < Vector2.Distance(q1, q2))
                return true;
            return false;
        }
    }
}
