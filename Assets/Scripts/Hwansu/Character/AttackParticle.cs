using UnityEngine;

public class AttackParticle : MonoBehaviour
{
    private Vector2 moveDirection;
    private float speed;
    private float lifetime;
    private float timer = 0f;
    private LayerMask monsterLayer;

    public void Initialize(Vector2 direction, float moveSpeed, float lifeTime, LayerMask targetLayer)
    {
        moveDirection = direction;
        speed = moveSpeed;
        lifetime = lifeTime;
        monsterLayer = targetLayer;
    }

    private void Update()
    {
        // X축 방향으로 이동
        transform.Translate(moveDirection * speed * Time.deltaTime, Space.World);

        // 수명 체크
        timer += Time.deltaTime;
        if (timer >= lifetime)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Monster 레이어와 충돌 시 파괴
        if (((1 << collision.gameObject.layer) & monsterLayer) != 0)
        {
            Destroy(gameObject);
        }
    }
}