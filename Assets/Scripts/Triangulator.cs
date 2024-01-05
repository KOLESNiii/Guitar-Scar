using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DelaunatorSharp;
using System.Linq;
using System;
using UnityEditor;

//Class for handling the delaunay triangulation
public class Triangulator : MonoBehaviour
{
    private IPoint[] Points;
    private Delaunator delaunator;

    //Initializes the triangulator with the points
    public void Initialise(IEnumerable<Vector2Int> points)
    {
        Points = new IPoint[points.Count()];
        for (int i = 0; i < points.Count(); i++) //Converts points to IPoints
        {
            Points[i] = new DelaunatorSharp.Point(points.ElementAt(i).x, points.ElementAt(i).y);
        }
        delaunator = new Delaunator(Points);
    }
    public delegate void AppendToGraphCallback(IPoint point1, IPoint point2);
    //Triangulates the points and appends the edges to the graph
    public void Triangulate(AppendToGraphCallback appendToGraphCallback)
    {
        HashSet<Edge> edges = new HashSet<Edge>();
        for (int t = 0; t < delaunator.Triangles.Count()/3; t++) //For each triangle
        {
            var points = pointsOfTriangle(t);
            for (int p = 0; p < 3; p++) //For each edge
            {
                var point1 = points.ElementAt(p % 3); //Get the two points of the edge
                var point2 = points.ElementAt((p + 1) % 3);
                var edge = new Edge(new Point((int)point1.X, (int)point1.Y), new Point((int)point2.X, (int)point2.Y));
                if (!edges.Contains(edge)) //If the edge has not already been added to the graph
                {
                    edges.Add(edge);
                    //Debug.DrawLine(new Vector3((float)point1.X, (float)point1.Y, 0), new Vector3((float)point2.X, (float)point2.Y, 0), Color.red, 1000);
                    appendToGraphCallback(point1, point2); //Append the edge to the graph
                }
            }
        }
    }

    //Returns the edges of a triangle
    public int[] edgesOfTriangle(int t)
    {
        return new int[] { 3 * t, 3 * t + 1, 3 * t + 2 };
    }

    //Returns the points of a triangle
    public IEnumerable<IPoint> pointsOfTriangle(int t)
    {
        return edgesOfTriangle(t).Select(e => Points[delaunator.Triangles[e]]);
    }
}
//A graph class which stores vertices and edges
public class Graph
{
    HashSet<Point> Vertices = new HashSet<Point>();
    HashSet<Edge> Edges = new HashSet<Edge>();
    public HashSet<Edge> SpanningTreeEdges = new HashSet<Edge>(); //Edges in the spanning tree
    HashSet<Edge> ReintroducedEdges = new HashSet<Edge>(); //Edges reintroduced to the graph

    //Appends an edge, specified by 2 points, to the graph
    public void AppendToGraph(IPoint point1, IPoint point2)
    {
        var vertex1 = new Point((int)point1.X, (int)point1.Y);
        var vertex2 = new Point((int)point2.X, (int)point2.Y);
        if (GetPointInGraph(vertex1) != null) //If the vertex is already in the graph, use the vertex in the graph
        {
            vertex1 = GetPointInGraph(vertex1);
        }
        else //If the vertex is not already in the graph, add it to the graph
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
        var edge = new Edge(vertex1, vertex2); //Create the edge
        Edges.Add(edge); //Add the edge to the graph
        vertex1.AddEdge(edge); //Assign the edge to the vertices
        vertex2.AddEdge(edge);
    }
    //Returns the point in the graph which has the same coordinates as the target point, or null if there is no such point
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
    //Performs Prim's algorithm to find the minimum spanning tree
    public void PerformPrims()
    {
        var SpanningTreeVertices = new HashSet<Point>();
        double minimumLengthFromCentre = 99999; 
        Point minimumLengthPoint = null;
        foreach (Point point in Vertices) //Find the point closest to the centre of the graph
        {
            double newLength = new Vector2(point.x, point.y).magnitude;
            if (newLength < minimumLengthFromCentre)
            {
                minimumLengthFromCentre = newLength;
                minimumLengthPoint = point;
            }
        }
        SpanningTreeVertices.Add(minimumLengthPoint); //Add the point closest to the centre of the graph to the spanning tree
        HashSet<Edge> ValidEdges = getValidEdges(SpanningTreeVertices); //Get the valid edges, which are edges that connect the vertices in the spanning tree to vertices not in the spanning tree
        while (SpanningTreeVertices.Count < Vertices.Count) //While there are still vertices not in the spanning tree
        {
            Edge shortestEdge = getMinimumLengthEdge(ValidEdges); //Find the shortest valid edge
            SpanningTreeEdges.Add(shortestEdge); //Add the shortest valid edge to the spanning tree
            var otherPoint = shortestEdge.getOtherPoint(SpanningTreeVertices); //Find the vertex of the edge which is not in the spanning tree
            SpanningTreeVertices.Add(otherPoint); //Add the other vertex to the spanning tree
            ValidEdges.Clear(); //Clear the valid edges
            foreach (Edge edge in getValidEdges(SpanningTreeVertices)) //Get the new valid edges
            {
                if (SpanningTreeVertices.Contains(edge.Points[0]) ^ SpanningTreeVertices.Contains(edge.Points[1])) //If the edge connects a vertex in the spanning tree to a vertex not in the spanning tree
                {
                    ValidEdges.Add(edge);
                }
            }
        }
    }
    //Draws the edges in the spanning tree for debugging purposes
    public void PrintPrims()
    {
        foreach (Edge edge in SpanningTreeEdges)
        {
            Debug.DrawLine((Vector3)edge.Points[0], (Vector3)edge.Points[1], Color.yellow, 1000);
        }
    }
    //Returns the valid edges, which are edges that connect the vertices in the spanning tree to vertices not in the spanning tree
    private HashSet<Edge> getValidEdges(HashSet<Point> spanningTreeVertices)
    {
        HashSet<Edge> validEdges = new HashSet<Edge>();
        foreach (Point vertex in spanningTreeVertices) //For each vertex in the spanning tree
        {
            foreach (Edge edge in vertex.Edges) //For each edge of the vertex
            {
                if (spanningTreeVertices.Contains(edge.Points[0]) ^ spanningTreeVertices.Contains(edge.Points[1])) //If the edge connects a vertex in the spanning tree to a vertex not in the spanning tree
                {
                    validEdges.Add(edge);
                }
            }
        }
        return validEdges;
    }
    //Returns the shortest edge in the set of edges
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
    //Gets all edges that are not in the spanning tree
    private IEnumerable<Edge> GetRemovedEdges()
    {
        return Edges.Except(SpanningTreeEdges);
    }
    //Adds some edges back to the graph
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
    //Draws the edges reintroduced to the graph for debugging purposes
    public void PrintReintroducedEdges()
    {
        foreach (Edge edge in ReintroducedEdges)
        {
            Debug.DrawLine((Vector3)edge.Points[0], (Vector3)edge.Points[1], Color.blue, 1000);
        }
    }
    //Returns the edges that are not in the spanning tree and are not at severely acute angles to edges in the spanning tree
    private IEnumerable<Edge> GetGoodEdges(IEnumerable<Edge> RemovedEdges)
    {
        HashSet<Edge> badEdges = new HashSet<Edge>();
        foreach (Edge remedge in RemovedEdges) //For each edge not in the spanning tree
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
                    if (Mathf.Abs(a1-a2) < Mathf.PI/6) //If the angle between the edge and an edge in the spanning tree is less than 30 degrees
                    {
                        badEdges.Add(remedge);  //Add the edge to the bad edges
                    }
                }
            }
        }
        return RemovedEdges.Except(badEdges); //Return the edges that are removed but not in the bad edges
    }
}
//A class for storing edges
public class Edge
{
    //The two points of the edge
    public Point[] Points
    {get; private set;}
    public float Length
    {get; private set;}
    public Edge(Point point1, Point point2)
    {
        Points = new Point[]{point1, point2};
        Length = new Vector2(point1.x-point2.x, point1.y-point2.y).magnitude;
    }
    //Returns the point of the edge that is not the one given as a parameter
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
    //Returns the direction of the edge from the point given as a parameter
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
    //Changes the way edges are equated
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
    //Changes the way edges are hashed, so hashing is commutative
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
//A class for storing points
public class Point
{
    public int x
    {get; private set;}
    public int y
    {get; private set;}
    //The edges that the point is a part of
    public HashSet<Edge> Edges
    {get; private set;}

    public Point(int x, int y)
    {
        Edges = new HashSet<Edge>();
        this.x = x;
        this.y = y;
    }

    //Adds an edge to the point
    public void AddEdge(Edge edge)
    {
        Edges.Add(edge);
    }

    //Changes the way points are dealt with mathematically
    public static Point operator -(Point point1, Point point2)
    {
        return new Point(point1.x-point2.x, point1.y-point2.y);
    }
    //Adds conversions
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