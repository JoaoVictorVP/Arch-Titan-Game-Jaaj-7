using JsScripting;
using System;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class ScriptEngineComponent : MonoBehaviour
{
    [TextArea(3, 30)]
    public string EmbedScript;
    public bool Embed;
    public TextAsset Script;

    [Header("Variables")]
    public Variables Variables;
    //public ScriptEngineVariable[] Variables;

    ScriptEngine engine;

    private async void Awake()
    {
#if UNITY_EDITOR
        try
        {
#endif
            engine = new ScriptEngine(Embed ? EmbedScript : Script.text);

            engine.Set("Self", gameObject);

            if (Variables)
                foreach (var variable in Variables.declarations)
                    engine.Set(variable.name, variable.value);

            engine.Run();
#if UNITY_EDITOR
        }
        catch(Exception exc) { Debug.LogError(exc); }
#endif

        if (engine.Has("Awake"))
            engine.Invoke("Awake");
        if (engine.Has("Start"))
            GetOrAddComponent<ScriptEngine_ListenStart>().Engine = engine;

        if (engine.Has("OnTriggerEnter")) GetOrAddComponent<ScriptEngine_ListenTriggerOnEnter>().Engine = engine;
        if (engine.Has("OnTriggerExit")) GetOrAddComponent<ScriptEngine_ListenTriggerOnExit>().Engine = engine;
        if (engine.Has("OnCollisionEnter")) GetOrAddComponent<ScriptEngine_ListenOnCollisionEnter>().Engine = engine;
        if (engine.Has("OnCollisionExit")) GetOrAddComponent<ScriptEngine_ListenOnCollisionExit>().Engine = engine;
        if (engine.Has("Update")) GetOrAddComponent<ScriptEngine_ListenUpdate>().Engine = engine;

#if UNITY_EDITOR
        if (!Embed)
        {
            if (!Script.text.Contains("### HOT RELOAD ###"))
                return;
            string path = System.IO.Path.GetFullPath(UnityEditor.AssetDatabase.GetAssetPath(Script));
            //Debug.Log(path);
            string dir = System.IO.Path.GetDirectoryName(path);
            var watcher = new System.IO.FileSystemWatcher(dir, System.IO.Path.GetFileName(path));

            watcher.NotifyFilter = System.IO.NotifyFilters.LastWrite | System.IO.NotifyFilters.Size;
            watcher.EnableRaisingEvents = true;

            //begin:
            await Task.Run(() => watcher.WaitForChanged(System.IO.WatcherChangeTypes.Changed));
            //watcher.Changed += (e, p) =>
            //{
            //    if(p.ChangeType == System.IO.WatcherChangeTypes.Changed)
            //    {
            UnityEditor.AssetDatabase.Refresh();
            //engine.Run(Script.text);

            Awake();
            //goto begin;
            //    }
            //};
        }
#endif
        //if (engine.Has("OnTriggerEnter")) this.AddComponent<ScriptEngine_ListenTriggerOnEnter>();
    }
    T GetOrAddComponent<T>() where T : Component
    {
        if (TryGetComponent<T>(out T component))
            return component;
        return gameObject.AddComponent<T>();
    }
    /*    [Serializable]
        public struct ScriptEngineVariable
        {
            public string Name;
            public VarType Type;
            public int Int;
            public float Float;
            public bool Bool;
            public string String;
            public Vector2 Vector2;
            public Vector3 Vector3;
            public UnityEngine.Object UnityObject;

            public object Value
            {
                get
                {
                    switch(Type)
                    {
                        case VarType.Bool: return Bool;
                        case VarType.Float: return Float;
                        case VarType.Int: return Int;
                        case VarType.String: return String;
                        case VarType.Vector2: return Vector2;
                        case VarType.Vector3: return Vector3;
                        case VarType.UnityObject: return UnityObject;
                    }
                    return null;
                }
            }

            public enum VarType
            {
                Int, Float, Bool, String, Vector2, Vector3, UnityObject
            }
        }*/
}