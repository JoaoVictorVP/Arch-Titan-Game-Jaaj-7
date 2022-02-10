using System;
using UnityEngine;
using UnityEngine.UIElements;

public class Market : MonoBehaviour
{
    public UIDocument Document;
    public UIDocument Alert;
    public VisualTreeAsset ItemTemplate;
    public MarketItem[] Items;

    void setupItem(MarketItem item)
    {
        var ui = ItemTemplate.Instantiate();

        ui.Q<VisualElement>("Icon").style.backgroundImage = new StyleBackground(item.Icon.texture);
        ui.Q<Label>("Name").text = item.Name;
        ui.Q<Label>("Price").text = $"MII$ {item.Price}";

        ui.AddManipulator(new Clickable(() =>
        {
            int money = Player.Money;
            if(money < item.Price)
            {
                ShowAlert($"Você não tem os MII$ {item.Price} necessários para comprar {item.Name}");
                return;
            }

            FindObjectOfType<Inventory>().AddItem(new Item { Asset = item.Asset, Count = item.Count }, new ItemAsset.Value[0]);

            money -= item.Price;

            Player.Money = money;
        }));

        Document.rootVisualElement.Q<VisualElement>("Content").Add(ui);
    }

    public void ShowAlert(string message)
    {
        UIManager.AddLock();
        Alert.gameObject.SetActive(true);
        Alert.rootVisualElement.Q<Button>("Close").clicked += () =>
        {
            UIManager.RemoveLock();
            Alert.gameObject.SetActive(false);
        };
        Alert.rootVisualElement.Q<Label>("Message").text = message;
    }

    bool open;
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.M))
        {
            open = !open;
            if (open)
                Open();
            else
                Close();
        }
    }
    void Open()
    {
        UIManager.AddLock();
        Document.rootVisualElement.Q<VisualElement>("Market").style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
    }
    void Close()
    {
        UIManager.RemoveLock();
        Document.rootVisualElement.Q<VisualElement>("Market").style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
    }

    private void Start()
    {
        foreach (var item in Items)
            setupItem(item);
    }

    [Serializable]
    public class MarketItem
    {
        public Sprite Icon => Asset.Icon;
        public string Name => Asset.Name;
        public int Price;

        public ItemAsset Asset;
        public int Count;

        //[NonSerialized]
        //public VisualElement Item;
    }
}