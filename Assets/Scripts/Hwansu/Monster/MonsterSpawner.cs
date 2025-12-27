using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    [Header("생성 설정")]
    [SerializeField] private GameObject monsterPrefab;
    [SerializeField] private Vector2 spawnPosition = new Vector2(10, 0);

    private Monster currentMonster;

    private void Start()
    {
        // 첫 번째 몬스터 생성
        SpawnMonster();
    }

    private void SpawnMonster()
    {
        GameObject monsterObj = Instantiate(monsterPrefab, spawnPosition, Quaternion.identity);
        Monster monster = monsterObj.GetComponent<Monster>();

        if (monster != null)
        {
            monster.Initialize(this);
            currentMonster = monster;

            // 몬스터 활성화 (플레이어 추적 모드)
            monster.SetWaitMode(false, Vector2.zero);
        }
    }

    public void OnMonsterDeath(Monster monster)
    {
        if (currentMonster == monster)
        {
            currentMonster = null;

            // 몬스터가 사망하면 즉시 새로운 몬스터 생성
            SpawnMonster();
        }
    }
}