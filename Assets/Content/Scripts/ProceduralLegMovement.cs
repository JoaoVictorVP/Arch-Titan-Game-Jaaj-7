using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class ProceduralLegMovement : MonoBehaviour
{
    static GameObject globalLegs;

    public LayerMask Mask;

    public Transform Forward;
    //public Transform Down;
    public GameObject[] Legs;
    public float limitStep = 1.5f;
    public float legSpeed = 0.5f;
    Leg[] legs;

    Vector3 oldPos;
    private void Update()
    {
        var pos = transform.position;
        //var offset = oldPos - pos;

        if(Vector3.Distance(pos, oldPos) > limitStep)
        {
            updateTargets();

            oldPos = pos;
        }

        foreach (var leg in legs)
            leg.target.transform.position = Vector3.MoveTowards(leg.target.transform.position, leg.finalPos, legSpeed);
    }
    void updateTargets()
    {
        bool q = Physics.queriesHitTriggers;
        Physics.queriesHitTriggers = false;
        //var down = Down ? -Down.up : -transform.up;
        var down = Vector3.down;
/*        foreach(var leg in legs)
        {
            if (Physics.Raycast(new Ray(leg.pos, randomDir(down)), out RaycastHit hit, 1000f, Mask))
                leg.finalPos = hit.point;
                //StartCoroutine(moveLeg(leg, hit.point, legSpeed));
        }*/

        int size = legs.Length;

        NativeArray<RaycastHit> hits = new(size, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);

        NativeArray<RaycastCommand> rays = new(size, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);

        for(int i = 0; i < size; i++)
            rays[i] = new RaycastCommand(legs[i].pos, randomDir(down), 1000f, Mask, 1);

        JobHandle handle = RaycastCommand.ScheduleBatch(rays, hits, 1);

        handle.Complete();

        for(int i = 0; i < size; i++)
        {
            var hit = hits[i];
            if (hit.collider)
                legs[i].finalPos = hit.point;
        }

        hits.Dispose();

        rays.Dispose();

        Physics.queriesHitTriggers = q;
    }
    Vector3 randomDir(Vector3 down)
    {
        int chance = Random.Range(0, 2);
        switch(chance)
        {
            case 0:
                return down;
            case 1:
                return Vector3.Lerp(down, new Vector3(Forward.localPosition.x, down.y, Forward.localPosition.z), 0.5f);
        }
        return down;
    }
    IEnumerator moveLeg(Leg leg, Vector3 point, float speed)
    {
        float t = 0;
        var legPos = leg.target.transform.position;
        while ((t += Time.deltaTime * speed) < 1f)
        {
            leg.target.transform.position = Vector3.Lerp(legPos, point, t);
            yield return null;
        }
    }

    private void Start()
    {
        if (globalLegs == null)
            globalLegs = new GameObject("Global Legs");

        legs = new Leg[Legs.Length];
        for (int i = 0; i < legs.Length; i++)
            legs[i] = new Leg (Legs[i], Legs[i].transform.Find("Target").gameObject);
        oldPos = transform.position;
        updateTargets();
    }
    class Leg
    {
        public readonly GameObject leg;
        public readonly GameObject target;
        public Vector3 pos => leg.transform.position;
        public Vector3 finalPos;

        public Leg(GameObject leg, GameObject target)
        {
            this.leg = leg;
            this.target = target;
            //startPos = leg.transform.position;

            target.transform.SetParent(globalLegs.transform, true);

#if UNITY_EDITOR
            target.name = $"Target -> {leg.transform.parent.name}, {leg.name}";
#endif
        }
    }
}
