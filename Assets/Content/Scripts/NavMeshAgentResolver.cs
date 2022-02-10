using UnityEngine;
using UnityEngine.AI;

public class NavMeshAgentResolver : MonoBehaviour
{
    public Transform Agent;
    public float ResolvingSpeed = 1f;
    public bool HandleX = true, HandleY = true, HandleZ = true;
    public bool HandleRotation = true;

    bool beStatic;
    Vector3 agentOldPos;
    private void Update()
    {
        var pos = transform.position;

        var agentPos = Agent.position;

        if (Vector3.Distance(agentPos, agentOldPos) > 1f)
            beStatic = false;
        
        if(Vector2.Distance(new Vector2(pos.x, pos.z), new Vector2(agentPos.x, agentPos.z)) < 1f)
        {
            beStatic = true;
            agentOldPos = agentPos;
        }

        if (beStatic)
            return;

        //agentOldPos = agentPos;

        var result = Vector3.MoveTowards(pos, agentPos, ResolvingSpeed);

        var rotRes = Quaternion.Slerp(transform.rotation, Agent.rotation, ResolvingSpeed * Time.deltaTime);

        if (HandleX) pos.x = result.x;
        if (HandleY) pos.y = result.y;
        if (HandleZ) pos.z = result.z;

        transform.position = pos;

        if(HandleRotation)
        transform.rotation = rotRes;
    }

    static GameObject handleAgents;
    private void Awake()
    {
        if (!handleAgents)
            handleAgents = new GameObject("Agents");
        Agent.SetParent(handleAgents.transform, true);
#if UNITY_EDITOR
        Agent.name = $"Agent ({name})";
#endif
    }
}
