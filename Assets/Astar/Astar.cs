using System;
using System.Collections;
using System.Collections.Generic;

public class Grid {
   public int index;
}

public class Graph
{
    int[,] graph;

    public Grid[] Neighbors(Grid current)
    {
        return null;
    }
    public int Cost(Grid first, Grid next) {
        return graph[first.index, next.index];
    }
}
public class PriorityQueue<U,T>
{
    SortedList priorityQueue;
    public int Count {
        get { return priorityQueue.Count; }
    }

    public PriorityQueue()
    {
        priorityQueue = new SortedList();
    }
    public T Push(U key,T value)
    {
        priorityQueue.Add(key, value);
        return value;
    }
    public T Pop()
    {
        T first = (T)priorityQueue.GetByIndex(0);
        priorityQueue.RemoveAt(0);
        return first;
    }
    public T Peek()
    {
        return (T)priorityQueue.GetByIndex(0);
    }
}


public class Astar {
    Graph graph;
    PriorityQueue<int,Grid> frontier;
    
    Grid[] came_from;
    int[] cost_so_far;

    int gridCount;
    public Astar() {
        came_from = new Grid[gridCount];
        cost_so_far = new int[gridCount];
        frontier = new PriorityQueue<int,Grid>();
    }
    
    public void ReadAllPath(Grid start,Grid goal)
    {
        frontier.Push(0,start);
        came_from[start.index] = null;
        cost_so_far[start.index] = 0;
        while (frontier.Count > 0)
        {
            Grid current = frontier.Pop();
            if (current == goal)
                break;
            Grid[] neighbors = graph.Neighbors(current);
            for (int i = 0; i < neighbors.Length; i++)
            {
                Grid next = neighbors[i];
                int new_cost = cost_so_far[current.index] + graph.Cost(current, next);
                if (came_from[next.index] == null || new_cost < cost_so_far[next.index])
                {
                    cost_so_far[next.index] = new_cost;
                    int priority = new_cost;
                    frontier.Push(priority,next);
                    came_from[next.index] = current;
                }
            }
        }
    }

    public void GetShortestPath(Grid start,Grid goal)
    {
        Grid current = goal;
        List<Grid> path = new List<Grid>();
        while (current != start)
        {
            path.Add(current);
            current = came_from[current.index];
        }
        path.Add(start);
        path.Reverse();
    }
}
