using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{


    public void NewGame()
    {
        DataPersistenceManager.Instance.NewGame();
    }

    public void ContinueGame()
    {
        DataPersistenceManager.Instance.LoadGame();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
