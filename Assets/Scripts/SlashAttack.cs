using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(SphereCollider))]
public class SlashAttack : MonoBehaviour
{
    public float extendTime = .3f;
    public float holdTime = .1f;
    public float retractTime = .3f;
    public int damage = 1;

    LineRenderer lr;
    SphereCollider tipCol;
    Vector3 startPos, endPos;

    float timer;
    enum Phase { Extend, Hold, Retract }
    Phase phase;

    public void Init(Vector3 start, Vector3 end)
    {
        startPos = start;
        endPos = end;
        lr = GetComponent<LineRenderer>();
        tipCol = GetComponent<SphereCollider>();
        tipCol.isTrigger = true;
        phase = Phase.Extend;
    }

    void Update()
    {
        timer += Time.deltaTime;

        switch (phase)
        {
            case Phase.Extend:
                DrawLine(Mathf.Clamp01(timer / extendTime));
                if (timer >= extendTime) { phase = Phase.Hold; timer = 0; }
                break;

            case Phase.Hold:
                DrawLine(1f);
                tipCol.enabled = true;                     // collider active
                if (timer >= holdTime) { phase = Phase.Retract; timer = 0; }
                break;

            case Phase.Retract:
                tipCol.enabled = false;
                DrawLine(1f - Mathf.Clamp01(timer / retractTime));
                if (timer >= retractTime) Destroy(gameObject);
                break;
        }
    }

    void DrawLine(float t)
    {
        Vector3 tip = Vector3.Lerp(startPos, endPos, t);
        lr.SetPosition(0, startPos);
        lr.SetPosition(1, tip);
        tipCol.transform.position = tip;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IDamageable dmg))
            dmg.TakeHit(damage, transform.position);
    }
}
