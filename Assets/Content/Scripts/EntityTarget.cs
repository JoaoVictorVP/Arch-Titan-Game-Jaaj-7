using UnityEngine;

public class EntityTarget : MonoBehaviour
{
    public Entity Binded => binded;

    Entity binded;
    private void Awake()
    {
        binded = GetComponentInParent<Entity>();

        binded.AsBind();
    }
}