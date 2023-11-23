using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DelaunatorSharp;
using System.Linq;

public class Triangulation : MonoBehaviour
{
    public Triangulation(IEnumerable<Vector2> points)
    {
        IPoint[] Points = new IPoint[points.Count()];
        for (int i = 0; i < points.Count(); i++)
        {
            Points[i] = new Point(points.ElementAt(i).x, points.ElementAt(i).y);
        }
        var delaunator = new Delaunator(Points);
        for (int e = 0; e < delaunator.Triangles.Count(); e++)
        {
            if (e < delaunator.Halfedges[e])
            {
                int p1 = delaunator.Triangles[e];
                int p2 = delaunator.Triangles[nextHalfedge(e)];
                Debug.DrawLine(new Vector3((float)Points[p1].X, (float)Points[p1].Y, 0), new Vector3((float)Points[p2].X, (float)Points[p2].Y, 0), Color.red, 1000f);

            }
        }
        
    }

    private int nextHalfedge(int e) 
    {
        return (e % 3 == 2) ? e - 2 : e + 1; 
    }
}
