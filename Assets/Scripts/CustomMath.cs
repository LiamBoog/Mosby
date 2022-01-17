using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomMath : MonoBehaviour
{
    #region public Methods

    /// <summary>
    /// Returns center of circle formed by 2 points and radius
    /// </summary>
    /// <param name="p1">First point on circumference</param>
    /// <param name="p2">Second point on circumference</param>
    /// <param name="radius">Radius of circle</param>
    /// <returns></returns>
    public static Vector2 CenterFromPointsAndRadius(Vector2 p1, Vector2 p2, float radius)
    {
        float q = Mathf.Sqrt(Mathf.Pow(p2.x - p1.x, 2f) + Mathf.Pow(p2.y - p1.y, 2f));

        float x1 = ((p1.x + p2.x) / 2f) + Mathf.Sqrt((radius * radius) - (q * q / 4f)) * ((p1.y - p2.y) / q);
        float x2 = ((p1.x + p2.x) / 2f) - Mathf.Sqrt((radius * radius) - (q * q / 4f)) * ((p1.y - p2.y) / q);
        float y1 = ((p1.y + p2.y) / 2f) + Mathf.Sqrt((radius * radius) - (q * q / 4f)) * ((p2.x - p1.x) / q);
        float y2 = ((p1.y + p2.y) / 2f) - Mathf.Sqrt((radius * radius) - (q * q / 4f)) * ((p2.x - p1.x) / q);

        return Random.Range(0f, 1f) > 0.5f ? new Vector2(x1, y1) : new Vector2(x2, y2);//randomly return one of 2 possible center points
    }

    /// <summary>
    /// Return the highest intersection point between the circle formed by center and radius, and the line segment formed by point1 and point2
    /// </summary>
    /// <param name="center">Center of the circle</param>
    /// <param name="radius">Radius of the circle</param>
    /// <param name="point1">First edge of line segment</param>
    /// <param name="point2">Second edge of line segment</param>
    /// <returns></returns>
    public static Vector2 IntersectionBetweenCircleAndLine(Vector2 center, float radius, Vector2 point1, Vector2 point2)
    {
        if (point1.y == point2.y)
        {
            float y = point1.y;
            float x1 = center.x + Mathf.Sqrt(radius * radius - Mathf.Pow(y - center.y, 2f));
            float x2 = center.x - Mathf.Sqrt(radius * radius - Mathf.Pow(y - center.y, 2f));

            if (x1 >= point1.x && x1 <= point2.x)
            {
                return new Vector2(x1, y);
            } else if (x2 >= point1.x && x2 <= point2.x)
            {
                return new Vector2(x2, y);
            } else
            {
                return Vector2.zero;
            }
        } else if (point1.x == point2.x)
        {
            float x = point1.x;
            float y1 = center.y + Mathf.Sqrt(radius * radius - Mathf.Pow(x - center.x, 2f));
            float y2 = center.y - Mathf.Sqrt(radius * radius - Mathf.Pow(x - center.x, 2f));

            if (y1 >= point1.y && y1 <= point2.y && y2 >= point1.y && y2 <= point2.y)
            {
                return y1 > y2 ? new Vector2(x, y1) : new Vector2(x, y2);
            } else if (y1 >= point1.y && y1 <= point2.y)
            {
                return new Vector2(x, y1);
            } else if (y2 >= point1.y && y2 <= point2.y)
            {
                return new Vector2(x, y2);
            } else
            {
                return Vector2.zero;
            }
        }

        return Vector2.zero;
    }

    #endregion
}
