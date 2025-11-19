using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;

public class RandomMaze : MonoBehaviour
{
    public int xLength = 21;
    public int yLength = 21;

    int[,] map =
    {

    };

    bool[,] visited;

    //Vector2Int goal = new Vector2Int(xLength - 2, yLength - 2);

    Vector2Int[] dirs = { new(1, 0), new(-1, 0), new(0, 1), new(0, -1) };

    void CountDown(int n)
    {
        if (n == 0) return;
        Debug.Log(n);
        CountDown(n - 1);
    }
    void Start()
    {
        visited = new bool[map.GetLength(0), map.GetLength(1)];
        bool ok = SearchMaze(1, 1);  //시작점 (1,1)
        Debug.Log(ok ? "출구 찾음!" : "출구 없음..");
    }

    bool SearchMaze(int x, int y)
    {
        //범위/벽/재방문 체크
        if (x < 0 || y < 0 || x >= map.GetLength(0) || y >= map.GetLength(1)) return false;
        if (map[x, y] == 1 || visited[x, y]) return false;

        //방문 표시
        visited[x, y] = true;
        Debug.Log($"이동: ({x},{y})");

        //목표 도달
        //if (x == goal.x && y == goal.y) return true;

        //4방향 재귀 탐색
        foreach (var d in dirs)
            if (SearchMaze(x + d.x, y + d.y)) return true;

        //막혔으면 되돌아감
        Debug.Log($"되돌아감: ({x},{y})");
        return false;
    }
}