namespace JsScripting
{
    public class ScriptEngine_ListenUpdate : ScriptEngine_Listener
    {
        private void Update()
        {
            Engine.Invoke(nameof(Update));
        }
    }
}