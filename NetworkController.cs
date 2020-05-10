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
    public void PlayerJump(string playerDataJson)
    {
        PlayerData playerData = JsonUtility.FromJson<PlayerData>(playerDataJson);
        GameManager.Instance.birdPlayers.Find(bird => bird.actorNumber == playerData.actorNumber).Jump(playerData);
    }

    // [PunRPC]
    // public void SummonPlayer(Player player)
    // {
    //     BirdPlayer birdPlayer = GameManager.Instance.GenerateBirdPlayer(player);
    //     GameManager.Instance.AddBirdPlayer(birdPlayer);
    // }

    [PunRPC]
    public void SyncPlayerList(string listJSON)
    {
        CanvasManager.Instance.ShowMessage("Sync!");
        List<BirdPlayer> list = JsonUtility.FromJson<List<BirdPlayer>>(listJSON);
        GameManager.Instance.birdPlayers = list;
    }

    public void UpdatePlayerList(List<BirdPlayer> list)
    {
        string listJSON = JsonUtility.ToJson(list);
        this.photonView.RPC("SyncPlayerList", RpcTarget.All, listJSON);
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
