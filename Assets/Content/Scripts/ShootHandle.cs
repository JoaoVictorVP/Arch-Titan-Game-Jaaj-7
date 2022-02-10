using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ShootHandle : MonoBehaviour
{
    public event Func<ShootHandle, GameObject, bool> OnTouch;
    public event Action<ShootHandle> OnReachDestination;

    public string Id = Guid.NewGuid().ToString();

    public LayerMask Mask;
    public Vector3 Destination;
    public float Speed;
    [Space(10)]
    public float DurationTime = 10;

    public void SendTo(Vector3 destination, float speed = 3, LayerMask mask = default)
    {
        Destination = destination;
        Speed = speed;
        if (mask != default)
            Mask = mask;
        time = DurationTime;
    }

    static Collider[] results = new Collider[32];
    float time;
    private void Update()
    {
        if ((time -= Time.deltaTime) < 0)
            Destroy(gameObject);

        var pos = transform.position;
        var npos = Vector3.MoveTowards(pos, pos + Destination, Speed);
        transform.position = npos;

        float dist = Vector3.Distance(pos, npos);

        if (dist > 1f)
        {
            float steps = dist / 0.3f;
            float stepL = 1f / steps;
            for(float t = 0; t <= 1f; t += stepL)
            {
                npos = Vector3.Lerp(pos, npos, t);
                if (step(npos))
                    return;
            }
        }
        else
            step(npos);

        if (Vector3.Distance(npos, Destination) < 0.1f)
            OnReachDestination?.Invoke(this);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    bool step(Vector3 pos)
    {
        int count;
        if ((count = Physics.OverlapSphereNonAlloc(pos, 0.3f, results, Mask, QueryTriggerInteraction.Ignore)) > 0)
        {
            for (int i = 0; i < count; i++)
            {
                if ((OnTouch?.Invoke(this, results[i].gameObject)) ?? false)
                {
                    results[i].GetComponent<Rigidbody>()?.AddExplosionForce(Speed, pos, 3 * transform.localScale.x);
                    return true;
                }
            }
        }
        return false;
    }
}
