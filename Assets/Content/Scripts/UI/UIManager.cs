using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    static SFPSC_FPSCamera fpsCam => Player.Instance?.GetComponentInChildren<SFPSC_FPSCamera>();
    int locks;

    public static void AddLock()
    {
        manager.locks++;

        //if(fpsCam)
        //    fpsCam = FindObjectOfType<SFPSC_FPSCamera>();

        fpsCam.enabled = false;
        UnityEngine.Cursor.visible = true;
        UnityEngine.Cursor.lockState = CursorLockMode.None;
    }

    public static void RemoveLock()
    {
        manager.locks--;
#if UNITY_EDITOR
        Debug.Log(manager.locks);
#endif
        if (manager.locks < 0) manager.locks = 0;

        if(manager.locks == 0)
        {
            fpsCam.enabled = true;
            UnityEngine.Cursor.visible = false;
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        }
    }

    /*    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void init()
        {
            manager.locks = 0;
            //fpsCam = FindObjectOfType<SFPSC_FPSCamera>();
        }*/

    static UIManager _m;
    static UIManager manager
    {
        get
        {
            if (!_m)
                _m = FindObjectOfType<UIManager>();
            return _m;
        }
    }
    public UIDocument Document;

    public static VisualElement SetDocument(VisualTreeAsset rootAsset)
    {
        manager.Document.visualTreeAsset = rootAsset;

        return manager.Document.rootVisualElement;
    }

    private void Awake()
    {
        //manager = this;
        //fpsCam = FindObjectOfType<SFPSC_FPSCamera>();
        manager.locks = 0;
    }
    private void Start()
    {
        if (!fpsCam) return;
        fpsCam.enabled = true;
        UnityEngine.Cursor.visible = false;
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
    }
}
