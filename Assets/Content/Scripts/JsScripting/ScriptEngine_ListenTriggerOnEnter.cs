using UnityEngine;

namespace JsScripting
{
    public class ScriptEngine_ListenTriggerOnEnter : ScriptEngine_Listener
    {
        private void OnTriggerEnter(Collider other)
        {
            Engine.Invoke(nameof(OnTriggerEnter), other);
        }
    }
}