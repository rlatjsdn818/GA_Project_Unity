using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class AISequence : MonoBehaviour
{

    public Button startButton;  //눌러서 브루트포스 시작
    string secretPin;
    Coroutine runningRoutine;

    // Start is called before the first frame update
    void Start()
    {
        // 랜덤 4자리 pin 생성 (0000 ~ 9999)
        secretPin = Random.Range(0, 10000).ToString("D4");
        Debug.Log($"[Auth] 생성된 PIN = {secretPin}");
        
    }
    
    public void OnStartButtonClicked()
    {
        if (runningRoutine != null)
        {
            Debug.Log("[Brute] 이미 실행 중입니다.");
            return;
        }

        runningRoutine = StartCoroutine(BruteForceRoutine());

    }

    IEnumerator BruteForceRoutine()
    {
        Debug.Log("[AI Combo] 시뮬레이션 시작. Max Cost: 15");

        Stopwatch sw = new Stopwatch();
        sw.Start();

        int maxCost = 15;
        int maxDamage = 0; // 찾은 최대 데미지
        string bestCombo = ""; // 최대 데미지를 낸 조합
        int totalCombinations = 0; // 시도한 총 조합 수

        // 퀵 샷 두장시도함
        for (int q = 0; q <= 2; q++)
        {
            // 헤비 샷 두장시도함
            for (int h = 0; h <= 2; h++)
            {
                // 멀티 샷 
                for (int m = 0; m <= 1; m++)
                {
                    // 트리플 샷
                    for (int t = 0; t <= 1; t++)
                    {
                        totalCombinations++; // 시도 횟수 증가

                        // Cost 계산
                        int currentCost = (q * 2) + (h * 3) + (m * 5) + (t * 7);

                        // 계산 후 Cost 제약 조건 검사
                        if (currentCost > maxCost)
                        {
                            continue; // 코스트 초과 후 다음 조합으로 넘어감
                        }

                        // 데미지 계산
                        int currentDamage = (q * 6) + (h * 8) + (m * 16) + (t * 24);

                        // 최대 데미지 갱신
                        if (currentDamage > maxDamage)
                        {
                            maxDamage = currentDamage;
                            bestCombo = $"Q:{q}, H:{h}, M:{m}, T:{t}";
                            Debug.Log($"[AI Combo] **제일높은거:** Damage={maxDamage}, Cost={currentCost}, Combo=({bestCombo})");
                        }

                        // 5. Unity 프레임 부하 관리
                        if (totalCombinations % 10 == 0)
                        {
                            yield return null;
                        }
                    }
                }
            }
        }

        sw.Stop();

        // 최종 결과 출력
        Debug.Log($"[AI Combo] **최적 조합 발견!** Max Damage: {maxDamage} (Combo: {bestCombo})");
        Debug.Log($"[AI Combo] 총 시도 조합 수: {totalCombinations}. 소요 시간: {sw.Elapsed.TotalSeconds:F5}초");
        runningRoutine = null;
    }
    // Update is called once per frame
    void Update()
    {

    }
}