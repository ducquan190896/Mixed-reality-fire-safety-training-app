using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollider : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private GameManager gameManager;
    [SerializeField] private PlayerCtl playerCtl;
    void Start()
    {
        if (!this.gameManager)
        {
            this.gameManager = FindObjectOfType<GameManager>();
        }
        this.LoadPlayerCtrl();
    }

    private void LoadPlayerCtrl()
    {
        if (this.playerCtl) return;
        this.playerCtl = transform.GetComponent<PlayerCtl>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (!gameManager) return;
        if (other.gameObject.tag == "Fire")
        {
            // minus the player health point here
             playerCtl.playerHealth.CollidedWithFire();
        }
        else if (other.gameObject.tag == "Door")
        {
            gameManager.onGameState.Invoke(GameState.Win);
            playerCtl.playerHealth.EndGameHandler();
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject.tag == "Fire")
        {
            playerCtl.playerHealth.EscapeFromFire();
        }
    }
}
