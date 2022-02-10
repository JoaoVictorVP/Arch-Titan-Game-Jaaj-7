using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class Player : Entity
{
    public static int Money
    {
        get => PlayerPrefs.GetInt("Money", 0);
        set
        {
            PlayerPrefs.SetInt("Money", value);
            if(Instance)
            Instance.moneyLabel.text = $"MII$ {value}";
        }
    }

    public static Player Instance { get; private set; }
    public UIDocument HUD;
    public List<WeaponHandle> Handles;

    public UIDocument MenuUI, OptionsUI, LooseUI, MarketUI;

    public Weapon CurrentWeapon => curWeapon;
    Label moneyLabel;
    ProgressBar lifeBar;
    VisualElement weaponUI;
    Item equippedWeapon;
    Weapon curWeapon;

    public bool IsEquippingWeapon => equippedWeapon != null;
    public bool IsEquippedWith(ItemAsset weapon) => equippedWeapon?.Asset == weapon;
    public bool IsEquippingSpecificWeapon(string assetName) => equippedWeapon?.Asset?.name == assetName;

    [Serializable]
    public struct WeaponHandle
    {
        public ItemAsset Item;
        public GameObject Handle; 
    }

    public void Equip(Item item)
    {
        if (died) return;
        if(equippedWeapon != null)
        {
            bool same = equippedWeapon == item;

            curWeapon.gameObject.SetActive(false);

            weaponUI.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);

            equippedWeapon = null;

            curWeapon = null;

            //Handles.Find(h => h.Item == equipp.Asset).Handle.SetActive(false);

            if(same)
                return;
        }

        weaponUI.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);

        weaponUI.Q<VisualElement>("Icon").style.backgroundImage = new StyleBackground(item.Asset.Icon.texture);
        weaponUI.Q<Label>("Weapon-Name").text = item.Asset.Name;
        //weaponUI.Q<Label>("Ammo").text = item.Data.Get("Ammo", 0)?.ToString();

        equippedWeapon = item;

        var handle = Handles.Find(h => h.Item == item.Asset).Handle;
        var weapon = handle.GetComponent<Weapon>();
        weapon.Ammo = (int)item.Data.Get("Ammo", 0);
        weapon.AllAmmo = (int)item.Data.Get("AllAmmo", 0);

        weapon.OnShot -= UpdateWeaponUI;
        weapon.OnShot += UpdateWeaponUI;
        weapon.OnReload -= UpdateWeaponUI;
        weapon.OnReload += UpdateWeaponUI;

        weapon.Bind(item);

        curWeapon = weapon;

        handle.SetActive(true);

        UpdateWeaponUI(weapon);
    }

    public void UpdateWeaponUI(Weapon weapon)
    {
        if (died) return;
        weaponUI.Q<Label>("Ammo").text = $"{weapon.Ammo}/{weapon.AllAmmo}";
    }

    bool died;
    public override void Die()
    {
        died = true;

        UIManager.AddLock();

        LooseUI.gameObject.SetActive(true);

        LooseUI.rootVisualElement.Q<Button>("Retry").clicked += () =>
        {
            var cur = SceneManager.GetActiveScene();
            SceneManager.LoadScene(cur.buildIndex, LoadSceneMode.Single);
            //SceneManager.LoadScene(0, LoadSceneMode.Single);
        };

         LooseUI.rootVisualElement.Q<Button>("Exit").clicked += () => SceneManager.LoadScene(0, LoadSceneMode.Single);
        //LooseUI.rootVisualElement.Q<Button>("Exit").clicked += () => Application.Quit();
    }

    public override void Damage(float ammount)
    {
        if (died) return;
        lifeBar.value = (Life - ammount);
        base.Damage(ammount);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Menu();
    }

    bool menu;
    void Menu()
    {
        menu = !menu;
        if (menu)
            OpenMenu();
        else
            CloseMenu();
    }

    void optionsClicked()
    {
        OptionsUI.gameObject.SetActive(true);
        UIManager.AddLock();
        OptionsMenu.BackButton += () =>
        {
            OptionsUI.gameObject.SetActive(false);
            UIManager.RemoveLock();
        };
        OptionsMenu.Setup(OptionsUI);
    }
    void exitClicked() => SceneManager.LoadScene(0, LoadSceneMode.Single);

    void OpenMenu()
    {
        UIManager.AddLock();

        MenuUI.gameObject.SetActive(true);

        MenuUI.rootVisualElement.Q<Button>("Continue").clicked += CloseMenu;

        MenuUI.rootVisualElement.Q<Button>("Options").clicked += optionsClicked;

        MenuUI.rootVisualElement.Q<Button>("Exit").clicked += exitClicked;
    }
    void CloseMenu()
    {
        UIManager.RemoveLock();

        MenuUI.gameObject.SetActive(false);

/*        MenuUI.rootVisualElement.Q<Button>("Continue").clicked -= CloseMenu;

        MenuUI.rootVisualElement.Q<Button>("Options").clicked -= optionsClicked;

        MenuUI.rootVisualElement.Q<Button>("Exit").clicked -= exitClicked;*/
    }

    public void AddMoney(int money)
    {
        Money += money;
        moneyLabel.text = $"MII$ {Money}";
    }

    private void Start()
    {
        var root = HUD.rootVisualElement;
        lifeBar = root.Q<ProgressBar>("Life");
        weaponUI = root.Q<VisualElement>("Weapon");
        lifeBar.value = Life;

        moneyLabel = root.Q<Label>("Money");
        moneyLabel.text = $"MII$ {Money}";

        Instance = this;

        lifeBar.Q<VisualElement>("unity-progress-bar").Query().OfType<VisualElement>().OfType<VisualElement>().AtIndex(0).style.color = new StyleColor(Color.green);
    }
}
