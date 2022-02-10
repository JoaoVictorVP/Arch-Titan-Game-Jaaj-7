using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    public readonly static Dictionary<TextAsset, ScriptEngine> Machines = new Dictionary<TextAsset, ScriptEngine>(32);
    public ItemAsset[] Items;
    HashSet<TextAsset> scripts = new HashSet<TextAsset>(32);

    private void Awake()
    {
        foreach (var item in Items)
            if(item.Script != null) scripts.Add(item.Script);

        foreach(var script in scripts)
        {
            //var scriptObj = new GameObject(script.name);
            //scriptObj.transform.SetParent(transform);
            //var machine = scriptObj.AddComponent<ScriptMachine>();

            //machine.graph.Instantiate(script.GetReference().AsReference());
            //machine.nest.SwitchToMacro(script);

            Machines[script] = new ScriptEngine(script.text, true);
        }
    }
}
