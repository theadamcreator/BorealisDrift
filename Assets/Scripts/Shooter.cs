using UnityEngine;
using UnityEngine.ProBuilder;

public class Shooter : MonoBehaviour
{
    public Camera cam;
    public Transform muzzle;               // empty GameObject at cam centre
    public Projectile projectilePrefab;
    public float shotCooldown = 0.25f;

    private float nextShotTime;

    public void TryShoot()
    {
        if (Time.time < nextShotTime) return;

        Instantiate(projectilePrefab, muzzle.position, cam.transform.rotation);
        EnergyBank.instance?.Consume(1.5f);             // drain 1.5 s
        nextShotTime = Time.time + shotCooldown;
    }
}
