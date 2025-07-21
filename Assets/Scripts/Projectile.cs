using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 25f;
    public float life = 3f;
    public ParticleSystem trailFX;
    public int damage = 1;

    void Start() => Destroy(gameObject, life);

    void Update() => transform.Translate(Vector3.forward * speed * Time.deltaTime);

    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IDamageable dmg))
            dmg.TakeHit(damage, transform.position); // TODO: add hit logic / VFX
        Destroy(gameObject);
    }
}