using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OldManager : MonoBehaviour
{
    public Transform krakenT;
    public Transform playerT;

    public Transform superiorT, inferiorT;
    bool superiorIsUp = true, inferiorIsUp = false;
    public float CD_krakenAnim = 1f, CD_birdJump = 1f;
    float lastTimeAnimated = 0f, lastTimeJumped = 0f;

    public float angularSpeed = 1f;
    public float circleRad = 1f;

    private Vector3 fixedPoint;
    private float currentAngle;
    Camera mainCamera;

    void Start()
    {
        fixedPoint = playerT.position;
        mainCamera = Camera.main;
    }

    void Update()
    {
        MoveBird();
        MoveKraken();
    }

    float posY = 0f;
    float newPosY = 0f;
    public float jumpForce = 6f;
    float camDistance = 1.4f;

    void MoveBirdOld()
    {
        if (Input.GetKeyDown(KeyCode.Space) && Time.time > lastTimeJumped + CD_birdJump) JumpBird();
        posY = Mathf.Lerp(posY, newPosY, Time.deltaTime * jumpForce);
        newPosY = Mathf.MoveTowards(newPosY, 0.0f, Time.deltaTime * (posY * jumpForce)); //Cuanto más alto más gravedad.

        currentAngle += -Input.GetAxis("Horizontal") * angularSpeed * Time.deltaTime;
        Vector3 offset = new Vector3(Mathf.Sin(currentAngle), posY, Mathf.Cos(currentAngle)) * circleRad;
        playerT.position = fixedPoint + offset;
        mainCamera.transform.position = fixedPoint + offset * camDistance;
        mainCamera.transform.LookAt(Vector3.zero);
        print(currentAngle);
        //currentAngle = Mathf.Clamp(currentAngle, -1f, 1f);
    }

    void MoveBird()
    {
        if (Input.GetKeyDown(KeyCode.Space) && Time.time > lastTimeJumped + CD_birdJump) JumpBird();
        posY = Mathf.Lerp(posY, newPosY, Time.deltaTime * jumpForce);
        newPosY = Mathf.MoveTowards(newPosY, 0.0f, Time.deltaTime * (posY * jumpForce));

        Vector3 offset = new Vector3(Mathf.Sin(currentAngle), posY, Mathf.Cos(currentAngle)) * circleRad;
        playerT.position = fixedPoint + offset;
        mainCamera.transform.position = fixedPoint + offset * camDistance;
        mainCamera.transform.LookAt(Vector3.zero);
    }

    void JumpBird()
    {
        lastTimeJumped = Time.time;
        newPosY = newPosY + jumpForce / 10;
    }


    public float velocityKraken = 10f;
    void MoveKraken()
    {
        krakenT.transform.Rotate(Vector3.up * Input.GetAxis("Vertical") * velocityKraken * Time.deltaTime);
        if (Input.GetKeyDown(KeyCode.F) && Time.time > lastTimeAnimated + CD_krakenAnim) Animate();
    }

    void Animate()
    {
        lastTimeAnimated = Time.time;
        foreach (Transform t in superiorT)
        {
            t.GetComponent<Animator>().Play(superiorIsUp ? "PataDown" : "PataUp");
        }
        superiorIsUp = !superiorIsUp;
        foreach (Transform t in inferiorT)
        {
            t.GetComponent<Animator>().Play(inferiorIsUp ? "PataDown" : "PataUp");
        }
        inferiorIsUp = !inferiorIsUp;
    }

}
