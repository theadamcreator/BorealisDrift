using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(CharacterController))]
public class ShadeBehavior : MonoBehaviour
{
    enum State { Idle, Orbit, Engage, Recover }
    State state;

    [Header("Movement")]
    public float driftSpeed = 2f;
    public float orbitSpeed = 60f;   // degrees per sec
    public float engageSpeed = 5f;
    public float recoverTime = 1f;

    [Header("Attack")]
    public EnemyProjectile projectilePrefab;
    public float chargeTime = .7f;
    public Transform muzzle;
    public float contactRadius = 1.2f;

    // cache
    CharacterController cc;
    ILightAttractor targetLight;
    float chargeTimer, recoverTimer;
    readonly List<ILightAttractor> attractors = new();

    void Awake() => cc = GetComponent<CharacterController>();

    void OnEnable() => InvokeRepeating(nameof(ScanLights), 0, .5f);
    void OnDisable() => CancelInvoke(nameof(ScanLights));

    void Update()
    {
        if (targetLight == null) { state = State.Idle; return; }

        float dist = Vector3.Distance(transform.position, targetLight.Position);

        switch (state)
        {
            case State.Idle:
                IdleMove();
                if (dist <= targetLight.AttractionRadius) state = State.Orbit;
                break;

            case State.Orbit:
                OrbitMove();
                if (dist <= targetLight.EngageRadius) { state = State.Engage; chargeTimer = chargeTime; }
                break;

            case State.Engage:
                EngageMove();
                chargeTimer -= Time.deltaTime;
                if (chargeTimer <= 0f) Fire();
                if (dist > targetLight.EngageRadius * 1.4f) state = State.Recover;
                break;

            case State.Recover:
                recoverTimer -= Time.deltaTime;
                if (recoverTimer <= 0f) state = State.Orbit;
                break;
        }
    }

    /* ---------- movement ---------- */
    void IdleMove()
    {
        // gentle random wander
        Vector3 dir = new Vector3(Mathf.PerlinNoise(Time.time, 0) - .5f, 0,
                                  Mathf.PerlinNoise(0, Time.time) - .5f).normalized;
        cc.Move(dir * driftSpeed * Time.deltaTime);
    }

    void OrbitMove()
    {
        Vector3 toLight = transform.position - targetLight.Position;
        Vector3 tangent = Vector3.Cross(Vector3.up, toLight).normalized;
        Vector3 desired = tangent * orbitSpeed * Mathf.Deg2Rad;   // radians
        cc.Move(desired * Time.deltaTime);
        FacePlayer();
    }

    void EngageMove()
    {
        Vector3 dir = (targetLight.Position - transform.position).normalized;
        cc.Move(dir * engageSpeed * Time.deltaTime);
        FacePlayer();
    }

    void FacePlayer()
    {
        Vector3 look = targetLight.Position - transform.position;
        look.y = 0;
        if (look.sqrMagnitude > .01f)
            transform.rotation = Quaternion.LookRotation(look);
    }

    /* ---------- attack ---------- */
    void Fire()
    {
        if (!projectilePrefab || !muzzle) return;
        Instantiate(projectilePrefab, muzzle.position, muzzle.rotation);
        recoverTimer = recoverTime;
        state = State.Recover;
    }

    /* ---------- scan for lights ---------- */
    void ScanLights()
    {
        attractors.Clear();
        foreach (var ls in GameObject.FindObjectsOfType<MonoBehaviour>())
            if (ls is ILightAttractor ia) attractors.Add(ia);

        targetLight = null;
        float best = 0;
        foreach (var a in attractors)
        {
            float score = a.Intensity / (Vector3.Distance(transform.position, a.Position) + 0.1f);
            if (score > best) { best = score; targetLight = a; }
        }
    }
}
