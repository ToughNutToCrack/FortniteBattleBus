using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject player;
    public CinemachineFreeLook busCamera, playerCamera;
    [Range(0, 10)]
    public float distanceFromBus = 2;
    public GameEvent playerJumped;

    bool canJump;

    private void Update()
    {
        if (canJump && Input.GetKeyUp(KeyCode.Space))
        {
            ActivatePlayer();
            canJump = false;
        }
    }
    public void EnableJumpFromBus()
    {
        canJump = true;
    }

    public void ActivatePlayer()
    {
        busCamera.Priority = 0;
        playerCamera.Priority = 1;
        player.SetActive(true);
        player.transform.position = busCamera.LookAt.position - (busCamera.LookAt.transform.forward * distanceFromBus);
        StartCoroutine(EnableCharacter());
    }

    public IEnumerator EnableCharacter()
    {
        yield return new WaitForSeconds(1);
        playerJumped.Raise();
    }
}
