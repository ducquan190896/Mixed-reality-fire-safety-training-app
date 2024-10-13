using System;
using System.Collections;
using System.Collections.Generic;
using Meta.XR.MRUtilityKit;
using UnityEngine;

public class FireSpreader : MonoBehaviour
{
    [SerializeField] public float fireDensity = 3f;
    [SerializeField] public GameObject firePrefab;
    [SerializeField] private int fireQuantity;
    // Start is called before the first frame update
    void Start()
    {
        this.SpreadingFireToCurrentRoom();
    }

    private void SpreadingFireToCurrentRoom()
    {
        if (!MRUK.Instance) return;
        MRUKRoom currentRoom = MRUK.Instance.GetCurrentRoom();
        MRUKAnchor floorAnchor = currentRoom.FloorAnchor;
        if (!floorAnchor.PlaneRect.HasValue) return;
        Rect floorRect = floorAnchor.PlaneRect.Value;
        float floorSquare = floorRect.size.x * floorRect.size.y;
        fireQuantity = (int)MathF.Round(floorSquare / this.fireDensity);
        for (int i = 0; i < fireQuantity; i++) {
            Instantiate(firePrefab, this.transform);
        }
    }
}
