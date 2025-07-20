using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public float speed = 12f;
    public int damage = 1;
    public float life = 4f;

    void Start() => Destroy(gameObject, life);

    void Update() => transform.Translate(Vector3.forward * speed * Time.deltaTime);

    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IDamageable dmg))
            dmg.TakeHit(damage, transform.position);

        Destroy(gameObject);
    }
}
