using UnityEngine;

namespace Jerry
{
    public class JerryMath
    {
        /// <summary>
        /// <para>计算点是否在一个区域内部</para>
        /// <para>支持凸/凹多边形</para>
        /// </summary>
        /// <param name="point">至少3个点组成的区域.</param>
        /// <param name="p">目标点.</param>
        public static bool Contains(Vector3[] point, Vector3 p)
        {
            bool result = false;
            int i, j;
            for (i = 0, j = point.Length - 1; i < point.Length; j = i++)
            {
                if ((point[i].z > p.z) != (point[j].z > p.z) &&
                    (p.x < (point[j].x - point[i].x) * (p.z - point[i].z) / (point[j].z - point[i].z) + point[i].x))
                {
                    result = !result;
                }
            }
            return result;
        }

        /// <summary>
        /// 直线和直线相交
        /// </summary>
        /// <param name="intersection"></param>
        /// <param name="linePoint1"></param>
        /// <param name="lineVec1"></param>
        /// <param name="linePoint2"></param>
        /// <param name="lineVec2"></param>
        /// <returns></returns>
        public static bool LineLineIntersection(out Vector3 intersection, Vector3 linePoint1, Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2)
        {
            Vector3 lineVec3 = linePoint2 - linePoint1;
            Vector3 crossVec1and2 = Vector3.Cross(lineVec1, lineVec2);
            Vector3 crossVec3and2 = Vector3.Cross(lineVec3, lineVec2);

            float planarFactor = Vector3.Dot(lineVec3, crossVec1and2);

            if (Mathf.Abs(planarFactor) < 0.0001f && crossVec1and2.sqrMagnitude > 0.0001f)
            {
                float s = Vector3.Dot(crossVec3and2, crossVec1and2) / crossVec1and2.sqrMagnitude;
                intersection = linePoint1 + (lineVec1 * s);
                return true;
            }
            else
            {
                intersection = Vector3.zero;
                return false;
            }
        }

        /// <summary>
        /// 线段与平面相交
        /// </summary>
        /// <param name="intersection"></param>
        /// <param name="linePoint1"></param>
        /// <param name="linePoint2"></param>
        /// <param name="planeNormal"></param>
        /// <param name="planePoint"></param>
        /// <returns></returns>
        public static bool LineSegmentPlaneIntersection(out Vector3 intersection, Vector3 linePoint1, Vector3 linePoint2, Vector3 planeNormal, Vector3 planePoint)
        {
            intersection = Vector3.zero;

            if (LinePlaneIntersection(out intersection, linePoint1, linePoint2, planeNormal, planePoint))
            {
                if (PointOnWhichSideOfLineSegment(linePoint1, linePoint2, intersection) == 0)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 直线与平面相交
        /// </summary>
        /// <param name="intersection"></param>
        /// <param name="linePoint1"></param>
        /// <param name="linePoint2"></param>
        /// <param name="planeNormal"></param>
        /// <param name="planePoint"></param>
        /// <returns></returns>
        public static bool LinePlaneIntersection(out Vector3 intersection, Vector3 linePoint1, Vector3 linePoint2, Vector3 planeNormal, Vector3 planePoint)
        {
            intersection = Vector3.zero;

            Plane pp = new Plane(planeNormal, planePoint);

            float dis1 = pp.GetDistanceToPoint(linePoint1);
            float dis2 = pp.GetDistanceToPoint(linePoint2);

            Vector3 line2Point1 = linePoint1 - pp.normal.normalized * dis1;
            Vector3 line2Point2 = linePoint2 - pp.normal.normalized * dis2;

            if (LineLineIntersection(out intersection, linePoint1, linePoint2 - linePoint1, line2Point1, line2Point2 - line2Point1))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// <para>点在线段的位置</para>
        /// <para>Returns 0 if point is on the line segment.</para>
        /// <para>Returns 1 if point is outside of the line segment and located on the side of linePoint1.</para>
        /// <para>Returns 2 if point is outside of the line segment and located on the side of linePoint2.</para>
        /// </summary>
        /// <param name="linePoint1"></param>
        /// <param name="linePoint2"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static int PointOnWhichSideOfLineSegment(Vector3 linePoint1, Vector3 linePoint2, Vector3 point)
        {
            Vector3 lineVec = linePoint2 - linePoint1;
            Vector3 pointVec = point - linePoint1;

            float dot = Vector3.Dot(pointVec, lineVec);

            //point is on side of linePoint2, compared to linePoint1
            if (dot > 0)
            {
                //point is on the line segment
                if (pointVec.magnitude <= lineVec.magnitude)
                {
                    return 0;
                }
                //point is not on the line segment and it is on the side of linePoint2
                else
                {
                    return 2;
                }
            }
            //Point is not on side of linePoint2, compared to linePoint1.
            //Point is not on the line segment and it is on the side of linePoint1.
            else
            {
                return 1;
            }
        }
    }
}