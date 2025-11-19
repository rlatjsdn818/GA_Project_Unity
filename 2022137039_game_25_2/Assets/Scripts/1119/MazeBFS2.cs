using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class MazeBFS2 : MonoBehaviour
{
    public GameObject player;
    public GameObject wallPrefab;
    public GameObject floorPrefab;

    private int[,] map;
    private int width = 21; // 기본 크기 (홀수)
    private int height = 21; // 기본 크기 (홀수)

    Vector2Int start = new Vector2Int(1, 1);
    Vector2Int goal = new Vector2Int(5, 5);

    //각 칸을 방문했는지 여부를 기록하는 2차원 불리언 배열을 선언
    bool[,] visited;

    //경로를 역추적하기 위해, **각 칸에 도달할 때 바로 직전 칸의 좌표(부모 노드)**를 저장하는 2차원 Vector2Int? 배열을 선언
    Vector2Int?[,] parent;

    //현재 위치에서 **이동할 수 있는 네 방향
    Vector2Int[] dirs =
    {
        new Vector2Int(1,0),
        new Vector2Int(-1,0),
        new Vector2Int(0,1),
        new Vector2Int(0,-1),
    };
    public int[,] GenerateMaze(int w, int h)
    {
        // 너비와 높이는 홀수여야 함
        width = w;
        height = h;
        map = new int[w, h];

        // 미로 전체를 벽(1)으로 초기화
        for (int x = 0; x < w; x++)
            for (int y = 0; y < h; y++)
                map[x, y] = 1;

        // 시작점 (1, 1)에서 재귀 호출 시작
        Generate(1, 1);
        return map;

        void Generate(int x, int y)
        {
            // 현재 셀을 통로(0)로 만듦 (방문 처리)
            map[x, y] = 0;

            // 이동할 방향 리스트를 무작위로 섞음
            var shuffledDirs = dirs.OrderBy(d => Random.value).ToList();

            foreach (var d in shuffledDirs)
            {
                // 이동할 이웃 셀의 좌표
                int nx = x + d.x;
                int ny = y + d.y;

                // 이웃 셀이 경계 내에 있고 아직 벽(1)인지 확인
                if (nx > 0 && nx < width - 1 && ny > 0 && ny < height - 1 && map[nx, ny] == 1)
                {
                    // **1. 벽 뚫기 (통로 생성)**
                    // 현재 셀과 이웃 셀 사이의 벽을 허물어 통로(0)로 만듦
                    map[x + d.x / 2, y + d.y / 2] = 0;

                    // **2. 재귀 호출 (깊이 우선 탐색)**
                    Generate(nx, ny);
                }
            }
            // 이 셀에서 더 이상 이동할 곳이 없으면 함수가 종료되고 
            // 이전 호출 위치로 되돌아감 (백트래킹 발생)
        }
    }
    void Start()
    {
        List<Vector2Int> path = FindPathBFS();  //FindPathBFS 함수를 호출하여 미로 경로를 찾고 그 결과를 path 리스트에 저장
    }

    List<Vector2Int> FindPathBFS()
    {

        //미로 맵의 **너비 (x축)**와 **높이 (y축)**를 가져옵니다.
        int w = map.GetLength(0);
        int h = map.GetLength(1);

        //visited와 parent 배열을 맵 크기에 맞게 초기화.
        visited = new bool[w, h];
        parent = new Vector2Int?[w, h];

        //BFS 탐색에 사용할 **큐 (Queue)**를 생성합니다. 큐는 탐색할 다음 위치들을 선입선출 (FIFO) 방식으로 저장.
        Queue<Vector2Int> q = new Queue<Vector2Int>();
        q.Enqueue(start);

        //시작 지점을 방문 처리.
        visited[start.x, start.y] = true;

        //큐가 빌 때까지 (더 이상 탐색할 위치가 없을 때까지) 반복.
        while (q.Count > 0)
        {
            //큐에서 **가장 오래된 위치 (현재 탐색할 위치)**를 꺼냅니다.
            Vector2Int cur = q.Dequeue();

            //목표 도착
            if (cur == goal)
            {
                Debug.Log("BFS: Goal 도착");
                return ReconstructPath();
            }

            //내 방향 이웃 탐색, 미리 정의된 네 방향 (dirs)을 순회하며 현재 위치의 이웃 칸을 탐색합니다.
            foreach (var d in dirs)
            {
                //이웃 칸의 새로운 좌표를 계산.
                int nx = cur.x + d.x;
                int ny = cur.y + d.y;

                if (!InBounds(nx, ny)) continue;  //전체 바운더리, 새로운 좌표가 맵의 경계를 벗어났는지 확인하고, 벗어났으면 다음 방향으로 넘어감
                if (map[nx, ny] == 1) continue; //벽, 새로운 칸이 벽인지 확인하고, 벽이면 다음 방향으로 넘어감
                if (visited[nx, ny]) continue;  //이미 방문함, 새로운 칸을 이미 방문했는지 확인하고, 방문했으면 다음 방향으로 넘어감..

                visited[nx, ny] = true;  //조건을 통과한 이웃 칸을 방문 처리합니다.
                parent[nx, ny] = cur;  //경로 복원용 부모, 이웃 칸에 도달하기 직전 칸은 cur이므로, 이웃 칸의 부모를 cur으로 설정합니다. 경로 복원에 핵심입니다.
                q.Enqueue(new Vector2Int(nx, ny));  //이웃 칸을 큐에 넣고 다음 탐색 대상으로 예약합니다.
            }
        }

        //while 루프가 끝날 때까지 목표에 도달하지 못했다면 (큐가 비었지만 goal을 못 찾았다면), 경로가 없다는 로그를 출력하고 null을 반환합니다.
        Debug.Log("BFS: 경로 없음");
        return null;
    }


    bool InBounds(int x, int y)  //주어진 좌표 $(x, y)$가 미로 맵의 유효한 경계 내에 있는지 확인하는 함수입니다.
    {
        return x >= 0 && y >= 0 &&
            x < map.GetLength(0) &&
            y < map.GetLength(1);
    }

    List<Vector2Int> ReconstructPath()  //parent 배열을 사용하여 목표 지점부터 시작 지점까지 역으로 거슬러 올라가며 경로를 재구성하는 함수입니다.
    {
        List<Vector2Int> path = new List<Vector2Int>();
        Vector2Int? cur = goal;  //현재 위치를 목표 지점인 goal로 설정하고 시작합니다.

        //goal => start 방향으로 parent 따라가기, 현재 위치가 유효할 때까지 반복합니다.
        while (cur.HasValue)
        {
            path.Add(cur.Value);  //현재 위치를 경로 리스트에 추가합니다.

            //현재 위치의 부모 노드를 다음 cur으로 설정하여 한 칸 뒤로 이동합니다.시작 지점의 부모는 null이므로, while 루프는 시작 지점에서 멈춥니다.
            cur = parent[cur.Value.x, cur.Value.y];
        }

        path.Reverse();  //경로가 현재 goal에서 start 순서로 되어 있으므로, start => goal 순서로 반전
        Debug.Log($"경로 길이: {path.Count}");
        foreach (var p in path)
        {
            Debug.Log(p);
        }
        return path;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
