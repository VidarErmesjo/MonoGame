using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Shapes;

namespace MonoGame
{
    public static class Collisions
    {
        private static bool PointInRectangle(Point2 M, Point2 A, Point2 B, Point2 C, Point2 D)
        {
            float dotAMAB = Vector2.Dot(A - M, A - B);
            float dotABAB = Vector2.Dot(A - B, A - B);
            float dotAMAD = Vector2.Dot(A - M, A - D);
            float dotADAD = Vector2.Dot(A - D, A - D);

            if((0f <= dotAMAB && dotAMAB <= dotABAB) && (0f <= dotAMAD && dotAMAD <= dotADAD))
                return true;

            return false;
        }

        public static bool Check(Point2 point, Polygon polygon)
        {
            if(polygon.Vertices.Length == 4)
                return PointInRectangle(point, polygon.Vertices[0], polygon.Vertices[1], polygon.Vertices[2], polygon.Vertices[3]);
            
            if(polygon.Vertices.Length == 3)
                return Trigonometry.PointInTriangle.Barycentric(point, polygon.Vertices[0], polygon.Vertices[1], polygon.Vertices[2]);

            return PointInRectangle(point, polygon.Vertices[0], polygon.Vertices[1], polygon.Vertices[2], polygon.Vertices[3]);
        }

        public static bool Check(Polygon A, Polygon B)
        {
            // Triangles
            if(A.Vertices.Length == 3 && B.Vertices.Length == 3)
                foreach(var a in A.Vertices)
                    foreach(var b in B.Vertices)
                        if( Trigonometry.PointInTriangle.Barycentric(a, B.Vertices[0], B.Vertices[1], B.Vertices[2]) ||
                            Trigonometry.PointInTriangle.Barycentric(b, A.Vertices[0], A.Vertices[1], A.Vertices[2]))
                                return true;

            // Slices (rectangles)
            if(A.Vertices.Length == 4 && B.Vertices.Length == 4)
                foreach(var a in A.Vertices)
                    foreach(var b in B.Vertices)
                        if( PointInRectangle(a, B.Vertices[0], B.Vertices[1], B.Vertices[2], B.Vertices[3]) ||
                            PointInRectangle(b, A.Vertices[0], A.Vertices[1], A.Vertices[2], A.Vertices[3]))
                            {
                                //RectangleF C = A.BoundingRectangle.Intersection(B.BoundingRectangle);
                                //System.Console.WriteLine(Vector2.Distance(a,b));
                                return true;
                            }

            return false;
        }

        /// <summary>
        /// Edge / diagonal intersection. [From OLC]
        /// <param name="A"> The polygon that intersects polygon B.
        /// <param name="B"> The polygon that intersects polygon A.
        /// </summary>
        public static bool Intersects(Polygon A, Polygon B)
        {   // Optimize for slices (quads)?
            if(!A.BoundingRectangle.Intersects(B.BoundingRectangle))
                return false;

            for(int shape = 0; shape < 2; shape++)
            {
                if(shape == 1)
                {
                    Polygon C = A;
                    A = B;
                    B = C;
                }

                for(int p = 0; p < A.Vertices.Length; p++)
                {
                    Vector2 beginA = A.BoundingRectangle.Center;
                    Vector2 endA = A.Vertices[p];

                    for(int q = 0; q < B.Vertices.Length; q++)
                    {
                        Vector2 beginB = B.Vertices[q];
                        Vector2 endB = B.Vertices[(q + 1) % B.Vertices.Length];

                        float h =   (endB.X - beginB.X) * (beginA.Y - endA.Y) -
                                    (beginA.X - endA.X) * (endB.Y - beginB.Y);

                        float t1 =  (beginB.Y - endB.Y) * (beginA.X - beginB.X) +
                                    (endB.X - beginB.X) * (beginA.Y - beginB.Y);

                        float t2 =  (beginA.Y - endA.Y) * (beginA.X - beginB.X) +
                                    (endA.X - beginA.X) * (beginA.Y - beginB.Y);

                        t1 /= h;
                        t2 /= h;

                        if(t1 >= 0f && t1 < 1f && t2 >= 0f && t2 < 1f)
                            return true;
                    }
                }
            }

            return false;
        }

        public static bool Intersects(Polygon A, Polygon B, out Vector2 position)
        {
            position = Vector2.Zero;

            if(!A.BoundingRectangle.Intersects(B.BoundingRectangle))
                return false;

            for(int shape = 0; shape < 2; shape++)
            {
                if(shape == 1)
                {
                    Polygon C = A;
                    A = B;
                    B = C;
                }

                for(int p = 0; p < A.Vertices.Length; p++)
                {
                    Vector2 beginA = A.BoundingRectangle.Center;
                    Vector2 endA = A.Vertices[p];

                    Vector2 displacement = Vector2.Zero;

                    for(int q = 0; q < B.Vertices.Length; q++)
                    {
                        Vector2 beginB = B.Vertices[q];
                        Vector2 endB = B.Vertices[(q + 1) % B.Vertices.Length];

                        float h =   (endB.X - beginB.X) * (beginA.Y - endA.Y) -
                                    (beginA.X - endA.X) * (endB.Y - beginB.Y);

                        float t1 =  (beginB.Y - endB.Y) * (beginA.X - beginB.X) +
                                    (endB.X - beginB.X) * (beginA.Y - beginB.Y);

                        float t2 =  (beginA.Y - endA.Y) * (beginA.X - beginB.X) +
                                    (endA.X - beginA.X) * (beginA.Y - beginB.Y);

                        t1 /= h;
                        t2 /= h;

                        if(t1 >= 0f && t1 < 1f && t2 >= 0f && t2 < 1f)
                        {
                            displacement += new Vector2(
                                (1f - t1) * (endA.X - beginA.X) * (shape == 0 ? -1 : +1),
                                (1f - t1) * (endA.Y - beginA.Y) * (shape == 0 ? -1 : +1));

                            //return true;
                        }
                    }

                    position += new Vector2(
                        displacement.X * (shape == 0 ? -1 : 1),
                        displacement.Y * (shape == 0 ? -1 : 1));
                }
            }

            return false;
        }
    }
}