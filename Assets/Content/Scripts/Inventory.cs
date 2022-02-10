using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Inventory : MonoBehaviour
{
    public UIDocument UI;
    public VisualTreeAsset ItemTemplate;
    VisualElement root;
    VisualElement container;

    public KeyCode OpenCloseKey = KeyCode.Tab;

    public List<Item> Items = new List<Item>(32);

    [Header("Needs")]
    public SFPSC_FPSCamera FPSCam;

    public void UpdateUI()
    {
        container.Clear();

        foreach(var item in Items)
            AddItemUI(item);
    }
    void AddItemUI(Item item)
    {
        var slot = ItemTemplate.Instantiate();
        slot.Q<VisualElement>(className: "item-icon").style.backgroundImage = new StyleBackground(item.Asset.Icon.texture);
        RefreshItemUI(item, slot);
        slot.AddManipulator(new Clickable(() =>
        {
            //(GameObject go, FlowGraph flow) scr;
            ScriptEngine scr;
            switch (item.Asset.Type)
            {
                case ItemAsset.ItemType.Consumable:
                    scr = ItemDatabase.Machines[item.Asset.Script];

                    if (!(bool)scr.Invoke("CanConsume"))
                        break;

                    scr.Invoke("Consume", item.Data.Get("Heal", 0));

                    //CustomEvent.Trigger(scr.go, "CanConsume");

                    //if (!scr.flow.variables.Get<bool>("CanRun"))
                    //    return;

                    //CustomEvent.Trigger(scr.go, "Consume", item.Data.Get("Heal", 0));
                    RemoveItem(item, 1);
                    SoundManager.Play(item.Asset.UseClip);
                    break;
                case ItemAsset.ItemType.Equippable:
                    FindObjectOfType<Player>().Equip(item);
                    break;
                case ItemAsset.ItemType.General:
                    var all = item.Data.All;
                    var array = ArrayPool<object>.New(all.Count);
                    int index = 0;
                    foreach (var item in all)
                    {
                        array[index] = item.Value;
                        index++;
                    }
                    scr = ItemDatabase.Machines[item.Asset.Script];

                    if (!(bool)scr.Invoke("CanUse", array))
                        break;

                    scr.Invoke("Use", array);

/*                    CustomEvent.Trigger(scr.go, "CanUse");

                    if (!scr.flow.variables.Get<bool>("CanRun"))
                        return;*/

                    //CustomEvent.Trigger(scr.go, "Use", array);
                    ArrayPool<object>.Free(array);
                    RemoveItem(item, 1);
                    SoundManager.Play(item.Asset.UseClip);
                    break;
            }
        }));
        item.UIData = slot;
        container.Add(slot);
    }
    void RefreshItemUI(Item item, VisualElement slot)
    {
        slot.Q<Label>(className: "item-name").text = $"<b>{item.Asset.Name}</b> <i>x{item.Count}</i>";
    }


    public void AddItem(Item item, ItemAsset.Value[] values)
    {
        if (values?.Length > 0 || item.Asset.Values?.Length > 0)
        {
            if (values?.Length > 0)
            {
                foreach (var value in values)
                {
                    var f = value.fValue;
                    if (f != null)
                        item.Data.Set(value.Key, f);
                }
            }
            else
                item.Asset.Setup(item);
        }

        if(item.Asset.CanStack())
        {
            var similar = Items.Find(i => i.Asset == item.Asset);

            if (similar != null)
            {
                similar.Asset.DoStack(similar, item);

                if(open)
                    RefreshItemUI(similar, similar.UIData);

                return;
            }
        }

        Items.Add(item);
        if (open)
            AddItemUI(item);
            //UpdateUI();
    }
    public void RemoveItem(Item item, int count)
    {
        item.Count -= count;
        if (item.Count <= 0)
        {
            Items.Remove(item);
            item.UIData.RemoveFromHierarchy();
        }
        else
            RefreshItemUI(item, item.UIData);
    }

    private void Update()
    {
        if(Input.GetKeyDown(OpenCloseKey))
        {
            bool final = !open;
            if (final)
                Open();
            else
                Close();
        }
    }

    bool open;
    public void Open()
    {
        open = true;

        UIManager.AddLock();

        //root.visible = true;
        root.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
        UpdateUI();
    }
    public void Close()
    {
        open = false;

        UIManager.RemoveLock();

        root.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
        //root.visible = false;
    }

    private void Awake()
    {
        root = UI.rootVisualElement.Q<VisualElement>("Window");
        //root.visible = false;
        container = root.Q<VisualElement>("Items");
    }
}

public class Item
{
    public ItemAsset Asset;
    ItemData data;
    public ItemData Data
    {
        get
        {
            if (data == null) data = new ItemData();
            return data;
        }
    }
    public int Count;

    public VisualElement UIData;
}
public class ItemData
{
    Dictionary<string, object> values = new Dictionary<string, object>(32);

    public Dictionary<string, object> All => values;
    public void Set(string key, object value) => values[key] = value;
    public void Remove(string key) => values.Remove(key);
    public object Get(string key, object defaultValue = null)
    {
        if(!values.ContainsKey(key))
        {
            if (defaultValue != null)
                return values[key] = defaultValue;
            return defaultValue;
        }
        return values[key];
    }
}
