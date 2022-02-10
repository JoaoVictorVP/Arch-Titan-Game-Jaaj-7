using System;
using System.Collections;
using System.Threading.Tasks;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("Props")]
    public int AllAmmo = 10;
    public int Ammo;
    public int AmmoDrawnPerReload = 10;
    [Header("Ammo")]
    public LayerMask ShootMask;
    public ShootHandle ShootHandle;
    public Transform ShootExit;
    [Space(10)]
    public float ShootDamage = 50;
    [Header("Components")]
    public Animation Anim;
    [Header("Procedural")]
    public float ShootSpeed = 1.5f;
    public float ShootForce = 3;
    public AnimationCurve ShootMove = new AnimationCurve(new Keyframe(0, 1), new Keyframe(0.5f, 3), new Keyframe(1, 1));
    public ParticleSystem ShootParticles;
    public float NormalizeSpeed = 3;
    //[Tooltip("Optional")]
    //public Transform Forward;
    [Header("Sounds")]
    public AudioClip ShootClip;
    public AudioClip ReloadClip, NoAmmoClip;
    [Header("Control")]
    public bool FromPlayer;

    public event Action<Weapon> OnShot, OnReload;

    Item item;
    public void Bind(Item item)
    {
        Ammo = (int)item.Data.Get("Ammo", 0);
        AllAmmo = (int)item.Data.Get("AllAmmo", 0);
        this.item = item;
    }

    public void UpdateItemData()
    {
        item.Data.Set("Ammo", Ammo);
        item.Data.Set("AllAmmo", AllAmmo);
    }

    //Coroutine shooting;
    bool shooting;
    public void Shoot()
    {
        if (shooting)
            return;
        if(Ammo <= 0)
        {
            SoundManager.Play(NoAmmoClip, transform.position);
            return;
        }
        //shooting = StartCoroutine(shoot());
        shoot(ShootExit.forward);
    }
    public void Shoot(Vector3 direction)
    {
        if (shooting)
            return;
        if (Ammo <= 0)
        {
            SoundManager.Play(NoAmmoClip, transform.position);
            return;
        }
        //shooting = StartCoroutine(shoot());
        shoot(direction);
    }
    async void shoot(Vector3 direction)
    {
        shooting = true;

        //var pos = transform.position;
        ShootParticles.Play();
        SoundManager.Play(ShootClip, transform.position);

        //body.AddForce(- transform.forward * ShootForce);

        if(ShootSpeed > 0) StartCoroutine(recoil());

        //await Task.Delay(TimeSpan.FromSeconds(ShootParticles.totalTime));
        doShoot(direction);

        while (ShootParticles.isPlaying)
            await Task.Delay(50);

        //transform.position = pos;
        Ammo--;
        item?.Data.Set("Ammo", Ammo);
        OnShot?.Invoke(this);

        shooting = false;

        OnFinishedShooting?.Invoke();
    }

    public event Action OnFinishedShooting;

    IEnumerator recoil()
    {
        float t = 0;
        var pos = transform.localPosition;
        while (((t += Time.deltaTime) * ShootSpeed) < 1f)
        {
            transform.localPosition = Vector3.Slerp(transform.localPosition, pos - (Player.Instance.transform.forward * (ShootMove.Evaluate(t) * ShootForce)), t);
            yield return null;
        }
    }

    async void doShoot(Vector3 direction)
    {
        if (!ShootHandle || !ShootExit) return;

        //if (Physics.Raycast(new Ray(ShootExit.position, ShootExit.forward), out var hit, 1000f, ShootMask, QueryTriggerInteraction.Ignore))
        //{
        var handle = Instantiate(ShootHandle.gameObject).GetComponent<ShootHandle>();

        handle.transform.position = ShootExit.transform.position;

//handle.SendTo(hit.point, handle.Speed, ShootMask == default ? handle.Mask : ShootMask);
        handle.SendTo(direction, handle.Speed, ShootMask == default ? handle.Mask : ShootMask);

        handle.OnTouch += (h, target) =>
        {
            if (target.TryGetComponent<Entity>(out var entity))
            {
                if (entity.IsBindedByTarget)
                    return false;

#if UNITY_EDITOR
                Debug.Log($"Hit {entity}");
#endif

                entity.Damage(ShootDamage);
                Destroy(handle);
                return true;
            }
            else if (target.TryGetComponent<EntityTarget>(out var entityTarget))
            {
#if UNITY_EDITOR
                Debug.Log($"Hit target of {entityTarget.Binded}");
#endif
                entityTarget.Binded.Damage(ShootDamage);
                Destroy(handle);
                return true;
            }
            return false;
        };
        //}
    }

    /*    IEnumerator shoot()
        {
            var pos = transform.position;
            ShotParticles.Play();
            SoundManager.Play(ShootClip);
            float t = 0;
            while((t += Time.deltaTime) < 1f)
            {
                transform.position += new Vector3(transform.forward.x, transform.forward.y, transform.forward.z) * ShotMove.Evaluate(t);
                yield return null;
            }
            transform.position = pos;
            Ammo--;
            item.Data.Set("Ammo", Ammo);
            OnShot?.Invoke(this);

            shooting = null;
        }*/

    bool reloading;
    public async void Reload()
    {
        if (reloading || shooting || AllAmmo <= 0 || Ammo >= AmmoDrawnPerReload)
            return;

        reloading = true;

        Anim.Play();
        SoundManager.Play(ReloadClip);
        await Task.Delay(TimeSpan.FromSeconds(Anim.clip.length));
        int reload = Mathf.Min(AmmoDrawnPerReload, AllAmmo);
        AllAmmo = Mathf.Max(AllAmmo - AmmoDrawnPerReload, 0);
        Ammo += reload;

        item?.Data.Set("Ammo", Ammo);
        item?.Data.Set("AllAmmo", AllAmmo);

        OnReload?.Invoke(this);

        reloading = false;
    }

    private void Update()
    {
        if (!FromPlayer)
            return;

        if (Input.GetMouseButtonDown(0))
            Shoot();
        if (Input.GetKeyDown(KeyCode.R))
            Reload();

        //if(!shooting)
        transform.localPosition = Vector3.Slerp(transform.localPosition, position, Time.deltaTime * NormalizeSpeed);
    }

    /*    Rigidbody body;*/
    Vector3 position;
    private void Awake()
    {
        position = transform.localPosition;
        //body = GetComponent<Rigidbody>();
    }
}
