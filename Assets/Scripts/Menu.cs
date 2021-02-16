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

    public TMP_InputField join_input;
    public NetworkManagerCustomMatch manager;

    public void OnHostGame()
    {
        manager.StartHost();
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
}
