using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Mirror;

public class Menu : MonoBehaviour
{
    public GameObject cam;
    public GameObject panel_principale;
    public GameObject panel_join;
    public GameObject paused_menu;
    public GameObject menu;
    public GameObject panel_credits;

    public TMP_InputField join_input;
    public NetworkManagerCustomMatch manager;

    private Transform cam_transform;

    public static bool isPaused;

    private void Start()
    {
        isPaused = false;
        cam_transform = cam.transform;
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
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            manager.StopHost();

        }
        else if (NetworkClient.isConnected)
        {
            manager.StopClient();
        }
        else if (NetworkServer.active)
        {
            manager.StopServer();
        }

        //cam.transform.position = cam_transform.position;
        //cam.transform.rotation = cam_transform.rotation;

        //cam.GetComponent<Cinemachine.CinemachineFreeLook>().m_XAxis.m_InputAxisName = "";
        //cam.GetComponent<Cinemachine.CinemachineFreeLook>().m_YAxis.m_InputAxisName = "";

        //isPaused = false;

        //paused_menu.SetActive(false);
        //menu.SetActive(true);
        SceneManager.LoadScene(0);
    }

    public void OnCredits()
    {
        panel_principale.SetActive(false);
        panel_credits.SetActive(true);
    }

    public void OnReturnCredits()
    {
        panel_credits.SetActive(false);
        panel_principale.SetActive(true);
    }
}
