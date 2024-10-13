using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class GameManager : MonoBehaviour
{
    // set unityEvent listener in gameManager, the playerCollider and playerHeather will invoke the event by gameState (loss, win)
    // the gameManager also manage the survivalTextMeshPro text and PlayingStatusTextMeshPro text though the gameState unityEvent

    [SerializeField] public UnityEvent<GameState> onGameState = new UnityEvent<GameState>();
    [SerializeField] public TextMeshProUGUI playingStatusText;

    // Start is called before the first frame update
    void Start()
    {
        playingStatusText.text = "";
        onGameState.AddListener(onGameStateListener);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void onGameStateListener(GameState state)
    {
        if (state == GameState.Win)
        {
            playingStatusText.text = "You win";
            playingStatusText.color = Color.green;
        }
        else if (state == GameState.Loss)
        {
            playingStatusText.text = "You lose";
            playingStatusText.color = Color.red;
        }
    }
}
