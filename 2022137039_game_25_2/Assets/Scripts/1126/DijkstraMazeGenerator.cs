using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DijkstraMazeGenerator : MonoBehaviour
{
    public int width = 21;
    public int height = 21;

    public GameObject wallPrefab;
    public GameObject floorPrefab;
    public GameObject forestPrefab;
    public GameObject mudPrefab;
    public GameObject pathPrefab;
    public GameObject playerPrefab;

    public float moveSpeed = 0.3f;

    private int[,] map;
    public Vector2Int startPos = new Vector2Int(1, 1);
    public Vector2Int goalPos;

    private GameObject playerInstance;
    private List<Vector2Int> path;

    public Pathfinder pathfinder;
    public PlayerMove playerMove;

    Vector2Int[] dirs =
    {
        new Vector2Int(1,0), new Vector2Int(-1,0),
        new Vector2Int(0,1), new Vector2Int(0,-1),
    };

    void Start()
    {
        goalPos = new Vector2Int(width - 2, height - 2);
        GenerateValidMap();
    }

    // 버튼 이벤트
    public void OnClickPathButton()
    {
        path = pathfinder.FindPathDijkstra(map, startPos, goalPos);

        if (path == null)
        {
            Debug.Log("경로 없음");
            return;
        }

        ShowPath();
        if (playerMove == null)
        {
            Debug.LogError("PlayerMove가 인스펙터에 연결되었음에도 불구하고 현재 Null입니다. 참조된 오브젝트가 파괴되었을 수 있습니다.");
        }
        else
        {
            Debug.Log("PlayerMove 객체 참조가 확인되었습니다. 이제 MoveAlongPath를 호출합니다.");

            playerMove.MoveAlongPath(path);
        }
        //playerMove.MoveAlongPath(path);
    }

    public void GenerateValidMap()
    {
        while (true)
        {
            GenerateRandomMap();

            if (CheckEscapeDFS(startPos, goalPos))
                break;
        }

        DrawMap();
        SpawnPlayer();
    }

    void GenerateRandomMap()
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

    bool CheckEscapeDFS(Vector2Int start, Vector2Int goal)
    {
        bool[,] visited = new bool[width, height];
        Stack<Vector2Int> st = new Stack<Vector2Int>();
        st.Push(start);
        visited[start.x, start.y] = true;

        while (st.Count > 0)
        {
            var cur = st.Pop();
            if (cur == goal) return true;

            foreach (var d in dirs)
            {
                int nx = cur.x + d.x;
                int ny = cur.y + d.y;

                if (nx < 0 || ny < 0 || nx >= width || ny >= height) continue;
                if (visited[nx, ny]) continue;
                if (map[nx, ny] == 0) continue;

                visited[nx, ny] = true;
                st.Push(new Vector2Int(nx, ny));
            }
        }
        return false;
    }

    void DrawMap()
    {
        foreach (Transform child in transform)
            //Destroy(child.gameObject);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GameObject prefab = null;
                switch (map[x, y])
                {
                    case 0: prefab = wallPrefab; break;
                    case 1: prefab = floorPrefab; break;
                    case 2: prefab = forestPrefab; break;
                    case 3: prefab = mudPrefab; break;
                }

                Instantiate(prefab, new Vector3(x, 0, y), Quaternion.identity, transform);
            }
        }
    }

    void SpawnPlayer()
    {
        playerInstance = Instantiate(playerPrefab, new Vector3(startPos.x, 1.2f, startPos.y), Quaternion.identity);
        playerMove = playerInstance.GetComponent<PlayerMove>();
    }

    void ShowPath()
    {
        foreach (var node in path)
        {
            GameObject p = Instantiate(pathPrefab, new Vector3(node.x, 0.1f, node.y), Quaternion.identity);
            p.transform.localScale = Vector3.one * 0.3f;
        }
    }
    void Update()
    {
        // 스페이스바 누르면 경로 탐색 + 이동
        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnClickPathButton();
        }
    }
    void Awake()
    {
        if (pathfinder == null)
            pathfinder = gameObject.AddComponent<Pathfinder>();
    }
}