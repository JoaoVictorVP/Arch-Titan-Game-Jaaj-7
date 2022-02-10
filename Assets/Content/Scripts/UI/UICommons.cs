using UnityEngine.UIElements;

public class UICommons
{
    public static object Query<T>(VisualElement element, string name) where T : VisualElement => element.Q<T>(name);
}