using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCtl : MonoBehaviour
{
    [SerializeField] public PlayerCollider playerCollider;
    [SerializeField] public PlayerHealth playerHealth;
    [SerializeField] public GameObject centerEyeCamera;

    // Start is called before the first frame update
    void Start()
    {
        this.LoadPlayerCollider();
        this.LoadPlayerHealth();
    }
    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        this.transform.position = centerEyeCamera.transform.position;
    }

    private void LoadPlayerHealth()
    {
        if (playerCollider) return;
        playerCollider = transform.GetComponent<PlayerCollider>();
    }

    private void LoadPlayerCollider()
    {
        if (playerHealth) return;
        playerHealth = transform.GetComponent<PlayerHealth>();
    }


}
