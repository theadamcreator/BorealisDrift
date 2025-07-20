using UnityEngine;

public class FloatingController : MonoBehaviour
{
    public enum FloatMode { AutoBob, PlayerInput }
    public FloatMode mode = FloatMode.AutoBob;

    [Header("Y-Bounds")]
    public float minY = 2f;
    public float maxY = 4f;

    [Header("Auto-Bob")]
    public float bobSpeed = 0.5f;

    [Header("Player Input")]
    public float ascendRate = 3f;       // units / s
    public string verticalAxis = "Jump";// map WHEEL/QE in Input Manager

    Transform _t;

    void Awake() => _t = transform;

    void LateUpdate()
    {
        Vector3 p = _t.position;

        if (mode == FloatMode.AutoBob)
        {
            float t = 0.5f + 0.5f * Mathf.Sin(Time.time * bobSpeed);
            p.y = Mathf.Lerp(minY, maxY, t);
        }
        else
        {
            float input = Input.GetAxisRaw(verticalAxis);
            p.y = Mathf.Clamp(p.y + input * ascendRate * Time.deltaTime, minY, maxY);
        }

        _t.position = p;
    }

    public void SetVerticalBand(float centerY, float halfRange)
{
    minY = centerY - halfRange;
    maxY = centerY + halfRange;
}
}
