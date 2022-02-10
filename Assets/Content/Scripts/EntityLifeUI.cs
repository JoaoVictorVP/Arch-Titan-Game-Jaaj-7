using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.EventSystems.EventTrigger;

public class EntityLifeUI : MonoBehaviour
{
    public static EntityLifeUI Instance => instance;
    static EntityLifeUI instance;

    public UIDocument Document;
    VisualElement root;

    public void Show() => Document.rootVisualElement.ElementAt(0).style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex); //Document.enabled = true;
    public void Hide() => Document.rootVisualElement.ElementAt(0).style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None); //Document.enabled = false;

    Entity currentBind;
    public void Bind(Entity entity)
    {
        currentBind = entity;

        root.Q<Label>("Name").text = entity.Name;

        var lifeBar = root.Q<ProgressBar>("Life");
        lifeBar.highValue = entity.InitialLife;
        lifeBar.value = entity.Life;

        entity.OnDamage -= onDamage;
        entity.OnDamage += onDamage;
    }
    void onDamage(float ammount)
    {
        var lifeBar = root.Q<ProgressBar>("Life");
        lifeBar.highValue = currentBind.InitialLife;
        lifeBar.value = currentBind.Life;
    }

    private void Awake()
    {
        instance = this;
        root = Document.rootVisualElement;
    }
}