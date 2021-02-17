using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Mirror;

public class Menu : MonoBehaviour
{
    public Camera cam;
    public GameObject panel_principale;
    public GameObject panel_join;
    public GameObject paused_menu;
    public GameObject menu;

    public TMP_InputField join_input;
    public NetworkManagerCustomMatch manager;

    public static bool isPaused;

    private void Start()
    {
        isPaused = false;
    }

    public void OnHostGame()
    {
        manager.StartHost();
    }

    public void HideMenu()
    {
        menu.SetActive(false);
    }

    public void ShowPauseMenu()
    {
        paused_menu.SetActive(true);
        isPaused = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void HidePauseMenu()
    {
        paused_menu.SetActive(false);
        isPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void OnExit()
    {
        Application.Quit();
    }

    public void OnJoinGame()
    {
        panel_principale.SetActive(false);
        panel_join.SetActive(true);
    }

    public void OnReturn()
    {
        join_input.text = "";
        panel_join.SetActive(false);
        panel_principale.SetActive(true);
    }

    public void OnJoinIP()
    {
        manager.StartClient();
        manager.networkAddress = join_input.text;
    }

    public void OnResume()
    {
        paused_menu.SetActive(false);
        NetworkPlayerController[] players = FindObjectsOfType<NetworkPlayerController>();
        foreach(NetworkPlayerController p in players)
        {
            if (p.isLocalPlayer)
                p.ResumePause();
        }
    }

    public void OnQuitPaused()
    {
        paused_menu.SetActive(false);
        menu.SetActive(true);
    }
}
