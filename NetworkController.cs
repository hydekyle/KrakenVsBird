﻿using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkController : MonoBehaviourPunCallbacks
{
    public static NetworkController Instance;

    private void Awake()
    {
        Instance = Instance ?? this;
    }

    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public void HydeTest()
    {
        StartCoroutine(TestStamp());
    }

    IEnumerator TestStamp()
    {
        int firstStamp = PhotonNetwork.ServerTimestamp;
        Debug.Log("Ahora: " + firstStamp);
        yield return new WaitForSeconds(1);
        Debug.Log("Milisegundos pasados: " + (PhotonNetwork.ServerTimestamp - firstStamp));
    }

    public override void OnConnectedToMaster()
    {
        CanvasManager.Instance.ShowMessage("Conectado a: " + PhotonNetwork.CloudRegion);
    }

    public void HostGame(string room_name)
    {
        PhotonNetwork.LocalPlayer.NickName = CanvasManager.Instance.GetTextNameValue();
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = 5;
        if (PhotonNetwork.CreateRoom(room_name)) CanvasManager.Instance.DesactivateMainMenu();
    }

    public void ConnectToRoom(string room_name)
    {
        if (PhotonNetwork.JoinRoom(room_name)) CanvasManager.Instance.DesactivateMainMenu();
    }

    public void CallRPC(string methodName, object obj)
    {
        this.photonView.RPC(methodName, RpcTarget.All, JsonUtility.ToJson(obj));
    }

    private Player GetMyselfPlayer()
    {
        return PhotonNetwork.LocalPlayer;
    }

    [PunRPC]
    public void BirdAction(string playerDataJson)
    {
        ActionData actionData = JsonUtility.FromJson<ActionData>(playerDataJson);
        if (GetMyselfPlayer().ActorNumber == actionData.actorNumber) StartCoroutine("CastMyAction", actionData);
        else CastAction(actionData);
    }

    private IEnumerator CastMyAction(ActionData actionData)
    {
        float timeWait = Mathf.Abs(PhotonNetwork.ServerTimestamp - actionData.actionTime) / 1000.0f;
        yield return new WaitForSeconds(timeWait);
        CastAction(actionData);
    }

    private void CastAction(ActionData actionData)
    {
        GameManager.Instance.birdPlayers.Find(bird => bird.actorNumber == actionData.actorNumber)?.DoAction(actionData);
    }

    [PunRPC]
    public void KrakenAction(string playerDataJson)
    {
        ActionData playerData = JsonUtility.FromJson<ActionData>(playerDataJson);
        GameManager.Instance.krakenPlayer.MoveKraken(playerData);
    }

    public List<string> GetPlayerNames()
    {
        List<string> names = new List<string>();
        foreach (var player in PhotonNetwork.CurrentRoom.Players)
        {
            names.Add(player.Value.NickName);
        }
        return names;
    }

    [PunRPC]
    public void CallPlayersToGame(string roundCallJSON)
    {
        RoundCall roundCall = JsonUtility.FromJson<RoundCall>(roundCallJSON);
        foreach (var playerName in roundCall.playersToCall)
            if (playerName == PhotonNetwork.LocalPlayer.NickName) ImInGame();
    }

    public void CallForMusic()
    {
        photonView.RPC("LetsGoMusic", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber);
    }

    [PunRPC]
    private void LetsGoMusic(int actorNumber)
    {
        if (actorNumber == PhotonNetwork.LocalPlayer.ActorNumber) Invoke("StartMusic", PhotonNetwork.GetPing() * 2 / 1000);
        else StartMusic();
    }

    private void StartMusic()
    {
        AudioManager.Instance.StartMusic();
    }

    public void ImInGame()
    {
        CanvasManager.Instance.ShowMessage("IM IN GAME");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("No se ha creado el room");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        CanvasManager.Instance.ShowMessage("Bienvenido, " + newPlayer.NickName);
        GameManager.Instance.NewPlayerEntered(newPlayer);
        //if (GetMyselfPlayer().IsMasterClient) UpdatePlayerList(GameManager.Instance.birdPlayers);
    }

    public override void OnCreatedRoom()
    {
        print("Room created!");
    }

    public override void OnJoinedRoom()
    {
        CanvasManager.Instance.ShowMessage("¡Conectado a la sala!");
        foreach (Player player in PhotonNetwork.PlayerList) GameManager.Instance.NewPlayerEntered(player);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        //GameManager.Instance.RemovePlayer(otherPlayer);
    }

}
