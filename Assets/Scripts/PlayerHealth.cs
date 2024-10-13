using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI availabilityStatusText;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private PlayerCtl playerCtl;

    [SerializeField] public float points = 100f;
    [SerializeField] public bool isOnFire = false;
    [SerializeField] public bool isEndGame = false;
    // Start is called before the first frame update
    void Start()
    {
        if (!this.gameManager)
        {
            this.gameManager = FindObjectOfType<GameManager>();
        }
        this.LoadPlayerCtrl();
        this.availabilityStatusText.text = "";
    }
    private void LoadPlayerCtrl()
    {
        if (this.playerCtl) return;
        this.playerCtl = transform.GetComponent<PlayerCtl>();
    }

    // Update is called once per frame
    void Update()
    {
        if (this.isEndGame) return;
        this.PlayerGetBurned();
        this.CheckPlayerSurvivability();
        this.UpdateAvailabilityStatusCanvas();
        this.PlayerHitFire();
    }

    // /// <summary>
    // /// This function is called every fixed framerate frame, if the MonoBehaviour is enabled.
    // /// </summary>
    // void FixedUpdate()
    // {
    //     if (this.isEndGame) return;
    //     this.UpdateAvailabilityStatusCanvas();
    // }

    private void UpdateAvailabilityStatusCanvas()
    {
        if (this.points < 0) return;
        float survivingPercent = MathF.Round(this.points);
        this.availabilityStatusText.text = survivingPercent.ToString() + "% survivability";
    }

    private void CheckPlayerSurvivability()
    {
        if (this.points > 0) return;
        gameManager.onGameState.Invoke(GameState.Loss);
    }

    private void PlayerGetBurned()
    {
        if (isOnFire) return;
        if (this.points < 0) return;
        points -= Time.deltaTime;
    }

    public void CollidedWithFire()
    {
        if (this.isEndGame) return;
        if (this.points < 0) return;
        this.isOnFire = true;

    }

    private void PlayerHitFire()
    {
        if (!this.isOnFire) return;
        this.points -= (Time.deltaTime * 3);
    }

    public void EscapeFromFire()
    {
        if (this.isOnFire)
        {
            this.isOnFire = false;
        }
    }

    public void EndGameHandler()
    {
        Time.timeScale = 0;
        this.isEndGame = true;
        SceneManager.LoadScene(0);
    }
}
