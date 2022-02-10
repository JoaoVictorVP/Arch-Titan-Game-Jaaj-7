using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenu : MonoBehaviour
{
    public int MainScene;
    public VisualTreeAsset MainMenuAsset;
    public VisualElement Root;

    public void Run()
    {
        void setup()
        {
            Root = UIManager.SetDocument(MainMenuAsset);

            Root.Q<Button>("Start").clicked += () => SceneManager.LoadScene(MainScene);
            Root.Q<Button>("Options").clicked += GetComponent<OptionsMenu>().Connect;
            Root.Q<Button>("Exit").clicked += Application.Quit;
        }

        OptionsMenu.BackButton += setup;

        setup();
    }

    private void Start()
    {
        QualitySettings.SetQualityLevel(OptionsMenu.Quality, true);

        Run();
    }
}
