using System;
using UnityEngine;
using UnityEngine.UIElements;

public class OptionsMenu : MonoBehaviour
{
    public static event Action BackButton;
    public static int Quality => PlayerPrefs.GetInt(nameof(Quality));
    public static float Volumn => PlayerPrefs.GetFloat(nameof(Volumn));
    public VisualTreeAsset OptionsAsset;
    public VisualElement Options;

    public static event Action<float> OnVolumnChange;

    public static void Setup(UIDocument optionsDoc)
    {
        var Options = optionsDoc.rootVisualElement;

        var quality = Options.Q<DropdownField>("GraphicsQuality");
        quality.choices = new(QualitySettings.names);
        quality.RegisterValueChangedCallback(newQuality =>
        {
            int index = Array.IndexOf(QualitySettings.names, newQuality);
            PlayerPrefs.SetInt(nameof(Quality), index);
            QualitySettings.SetQualityLevel(index, true);
        });

        var volumn = Options.Q<Slider>(nameof(Volumn));
        volumn.RegisterValueChangedCallback(v =>
        {
            OnVolumnChange?.Invoke(v.newValue);
            PlayerPrefs.SetFloat(nameof(Volumn), v.newValue);
        });

        Options.Q<Button>("Back").clicked += () => BackButton?.Invoke();
    }
    public void Connect()
    {
        Options = UIManager.SetDocument(OptionsAsset);

        var quality = Options.Q<DropdownField>("GraphicsQuality");
        quality.choices = new(QualitySettings.names);
        quality.RegisterValueChangedCallback(newQuality =>
        {
            int index = Array.IndexOf(QualitySettings.names, newQuality);
            PlayerPrefs.SetInt(nameof(Quality), index);
            QualitySettings.SetQualityLevel(index, true);
        });

        var volumn = Options.Q<Slider>(nameof(Volumn));
        volumn.RegisterValueChangedCallback(v =>
        {
            OnVolumnChange?.Invoke(v.newValue);
            PlayerPrefs.SetFloat(nameof(Volumn), v.newValue);
        });

        Options.Q<Button>("Back").clicked += () => BackButton?.Invoke();
    }
}
