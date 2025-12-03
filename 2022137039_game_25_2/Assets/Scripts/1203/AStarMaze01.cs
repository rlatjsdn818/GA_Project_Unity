using System.Collections.Generic;
using UnityEngine;

public class AStarMaze01 : MonoBehaviour
{
    public int width = 21;
    public int height = 21;

    int[,] map;
    public Vector2Int startPos = new Vector2Int(1, 1);
    public Vector2Int goalPos = new Vector2Int(19, 19);

    public GameObject wallPrefab;
    public GameObject floorPrefab;
    public GameObject forestPrefab;
    public GameObject mudPrefab;
    public GameObject pathPrefab;
    public GameObject playerPrefab;

    private List<GameObject> pathObjects = new List<GameObject>();

    GameObject playerObj;

    Vector2Int[] dirs =
    {
        new Vector2Int(1,0),
        new Vector2Int(-1,0),
        new Vector2Int(0,1),
        new Vector2Int(0,-1)
    };

    void Start()
    {
        GenerateValidMap();
        DrawMap();

        List<Vector2Int> path = RunAStar(map, startPos, goalPos);

        if (path == null)
            Debug.Log("경로 없음");
        else
        {
            ShowPath(path);
            SpawnPlayer(path);
        }
    }

    // ===================== 미로 생성 =====================
    void GenerateValidMap()
    {
        while (true)
        {
            GenerateMaze();

            if (CheckEscapeDFS(startPos, goalPos))
            {
                Debug.Log("탈출 가능한 미로 생성 완료");
                break;
            }
        }
    }

    void GenerateMaze()
    {
        map = new int[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                //랜덤
                if (x == 0 || y == 0 || x == width - 1 || y == height - 1)
                    map[x, y] = 0; // 벽
                else
                {
                    float r = Random.value;
                    if (r < 0.5f) map[x, y] = 1; // 땅
                    else if (r < 0.7f) map[x, y] = 2; // 숲
                    else if (r < 0.85f) map[x, y] = 3; // 진흙
                    else map[x, y] = 0; // 벽
                }
            }
        }

        map[startPos.x, startPos.y] = 1;
        map[goalPos.x, goalPos.y] = 1;
    }

    void Carve(int x, int y)
    {
        map[x, y] = 1;

        List<Vector2Int> randDirs = new()
        {
            new Vector2Int(2,0),
            new Vector2Int(-2,0),
            new Vector2Int(0,2),
            new Vector2Int(0,-2)
        };

        for (int i = 0; i < randDirs.Count; i++)
        {
            int r = Random.Range(i, randDirs.Count);
            (randDirs[i], randDirs[r]) = (randDirs[r], randDirs[i]);
        }

        foreach (var d in randDirs)
        {
            int nx = x + d.x;
            int ny = y + d.y;

            if (nx > 0 && ny > 0 && nx < width - 1 && ny < height - 1)
            {
                if (map[nx, ny] == 0)
                {
                    map[x + d.x / 2, y + d.y / 2] = 1;
                    Carve(nx, ny);
                }
            }
        }
    }

    void AddTerrainTypes()
    {
        for (int x = 1; x < width - 1; x++)
            for (int y = 1; y < height - 1; y++)
            {
                if (map[x, y] == 1)
                {
                    float r = Random.value;
                    if (r < 0.1f) map[x, y] = 2;
                    else if (r < 0.15f) map[x, y] = 3;
                }
            }
    }

    bool CheckEscapeDFS(Vector2Int start, Vector2Int goal)
    {
        bool[,] visited = new bool[width, height];
        return DFS(start.x, start.y);

        bool DFS(int x, int y)
        {
            if (!InBounds(x, y)) return false;
            if (map[x, y] == 0) return false;
            if (visited[x, y]) return false;

            visited[x, y] = true;
            if (x == goal.x && y == goal.y) return true;

            foreach (var d in dirs)
                if (DFS(x + d.x, y + d.y)) return true;

            return false;
        }
    }

    bool InBounds(int x, int y) =>
        x >= 0 && y >= 0 && x < width && y < height;

    // ===================== 시각화 =====================
    void DrawMap()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GameObject obj = null;

                switch (map[x, y])
                {
                    case 0: obj = wallPrefab; break;
                    case 1: obj = floorPrefab; break;
                    case 2: obj = forestPrefab; break;
                    case 3: obj = mudPrefab; break;
                }

                Instantiate(obj, new Vector3(x, 0, y), Quaternion.identity, transform);
            }
        }
    }

    void ShowPath(List<Vector2Int> path)
    {
        // 이전 길 표시 제거
        foreach (var obj in pathObjects)
            Destroy(obj);
        pathObjects.Clear();

        // 새 길 표시
        foreach (var p in path)
        {
            GameObject obj = Instantiate(pathPrefab, new Vector3(p.x, 0.1f, p.y), Quaternion.identity);
            pathObjects.Add(obj);
        }
    }


    void SpawnPlayer(List<Vector2Int> path)
    {
        playerObj = Instantiate(playerPrefab, new Vector3(startPos.x, 0.5f, startPos.y), Quaternion.identity);
        StartCoroutine(MoveAlong(path));
    }

    System.Collections.IEnumerator MoveAlong(List<Vector2Int> path)
    {
        foreach (var p in path)
        {
            Vector3 target = new Vector3(p.x, 0.5f, p.y);
            while (Vector3.Distance(playerObj.transform.position, target) > 0.01f)
            {
                playerObj.transform.position =
                    Vector3.MoveTowards(playerObj.transform.position, target, Time.deltaTime * 3f);
                yield return null;
            }
        }
    }

    // ===================== A* =====================
    List<Vector2Int> RunAStar(int[,] map, Vector2Int start, Vector2Int goal)
    {
        int w = map.GetLength(0);
        int h = map.GetLength(1);

        int[,] gCost = new int[w, h];
        bool[,] visited = new bool[w, h];
        Vector2Int?[,] parent = new Vector2Int?[w, h];

        for (int x = 0; x < w; x++)
            for (int y = 0; y < h; y++)
                gCost[x, y] = int.MaxValue;

        List<Vector2Int> open = new() { start };
        gCost[start.x, start.y] = 0;

        while (open.Count > 0)
        {
            int best = 0;
            int bestF = gCost[open[0].x, open[0].y] + H(open[0], goal);

            for (int i = 1; i < open.Count; i++)
            {
                int f = gCost[open[i].x, open[i].y] + H(open[i], goal);
                if (f < bestF)
                {
                    best = i;
                    bestF = f;
                }
            }

            Vector2Int cur = open[best];
            open.RemoveAt(best);

            if (visited[cur.x, cur.y]) continue;
            visited[cur.x, cur.y] = true;

            if (cur == goal) return Reconstruct(parent, start, goal);

            foreach (var d in dirs)
            {
                int nx = cur.x + d.x;
                int ny = cur.y + d.y;

                if (!InBounds(nx, ny)) continue;
                if (map[nx, ny] == 0) continue;
                if (visited[nx, ny]) continue;

                int newG = gCost[cur.x, cur.y] + TileCost(map[nx, ny]);

                if (newG < gCost[nx, ny])
                {
                    gCost[nx, ny] = newG;
                    parent[nx, ny] = cur;

                    if (!open.Contains(new Vector2Int(nx, ny)))
                        open.Add(new Vector2Int(nx, ny));
                }
            }
        }

        return null;
    }

    int H(Vector2Int a, Vector2Int b) =>
        Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);

    int TileCost(int tile) =>
        tile switch { 1 => 1, 2 => 3, 3 => 5, _ => int.MaxValue };

    List<Vector2Int> Reconstruct(Vector2Int?[,] parent, Vector2Int start, Vector2Int goal)
    {
        List<Vector2Int> path = new();
        Vector2Int? cur = goal;

        while (cur.HasValue)
        {
            path.Add(cur.Value);
            if (cur.Value == start) break;
            cur = parent[cur.Value.x, cur.Value.y];
        }
        path.Reverse();
        return path;
    }
    public void OnClickPathButton()
    {
        List<Vector2Int> path = RunAStar(map, startPos, goalPos);
        if (path == null)
            Debug.Log("경로 없음");
        else
            ShowPath(path);
    }
}
