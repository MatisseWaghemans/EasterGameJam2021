using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : Singleton<UIManager>
{
    [Header("Sub-Behaviours")]
    public UIMenuBehaviour uiMenuBehaviour;

    public GameObject StartGame;

    public void SetupManager()
    {
        uiMenuBehaviour.SetupBehaviour();
    }

    public void UpdateUIMenuState(bool newState)
    {
        uiMenuBehaviour.UpdateUIMenuState(newState);
    }

    public void MenuButtonOptionSelected(int newPanelID)
    {
        uiMenuBehaviour.SwitchUIContextPanels(newPanelID);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void Restart()
    {
        GameManager.Instance.Restart();
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

}
