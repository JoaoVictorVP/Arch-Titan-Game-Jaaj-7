using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class WinManager : MonoBehaviour
{
    public UIDocument Win;

    private void Awake()
    {
        Win.rootVisualElement.Q<Button>("Finish").clicked += () => Application.Quit();
    }
}
