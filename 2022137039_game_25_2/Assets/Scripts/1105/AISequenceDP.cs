using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class AISequenceDP : MonoBehaviour
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

        //runningRoutine = StartCoroutine(FindOptimalComboDP());

    }
    // 카드 정보를 구조체로 정의하면 관리하기 더 편리함
    struct CardInfo
    {
        public string name;
        public int damage;
        public int cost;
        public int maxCount;
    }

    public void FindOptimalComboDP()
    {
        Debug.Log("[AI Combo DP] 시뮬레이션 시작. Max Cost: 15");

        Stopwatch sw = new Stopwatch();
        sw.Start();

        int maxCost = 15;
        // dp[i] = Cost i 로 얻을 수 있는 최대 Damage
        int[] dp = new int[maxCost + 1];

        // 어떤 카드를 사용했는지 추적하기 위한 배열 (선택 사항이지만, 조합 추적에 필요)
        string[] comboTrace = new string[maxCost + 1];

        // 1. 카드 정보 설정
        CardInfo[] cards = new CardInfo[]
        {
            new CardInfo { name = "Q", damage = 6, cost = 2, maxCount = 2 }, // 퀵 샷
            new CardInfo { name = "H", damage = 8, cost = 3, maxCount = 2 }, // 헤비 샷
            new CardInfo { name = "M", damage = 16, cost = 5, maxCount = 1 }, // 멀티 샷
            new CardInfo { name = "T", damage = 24, cost = 7, maxCount = 1 }  // 트리플 샷
        };
        // ----------------------------------------------------

        // 2. DP 테이블 채우기
        foreach (var card in cards)
        {
            // 각 카드를 'card.maxCount' 만큼 반복 사용
            for (int k = 0; k < card.maxCount; k++)
            {
                // 현재 Cost (maxCost 부터 card.cost 까지 역순으로 순회)
                for (int i = maxCost; i >= card.cost; i--)
                {
                    int prevDamage = dp[i - card.cost]; // 이전 Cost로 달성한 최대 데미지
                    int newDamage = prevDamage + card.damage; // 현재 카드를 추가하여 얻을 수 있는 데미지

                    // 현재 Cost i 에서, 카드를 추가한 데미지(newDamage)가 
                    // 기존 데미지(dp[i])보다 높으면 갱신
                    if (newDamage > dp[i])
                    {
                        dp[i] = newDamage; // 최대 데미지 갱신

                        // 조합 추적 갱신
                        if (comboTrace[i - card.cost] == null)
                            comboTrace[i] = $"{card.name}:1";
                        else
                            comboTrace[i] = comboTrace[i - card.cost] + $", {card.name}:1";
                    }
                }
            }
        }

        sw.Stop();

        // 3. 결과 찾기 (dp 배열 전체를 순회하여 최대값과 그 Cost를 찾음)
        int finalMaxDamage = 0;
        int finalCost = 0;
        string finalCombo = "";

        for (int i = 0; i <= maxCost; i++)
        {
            if (dp[i] > finalMaxDamage)
            {
                finalMaxDamage = dp[i];
                finalCost = i;
                // 추적 배열을 이용하면 정확한 카드 장수를 파악하기 어렵기 때문에
                // 최종적으로는 조합을 재구성하는 로직이 필요함.
            }
        }

        // DP는 데미지 최대화에 초점을 맞추며, '조합 재구성'은 복잡해지므로,
        // 최종적으로는 '최대 데미지'와 '사용 코스트'만 출력
        Debug.Log($"[AI Combo DP] **최적 조합 발견!** Max Damage: {finalMaxDamage} (사용 Cost: {finalCost})");
        Debug.Log($"[AI Combo DP] 소요 시간: {sw.Elapsed.TotalSeconds:F5}초");
    }
}