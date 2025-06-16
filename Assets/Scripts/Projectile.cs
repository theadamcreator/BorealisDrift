using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 25f;
    public float life = 3f;
    public ParticleSystem trailFX;

    void Start() => Destroy(gameObject, life);

    void Update() => transform.Translate(Vector3.forward * speed * Time.deltaTime);

    void OnTriggerEnter(Collider other)
    {
        // TODO: add hit logic / VFX
        Destroy(gameObject);
    }
}