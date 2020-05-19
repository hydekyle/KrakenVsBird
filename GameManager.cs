using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public BirdScriptable birds_table;
    public GameObject prefab_bird_player;
    public List<BirdPlayer> birdPlayers = new List<BirdPlayer>();
    public KrakenPlayer krakenPlayer;
    public Transform krakenCenter;
    public Transform player1Transform, player2Transform, player3Transform, player4Transform;

    //Db db = new Db();

    private void Awake()
    {
        Instance = Instance ?? this;
    }

    private void Update()
    {
        foreach (BirdPlayer bird in birdPlayers) bird.Update();
        krakenPlayer?.Update();
        InputResolver();
    }

    private void InputResolver()
    {
        if (ImKrakenPlayer())
        {
            if (!Input.GetKey(KeyCode.Mouse0) && !Input.GetKey(KeyCode.Mouse1)) SendAction(PlayerAction.None);
            if (Input.GetKey(KeyCode.Mouse0) && Input.GetKey(KeyCode.Mouse1)) SendAction(PlayerAction.Both);
            if (Input.GetKeyUp(KeyCode.Mouse0) && Input.GetKey(KeyCode.Mouse1)) SendAction(PlayerAction.MoveRight);
            if (Input.GetKeyUp(KeyCode.Mouse1) && Input.GetKey(KeyCode.Mouse0)) SendAction(PlayerAction.MoveLeft);
        }

        if (Input.GetKeyDown(KeyCode.Mouse0)) SendAction(PlayerAction.MoveLeft);
        if (Input.GetKeyDown(KeyCode.Mouse1)) SendAction(PlayerAction.MoveRight);


        if (Input.GetKeyDown(KeyCode.R)) StartMultiplayer();
        if (Input.GetKeyDown(KeyCode.P)) Cheats();
    }

    public void StartMultiplayer()
    {
        if (PhotonNetwork.PlayerList.Length > 1)
        {
            Debug.Log("Empezamos");
            StartRound();
        }
        else
        {
            Debug.Log("Get a friend");
        }
    }

    bool ImKrakenPlayer()
    {
        return krakenPlayer?.actorNumber == PhotonNetwork.LocalPlayer.ActorNumber;
    }

    public float lastTimeAction = 0f;
    float actionCD = 0.3f;
    void SendAction(PlayerAction action)
    {
        if (Time.time < lastTimeAction + actionCD) return;
        lastTimeAction = Time.time;
        ActionData playerData = new ActionData();
        playerData.action = action;
        playerData.actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
        playerData.actionTime = PhotonNetwork.ServerTimestamp + PhotonNetwork.GetPing() * 2;

        if (ImKrakenPlayer())
        {
            playerData.pos_angle = krakenPlayer.rotation;
            playerData.pos_height = krakenPlayer.height;
            NetworkController.Instance.CallRPC("KrakenAction", playerData);
        }
        else
        {
            BirdPlayer birdPlayer = GetMyBirdPlayer();
            if (birdPlayer != null)
            {
                playerData.pos_angle = birdPlayer.rotationPoint.eulerAngles.y;
                playerData.pos_height = birdPlayer.birdTransform.localPosition.y;
                NetworkController.Instance.CallRPC("BirdAction", playerData);
            }
        }
    }

    public bool IsKrakenPlayer(int actorNumber)
    {
        return actorNumber == krakenPlayer.actorNumber;
    }

    public void StartRound()
    {
        RoundCall roundCall = new RoundCall();
        roundCall.playersToCall = NetworkController.Instance.GetPlayerNames();
        roundCall.timeStamp = PhotonNetwork.Time;
        NetworkController.Instance.CallRPC("CallPlayersToGame", roundCall);
    }

    public BirdPlayer GetMyBirdPlayer()
    {
        return birdPlayers.Find(bird => bird.actorNumber == PhotonNetwork.LocalPlayer.ActorNumber);
    }

    public void AddKrakenPlayer(KrakenPlayer krakenPlayer)
    {
        this.krakenPlayer = krakenPlayer;
    }

    public KrakenPlayer GenerateKrakenPlayer(Player player)
    {
        return new KrakenPlayer(player.NickName, player.ActorNumber, krakenCenter, cheatStats);
    }

    public void AddBirdPlayer(BirdPlayer birdPlayer)
    {
        GetBirdTransform(birdPlayer.actorNumber).GetComponent<SpriteRenderer>().sprite = birdPlayer.sprites[0];
        birdPlayers.Add(birdPlayer);
        print("PlayerBird Added!");
    }

    public Transform GetBirdTransform(int actorNumber)
    {
        Transform birdTransform;
        switch (actorNumber - 1)
        {
            case 1: birdTransform = player1Transform.GetChild(0); break;
            case 2: birdTransform = player2Transform.GetChild(0); break;
            case 3: birdTransform = player3Transform.GetChild(0); break;
            default: birdTransform = player4Transform.GetChild(0); break;
        }
        return birdTransform;
    }

    public Transform GetBirdRotationTransform(int actorNumber)
    {
        Transform birdTransform;
        switch (actorNumber - 1)
        {
            case 1: birdTransform = player1Transform; break;
            case 2: birdTransform = player2Transform; break;
            case 3: birdTransform = player3Transform; break;
            default: birdTransform = player4Transform; break;
        }
        return birdTransform;
    }

    public BirdPlayer GenerateBirdPlayer(Player player)
    {
        int playerNumber = player.ActorNumber;
        Bird selectedBird = birds_table.birds.Find(bird => bird.name == "Dracula");
        Transform rotationTransform = GetBirdRotationTransform(playerNumber);
        Transform birdTransform = GetBirdTransform(playerNumber);
        SpriteRenderer spriteRenderer = birdTransform.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = selectedBird.sprites[0];

        BirdPlayer birdPlayer = new BirdPlayer(
            player.NickName,
            player.ActorNumber,
            selectedBird.stats,
            rotationTransform,
            birdTransform,
            selectedBird.sprites
            );

        return birdPlayer;
    }

    private Bird GetRandomBird()
    {
        Rarity rarity;
        int chance = UnityEngine.Random.Range(0, 100);
        if (chance > 90) rarity = Rarity.epic;
        else if (chance > 60) rarity = Rarity.rare;
        else rarity = Rarity.common;

        List<Bird> birds = birds_table.birds.FindAll(bird => bird.rarity == rarity);
        return birds[UnityEngine.Random.Range(0, birds.Count)];
    }

    public void RemovePlayer(Player player)
    {
        BirdPlayer birdPlayer = birdPlayers.Find(bird => bird.actorNumber == player.ActorNumber);
        birdPlayer.birdTransform.GetComponent<SpriteRenderer>().sprite = null;
        birdPlayers.Remove(birdPlayer);
    }

    public Stats cheatStats;
    void Cheats()
    {
        birdPlayers[0].stats = cheatStats;
    }

    public void NewPlayerEntered(Player player)
    {
        if (player.ActorNumber < 2) AddBirdPlayer(GenerateBirdPlayer(player));
        else AddKrakenPlayer(GenerateKrakenPlayer(player));
    }

    // private async Task Testing()
    // {
    //     await TestDB();
    // }

    // private async Task TestDB()
    // {
    //     var data = await db.DameDatosAsync();
    //     print("Toma datos: " + data);
    // }

    // List<string> GetBirdNameList()
    // {
    //     List<string> value = new List<string>();
    //     string path = "Assets/Resources/Birds";
    //     DirectoryInfo dir = new DirectoryInfo(path);

    //     foreach (DirectoryInfo f in dir.EnumerateDirectories())
    //     {
    //         value.Add(f.Name);
    //     }

    //     return value;
    // }
}




