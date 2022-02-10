using UnityEngine;

namespace JsScripting
{
    public class ScriptEngine_ListenOnCollisionEnter : ScriptEngine_Listener
    {
        private void OnCollisionEnter(Collision collision)
        {
            Engine.Invoke(nameof(OnCollisionEnter), collision);
        }
    }
}