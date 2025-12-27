using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    [Header("생성 설정")]
    [SerializeField] private GameObject monsterPrefab;
    [SerializeField] private Vector2 spawnPosition = new Vector2(10, 0);
    [SerializeField] private float spawnInterval = 3f;

    [Header("대기 간격")]
    [SerializeField] private float queueSpacing = 1f; // 몬스터 간 X축 간격

    private List<Monster> monsters = new List<Monster>();

    private void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    private void Update()
    {
        // 매 프레임마다 몬스터 간격 유지
        UpdateAllMonsterPositions();
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            SpawnMonster();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnMonster()
    {
        GameObject monsterObj = Instantiate(monsterPrefab, spawnPosition, Quaternion.identity);
        Monster monster = monsterObj.GetComponent<Monster>();

        if (monster != null)
        {
            monster.Initialize(this);
            monsters.Add(monster);

            // 첫 번째 몬스터는 바로 활성화
            if (monsters.Count == 1)
            {
                monster.SetWaitMode(false, Vector2.zero);
            }
            else
            {
                // 바로 앞 몬스터 위치에서 간격만큼 뒤에 배치
                UpdateAllMonsterPositions();
            }
        }
    }

    private void UpdateAllMonsterPositions()
    {
        for (int i = 0; i < monsters.Count; i++)
        {
            if (i == 0)
            {
                // 첫 번째 몬스터는 플레이어 추적
                monsters[i].SetWaitMode(false, Vector2.zero);
            }
            else
            {
                // 앞 몬스터의 X좌표에서 queueSpacing만큼 뒤에, Y좌표는 동일하게
                Vector2 frontMonsterPos = monsters[i - 1].transform.position;
                Vector2 waitPos = new Vector2(frontMonsterPos.x + queueSpacing, frontMonsterPos.y);

                monsters[i].SetWaitMode(true, waitPos);
            }
        }
    }

    public void OnMonsterDeath(Monster monster)
    {
        monsters.Remove(monster);
        // 모든 몬스터 위치 재조정
        UpdateAllMonsterPositions();
    }
}