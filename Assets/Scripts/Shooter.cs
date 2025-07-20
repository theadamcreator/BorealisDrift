using UnityEngine;

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

        Ray ray = Camera.main.ViewportPointToRay(new Vector3(.5f, .5f, 0));   // screen centre
        Vector3 dir = ray.direction;
        Vector3 spawnPos = muzzle.position;

        if (Physics.Raycast(ray, out RaycastHit hit, 500f))
            dir = (hit.point - spawnPos).normalized;       // steer towards what you see

        Projectile p = Instantiate(projectilePrefab, spawnPos, Quaternion.LookRotation(dir));
        EnergyBank.instance?.Consume(1.5f);             // drain 1.5 s
        nextShotTime = Time.time + shotCooldown;
    }
}
