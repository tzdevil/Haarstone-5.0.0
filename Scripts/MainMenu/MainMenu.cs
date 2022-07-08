using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name == "MainMenu_Mobile" ? "GameScene_Mobile" : "GameScene_PC");
    }

    public void GoToCollection()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name == "MainMenu_Mobile" ? "Collection_Mobile" : "Collection_PC");
    }
}
