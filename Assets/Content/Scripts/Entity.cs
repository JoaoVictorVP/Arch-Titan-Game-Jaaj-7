using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.VFX;

public class Entity : MonoBehaviour
{
    public string Name;
    public float InitialLife => initialLife;
    public float Life;
    [Tooltip("Only apply to renderers with material that applies Damage Shader Pass")]
    public MeshRenderer[] Renderers;
    public bool IsBindedByTarget => _binded;

    bool _binded;
    public void AsBind() => _binded = true;

    public event System.Action<float> OnDamage;

    public UnityEvent UOnDie;
    public event System.Action OnDie;

    public VisualEffect DieEffect;

    public int MoneyOnDeath;

    public virtual async void Damage(float ammount)
    {
        Life -= ammount;
        OnDamage?.Invoke(ammount);
        if (Life < 0)
        {
            Die();

            return;
        }

        if (Renderers.Length == 0 || ammount < 0)
            return;

        StartCoroutine(toWeight(0, 1));
        await Task.Delay(1000);
        StartCoroutine(toWeight(1, 0));
    }
    IEnumerator toWeight(float from, float to)
    {
        int weightID = Shader.PropertyToID("_DamageWeight");

        float t = 0;
        while((t += Time.deltaTime) < 1f)
        {
            foreach (var renderer in Renderers)
                renderer.material.SetFloat(weightID, Mathf.Lerp(from, to, t));
            yield return null;
        }
    }
    public virtual void Die()
    {
        UOnDie?.Invoke();

        OnDie?.Invoke();

        if (MoneyOnDeath != 0)
            Player.Money += MoneyOnDeath;

        doDieEffect();
    }
    bool doingDieEffect;
    async void doDieEffect()
    {
        if (doingDieEffect)
            return;
        doingDieEffect = true;

        var pos = transform.position;

        Destroy(gameObject);

        if (DieEffect)
            return;

        var efx = Instantiate(DieEffect.gameObject).GetComponent<VisualEffect>();

        efx.transform.position = pos;

        efx.Play();

        await Task.Delay(3000);

        Destroy(efx.gameObject);
    }

    float initialLife;
    private void Awake()
    {
        AutoShowEntityLifeUI.Entities.Add(this);

        initialLife = Life;

        if (string.IsNullOrEmpty(Name))
            Name = name;
    }

    private void OnDestroy()
    {
        AutoShowEntityLifeUI.Entities.Remove(this);
    }
}
