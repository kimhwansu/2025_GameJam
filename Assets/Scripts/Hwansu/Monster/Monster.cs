using UnityEngine;
using System;

public class Monster : MonoBehaviour
{
    [SerializeField] private int maxHealth = 10;
    [SerializeField] private int currentHealth;

    [Header("이동 설정")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float minDistance = 1f;
    [SerializeField] private float detectionRange = 10f;

    [Header("사망 시 효과")]
    [SerializeField] private int goldReward = 50;

    public event Action OnMonsterDeath;

    private Transform playerTransform;
    private bool isChasing = false;

    private void Start()
    {
        currentHealth = maxHealth;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            playerTransform = player.transform;
    }

    private void Update()
    {
        if (playerTransform != null)
            ChasePlayer();
    }

    private void ChasePlayer()
    {
        float distance = Vector2.Distance(transform.position, playerTransform.position);

        if (distance <= detectionRange && distance > minDistance)
        {
            isChasing = true;

            Vector2 direction = (playerTransform.position - transform.position).normalized;
            transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, moveSpeed * Time.deltaTime);

            // 2D 회전 (Sprite가 오른쪽을 바라본 상태라면 Z축 회전)
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
        else
        {
            isChasing = false;
        }
    }

    public void TakeDamage()
    {
        currentHealth -= 1;
        currentHealth = Mathf.Max(0, currentHealth);

        if (currentHealth <= 0)
            Die();
    }

    private void Die()
    {
        if (GoldManager.Instance != null)
            GoldManager.Instance.AddGold(goldReward);

        OnMonsterDeath?.Invoke();

        Destroy(gameObject);
    }
}
