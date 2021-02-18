using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DataManager : NetworkBehaviour
{
    [HideInInspector] [SyncVar (hook = nameof(RpcUpdateHud_P1Health))] public int p1Health = -1;
    [HideInInspector] [SyncVar (hook = nameof(RpcUpdateHud_P2Health))] public int p2Health = -1;
    [HideInInspector] [SyncVar (hook = nameof(RpcUpdateHud_P1Score))] public int p1Score = -1;
    [HideInInspector] [SyncVar (hook = nameof(RpcUpdateHud_P2Score))] public int p2Score = -1;

    [HideInInspector] [SyncVar] public float roundCountdown;
    [HideInInspector] [SyncVar (hook = nameof(RpcUpdateCountdown))] public int roundCountdownInt;

    public TextMeshProUGUI p1OrangeHealthText;
    public TextMeshProUGUI p2OrangeHealthText;
    public TextMeshProUGUI p1BlueHealthText;
    public TextMeshProUGUI p2BlueHealthText;

    public TextMeshProUGUI p1OrangeScoreText;
    public TextMeshProUGUI p2OrangeScoreText;
    public TextMeshProUGUI p1BlueScoreText;
    public TextMeshProUGUI p2BlueScoreText;

    [ClientRpc]
    public void RpcUpdateHud_P1Health(int oldValue, int newValue) {
        //GameObject p1s = GameObject.Find("P1Health");
        //Debug.Log(p1s);
        //p1s.GetComponent<Text>().text = newValue.ToString();
        p1OrangeHealthText.text = newValue.ToString();
        p1BlueHealthText.text = newValue.ToString();
    }

    [ClientRpc]
    public void RpcUpdateHud_P2Health(int oldValue, int newValue) {
        //GameObject p2s = GameObject.Find("P2Health");
        //Debug.Log(p2s);
        //p2s.GetComponent<Text>().text = newValue.ToString();
        p2OrangeHealthText.text = newValue.ToString();
        p2BlueHealthText.text = newValue.ToString();
    }

    [ClientRpc]
    public void RpcUpdateHud_P1Score(int oldValue, int newValue) {
        //GameObject p1s = GameObject.Find("P1Score");
        //Debug.Log(p1s);
        //p1s.GetComponent<Text>().text = newValue.ToString();
        p1OrangeScoreText.text = newValue.ToString();
        p1BlueScoreText.text = newValue.ToString();
    }

    [ClientRpc]
    public void RpcUpdateHud_P2Score(int oldValue, int newValue) {
        //GameObject p2s = GameObject.Find("P2Score");
        //Debug.Log(p2s);
        //p2s.GetComponent<Text>().text = newValue.ToString();
        p2OrangeScoreText.text = newValue.ToString();
        p2BlueScoreText.text = newValue.ToString();
    }

    [ClientRpc]
    public void RpcUpdateCountdown(int oldValue, int newValue) {
        GameObject p2s = GameObject.Find("Countdown");
        Debug.Log(p2s);
        p2s.GetComponent<TextMeshProUGUI>().text = newValue > 0 ? (newValue).ToString() : "";
        //p2OrangeScoreText.text = newValue.ToString();
        //p2BlueScoreText.text = newValue.ToString();
        switch (newValue)
        {
            case 3:
            GetComponents<AudioSource>()[0].Play();
            break;
            case 2:
            GetComponents<AudioSource>()[1].Play();
            break;
            case 1:
            GetComponents<AudioSource>()[2].Play();
            break;
            default:
            break;
        }
    }
    
    public void ResetHealth() {
        if(isServer) {
            Debug.Log("Reset Health");
            p1Health = 3;
            p2Health = 3;
            roundCountdownInt = 4;
            roundCountdown = 4.0f;
        } else {
            CmdResetHealth();
        }
    }

    [Command]
    public void CmdResetHealth() {
        ResetHealth();
    }

    public void InitMatchData() {
        if(isServer) {
            Debug.Log("Init Match");
            p1Health = 3;
            p2Health = 3;
            p1Score = 1;
            p1Score--;
            p2Score = 1;
            p2Score--;
            roundCountdownInt = 4;
            roundCountdown = 4.0f;
        } else {
            CmdInitMatchData();
        }
    }

    [Command]
    public void CmdInitMatchData() {
        InitMatchData();
    }

    public void RemoveP1Health() {
        if(isServer) {
            Debug.Log("Remove P1 Health");
            p1Health--;
        } else {
            CmdRemoveP1Health();
        }
    }

    [Command]
    public void CmdRemoveP1Health() {
        RemoveP1Health();
    }

    public void RemoveP2Health() {
        if(isServer) {
            Debug.Log("Remove P2 Health");
            p2Health--;
        } else {
            CmdRemoveP2Health();
        }
    }

    [Command]
    public void CmdRemoveP2Health() {
        RemoveP2Health();
    }

    public void AddP1Score () {
        if(isServer) {
            Debug.Log("Add P1 Score");
            p1Score++;
        } else {
            CmdAddP1Score();
        }
    }

    [Command]
    public void CmdAddP1Score() {
        AddP1Score();
    }

    public void AddP2Score () {
        if(isServer) {
            Debug.Log("Add P2 Score");
            p2Score++;
        } else {
            CmdAddP2Score();
        }
    }

    [Command]
    public void CmdAddP2Score() {
        AddP2Score();
    }

    public void UpdateCountdown () {
        if(isServer) {
            Debug.Log("UpdateCountdown");
            roundCountdownInt--;
        } else {
            CmdAddP2Score();
        }
    }

    [Command]
    public void CmdUpdateCountdown () {
        UpdateCountdown();
    }

    public void Update() {
        if(isServer) {
            if(roundCountdown > 0.0f)
                roundCountdown -= Time.deltaTime;

            if(roundCountdownInt > (int)roundCountdown) {
                UpdateCountdown();
            }
        }
    }
}
