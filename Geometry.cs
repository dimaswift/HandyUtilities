using UnityEngine;
using System.Collections.Generic;

namespace HandyUtilities
{
    public static class Geometry
    {
        public static float DistanceToLine(Ray ray, Vector3 point)
        {
            return Vector3.Cross(ray.direction, point - ray.origin).magnitude;
        }

        public static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
        {
            Vector3 dir = point - pivot;
            dir = Quaternion.Euler(angles) * dir;
            point = dir + pivot;
            return point;
        }

        public static Vector2 RandomLinearDirection()
        {
            switch (Random.Range(0, 4))
            {

                case 0:
                    return Vector2.up;
                case 1:
                    return Vector2.down;
                case 2:
                    return Vector2.left;
                case 3:
                    return Vector2.right;
            }
            return Vector2.up;
        }

        public static Vector2 Normal(Vector2 a, Vector2 b, bool invert = false)
        {
            var dx = b.x - a.x;
            var dy = b.y - a.y;
            return invert ? new Vector2(-dy, dx) : new Vector2(dy, -dx);
        }

        public static Mesh CreateQuadMesh()
        {
            var quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
            var mesh = quad.GetComponent<MeshFilter>().mesh;
            Object.Destroy(quad);
            return mesh;
        }

        public static void VerifyPolygon(ref List<Vector2> points)
        {
            for (int i = 0; i < points.Count; i++)
            {
                var n = points.NextIndex(i);
                var c = points[i];
                if (Vector2.Distance(points[n], c) < 0.05f)
                {
                    points[n] = Vector2.MoveTowards(points[n], points.NextItem(n), 0.05f);
                }
            }
        }

        public static void FixTooClosePolygonPoints(ref List<Vector3> points)
        {
            for (int i = 0; i < points.Count; i++)
            {
                var n = points.NextIndex(i);
                var c = points[i];
                if (Vector2.SqrMagnitude(points[n] - c) < 0.05f)
                {
                    points[n] = Vector2.MoveTowards(points[n], points.NextItem(n), 0.2f);
                }
            }
        }

        public static void FixTooClosePolygonPoints(ref Vector2[] points)
        {
            for (int i = 0; i < points.Length; i++)
            {
                var n = points.NextIndex(i);
                var c = points[i];
                if (Vector2.SqrMagnitude(points[n] - c) < 0.2f)
                {
                    points[n] = Vector2.MoveTowards(points[n], points.NextItem(n), 0.2f);
                }
            }
        }

        public static void FixTooClosePolygonPoints(ref Vector3[] points)
        {
            for (int i = 0; i < points.Length; i++)
            {
                var n = points.NextIndex(i);
                var c = points[i];
                if (Vector2.SqrMagnitude(points[n] - c) < 0.2f)
                {
                    points[n] = Vector2.MoveTowards(points[n], points.NextItem(n), 0.2f);
                }
            }
        }

        public static bool RoughlyEqual(float a, float b)
        {
            float treshold = 10f;
            return (Mathf.Abs(a - b) < treshold);
        }

        public static Vector2[] DrawCircle(Vector3 center, float radius, int sides, float offset = 0f, bool clockWise = true)
        {
            Vector2[] circle = new Vector2[sides];
            var step = 360f / sides;
            var dir = clockWise ? Vector3.right : Vector3.left;
            for (var i = 0; i < sides; i++)
                circle[i] = center + (Quaternion.AngleAxis((step * i) - offset + (step / 2), Vector3.forward) * (dir * radius));
            return circle;
        }

        public static Vector2[] DrawLocalCircle(float radius, int sides, float offset = 0)
        {
            Vector2[] circle = new Vector2[sides];
            for (var i = 0; i < sides; i++)
                circle[i] = Quaternion.AngleAxis(((360f / sides) * i + offset), Vector3.forward) * (Vector3.right * radius);
            return circle;
        }

        public static Vector3[] ConvertPolygon(Vector3[] points, Transform target, Space toSpace)
        {
            for (int i = 0; i < points.Length; i++)
            {
                points[i] = toSpace == Space.Self ? target.InverseTransformPoint(points[i]) : target.TransformPoint(points[i]);
            }
            return points;
        }

        public static bool PointLiesWithin(Vector2 a, Vector2 b, Vector2 c)
        {
            float d = Mathf.Abs((c.x - a.x) * (b.y - a.y) - (c.y - a.y) * (b.x - a.x));
            if (d < 0.1f)
                return true;
            else return false;
        }

        public static Vector3 RandomVector3(float min, float max)
        {
            var c = new Vector3();
            c.x = Random.Range(min, max);
            c.y = Random.Range(min, max);
            c.z = Random.Range(min, max);
            if (Random.value < .5f)
                c.x *= -1;
            if (Random.value < .5f)
                c.y *= -1;
            if (Random.value < .5f)
                c.z *= -1;
            return c;
        }

        public static Vector2 RandomVector2(float min, float max)
        {
            var c = new Vector2();
            c.x = Random.Range(min, max);
            c.y = Random.Range(min, max);
            if (Random.value < .5f)
                c.x *= -1;
            if (Random.value < .5f)
                c.y *= -1;
            return c;
        }

        public static Vector3 RandomDirection(float lenght)
        {
            var a = Quaternion.AngleAxis(Random.Range(0, 360), Vector3.up);
            return a * Vector3.forward * lenght;
        }

        public static float SqrMagnitude2(Vector2 v)
        {
            return v.sqrMagnitude;
        }

        public static float SqrMagnitude2(Vector3 v)
        {
            v.z = 0;
            return v.sqrMagnitude;
        }

        public static float SqrMagnitude2(Vector2 v1, Vector2 v2)
        {
            return (v1 - v2).sqrMagnitude;
        }

        public static float SqrMagnitude2(Vector3 v1, Vector3 v2)
        {
            v1.z = 0;
            v2.z = 0;
            return (v1 - v2).sqrMagnitude;
        }

        public static float SqrMagnitude2(Vector2 v1, Vector3 v2)
        {
            v2.z = 0;
            return (new Vector3(v1.x, v1.y) - v2).sqrMagnitude;
        }

        public static float SqrMagnitude2(Vector3 v1, Vector2 v2)
        {
            v1.z = 0;
            return (new Vector3(v2.x, v2.y) - v1).sqrMagnitude;
        }

        public static Vector2 GetClosestPointOnLineSegment(Vector2 A, Vector2 B, Vector2 P)
        {
            Vector2 AP = P - A;
            Vector2 AB = B - A;

            float magnitudeAB = AB.SqrMagnitude();
            float ABAPproduct = Vector2.Dot(AP, AB);
            float distance = ABAPproduct / magnitudeAB;

            if (distance < 0)
            {
                return A;

            }
            else if (distance > 1)
            {
                return B;
            }
            else
            {
                return A + AB * distance;
            }
        }

        public static float ClampAngle(float angle, float min, float max)
        {
            if (angle < 90 || angle > 270)
            {
                if (angle > 180) angle -= 360;
                if (max > 180) max -= 360;
                if (min > 180) min -= 360;
            }
            angle = Mathf.Clamp(angle, min, max);
            if (angle < 0) angle += 360;
            return angle;
        }

        public static float ThreeSideAngle(float a, float b, float c)
        {
            return Mathf.Acos((b * b + c * c - a * a) / (2 * b * c)) * 180 / Mathf.PI;
        }

        public static int SearchRightPoint(Vector2[] p)
        {
            Vector2 right = p[0];
            int num = 0;
            for (int i = 1; i < p.Length; i++)
                if (p[i].x > right.x)
                {
                    right = p[i];
                    num = i;
                }
            return num;
        }

        public static int SearchRightPoint(List<Vector2> p)
        {
            Vector2 right = p[0];
            int num = 0;
            for (int i = 1; i < p.Count; i++)
                if (p[i].x > right.x)
                {
                    right = p[i];
                    num = i;
                }
            return num;
        }

        public static int SearchRightPoint(List<Vector3> p)
        {
            Vector2 right = p[0];
            int num = 0;
            for (int i = 1; i < p.Count; i++)
                if (p[i].x > right.x)
                {
                    right = p[i];
                    num = i;
                }
            return num;
        }

        public static int SearchRightPoint(Vector3[] p)
        {
            Vector2 right = p[0];
            int num = 0;
            for (int i = 1; i < p.Length; i++)
                if (p[i].x > right.x)
                {
                    right = p[i];
                    num = i;
                }
            return num;
        }

        public static Vector2 FindSegmentIntersection(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
        {
            float s1_x, s1_y, s2_x, s2_y;
            s1_x = p1.x - p0.x; s1_y = p1.y - p0.y;
            s2_x = p3.x - p2.x; s2_y = p3.y - p2.y;

            float s, t;
            s = (-s1_y * (p0.x - p2.x) + s1_x * (p0.y - p2.y)) / (-s2_x * s1_y + s1_x * s2_y);
            t = (s2_x * (p0.y - p2.y) - s2_y * (p0.x - p2.x)) / (-s2_x * s1_y + s1_x * s2_y);

            if (s >= 0 && s <= 1 && t >= 0 && t <= 1)
            {
                return new Vector2(p0.x + (t * s1_x), p0.y + (t * s1_y));
            }

            else return Vector2.zero;
        }

        public static Vector3 FindSegmentIntersection(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            float s1_x, s1_y, s2_x, s2_y;
            s1_x = p1.x - p0.x; s1_y = p1.y - p0.y;
            s2_x = p3.x - p2.x; s2_y = p3.y - p2.y;

            float s, t;
            s = (-s1_y * (p0.x - p2.x) + s1_x * (p0.y - p2.y)) / (-s2_x * s1_y + s1_x * s2_y);
            t = (s2_x * (p0.y - p2.y) - s2_y * (p0.x - p2.x)) / (-s2_x * s1_y + s1_x * s2_y);

            if (s >= 0 && s <= 1 && t >= 0 && t <= 1)
            {
                return new Vector3(p0.x + (t * s1_x), p0.y + (t * s1_y));
            }

            else return Vector3.zero;
        }

        static int checkIntersection(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2)
        {

            var d = (a1.x - a2.x) * (b2.y - b1.y) - (a1.y - a2.y) * (b2.x - b1.x);
            var da = (a1.x - b1.x) * (b2.y - b1.y) - (a1.y - b1.y) * (b2.x - b1.x);
            var db = (a1.x - a2.x) * (a1.y - b1.y) - (a1.y - a2.y) * (a1.x - b1.x);

            if (Mathf.Abs(d) < Mathf.Epsilon)
                return 0;
            else
            {
                var ta = da / d;
                var tb = db / d;
                if ((Mathf.Abs(ta) < Mathf.Epsilon) && ((0 <= tb) && (tb <= 1)))
                    return 2;
                else
                    if ((0 <= ta) && (0 <= tb) && (tb <= 1))
                    return 1;
                else return -1;
            }
        }

        public static float PolygonSquare(List<Vector2> polygon)
        {
            float area = 0;
            for (int i = 0; i < polygon.Count; i++)
            {
                area +=
                    (polygon.NextItem(i).x - polygon[i].x) *
                    (polygon.NextItem(i).y + polygon[i].y) / 2;
            }
            return Mathf.Abs(area);
        }

        public static float PolygonSquare(List<Vector3> polygon)
        {
            float area = 0;
            for (int i = 0; i < polygon.Count; i++)
            {
                area +=
                    (polygon.NextItem(i).x - polygon[i].x) *
                    (polygon.NextItem(i).y + polygon[i].y) / 2;
            }
            return Mathf.Abs(area);
        }

        public static float PolygonSquare(Vector3[] points)
        {
            float area = 0;
            for (int i = 0; i < points.Length; i++)
            {
                var next = points.NextIndex(i);
                area +=
                    (points[next].x - points[i].x) *
                    (points[next].y + points[i].y) / 2;
            }
            return Mathf.Abs(area);
        }

        public static float PolygonSquare(Vector2[] points)
        {
            float area = 0;
            for (int i = 0; i < points.Length; i++)
            {
                var next = points.NextIndex(i);
                area +=
                    (points[next].x - points[i].x) *
                    (points[next].y + points[i].y) / 2;
            }
            return Mathf.Abs(area);
        }

        static public Vector2 Intersection(Vector3 A, Vector3 B, Vector3 C, Vector3 D)
        {
            float xo = A.x, yo = A.y, zo = A.z;
            float p = B.x - A.x, q = B.y - A.y, r = B.z - A.z;

            float x1 = C.x, y1 = C.y, z1 = C.z;
            float p1 = D.x - C.x, q1 = D.y - C.y, r1 = D.z - C.z;

            float x = (xo * q * p1 - x1 * q1 * p - yo * p * p1 + y1 * p * p1) /
                (q * p1 - q1 * p);
            float y = (yo * p * q1 - y1 * p1 * q - xo * q * q1 + x1 * q * q1) /
                (p * q1 - p1 * q);
            float z = (zo * q * r1 - z1 * q1 * r - yo * r * r1 + y1 * r * r1) /
                (q * r1 - q1 * r);
            return new Vector3(x, y, z);
        }

        public static Vector2 LineIntersectionPoint(Vector2 ps1, Vector2 pe1, Vector2 ps2, Vector2 pe2)
        {
            float A1 = pe1.y - ps1.y;
            float B1 = ps1.x - pe1.x;
            float C1 = A1 * ps1.x + B1 * ps1.y;

            float A2 = pe2.y - ps2.y;
            float B2 = ps2.x - pe2.x;
            float C2 = A2 * ps2.x + B2 * ps2.y;

            float delta = A1 * B2 - A2 * B1;
            if (delta == 0)
                return Vector2.zero;

            return new Vector2(
                (B2 * C1 - B1 * C2) / delta,
                (A1 * C2 - A2 * C1) / delta
            );
        }

        public static float Sign(Vector3 p1, Vector3 p2, Vector3 p3)
        {
            return (p1.x - p3.x) * (p2.y - p3.y) - (p2.x - p3.x) * (p1.y - p3.y);
        }

        public static bool IsInsideTriangle(Vector3 point, Vector3 v1, Vector3 v2, Vector3 v3)
        {
            bool b1, b2, b3;
            b1 = Sign(point, v1, v2) < 0.0f;
            b2 = Sign(point, v2, v3) < 0.0f;
            b3 = Sign(point, v3, v1) < 0.0f;
            return ((b1 == b2) && (b2 == b3));
        }

        public static bool IsInside(Vector3 a, Vector3[] plg)
        {
            Vector2 ax = new Vector2(a.x + plg[SearchRightPoint(plg)].x, a.y);
            int k = 0;
            int i1;
            int i2;
            int r;
            for (int i = 0; i < plg.Length; i++)
            {
                i1 = i;
                i2 = i + 1;
                if (i == plg.Length - 1) i2 = 0;

                r = checkIntersection(a, ax, plg[i1], plg[i2]);
                if (r == 2)
                {
                    return false;
                }
                if (r == 1)
                    k = 1 - k;
            }
            if (k == 0)
                return false;
            else
                return true;
        }

        public static bool IsInside(Vector2 a, Vector2[] plg)
        {
            if (a == Vector2.zero)
            {
                a.x = 0.00001f;
                a.y = 0.00001f;
            }
            Vector2 ax = new Vector2(a.x + plg[SearchRightPoint(plg)].x, a.y);
            int k = 0;
            int i1;
            int i2;
            int r;
            for (int i = 0; i < plg.Length; i++)
            {
                i1 = i;
                i2 = i + 1;
                if (i == plg.Length - 1) i2 = 0;

                r = checkIntersection(a, ax, plg[i1], plg[i2]);
                if (r == 2)
                {
                    return false;
                }
                if (r == 1)
                    k = 1 - k;
            }
            if (k == 0)
                return false;
            else
                return true;
        }

        public static bool IsInside(Vector3 a, List<Vector3> plg)
        {
            Vector2 ax = new Vector2(a.x + plg[SearchRightPoint(plg)].x, a.y);
            int k = 0;
            int i1;
            int i2;
            int r;
            for (int i = 0; i < plg.Count; i++)
            {
                i1 = i;
                i2 = i + 1;
                if (i == plg.Count - 1) i2 = 0;

                r = checkIntersection(a, ax, plg[i1], plg[i2]);
                if (r == 2)
                {
                    return false;
                }
                if (r == 1)
                    k = 1 - k;
            }
            if (k == 0)
                return false;
            else
                return true;
        }

        public static bool IsInside(Vector2 a, List<Vector2> plg)
        {
            Vector2 ax = new Vector2(a.x + plg[SearchRightPoint(plg)].x, a.y);
            int k = 0;
            int i1;
            int i2;
            int r;
            for (int i = 0; i < plg.Count; i++)
            {
                i1 = i;
                i2 = i + 1;
                if (i == plg.Count - 1) i2 = 0;

                r = checkIntersection(a, ax, plg[i1], plg[i2]);
                if (r == 2)
                {
                    return false;
                }
                if (r == 1)
                    k = 1 - k;
            }
            if (k == 0)
                return false;
            else
                return true;
        }

        public static Vector2[] CreateClockWiseCircle(Vector2[] array, int a, int b, Vector2 pointA, Vector2 pointB)
        {
            List<Vector2> points = new List<Vector2>();
            points.Add(pointA);
            points.Add(array[a]);
            for (int i = 0; i < array.Length; i++)
            {
                a = array.PreviousIndex(a);
                if (a != b)
                {
                    points.Add(array[a]);
                }
                else break;
            }
            points.Add(pointB);
            return points.ToArray();
        }

        public static Vector2[] CreateCounterClockWiseCircle(Vector2[] array, int a, int b, Vector2 pointA, Vector2 pointB)
        {
            List<Vector2> points = new List<Vector2>(array.Length);
            points.Add(pointA);
            for (int i = 0; i < array.Length; i++)
            {
                a = array.NextIndex(a);
                if (a != array.NextIndex(b))
                {
                    points.Add(array[a]);
                }
                else break;
            }
            points.Add(pointB);
            return points.ToArray();
        }

        public static List<Vector3> CreateNoiseCircle(float radius, Vector3 center, int sides, float depthNoise, float sideNoise, float noiseFactor)
        {
            var circle = new List<Vector3>(sides);
            var depthMax = radius * depthNoise;
            var sideMax = (360f / sides) * sideNoise;
            var dn = UnityEngine.Random.Range(-depthMax, depthMax);
            var sn = UnityEngine.Random.Range(0, sideMax);
            var depthStep = dn;
            int safe = 0;
            //  noiseFactor = (1f - noiseFactor) * depthNoise;
            for (int i = 0; i < sides; i++)
            {
                depthStep = UnityEngine.Random.Range(-depthMax, depthMax);
                while (safe < 10 && Mathf.Abs(depthStep - dn) > noiseFactor)
                {
                    depthStep = UnityEngine.Random.Range(-depthMax, depthMax);
                    safe++;
                }
                safe = 0;
                dn = depthStep;
                sn = UnityEngine.Random.Range(0, sideMax);
                var cp = center + Quaternion.AngleAxis((((360f / sides) * i) + sn), Vector3.forward) * (Vector2.left * (radius + dn));
                circle.Add(cp);
            }
            return circle;
        }

        public static List<Vector3> CreateNoiseLine(float lengh, Vector3 origin, Vector3 direction, int segments, float depthNoise, float segmentNoise)
        {
            var line = new List<Vector3>(segments);
            line.Add(origin);

            for (int i = 0; i < segments; i++)
            {
                direction.x += UnityEngine.Random.Range(-depthNoise, depthNoise);
                direction.y += UnityEngine.Random.Range(-depthNoise, depthNoise);
                origin += direction * lengh;
                line.Add(origin);
            }
            return line;
        }

        public static float FindDistanceToSegment(Vector2 pt, Vector2 p1, Vector2 p2, out Vector2 closest)
        {
            float dx = p2.x - p1.x;
            float dy = p2.y - p1.y;
            if ((dx == 0) && (dy == 0))
            {
                closest = p1;
                dx = pt.x - p1.x;
                dy = pt.y - p1.y;
                return Mathf.Sqrt(dx * dx + dy * dy);
            }

            float t = ((pt.x - p1.x) * dx + (pt.y - p1.y) * dy) /
                (dx * dx + dy * dy);

            if (t < 0)
            {
                closest = new Vector2(p1.x, p1.y);
                dx = pt.x - p1.x;
                dy = pt.y - p1.y;
            }
            else if (t > 1)
            {
                closest = new Vector2(p2.x, p2.y);
                dx = pt.x - p2.x;
                dy = pt.y - p2.y;
            }
            else
            {
                closest = new Vector2(p1.x + t * dx, p1.y + t * dy);
                dx = pt.x - closest.x;
                dy = pt.y - closest.y;
            }

            return Mathf.Sqrt(dx * dx + dy * dy);
        }

        public static float FindDistanceToSegment(Vector2 pt, Vector2 p1, Vector2 p2)
        {
            float dx = p2.x - p1.x;
            float dy = p2.y - p1.y;
            if ((dx == 0) && (dy == 0))
            {
                dx = pt.x - p1.x;
                dy = pt.y - p1.y;
                return Mathf.Sqrt(dx * dx + dy * dy);
            }

            float t = ((pt.x - p1.x) * dx + (pt.y - p1.y) * dy) /
                (dx * dx + dy * dy);

            if (t < 0)
            {
                dx = pt.x - p1.x;
                dy = pt.y - p1.y;
            }
            else if (t > 1)
            {
                dx = pt.x - p2.x;
                dy = pt.y - p2.y;
            }
            else
            {
                var closest = new Vector2(p1.x + t * dx, p1.y + t * dy);
                dx = pt.x - closest.x;
                dy = pt.y - closest.y;
            }

            return Mathf.Sqrt(dx * dx + dy * dy);
        }

        public static Vector2[] FlipPolygon(Vector2[] polygon, Vector2 normal)
        {
            for (int i = 0; i < polygon.Length; i++)
            {
                polygon[i] = Vector2.Reflect(polygon[i], normal);
            }
            return polygon;
        }

        public static Vector2 GetMiddle(Vector2 a, Vector2 b)
        {
            a.x -= (a.x - b.x) / 2f;
            a.y -= (a.y - b.y) / 2f;
            return a;
        }

        public static Vector3 MiddlePoint(Vector3 a, Vector3 b)
        {
            a.x -= (a.x - b.x) / 2f;
            a.y -= (a.y - b.y) / 2f;
            a.z -= (a.z - b.z) / 2f;
            return a;
        }

        public static Vector3 GetCentroid(Vector2[] polygon)
        {
            float x = 0, y = 0, area = 0, k;
            var count = polygon.Length;
            Vector3 a, b = polygon[count - 1];

            for (int i = 0; i < count; i++)
            {
                a = polygon[i];

                k = a.y * b.x - a.x * b.y;
                area += k;
                x += (a.x + b.x) * k;
                y += (a.y + b.y) * k;

                b = a;
            }
            area *= 3;

            return (area == 0) ? Vector3.zero : new Vector3(x /= area, y /= area, 0);
        }

        public static Vector3 GetCentroid(Vector3[] polygon)
        {
            float x = 0, y = 0, area = 0, k;
            var count = polygon.Length;
            Vector3 a, b = polygon[count - 1];

            for (int i = 0; i < count; i++)
            {
                a = polygon[i];

                k = a.y * b.x - a.x * b.y;
                area += k;
                x += (a.x + b.x) * k;
                y += (a.y + b.y) * k;

                b = a;
            }
            area *= 3;

            return (area == 0) ? Vector3.zero : new Vector3(x /= area, y /= area, 0);
        }

        public static Vector3 GetCentroid(List<Vector2> polygon)
        {
            float x = 0, y = 0, area = 0, k;
            var count = polygon.Count;
            Vector3 a, b = polygon[count - 1];

            for (int i = 0; i < count; i++)
            {
                a = polygon[i];

                k = a.y * b.x - a.x * b.y;
                area += k;
                x += (a.x + b.x) * k;
                y += (a.y + b.y) * k;

                b = a;
            }
            area *= 3;

            return (area == 0) ? Vector3.zero : new Vector3(x /= area, y /= area, 0);
        }

        public static Vector3 GetCentroid(List<Vector3> polygon)
        {
            float x = 0, y = 0, area = 0, k;
            var count = polygon.Count;
            Vector3 a, b = polygon[count - 1];

            for (int i = 0; i < count; i++)
            {
                a = polygon[i];

                k = a.y * b.x - a.x * b.y;
                area += k;
                x += (a.x + b.x) * k;
                y += (a.y + b.y) * k;

                b = a;
            }
            area *= 3;

            return (area == 0) ? Vector3.zero : new Vector3(x /= area, y /= area, 0);
        }

        public static Vector2 Perpendicular(Vector2 original)
        {
            float x = original.x;
            float y = original.y;
            y = -y;
            return new Vector2(y, x);
        }

        public static Vector3 Perpendicular(Vector3 original)
        {
            float x = original.x;
            float y = original.y;
            y = -y;
            return new Vector3(y, x);
        }

        public static float DotProduct(Vector3 pointA, Vector3 pointB, Vector3 pointC)
        {
            float AB_0 = 0;
            float AB_1 = 0;
            float BC_0 = 0;
            float BC_1 = 0;

            AB_0 = pointB[0] - pointA[0];
            AB_1 = pointB[1] - pointA[1];
            BC_0 = pointC[0] - pointB[0];
            BC_1 = pointC[1] - pointB[1];
            float dot = AB_0 * BC_0 + AB_1 * BC_1;

            return dot;
        }

        public static float LineToPointDistance(Vector3 pointA, Vector3 pointB, Vector3 pointC, bool isSegment)
        {
            float dist = Cross(pointA, pointB, pointC) / Vector3.Distance(pointA, pointB);
            if (isSegment)
            {
                float dot1 = DotProduct(pointA, pointB, pointC);
                if (dot1 > 0)
                    return Vector3.Distance(pointB, pointC);

                float dot2 = DotProduct(pointB, pointA, pointC);
                if (dot2 > 0)
                    return Vector3.Distance(pointA, pointC);
            }
            return Mathf.Abs(dist);
        }

        public static float Hypotenuse(float a, float b)
        {
            return Mathf.Sqrt(Mathf.Pow(a, 2) + Mathf.Pow(b, 2));
        }

        public static bool IsConvex(Vector2[] polygon)
        {
            bool got_negative = false;
            bool got_positive = false;
            int num_points = polygon.Length;
            int B, C;
            for (int A = 0; A < num_points; A++)
            {
                B = (A + 1) % num_points;
                C = (B + 1) % num_points;

                float cross_product =
                    CrossProductLength(
                        polygon[A].x, polygon[A].y,
                        polygon[B].x, polygon[B].y,
                        polygon[C].x, polygon[C].y);
                if (cross_product < 0)
                {
                    got_negative = true;
                }
                else if (cross_product > 0)
                {
                    got_positive = true;
                }
                if (got_negative && got_positive) return false;
            }

            return true;
        }

        public static float CrossProductLength(float Ax, float Ay, float Bx, float By, float Cx, float Cy)
        {
            float BAx = Ax - Bx;
            float BAy = Ay - By;
            float BCx = Cx - Bx;
            float BCy = Cy - By;

            return (BAx * BCy - BAy * BCx);
        }

        public static float Cross(Vector3 a, Vector3 b, Vector3 c)
        {
            return ((b.x - a.x) * (c.y - a.y) - (b.y - a.y) * (c.x - a.x));
        }

        public static Vector2[] Square(float size)
        {
            return new Vector2[] { new Vector2(-size, -size), new Vector2(-size, size), new Vector2(size, size), new Vector2(size, -size) };
        }

        public static Vector4 QuaternionToVector4(Quaternion q)
        {
            return new Vector4(q.x, q.y, q.z, q.w);
        }

    }
}

