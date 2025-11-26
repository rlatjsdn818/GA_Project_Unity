using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    public List<Vector2Int> FindPathDijkstra(int[,] map, Vector2Int start, Vector2Int goal)
    {
        int width = map.GetLength(0);
        int height = map.GetLength(1);

        int[,] dist = new int[width, height];
        Vector2Int[,] parent = new Vector2Int[width, height];

        var pq = new SimplePriorityQueue<Vector2Int>();

        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                dist[x, y] = int.MaxValue;

        dist[start.x, start.y] = 0;
        pq.Enqueue(start, 0);

        Vector2Int[] dirs =
        {
            new Vector2Int(1,0), new Vector2Int(-1,0),
            new Vector2Int(0,1), new Vector2Int(0,-1),
        };

        while (pq.Count > 0)
        {
            var cur = pq.Dequeue();

            if (cur == goal)
                return BuildPath(parent, start, goal);

            foreach (var d in dirs)
            {
                int nx = cur.x + d.x;
                int ny = cur.y + d.y;

                if (nx < 0 || ny < 0 || nx >= width || ny >= height) continue;
                if (map[nx, ny] == 0) continue;

                int cost = TerrainCost(map[nx, ny]);
                int nd = dist[cur.x, cur.y] + cost;

                if (nd < dist[nx, ny])
                {
                    dist[nx, ny] = nd;
                    parent[nx, ny] = cur;
                    pq.Enqueue(new Vector2Int(nx, ny), nd);
                }
            }
        }

        return null;
    }

    int TerrainCost(int tile)
    {
        switch (tile)
        {
            case 1: return 1;
            case 2: return 3;
            case 3: return 6;
        }
        return 999;
    }

    List<Vector2Int> BuildPath(Vector2Int[,] parent, Vector2Int start, Vector2Int goal)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        var cur = goal;

        while (cur != start)
        {
            path.Add(cur);
            cur = parent[cur.x, cur.y];
        }

        path.Add(start);
        path.Reverse();
        return path;
    }
}