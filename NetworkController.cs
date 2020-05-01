using System.Collections;
using System.Collections.Generic;
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F)) SendPlayerData();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Conectado al multiplayer: " + PhotonNetwork.CloudRegion);
    }

    public void HostGame(string room_name)
    {
        PhotonNetwork.LocalPlayer.NickName = CanvasManager.Instance.GetTextNameValue();
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = 5;
        PhotonNetwork.CreateRoom(room_name);
    }

    public void ConnectToRoom(string room_name)
    {
        PhotonNetwork.JoinRoom(room_name);
    }

    public override void OnCreatedRoom()
    {
        print("Room created!");
        GameManager.Instance.SpawnPlayer(PhotonNetwork.LocalPlayer);
    }

    public override void OnJoinedRoom()
    {
        print("Joined to a room!");

    }

    void SendPlayerData()
    {
        PlayerData playerData;
        playerData.pos_angle = 1f;
        playerData.pos_height = 2f;
        playerData.actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
        playerData.action = PlayerAction.JumpRight;
        this.photonView.RPC("PlayerJump", RpcTarget.All, JsonUtility.ToJson(playerData));
    }

    private Player GetMyselfPlayer()
    {
        return PhotonNetwork.LocalPlayer;
    }

    [PunRPC]
    public void PlayerJump(string playerDataJson)
    {
        PlayerData playerData = JsonUtility.FromJson<PlayerData>(playerDataJson);
        print("Hola " + playerData.actorNumber);
    }

    public void DameInfo()
    {
        print("Ping: " + PhotonNetwork.GetPing());
        print("Master id of room: " + PhotonNetwork.CurrentRoom.masterClientId);
        print("Players in room: " + PhotonNetwork.CountOfPlayersInRooms);
        print("Rooms numbers: " + PhotonNetwork.CountOfRooms);
        foreach (var player in PhotonNetwork.CurrentRoom.Players)
        {
            print(player.Key + ": " + player.Value);
        }
        print("Are we Master? " + PhotonNetwork.IsMasterClient);
        CanvasManager.Instance.ShowMessage("My Number: " + PhotonNetwork.LocalPlayer.ActorNumber);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        ConnectToRoom("Test"); //Si ya está creada, unirse
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        CanvasManager.Instance.ShowMessage("Bienvenido, " + newPlayer.NickName);
        GameManager.Instance.SpawnPlayer(newPlayer);
    }

}
