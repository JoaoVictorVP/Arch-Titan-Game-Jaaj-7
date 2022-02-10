namespace JsScripting
{
    public class ScriptEngine_ListenStart : ScriptEngine_Listener
    {
        private void Start()
        {
            Engine.Invoke(nameof(Start));
        }
    }
}