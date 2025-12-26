using System.Collections.Generic;
using UnityEngine;

public class ItemManager : Singleton<ItemManager>
{
    [Header("Spawn")]
    [SerializeField] private Transform[] spawnPoints = new Transform[6]; // 6개의 스폰 위치
    [SerializeField] private List<GameObject> itemPrefabs;
    [SerializeField] private int minSpawnCount = 2; 
    
    [Header("Spawn Area")]
    [SerializeField] private Vector2 spawnAreaCenter = Vector2.zero; // 스폰 영역 중심
    [SerializeField] private Vector2 spawnAreaSize = new Vector2(10f, 10f); // 스폰 영역 크기 (너비, 높이)

    private readonly List<ScannableItem> spawnedItems = new();
    private int currentMaxSortingOrder = 0; // 현재 가장 높은 sortingOrder
    
    // 드래그 시작 시 호출하여 새로운 최대 sortingOrder를 반환
    public int GetNextSortingOrder()
    {
        currentMaxSortingOrder++;
        return currentMaxSortingOrder;
    }
    
    // 스폰 포인트들을 지정된 범위 내의 랜덤 위치로 이동
    public void RandomizeSpawnPoints()
    {
        foreach (var spawnPoint in spawnPoints)
        {
            if (spawnPoint != null)
            {
                // 네모 모양 범위 내의 랜덤 위치 계산
                float randomX = Random.Range(
                    spawnAreaCenter.x - spawnAreaSize.x / 2f,
                    spawnAreaCenter.x + spawnAreaSize.x / 2f
                );
                float randomY = Random.Range(
                    spawnAreaCenter.y - spawnAreaSize.y / 2f,
                    spawnAreaCenter.y + spawnAreaSize.y / 2f
                );
                
                // Z 좌표는 기존 값 유지
                Vector3 newPosition = new Vector3(randomX, randomY, spawnPoint.position.z);
                spawnPoint.position = newPosition;
            }
        }
    }

    // 랜덤 위치에 랜덤 아이템 스폰
    public List<ScannableItem> SpawnItems()
    {
        // 기존 아이템 정리
        ClearItems();
        
        // 사용 가능한 스폰포인트 인덱스 리스트 생성
        List<int> availableIndices = new List<int>();
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            if (spawnPoints[i] != null)
                availableIndices.Add(i);
        }

        // 1개에서 availableIndices.Count 범위로 랜덤 생성
        int countToSpawn = Random.Range(minSpawnCount, availableIndices.Count + 1);
        List<int> selectedIndices = new List<int>();

        // 중복 없이 랜덤 인덱스 선택
        for (int i = 0; i < countToSpawn; i++)
        {
            int randomIndex = Random.Range(0, availableIndices.Count);
            selectedIndices.Add(availableIndices[randomIndex]);
            availableIndices.RemoveAt(randomIndex); // 중복 방지
        }

        // 선택된 위치에 랜덤 아이템 생성
        foreach (int index in selectedIndices)
        {
            var prefab = itemPrefabs[Random.Range(0, itemPrefabs.Count)];

            var go = Instantiate(prefab, spawnPoints[index].position, spawnPoints[index].rotation);
            go.SetActive(true); // 프리팹이 비활성화되어 있을 수 있으므로 활성화
            
            // ScannableItem 컴포넌트 찾아서 리스트에 추가 (바코드가 자식일 수도 있으니 GetComponentInChildren 사용)
            var item = go.GetComponentInChildren<ScannableItem>();
            if (item != null) spawnedItems.Add(item);
        }
        
        return spawnedItems;
    }
    
    // 스폰된 아이템 리스트 반환
    public List<ScannableItem> GetSpawnedItems()
    {
        return spawnedItems;
    }
    
    // 아이템 정리 및 sortingOrder 초기화
    public void ClearItems()
    {
        // 스폰된 물건 제거
        for (int i = 0; i < spawnedItems.Count; i++)
        {
            if (spawnedItems[i] != null)
                Destroy(spawnedItems[i].transform.root.gameObject); // 바코드가 자식이면 root로 제거
        }
        spawnedItems.Clear();
        
        // sortingOrder 초기화 (다음 라운드를 위해)
        currentMaxSortingOrder = 0;
    }
    
    // 에디터에서 스폰 영역을 시각적으로 표시하는 Gizmo
    // 노란색 구 = 중심점, 초록색 박스 = 스폰 영역
    private void OnDrawGizmos()
    {
        // 스폰 영역 중심점 표시
        Gizmos.color = Color.yellow;
        Vector3 center = new Vector3(spawnAreaCenter.x, spawnAreaCenter.y, 0);
        Gizmos.DrawWireSphere(center, 0.2f);
        
        // 스폰 영역 사각형 표시
        Gizmos.color = Color.green;
        Vector3 size = new Vector3(spawnAreaSize.x, spawnAreaSize.y, 0);
        Gizmos.DrawWireCube(center, size);
    }
}

