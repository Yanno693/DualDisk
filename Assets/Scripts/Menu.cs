using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        //cam.GetComponent<Cinemachine.CinemachineFreeLook>().enabled = false;
    }

    public void OnExit()
    {
        Application.Quit();
    }
}
