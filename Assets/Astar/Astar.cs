using System;
using System.Collections;
using Priority_Queue;
using System.Collections.Generic;
using UnityEngine;

public enum GridType {
    Wall,
    Forest,
    Ground,
}
public class Grid : MonoBehaviour {
    public int x;
    public int y;
    public GridType currentType = GridType.Ground;
    private void OnMouseDown()
    {
        Test.ChageType(this,this.currentType);
    }
    public Grid(int x, int y) {
        this.x = x;
        this.y = y;
    }
}

public interface WeightedGraph<T>
{
    T[] Neighbors(T current);
    int Cost(T next);
}

public class SquareGrid : WeightedGraph<Grid>
{
    private int width, height;
    private Grid[,] edges;
    private HashSet<Grid> walls = new HashSet<Grid>();
    private HashSet<Grid> forests = new HashSet<Grid>();
    private static readonly Grid[] Dirs = new[]
    {
        new Grid(1,0),
        new Grid(0,-1),
        new Grid(-1,0),
        new Grid(0,1),
    };
    public int Width
    {
        get { return width; }   
    }
    public int Height {
        get { return height; }
    }
    public SquareGrid(int width,int height)
    {
        this.width = width;
        this.height = height;
        edges = new Grid[width,height];
    }

    public void AddWall(Grid grid) {
        walls.Add(grid);
    }

    public void RemoveWall(Grid grid) {
        walls.Remove(grid);
    }
    public void AddForest(Grid grid)
    {
        forests.Add(grid);
    }
    public void RemoveForest(Grid grid)
    {
        forests.Remove(grid);
    }

    public void AddEdge(Grid grid)
    {
        if (Passable(grid)&& InBounds(grid))
        {
            edges[grid.x, grid.y] = grid;
        }
    }
    public void RemoveEdge(Grid grid) {
        if (Passable(grid) && InBounds(grid))
        {
            if (edges[grid.x, grid.y] != null)
            {
                edges[grid.x, grid.y] = null;
            }
        }
    }

    public bool Passable(Grid grid) {
        return !walls.Contains(grid);
    }

    public bool InBounds(Grid id) {
        return id.x >= 0 && id.x < width && 0 <= id.y && id.y < height;
    }
    public bool InBounds(int x,int y)
    {
        return x >= 0 && x < width && 0 <= y && y < height;
    }
    public int Cost(Grid next)
    {
        return forests.Contains(next) ? 5 : 1;
    }

    public Grid[] Neighbors(Grid current)
    {
        List<Grid> neighbors = new List<Grid>();
        for (int i = 0; i < Dirs.Length; i++)
        {
            if (InBounds(current.x + Dirs[i].x, current.y + Dirs[i].y))
            {
                Grid next = edges[current.x + Dirs[i].x, current.y + Dirs[i].y];
                if (Passable(next))
                {
                    neighbors.Add(next);
                }
            }
        }
        return neighbors.ToArray();
    }
}

public class PriorityQueue<U,T> where T: IComparable<T>
{
   
    SimplePriorityQueue<U,T> priorityQueue;
    public int Count {
        get { return priorityQueue.Count; }
    }

    public PriorityQueue()
    {
        priorityQueue = new SimplePriorityQueue<U,T>();
    }
    public U Push(U key, T value)
    {
        priorityQueue.Enqueue(key,value);
        return key;
    }
    public U Pop()
    {
        return priorityQueue.Dequeue();
    }
    public U Peek()
    {
        return priorityQueue.First;
    }
}


public class Astar {
    SquareGrid graph;
    PriorityQueue<Grid, double> frontier;  
    private Dictionary<Grid,Grid> came_from;
    private Dictionary<Grid,double> cost_so_far;
    public Astar(SquareGrid graph) {
        this.graph = graph;
    }

    public int Heuristic(Grid a, Grid b) {
        return Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y); 
    }

    //使用开始的实际距离来优先排列
    //public void Dijkstra(Grid start, Grid goal)
    //{
    //    came_from = new Dictionary<Grid, Grid>();
    //    cost_so_far = new Dictionary<Grid, double>();
    //    frontier = new PriorityQueue<Grid, double>();
    //    frontier.Push(start, 0);
    //    came_from[start] = start;
    //    cost_so_far[start] = 0;
    //    while (frontier.Count > 0)
    //    {
    //        Grid current = frontier.Pop();
    //        if (current == goal)
    //            break;
    //        Grid[] neighbors = graph.Neighbors(current);
    //        for (int i = 0; i < neighbors.Length; i++)
    //        {
    //            Grid next = neighbors[i];
    //            double new_cost = cost_so_far[current] + graph.Cost(next);
    //            if (came_from.ContainsKey(next) || new_cost < cost_so_far[next])
    //            {
    //                cost_so_far[next] = new_cost;
    //                double priority = new_cost;
    //                frontier.Push(next, priority);
    //                came_from[next] = current;
    //            }
    //        }
    //    }
    //}
    //以离终点的距离作为优先级来排列
    //public void GreedyBestFirstSearch(Grid start, Grid goal)
    //{
    //    came_from = new Dictionary<Grid, Grid>();
    //    cost_so_far = new Dictionary<Grid, double>();
    //    frontier = new PriorityQueue<Grid, double>();
    //    frontier.Push(start, 0);
    //    came_from[start] = start;
    //    while (frontier.Count > 0)
    //    {
    //        Grid current = frontier.Pop();
    //        if (current == goal)
    //            break;
    //        Grid[] neighbors = graph.Neighbors(current);
    //        for (int i = 0; i < neighbors.Length; i++)
    //        {
    //            Grid next = neighbors[i];
    //            if (came_from.ContainsKey(next))
    //            {
    //                int priority = Heuristic(goal,next);
    //                frontier.Push(next, priority);
    //                came_from[next] = current;
    //            }
    //        }
    //    }
    //}
    public void AstarSearch(Grid start, Grid goal)
    {
        came_from = new Dictionary<Grid, Grid>();
        cost_so_far = new Dictionary<Grid, double>();
        frontier = new PriorityQueue<Grid, double>();
        //if (start != null)
        //{
        //    Debug.Log("start " + start.name);
        //    Debug.Log("end " + goal.name);
        //}
        frontier.Push(start, 0);
        came_from.Add(start, start);
        //came_from[start] = start;
        cost_so_far.Add(start, 0);
        //cost_so_far[start] = 0;
        while (frontier.Count > 0)
        {
            Grid current = frontier.Pop();
            if (current == goal)
                break;
            Grid[] neighbors = graph.Neighbors(current);
            for (int i = 0; i < neighbors.Length; i++)
            {
                Grid next = neighbors[i];
                double new_cost = cost_so_far[current] + graph.Cost(next);
                if (!cost_so_far.ContainsKey(next) || new_cost < cost_so_far[next])
                {
                    if (cost_so_far.ContainsKey(next))
                    {
                        cost_so_far[next] = new_cost;
                    }
                    else
                        cost_so_far.Add(next, new_cost);
                   
                    double priority = new_cost + Heuristic(goal, next);
                    frontier.Push(next, priority);
                    if(came_from.ContainsKey(next))
                        came_from[next] = current;
                    else
                        came_from.Add(next,current);

                }
            }
        }
    }

    public List<Grid> GetShortestPath(Grid start,Grid goal)
    {
        if (!graph.Passable(goal) || !graph.Passable(start))
        {
            Debug.Log("不能形成路径");
            return null;
        }
        AstarSearch(start, goal);
        Grid current = goal;
        List<Grid> path = new List<Grid>();
        while (current != start)
        {
            path.Add(current);
            if (current == null)
            {
                Debug.Log("current");
            }
            if (came_from == null)
            {
                Debug.Log("came_from");
            }
            current = came_from[current];
        }
        path.Add(start);
        path.Reverse();
        return path;
    }
}
