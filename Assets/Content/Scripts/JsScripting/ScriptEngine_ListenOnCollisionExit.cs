using UnityEngine;

namespace JsScripting
{
    public class ScriptEngine_ListenOnCollisionExit : ScriptEngine_Listener
    {
        private void OnCollisionExit(Collision collision)
        {
            Engine.Invoke(nameof(OnCollisionExit), collision);
        }
    }
}