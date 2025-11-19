using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MazeManager : MonoBehaviour
{
    // ====== Unity Editor에서 연결할 프리팹 ======
    public GameObject wallPrefab;
    public GameObject floorPrefab;
    public Transform mazeContainer; // 씬 정리용 부모 오브젝트

    private const float TILE_SIZE = 1f;

    // ====== 미로 데이터 ======
    private int[,] map;
    private int width = 21; // 기본 크기 (홀수)
    private int height = 21; // 기본 크기 (홀수)

    private Vector2Int startPos = new Vector2Int(1, 1);
    private Vector2Int goalPos;

    // 네 방향 (재귀 백트래킹용: 2칸 간격)
    private Vector2Int[] dirs =
    {
        new Vector2Int(0, 2), new Vector2Int(0, -2),
        new Vector2Int(2, 0), new Vector2Int(-2, 0)
    };

    // 네 방향 (DFS/BFS 탐색용: 1칸 간격)
    private Vector2Int[] pathDirs =
    {
        new Vector2Int(0, 1), new Vector2Int(0, -1),
        new Vector2Int(1, 0), new Vector2Int(-1, 0)
    };

    void Start()
    {
        // 미로 크기 설정 (Tip: xLength-2, yLength-2에 Goal 위치)
        goalPos = new Vector2Int(width - 2, height - 2);

        // Start() 시점에 탈출 가능한 미로가 생성될 때까지 반복
        GenerateAndVisualizeMaze();
    }

    // Unity Input 처리 (과제 4번: Space키로 재생성)
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GenerateAndVisualizeMaze();
            Debug.Log("미로 재생성 완료!");
        }
    }

    // =======================================================
    // 1. 미로 생성 및 3. 탈출 불가 시 재생성 로직 통합
    // =======================================================
    private void GenerateAndVisualizeMaze()
    {
        // 탈출 가능한 미로가 나올 때까지 무한 반복
        while (true)
        {
            map = GenerateRandomMap(width, height);

            // 2. DFS로 탈출 가능 여부 파악
            if (CheckPathDFS(startPos, goalPos, map))
            {
                Debug.Log("탈출 가능한 미로 생성 성공!");
                VisualizeMaze(map); // 4. 탈출 가능하면 시각화
                break; // 루프 탈출
            }
            Debug.Log("탈출 불가능한 미로. 새로 생성합니다.");
        }
    }

    // -------------------------------------------------------
    // [1] 재귀 백트래킹을 이용한 랜덤 맵 생성
    // -------------------------------------------------------
    private int[,] GenerateRandomMap(int w, int h)
    {
        int[,] newMap = new int[w, h];

        // 맵 전체를 벽(1)으로 초기화
        for (int x = 0; x < w; x++)
            for (int y = 0; y < h; y++)
                newMap[x, y] = 1;

        // 재귀 함수 (GenerateMaze)
        void Generate(int x, int y)
        {
            newMap[x, y] = 0; // 현재 셀을 통로(0)로 만듦

            var shuffledDirs = dirs.OrderBy(d => Random.value).ToList();

            foreach (var d in shuffledDirs)
            {
                int nx = x + d.x;
                int ny = y + d.y;

                if (nx > 0 && nx < w - 1 && ny > 0 && ny < h - 1 && newMap[nx, ny] == 1)
                {
                    // 벽 뚫기 (중간 셀을 통로로)
                    newMap[x + d.x / 2, y + d.y / 2] = 0;
                    Generate(nx, ny);
                }
            }
        }

        // 시작점 (1, 1)에서 생성 시작
        Generate(startPos.x, startPos.y);
        return newMap;
    }

    // -------------------------------------------------------
    // [2] DFS를 이용한 경로 존재 여부 체크 (탈출 여부 파악)
    // -------------------------------------------------------
    private bool CheckPathDFS(Vector2Int start, Vector2Int goal, int[,] currentMap)
    {
        bool[,] visited = new bool[width, height];
        Stack<Vector2Int> stack = new Stack<Vector2Int>();

        stack.Push(start);
        visited[start.x, start.y] = true;

        while (stack.Count > 0)
        {
            Vector2Int cur = stack.Pop();

            if (cur == goal) return true; // 목표 도착

            foreach (var d in pathDirs) // 1칸 이동 방향 사용
            {
                int nx = cur.x + d.x;
                int ny = cur.y + d.y;

                if (nx >= 0 && nx < width && ny >= 0 && ny < height &&
                    currentMap[nx, ny] == 0 && !visited[nx, ny]) // 경계, 통로, 미방문
                {
                    visited[nx, ny] = true;
                    stack.Push(new Vector2Int(nx, ny));
                }
            }
        }
        return false; // 경로 없음
    }

    // -------------------------------------------------------
    // [4] 시각화 로직
    // -------------------------------------------------------
    private void VisualizeMaze(int[,] currentMap)
    {
        float offsetX = (width - 1) * TILE_SIZE / 2f;
        float offsetY = (height - 1) * TILE_SIZE / 2f;

        // 기존 오브젝트 정리
        if (mazeContainer != null)
        {
            foreach (Transform child in mazeContainer)
                Destroy(child.gameObject);
        }
        else
        {
            mazeContainer = new GameObject("MazeContainer").transform;
        }

        // 맵 순회 및 프리팹 인스턴스화
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GameObject prefabToInstantiate = (currentMap[x, y] == 1) ? wallPrefab : floorPrefab;

                if (prefabToInstantiate != null)
                {
                    Vector3 position = new Vector3(x * TILE_SIZE - offsetX, 0, y * TILE_SIZE - offsetY);
                    GameObject tile = Instantiate(prefabToInstantiate, position, Quaternion.identity);
                    tile.transform.SetParent(mazeContainer);

                    // 시작점과 목표 지점 시각적 표시 (선택 사항)
                    if (x == startPos.x && y == startPos.y)
                    {
                        tile.name = "StartTile";
                        // 여기에 시작 타일 색상 변경 등 추가 가능
                    }
                    else if (x == goalPos.x && y == goalPos.y)
                    {
                        tile.name = "GoalTile";
                        // 여기에 목표 타일 색상 변경 등 추가 가능
                    }
                }
            }
        }
    }

    // 이 클래스의 외부에서 맵 정보에 접근할 수 있도록 Getter 제공
    public int[,] GetMap() => map;
    public Vector2Int GetStartPos() => startPos;
    public Vector2Int GetGoalPos() => goalPos;
}
