using UnityEngine;

namespace JsScripting
{
    public class ScriptEngine_ListenTriggerOnExit : ScriptEngine_Listener
    {
        private void OnTriggerExit(Collider other)
        {
            Engine.Invoke(nameof(OnTriggerExit), other);
        }
    }
}