using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    public GameObject wallPrefab;
    public GameObject floorPrefab;

    public Transform mazeContainer;

    private const float TILE_SIZE = 1f;


    // 미로 맵 (1: 벽, 0: 통로)
    int[,] map;
    int width;
    int height;

    void Start()
    {
        // 원하는 미로 크기 (홀수)를 설정하고 미로 생성 시작
        // 이 함수가 호출되어야 GenerateMaze와 VisualizeMaze가 실행됩니다.
        GenerateMaze(21, 21);
    }
    // 네 방향 (상, 하, 좌, 우)
    Vector2Int[] dirs =
    {
        new Vector2Int(0, 2),  // 세 칸 간격으로 이동 (벽을 뚫기 위함)
        new Vector2Int(0, -2),
        new Vector2Int(2, 0),
        new Vector2Int(-2, 0),
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
        VisualizeMaze(map); // 미로 생성이 완료되면 시각화 함수 호출
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

    /// int[,] map 데이터를 읽어 Unity 씬에 프리팹으로 시각화합니다.
    public void VisualizeMaze(int[,] map)
    {
        // 맵 중앙을 월드 좌표 (0, 0, 0)에 맞추기 위한 오프셋
        // (W-1)/2, (H-1)/2 로 계산하여 홀수 맵의 중앙을 찾음
        float offsetX = (width - 1) * TILE_SIZE / 2f;
        float offsetY = (height - 1) * TILE_SIZE / 2f;

        // 기존에 생성된 미로 오브젝트를 모두 제거 (새로운 미로 생성 시 필요)
        if (mazeContainer != null)
        {
            // mazeContainer 내부의 모든 자식 오브젝트를 파괴합니다.
            foreach (Transform child in mazeContainer)
            {
                Destroy(child.gameObject);
            }
        }
        else
        {
            // 부모 오브젝트가 없으면 "MazeContainer" 이름으로 새로 생성합니다.
            mazeContainer = new GameObject("MazeContainer").transform;
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GameObject prefabToInstantiate;

                // 1은 벽, 0은 통로(바닥)
                if (map[x, y] == 1)
                {
                    prefabToInstantiate = wallPrefab;
                }
                else
                {
                    prefabToInstantiate = floorPrefab;
                }

                // 프리팹이 할당되어 있는지 확인
                if (prefabToInstantiate != null)
                {
                    // 월드 좌표 계산: 그리드 좌표 * 타일 크기 - 오프셋
                    Vector3 position = new Vector3(
                        x * TILE_SIZE - offsetX,
                        0, // Y축은 높이 (바닥 0, 벽은 높이를 가짐)
                        y * TILE_SIZE - offsetY
                    );

                    // 프리팹 인스턴스화 (복제)
                    GameObject tile = Instantiate(prefabToInstantiate, position, Quaternion.identity);

                    // 생성된 오브젝트를 MazeContainer 하위로 정리
                    tile.transform.SetParent(mazeContainer);
                }
            }
        }
    }
}
