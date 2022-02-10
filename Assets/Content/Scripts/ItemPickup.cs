using UnityEngine;

public class ItemPickup : MonoBehaviour, IMousePointable
{
    static GameObject pickupUI;

    public ItemAsset Item;
    public int Count = 1;
    public ItemAsset.Value[] Values;

    bool locked;
    public void WhileMouseIn()
    {
        if (locked)
            return;

        if(Input.GetKeyDown(KeyCode.E))
        {
            locked = true;

            FindObjectOfType<Inventory>().AddItem(new Item { Asset = Item, Count = Count }, Values);

            OnPointerExit();

            Destroy(gameObject);
        }
    }

    private void Awake()
    {
        if (pickupUI == null)
            pickupUI = GameObject.Find("Help-Canvas").transform.Find("PickUp UI").gameObject;
    }

    public void OnPointerEnter()
    {
        pickupUI.SetActive(true);
    }

    public void OnPointerExit()
    {
        pickupUI.SetActive(false);
    }
}