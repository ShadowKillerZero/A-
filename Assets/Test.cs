using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Priority_Queue;

public class Test : MonoBehaviour {

    public int length, weight;
    static SquareGrid squareGrid;
    Astar astar;
    Grid start;
    Grid end;
    public static void ChageType(Grid grid,GridType gridType) {
        switch (gridType)
        {
            case GridType.Wall:
                grid.currentType = GridType.Ground;
                squareGrid.RemoveWall(grid);
                grid.GetComponent<MeshRenderer>().material.color = Color.white;
                break;
            case GridType.Forest:
                grid.currentType = GridType.Wall;
                squareGrid.RemoveForest(grid);
                squareGrid.AddWall(grid);
                grid.GetComponent<MeshRenderer>().material.color = Color.yellow;
                break;
            case GridType.Ground:
                grid.currentType = GridType.Forest;
                squareGrid.AddForest(grid);
                grid.GetComponent<MeshRenderer>().material.color = Color.green;
                break;
            default:
                break;
        }
    }
    // Use this for initialization
    void Start () {
        //SimplePriorityQueue<string> simPriQueue = new SimplePriorityQueue<string>();
        //simPriQueue.Enqueue("111", 1);
        //simPriQueue.Enqueue("111", 1);
        //simPriQueue.Enqueue("111", 1);
        //while (simPriQueue.Count>0)
        //{
        //    Debug.Log(simPriQueue.Dequeue());
        //}
        Grid[,] Grids = new Grid[weight, length];
        squareGrid = new SquareGrid(weight,length);
        int count = 0;
        for (int i = 0; i < length; i++)
        {
            for (int j = 0; j < weight; j++)
            {
                count++;
                GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                go.name = count.ToString();
                go.transform.position = new Vector3(i, 0, j);
                go.transform.localScale *= 0.7f;
                go.GetComponent<MeshRenderer>().material.color = Color.white;
                Grid grid = go.AddComponent<Grid>();
                grid.x = i;grid.y = j;
                squareGrid.AddEdge(grid);
                if (i == 0 && j == 0)
                {
                    start = grid;
                    grid.gameObject.GetComponent<MeshRenderer>().material.color = Color.black;
                }
                if (i == 15&&j == 15)
                {
                    end = grid;
                    grid.gameObject.GetComponent<MeshRenderer>().material.color = Color.black;
                }
                Grids[i, j] = grid;
                grid.currentType = GridType.Ground;
            }
        }
        for (int i = 0; i < 100; i++)
        {
            int x = Random.Range(0, 19);
            int y = Random.Range(0, 19);
            if (x == 0 && y == 0)
            {
                continue;
            }
            if (x == 15 && y == 15)
            {
                continue;
            }
            Grid grid = Grids[x, y];
            grid.currentType = GridType.Wall;
            grid.gameObject.GetComponent<MeshRenderer>().material.color = Color.yellow;
            squareGrid.AddWall(grid);
        }
        for (int i = 0; i < 50; i++)
        {
            int x = Random.Range(0, 19);
            int y = Random.Range(0, 19);
            if (x == 0 && y == 0)
            {
                continue;
            }
            if (x == 15 && y == 15)
            {
                continue;
            }
            Grid grid = Grids[x, y];
            grid.currentType = GridType.Forest;
            if (squareGrid.Passable(grid))
            {
                grid.gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
                squareGrid.AddForest(grid);
            }
        }
        astar = new Astar(squareGrid);
    }

    List<Grid> gridList;
    // Update is called once per frame
    void Update () {
        if (Input.GetMouseButtonDown(1))
        {
            if (gridList!=null)
            {
                for (int i = 0; i < gridList.Count; i++)
                {
                    
                    Grid grid = gridList[i];
                    if (grid.currentType == GridType.Ground)
                    {
                        Debug.Log(i);
                        grid.gameObject.GetComponent<MeshRenderer>().material.color = Color.white;
                    }
                    else if (grid.currentType == GridType.Forest) {
                        grid.gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
                    }

                }
            }
            gridList = astar.GetShortestPath(start, end);
            for (int i = 0; i < gridList.Count; i++)
            {
                gridList[i].gameObject.GetComponent<MeshRenderer>().material.color = Color.red;
            }
        }
	}
}
