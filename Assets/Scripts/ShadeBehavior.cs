using UnityEngine;

public class ShadeBehavior : MonoBehaviour
{
    enum State { Approach, OrbitHold, SpiralIn, CloseOrbit }
    State state;

    [Header("Orbit Spiral")]
    public float orbitRadius = 9f;    // R₀
    public float minRadius = 3f;    // Rmin
    public int stepsToClose = 6;     // spiral clicks
    public float orbitSpeedDeg = 50f;   // ° / s

    [Header("Slash Attack")]
    public SlashAttack slashPrefab;     // <-- NEW prefab
    public Transform muzzle;
    public float slashCooldown = 0.8f;

    // ------- cache ----------
    CharacterController cc;
    ILightAttractor player;
    float nextSlashTime;
    float desiredRadius;
    float shrinkStep;

    void Awake()
    {
        cc = GetComponent<CharacterController>();
        desiredRadius = orbitRadius;
        shrinkStep = (orbitRadius - minRadius) / stepsToClose;
    }
    void Start()
    {
        // assume one LightSource on player
        player = GameObject.FindObjectOfType<LightSource>();
        state = State.Approach;
    }

    void Update()
    {
        if (player == null) return;

        Vector3 toP = player.Position - transform.position;
        float dist = toP.magnitude;

        switch (state)
        {
            /* ---------- APPROACH ---------- */
            case State.Approach:
                MoveToward(toP.normalized, 5f);
                if (dist <= orbitRadius + 0.5f)
                    state = State.OrbitHold;
                break;

            /* ---------- ORBIT HOLD ---------- */
            case State.OrbitHold:
                OrbitAround(toP, desiredRadius);
                TrySlash();
                if (Time.time >= nextSlashTime) StartSpiral();
                break;

            /* ---------- SPIRAL IN ---------- */
            case State.SpiralIn:
                OrbitAround(toP, desiredRadius);
                TrySlash();
                if (desiredRadius <= minRadius + 0.1f)
                    state = State.CloseOrbit;
                break;

            /* ---------- CLOSE ORBIT ---------- */
            case State.CloseOrbit:
                OrbitAround(toP, minRadius);
                TrySlash();
                break;
        }
    }

    /* ================= helpers ================ */

    void OrbitAround(Vector3 toP, float targetR)
    {
        Vector3 tangent = Vector3.Cross(Vector3.up, toP).normalized *
                          orbitSpeedDeg * Mathf.Deg2Rad;
        Vector3 radial = toP.normalized *
                          Mathf.Clamp(toP.magnitude - targetR, -3f, 3f);

        cc.Move((tangent + radial) * Time.deltaTime);
        FacePlayer(toP);
    }

    void MoveToward(Vector3 dir, float spd) =>
        cc.Move(dir * spd * Time.deltaTime);

    void FacePlayer(Vector3 toP)
    {
        toP.y = 0;
        if (toP.sqrMagnitude > .01f)
            transform.rotation = Quaternion.LookRotation(toP);
    }

    /* ---------- slash ---------- */
    void TrySlash()
    {
        if (Time.time < nextSlashTime) return;
        if (!slashPrefab || !muzzle) return;

        var s = Instantiate(slashPrefab, muzzle.position, Quaternion.identity);
        s.Init(muzzle.position, player.Position);

        nextSlashTime = Time.time + slashCooldown;
    }

    void StartSpiral()
    {
        desiredRadius = Mathf.Max(minRadius, desiredRadius - shrinkStep);
        state = State.SpiralIn;
    }
}