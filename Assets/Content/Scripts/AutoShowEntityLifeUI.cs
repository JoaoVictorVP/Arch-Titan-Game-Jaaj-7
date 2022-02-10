using System.Collections.Generic;
using UnityEngine;

public class AutoShowEntityLifeUI : MonoBehaviour
{
    public static List<Entity> Entities = new List<Entity>(32);

    public float MaxDistance = 30;

    private void Update()
    {
        // const string playerTag = "Player";

        var pos = Player.Instance.transform.position;

        Entity mostProx = null;
        float minDist = MaxDistance;
        foreach(var entity in Entities)
        {
            if (entity == Player.Instance)
                continue;

            float dist = Vector3.Distance(pos, entity.transform.position);
            if(dist < minDist)
            {
                mostProx = entity;
                minDist = dist;
            }
        }

        if(mostProx == null)
            EntityLifeUI.Instance.Hide();
        else
        {
            var lifeUI = EntityLifeUI.Instance;
            lifeUI.Bind(mostProx);
            lifeUI.Show();
        }
    }
}