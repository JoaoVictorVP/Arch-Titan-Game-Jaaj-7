using Jint;
using Jint.Native;
using UnityEngine;

public class ScriptEngine
{
    static Options options;
    static ScriptEngine()
    {
        options = new Options();
        options.AllowClr(typeof(ScriptEngine).Assembly, typeof(string).Assembly, typeof(GameObject).Assembly);
    }

    public void Set(string name, object value) => engine.SetValue(name, value);

    public object Get(string name) => engine.GetValue(name).ToObject();

    public bool Has(string name)
    {
        var res = engine.GetValue(name);
        return res != JsValue.Undefined;
    }

    public object Invoke(string function, params object[] args)
    {
        try
        {
            return engine.Invoke(function, args).ToObject();
        }
        catch
        {
            return null;
        }
    }

    public void Run(string script = null) => engine.Execute(script ?? this.script);

    string script;
    Engine engine;
    public ScriptEngine(string script, bool run = false)
    {
        engine = new Engine(options);
        this.script = script;
        if(run)
            engine.Execute(script);
    }
}
