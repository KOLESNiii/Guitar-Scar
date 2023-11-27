using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DelaunatorSharp;
using System.Linq;
using System;
using UnityEditor;

public class Triangulator : MonoBehaviour
{
    private IPoint[] Points;
    private Delaunator delaunator;

    public void Initialise(IEnumerable<Vector2Int> points)
    {
        Points = new IPoint[points.Count()];
        for (int i = 0; i < points.Count(); i++)
        {
            Points[i] = new DelaunatorSharp.Point(points.ElementAt(i).x, points.ElementAt(i).y);
        }
        delaunator = new Delaunator(Points);
    }
    public delegate void AppendToGraphCallback(IPoint point1, IPoint point2);
    public void Triangulate(AppendToGraphCallback appendToGraphCallback)
    {
        HashSet<Edge> edges = new HashSet<Edge>();
        for (int t = 0; t < delaunator.Triangles.Count()/3; t++)
        {
            var points = pointsOfTriangle(t);
            for (int p = 0; p < 3; p++)
            {
                var point1 = points.ElementAt(p % 3);
                var point2 = points.ElementAt((p + 1) % 3);
                var edge = new Edge(new Point((int)point1.X, (int)point1.Y), new Point((int)point2.X, (int)point2.Y));
                if (!edges.Contains(edge))
                {
                    edges.Add(edge);
                    //Debug.DrawLine(new Vector3((float)point1.X, (float)point1.Y, 0), new Vector3((float)point2.X, (float)point2.Y, 0), Color.red, 1000);
                    appendToGraphCallback(point1, point2);
                }
            }
        }
    }

    public int[] edgesOfTriangle(int t)
    {
        return new int[] { 3 * t, 3 * t + 1, 3 * t + 2 };
    }

    public IEnumerable<IPoint> pointsOfTriangle(int t)
    {
        return edgesOfTriangle(t).Select(e => Points[delaunator.Triangles[e]]);
    }
}
public class Graph
{
    HashSet<Point> Vertices = new HashSet<Point>();
    HashSet<Edge> Edges = new HashSet<Edge>();
    public HashSet<Edge> SpanningTreeEdges = new HashSet<Edge>();
    HashSet<Edge> ReintroducedEdges = new HashSet<Edge>();

    public void AppendToGraph(IPoint point1, IPoint point2)
    {
        var vertex1 = new Point((int)point1.X, (int)point1.Y);
        var vertex2 = new Point((int)point2.X, (int)point2.Y);
        if (GetPointInGraph(vertex1) != null)
        {
            vertex1 = GetPointInGraph(vertex1);
        }
        else
        {
            Vertices.Add(vertex1);
        }
        if (GetPointInGraph(vertex2) != null)
        {
            vertex2 = GetPointInGraph(vertex2);
        }
        else
        {
            Vertices.Add(vertex2);
        }
        var edge = new Edge(vertex1, vertex2);
        Edges.Add(edge);
        vertex1.AddEdge(edge);
        vertex2.AddEdge(edge);
    }
    private Point GetPointInGraph(Point target)
    {
        foreach (Point point in Vertices)
        {
            if (point.x == target.x && point.y == target.y)
            {
                return point;
            }
        }
        return null;
    }
    public void PerformPrims()
    {
        var SpanningTreeVertices = new HashSet<Point>();
        double minimumLengthFromCentre = 99999;
        Point minimumLengthPoint = null;
        foreach (Point point in Vertices)
        {
            double newLength = new Vector2(point.x, point.y).magnitude;
            if (newLength < minimumLengthFromCentre)
            {
                minimumLengthFromCentre = newLength;
                minimumLengthPoint = point;
            }
        }
        SpanningTreeVertices.Add(minimumLengthPoint);
        HashSet<Edge> ValidEdges = getValidEdges(SpanningTreeVertices);
        while (SpanningTreeVertices.Count < Vertices.Count)
        {
            Edge shortestEdge = getMinimumLengthEdge(ValidEdges);
            SpanningTreeEdges.Add(shortestEdge);
            var otherPoint = shortestEdge.getOtherPoint(SpanningTreeVertices);
            SpanningTreeVertices.Add(otherPoint);
            ValidEdges.Clear();
            foreach (Edge edge in getValidEdges(SpanningTreeVertices))
            {
                if (SpanningTreeVertices.Contains(edge.Points[0]) ^ SpanningTreeVertices.Contains(edge.Points[1]))
                {
                    ValidEdges.Add(edge);
                }
            }
        }
    }
    public void PrintPrims()
    {
        foreach (Edge edge in SpanningTreeEdges)
        {
            Debug.DrawLine((Vector3)edge.Points[0], (Vector3)edge.Points[1], Color.yellow, 1000);
        }
    }
    private HashSet<Edge> getValidEdges(HashSet<Point> spanningTreeVertices)
    {
        HashSet<Edge> validEdges = new HashSet<Edge>();
        foreach (Point vertex in spanningTreeVertices)
        {
            foreach (Edge edge in vertex.Edges)
            {
                if (spanningTreeVertices.Contains(edge.Points[0]) ^ spanningTreeVertices.Contains(edge.Points[1]))
                {
                    validEdges.Add(edge);
                }
            }
        }
        return validEdges;
    }
    private Edge getMinimumLengthEdge(HashSet<Edge> edges)
    {
        double minimumLength = 99999;
        Edge minimumLengthEdge = null;
        foreach (Edge edge in edges)
        {
            double newLength = edge.Length;
            if (newLength < minimumLength)
            {
                minimumLength = newLength;
                minimumLengthEdge = edge;
            }
        }
        if (minimumLengthEdge == null)
        {
            throw new Exception("No minimum length edge found");
        }
        return minimumLengthEdge;
    }
    private IEnumerable<Edge> GetRemovedEdges()
    {
        return Edges.Except(SpanningTreeEdges);
    }
    public void ReintroduceSomeRemovedEdges()
    {
        var ValidEdgesToReintroduce = GetGoodEdges(GetRemovedEdges()).ToHashSet();
        int maxReintroductions = GetRemovedEdges().Count() / 3;
        int reintroductions = 0;
        while (ValidEdgesToReintroduce.Count() > 0 && reintroductions < maxReintroductions)
        {
            Edge edgeToReintroduce = ValidEdgesToReintroduce.ElementAt(UnityEngine.Random.Range(0, ValidEdgesToReintroduce.Count()));
            ReintroducedEdges.Add(edgeToReintroduce);
            SpanningTreeEdges.Add(edgeToReintroduce);
            ValidEdgesToReintroduce = GetGoodEdges(GetRemovedEdges()).ToHashSet();
            reintroductions++;
        }
    }
    public void PrintReintroducedEdges()
    {
        foreach (Edge edge in ReintroducedEdges)
        {
            Debug.DrawLine((Vector3)edge.Points[0], (Vector3)edge.Points[1], Color.blue, 1000);
        }
    }
    private IEnumerable<Edge> GetGoodEdges(IEnumerable<Edge> RemovedEdges)
    {
        HashSet<Edge> badEdges = new HashSet<Edge>();
        foreach (Edge remedge in RemovedEdges)
        {
            foreach (Point vertex in Vertices)
            {
                foreach (Edge edge in vertex.Edges.Intersect(SpanningTreeEdges))
                {
                    if (!remedge.Points.Contains(vertex))
                    {
                        continue;
                    }
                    var d1 = edge.DirectionFromPoint(vertex);
                    var d2 = remedge.DirectionFromPoint(vertex);
                    var a1 = Mathf.Atan2(d1.y, d1.x);
                    var a2 = Mathf.Atan2(d2.y, d2.x);
                    if (Mathf.Abs(a1-a2) < Mathf.PI/6)
                    {
                        badEdges.Add(remedge);
                    }
                }
            }
        }
        return RemovedEdges.Except(badEdges);
    }
}
public class Edge
{
    public Point[] Points
    {get; private set;}
    public float Length
    {get; private set;}
    public Edge(Point point1, Point point2)
    {
        Points = new Point[]{point1, point2};
        Length = new Vector2(point1.x-point2.x, point1.y-point2.y).magnitude;
    }
    public Point getOtherPoint(IEnumerable<Point> points)
    {
        if (points.Contains(Points[0]))
        {
            return Points[1];
        }
        else if (points.Contains(Points[1]))
        {
            return Points[0];
        }
        else
        {
            throw new Exception("Point not in edge");
        }
    }
    public Vector2Int DirectionFromPoint(Vector2Int point)
    {
        if (point == Points[0].ToVector2Int())
        {
            return Points[1]-Points[0];
        }
        else if (point == Points[1].ToVector2Int())
        {
            return Points[0]-Points[1];
        }
        else
        {
            Debug.Log("Edge: " + Points[0].ToVector2Int() + " " + Points[1].ToVector2Int() + " Point: " + point);
            throw new Exception("Point not in edge");
        }
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        } 
        else
        {
            Edge edge = (Edge)obj;
            return (Points[0].Equals(edge.Points[0]) && Points[1].Equals(edge.Points[1])) || (Points[0].Equals(edge.Points[1]) && Points[1].Equals(edge.Points[0]));
        }
    }

    public override int GetHashCode()
    {
        var val1 = new Vector2Int(Points[0].x, Points[0].y).magnitude;
        var val2 = new Vector2Int(Points[1].x, Points[1].y).magnitude;
        if (val1 > val2)
        {
            return HashCode.Combine(Points[1], Points[0]);
        }
        return HashCode.Combine(Points[0], Points[1]);
    }
}
public class Point
{
    public int x
    {get; private set;}
    public int y
    {get; private set;}
    public HashSet<Edge> Edges
    {get; private set;}

    public Point(int x, int y)
    {
        Edges = new HashSet<Edge>();
        this.x = x;
        this.y = y;
    }

    public void AddEdge(Edge edge)
    {
        Edges.Add(edge);
    }

    public static Point operator -(Point point1, Point point2)
    {
        return new Point(point1.x-point2.x, point1.y-point2.y);
    }

    public static implicit operator Vector2Int(Point point)
    {
        return new Vector2Int(point.x, point.y);
    }
    
    public static explicit operator Vector3(Point point)
    {
        return new Vector3(point.x, point.y, 0);
    }

    public Vector2Int ToVector2Int()
    {
        return new Vector2Int(x, y);
    }
}