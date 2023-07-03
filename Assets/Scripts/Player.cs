using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [Header("Ragdoll components")]
    [SerializeField] Rigidbody[] all_rb;
    [SerializeField] Collider[] all_col;

    [SerializeField] Animator animator;
    [SerializeField] PlayerMover playerMover;

    [SerializeField] GameObject loseScreen;

    [SerializeField] GameObject cubeHolder;

    public static Player instance;

    [SerializeField] CameraShaker cameraShaker;

    private void Awake()
    {
        if (instance == null) instance = this;
        RagdollActivate(false);
    }

    //TEST
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) RagdollActivate(true);
    }

    private void RagdollActivate(bool activate)
    {
        foreach (Rigidbody rb in all_rb) rb.isKinematic = !activate;
        foreach (Collider col in all_col) col.isTrigger = !activate;
        animator.enabled = !activate;        
    }


    private void PlayerDeath()
    {
        RagdollActivate(true);
        playerMover.SetMoveAble(false);
        cameraShaker.BigShake();
    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag=="Wall")
        {
            PlayerDeath();
        }
    }

    public void RestartLevel()
    {
        int levelIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(levelIndex);
    }
}
