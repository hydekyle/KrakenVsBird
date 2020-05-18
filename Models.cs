using System;
using System.Collections.Generic;
using System.Threading.Tasks;
// using Firebase;
// using Firebase.Database;
using UnityEngine;

public class KrakenPlayer
{
    public int actorNumber;
    public string name;
    public Transform inferiorT, superiorT, rotationPoint;
    public Stats stats;
    private float legMaxTop, legMinTop;
    public PlayerAction lastAction = PlayerAction.None;

    public KrakenPlayer(string name, int ID, Transform centerT, Stats stats)
    {
        this.name = name;
        this.actorNumber = ID;
        this.rotationPoint = centerT;
        this.inferiorT = centerT.Find("Inferior");
        this.superiorT = centerT.Find("Superior");
        this.legMaxTop = superiorT.localPosition.y;
        this.legMinTop = inferiorT.localPosition.y;
        this.stats = stats;
    }

    public float rotation, height;
    float rotationForce = 0f;

    public void Update()
    {
        if (lastAction != PlayerAction.Both) Rotate(1f);
    }

    public void Rotate(float velocity)
    {
        bool isIdle = lastAction == PlayerAction.None;
        rotationPoint.Rotate(Vector3.up * Time.deltaTime * stats.velocity * rotationForce * 2, Space.Self);
        inferiorT.localPosition = Vector3.Lerp(
            inferiorT.localPosition,
            new Vector3(inferiorT.localPosition.x, isIdle ? legMinTop : legMaxTop, inferiorT.localPosition.z),
            isIdle ? Time.deltaTime * velocity * 3 : Time.deltaTime * velocity
            );
        superiorT.localPosition = Vector3.Lerp(
            superiorT.localPosition,
            new Vector3(superiorT.localPosition.x, isIdle ? legMaxTop : legMinTop, superiorT.localPosition.z),
            isIdle ? Time.deltaTime * velocity * 3 : Time.deltaTime * velocity
            );
    }

    public void FreezePosition()
    {
        rotationForce = 0f;
        lastAction = PlayerAction.Both;
    }

    public void RotateStop()
    {
        rotationForce = 0f;
        lastAction = PlayerAction.None;
    }

    public void RotateLeft()
    {
        rotationForce = -3f * stats.force;
        lastAction = PlayerAction.MoveLeft;
    }

    public void RotateRight()
    {
        rotationForce = 3f * stats.force;
        lastAction = PlayerAction.MoveRight;
    }

    public void MoveKraken(PlayerData playerData)
    {
        PlayerAction action = playerData.action;
        if (action == PlayerAction.MoveLeft) RotateLeft();
        if (action == PlayerAction.MoveRight) RotateRight();
        if (action == PlayerAction.Both) FreezePosition();
        if (action == PlayerAction.None) RotateStop();
    }
}

public class BirdPlayer
{
    public int actorNumber;
    public string name;
    public Transform rotationPoint, birdTransform;
    public Stats stats;
    public Sprite[] sprites;
    public SpriteRenderer spriteRenderer;

    public BirdPlayer(string name, int ID, Stats stats, Transform rotationPoint, Transform birdTransform, Sprite[] sprites)
    {
        this.name = name;
        this.actorNumber = ID;
        this.rotationPoint = rotationPoint;
        this.birdTransform = birdTransform;
        this.stats = stats;
        this.sprites = sprites;
        spriteRenderer = birdTransform.GetComponent<SpriteRenderer>();
        targetPOS = new Vector3(birdTransform.localPosition.x, groundPosition, birdTransform.localPosition.z);
    }

    float groundPosition = 5f;
    float skyPosition = 18f;
    float flutterForce = 0f;
    float gravityValue = 0f;
    int jumpAmount = 1;
    bool movingRight = false;
    bool movingLeft = false;
    PlayerAction lastAction = PlayerAction.None;
    Vector3 pos = Vector3.zero;

    float delta = 0f;
    float deltaRotation = 0f;

    public void Update()
    {
        if (lastAction != PlayerAction.None) delta += Time.fixedDeltaTime / 4;
        InterpolateMovement(delta);
        if (Mathf.Approximately(GetHeightValue(), groundPosition))
            TouchGround();
    }

    Vector3 targetPOS;

    void SetTargetPos()
    {
        if (lastAction == PlayerAction.None || lastAction == PlayerAction.Both)
        {
            targetPOS = new Vector3(MyPosition().x, groundPosition, MyPosition().z);
        }
        if (lastAction == PlayerAction.MoveLeft)
        {
            targetPOS = new Vector3(MyPosition().x, Mathf.Clamp(MyPosition().y, MyPosition().y + 4, skyPosition), MyPosition().z);
            //SetRotation(MyRotationValue() - Mathf.Clamp(delta, 0f, 1f));
        }
        if (lastAction == PlayerAction.MoveRight)
        {
            targetPOS = new Vector3(MyPosition().x, Mathf.Clamp(MyPosition().y, MyPosition().y + 4, skyPosition), MyPosition().z);
            //SetRotation(MyRotationValue() + Mathf.Clamp(delta, 0f, 1f));
        }
    }

    void InterpolateMovement(float delta)
    {
        if (lastAction != PlayerAction.None && delta <= 1.0f)
        {
            SetHeight(GetHeightValue() + delta * 3);
        }
        else if (delta > 1.3f) SetNoAction();
    }

    void SetNoAction()
    {
        lastAction = PlayerAction.None;
        delta = 0.0f;
    }

    Vector3 MyPosition()
    {
        return birdTransform.localPosition;
    }

    void TouchGround()
    {
        spriteRenderer.sprite = sprites[0];
        lastAction = PlayerAction.None;
    }

    public void Jump(PlayerData playerData)
    {
        delta = 0.0f;
        SetTargetPos();
        lastAction = playerData.action;
        FlutterAnim(playerData.action);
    }

    void FlutterAnim(PlayerAction action)
    {
        if (action == PlayerAction.MoveRight)
        {
            spriteRenderer.flipX = true;
            movingLeft = false;
            movingRight = true;
        }
        else
        {
            spriteRenderer.flipX = false;
            movingLeft = true;
            movingRight = false;
        }
        spriteRenderer.sprite = spriteRenderer.sprite == sprites[0] ? sprites[1] : sprites[0];
    }

    void SetRotation(float value)
    {
        rotationPoint.eulerAngles = new Vector3(0, value, 0);
    }

    void SetHeight(float value)
    {
        birdTransform.localPosition = new Vector3(
            birdTransform.localPosition.x,
            Mathf.Clamp(value, groundPosition, skyPosition),
            birdTransform.localPosition.z
            );
    }

    float MyRotationValue()
    {
        return rotationPoint.eulerAngles.y;
    }

    float GetHeightValue()
    {
        return birdTransform.localPosition.y;
    }
}

[Serializable]
public struct HostData
{
    public List<BirdPlayer> players;
}

[Serializable]
public enum PlayerAction { None, MoveRight, MoveLeft, Both };

[Serializable]
public struct PlayerData
{
    public int actorNumber;
    public float pos_angle, pos_height;
    public PlayerAction action;
}

public enum Rarity { common, rare, epic };

[Serializable]
public class Bird
{
    public Rarity rarity;
    public string name;
    public Stats stats;
    public Sprite[] sprites;
}

[Serializable]
public struct Stats
{
    public byte velocity, weight, force;
    public Stats(byte velocity, byte weight, byte force)
    {
        this.velocity = velocity;
        this.weight = weight;
        this.force = force;
    }
}

public struct RoundCall
{
    public List<string> playersToCall;
    public double timeStamp;
}

public enum StatusDB { Disconnected, Connecting, Connected };

// public class Db
// {
//     FirebaseApp app;
//     FirebaseDatabase database;
//     public StatusDB status = StatusDB.Disconnected;
//     DatabaseReference playersRef;

//     private void Init()
//     {
//         app = Firebase.FirebaseApp.DefaultInstance;
//         database = FirebaseDatabase.DefaultInstance;
//         status = StatusDB.Connected;
//         playersRef = database.RootReference.Child("Players");
//     }

//     public async Task<DataSnapshot> DameDatosAsync()
//     {
//         await CheckStatusDB();
//         DataSnapshot value = await playersRef.GetValueAsync();
//         return value;
//     }

//     public async Task CheckStatusDB()
//     {
//         if (status != StatusDB.Connected) await TryConnect();
//     }

//     public async Task TryConnect()
//     {
//         UnityEngine.Debug.Log("Conectando con Firebase");
//         status = StatusDB.Connecting;
//         var dependencyStatus = await Firebase.FirebaseApp.CheckAndFixDependenciesAsync();
//         if (dependencyStatus == Firebase.DependencyStatus.Available)
//         {
//             Init();
//         }
//         else
//         {
//             UnityEngine.Debug.LogError(System.String.Format(
//               "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
//             status = StatusDB.Disconnected;
//         }
//     }
// }
