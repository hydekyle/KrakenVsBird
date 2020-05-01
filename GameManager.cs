using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using System;
using System.IO;
using Photon.Pun;
using Photon.Realtime;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public BirdScriptable birds_table;
    public GameObject prefab_bird_player;
    public List<BirdPlayer> birdPlayers = new List<BirdPlayer>();
    public Transform player1Transform, player2Transform, player3Transform, player4Transform;

    Db db = new Db();

    private void Awake()
    {
        Instance = Instance ?? this;
    }

    private void Update()
    {
        foreach (BirdPlayer bird in birdPlayers) bird.Update();
        if (Input.GetKeyDown(KeyCode.Mouse0))
            birdPlayers.Find(bird => bird.ID == PhotonNetwork.LocalPlayer.ActorNumber)?.JumpLeft();

        if (Input.GetKeyDown(KeyCode.Mouse1))
            birdPlayers.Find(bird => bird.ID == PhotonNetwork.LocalPlayer.ActorNumber)?.JumpRight();

        if (Input.GetKeyDown(KeyCode.P)) Cheats();
    }
    public Stats cheatStats;
    void Cheats()
    {
        birdPlayers[0].stats = cheatStats;
    }

    public void SpawnPlayer(Player player)
    {
        AddBirdPlayer(GenerateBirdPlayer(player));
        CanvasManager.Instance.DisplayPlayerOnTab(player.NickName, player.ActorNumber);
    }

    public void AddBirdPlayer(BirdPlayer birdPlayer)
    {
        GetBirdTransform(birdPlayer.ID).GetComponent<SpriteRenderer>().sprite = birdPlayer.sprites[0];
        birdPlayers.Add(birdPlayer);
        print("PlayerBird Added!");
    }

    public Transform GetBirdTransform(int actorNumber)
    {
        Transform birdTransform;
        switch (actorNumber)
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
        switch (actorNumber)
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




