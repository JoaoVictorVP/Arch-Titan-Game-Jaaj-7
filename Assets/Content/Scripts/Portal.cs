using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class Portal : Entity
{
    public GameObject RefPortal;
    public GameObject[] Spawn;
    public Vector2 SpawnTime = new Vector2(15, 60);

    float curSpawn = 30;
    private void Update()
    {
        if((curSpawn -= Time.deltaTime) <= 0)
        {
            curSpawn = UnityEngine.Random.Range(SpawnTime.x, SpawnTime.y);
            DoSpawn();
        }
    }
    async void DoSpawn()
    {
        RefPortal.SetActive(true);

        await Task.Delay(3000);

        var spawn = Instantiate(Spawn[UnityEngine.Random.Range(0, Spawn.Length)]);

        spawn.transform.position = RefPortal.transform.position;

        await Task.Delay(3000);

        RefPortal.SetActive(false);
    }
    public override void Damage(float ammount)
    {
        base.Damage(ammount);
        SpawnTime *= 0.95f;
    }
    /*    IEnumerator doSmooth(Action<float> onTime, float time = 1f)
        {
            float t = time;
            while((t -= Time.deltaTime) >= 0)
            {
                onTime(t / time);
                yield return null;
            }
        }*/

    public override void Die()
    {
        Destroy(gameObject);
    }
}