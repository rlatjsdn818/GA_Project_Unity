using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Pathfinder : MonoBehaviour
{
    private Vector2Int[] dirs =
    {
        new Vector2Int(1,0), new Vector2Int(-1,0),
        new Vector2Int(0,1), new Vector2Int(0,-1)
    };

    public class Node
    {
        public Vector2Int pos;
        public float cost;
        public Node(Vector2Int pos, float cost)
        {
            this.pos = pos;
            this.cost = cost;
        }
    }

    public List<Vector2Int> FindPathDijkstra(int[,] map, Vector2Int start, Vector2Int goal)
    {
        int width = map.GetLength(0);
        int height = map.GetLength(1);

        float[,] dist = new float[width, height];
        Vector2Int[,] prev = new Vector2Int[width, height];
        bool[,] visited = new bool[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                dist[x, y] = Mathf.Infinity;
            }
        }

        dist[start.x, start.y] = 0f;

        SimplePriorityQueue<Node> pq = new SimplePriorityQueue<Node>();
        pq.Enqueue(new Node(start, 0f), 0f);

        while (pq.Count > 0)
        {
            Node curNode = pq.Dequeue();
            Vector2Int pos = curNode.pos;

            if (visited[pos.x, pos.y]) continue;
            visited[pos.x, pos.y] = true;

            if (pos == goal) break;

            foreach (var d in dirs)
            {
                int nx = pos.x + d.x;
                int ny = pos.y + d.y;

                if (nx < 0 || ny < 0 || nx >= width || ny >= height) continue;
                if (map[nx, ny] == 0) continue; // 벽

                float tileCost = GetCostFromTileType(map[nx, ny]);
                float newDist = dist[pos.x, pos.y] + tileCost;

                if (newDist < dist[nx, ny])
                {
                    dist[nx, ny] = newDist;
                    prev[nx, ny] = pos;
                    pq.Enqueue(new Node(new Vector2Int(nx, ny), newDist), newDist);
                }
            }
        }

        if (dist[goal.x, goal.y] == Mathf.Infinity)
            return null;

        List<Vector2Int> path = new List<Vector2Int>();
        Vector2Int curPos = goal;
        while (curPos != start)
        {
            path.Add(curPos);
            curPos = prev[curPos.x, curPos.y];
        }
        path.Add(start);
        path.Reverse();

        return path;
    }

    float GetCostFromTileType(int tileType)
    {
        switch (tileType)
        {
            case 1: return 1f; // 땅
            case 2: return 3f; // 숲
            case 3: return 5f; // 진흙
            default: return Mathf.Infinity;
        }
    }
}