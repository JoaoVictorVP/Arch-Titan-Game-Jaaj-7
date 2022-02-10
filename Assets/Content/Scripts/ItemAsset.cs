using System;
using UnityEngine;
// using static UnityEditor.Progress;

[CreateAssetMenu(menuName = "Inventory/Item Asset")]
public class ItemAsset : ScriptableObject
{
    public string Name;
    public Sprite Icon;
    public ItemType Type;
    public TextAsset Script;
    //public Unity.VisualScripting.ScriptGraphAsset Script;
    public Value[] Values;
    [Header("Sound")]
    public AudioClip UseClip;

    public bool CanStack()
    {
        if(Script != null)
        {
            var scr = ItemDatabase.Machines[Script];
            if(scr != null)
            {
                if (scr.Has("CanStack"))
                    return (bool)scr.Invoke("CanStack");
            }
        }
        return false;
    }

    public void DoStack(Item original, Item toStack)
    {
        if (Script != null)
        {
            var scr = ItemDatabase.Machines[Script];
            if (scr != null)
            {
                if (scr.Has("Stack"))
                     scr.Invoke("Stack", original, toStack);
            }
        }
    }

    public void Setup(Item item)
    {
        foreach(var value in Values)
        {
            object f = value.fValue;
            if (f != null)
                item.Data.Set(value.Key, f);
        }
    }

    public enum ItemType
    {
        None,
        Consumable,
        Equippable,
        General
    }

    [Serializable]
    public struct Value
    {
        public string Key;
        public VType Type;
        public object fValue
        {
            get
            {
                switch (Type)
                {
                    case VType.Integer: return ValueInt;
                    case VType.Float: return ValueFloat;
                    case VType.Boolean: return ValueBool;
                    case VType.String: return ValueString;
                    case VType.ItemAsset: return ValueItemAsset;
                }
                return null;
            }
        }

        public int ValueInt;
        public float ValueFloat;
        public bool ValueBool;
        public string ValueString;
        public ItemAsset ValueItemAsset;

        public enum VType
        {
            None,
            Integer,
            Float,
            Boolean,
            String,
            ItemAsset
        }
    }
}
