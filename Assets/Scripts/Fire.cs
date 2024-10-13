using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Meta.XR.MRUtilityKit;
using UnityEngine;

[Serializable]
public struct FlameState
{
    public float flameHeight;
    public float flameScale;
    public float elapse;
    public float currentElapse;
    public Color flameColor;
    public Vector3 flamePos;
    public Vector3 scaleObject;
    public Material flameStateMaterial;

    public bool Tick(ref Matrix4x4 matrix, AnimationCurve heightCurve, AnimationCurve scaleCurve, Matrix4x4 localMatrix)
    {
        if (currentElapse > elapse) return true;
        float elapseT = currentElapse / elapse;

        // rise the flame
        float heightT = heightCurve.Evaluate(elapseT);
        flamePos.y = heightT * flameHeight;

        // rise the scale
        float scaleT = scaleCurve.Evaluate(elapseT);
        Vector3 scale = Vector3.one * scaleT * flameScale * 0.5f;
        scaleObject = scale;
        Matrix4x4 flameMatrix = Matrix4x4.zero;
        flameMatrix.SetTRS(flamePos, Quaternion.identity, scale);
        matrix = flameMatrix * localMatrix;

        currentElapse += Time.deltaTime;
        return false;
    }
}
public class Fire : MonoBehaviour
{
    // Start is called before the first frame  update
    [SerializeField] public Mesh mesh;
    [SerializeField] public Material material;
    [SerializeField] private Gradient flameGradient;
    [SerializeField] public float maxHeight = 2f;
    [SerializeField] public float minHeight = 0.5f;
    [SerializeField] public float maxScale = 0.15f;
    [SerializeField] public float minScale = 0.01f;

    [SerializeField] public float maxLifeTime = 2f;
    [SerializeField] public float minLifeTime = 0.5f;
    [SerializeField] public int flameNumber = 20;

    [SerializeField] private AnimationCurve heightCurve;
    [SerializeField] private AnimationCurve scaleCurve;

    [SerializeField] private Matrix4x4[] matrices;
    [SerializeField] private FlameState[] flames;

    [SerializeField] public Vector3 firePosition;
    [SerializeField] public float spawnRadius = 1f;
    private Matrix4x4 localMatrix;
    void Start()
    {
        matrices = new Matrix4x4[this.flameNumber];
        flames = new FlameState[this.flameNumber];
        localMatrix = this.transform.localToWorldMatrix;
        this.LoadFirePosition();
        for (var i = 0; i < flames.Length; i++)
        {
            InstanstiateFlateState(ref flames[i]);
        }
    }

    private void LoadFirePosition()
    {
        Debug.Log("LoadFirePosition");
        if (MRUK.Instance) this.firePosition = Vector3.zero;
        bool success = MRUK.Instance.GetCurrentRoom().GenerateRandomPositionOnSurface(MRUK.SurfaceType.FACING_UP, 0.1f, LabelFilter.Included(MRUKAnchor.SceneLabels.FLOOR), out Vector3 position, out Vector3 normal);
        if (success)
        {
            this.transform.position = position;
            this.firePosition = position;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        for (var i = 0; i < flames.Length; i++)
        {
            bool isNew = flames[i].Tick(ref matrices[i], heightCurve, scaleCurve, localMatrix);
            if (isNew)
            {
                InstanstiateFlateState(ref flames[i]);
                Graphics.DrawMesh(mesh, matrices[i], flames[i].flameStateMaterial, 0);
            }
        }
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        if (!mesh || !material || matrices.Length <= 0) return;
        // Graphics.DrawMeshInstanced(mesh, 0, material, matrices);
        for (var i = 0; i < flames.Length; i++)
        {
            Graphics.DrawMesh(mesh, matrices[i], flames[i].flameStateMaterial, 0);
        }
    }

    public void InstanstiateFlateState(ref FlameState flameState)
    {
        // Generate a random angle (in radians)
        float angle = UnityEngine.Random.Range(0f, Mathf.PI * 2f);

        // Generate a random distance from the center within the spawnRadius
        float radius = UnityEngine.Random.Range(0f, spawnRadius);

        // Calculate the flame position within the circle
        float x = firePosition.x + Mathf.Cos(angle) * radius;
        float z = firePosition.z + Mathf.Sin(angle) * radius;

        // Set the flame's position
        flameState.flamePos = new Vector3(x, 0f, z);

        //  float randomRadius = UnityEngine.Random.Range(-this.spawnRadius, this.spawnRadius);
        // flameState.flamePos = new Vector3(firePosition.x + randomRadius, 0f, firePosition.z + randomRadius);

        Debug.Log("InstanstiateFlateState");
        flameState.flameHeight = UnityEngine.Random.Range(this.maxHeight, this.minHeight);
        flameState.flameScale = UnityEngine.Random.Range(this.maxScale, this.minScale);
        flameState.elapse = UnityEngine.Random.Range(this.maxLifeTime, this.minLifeTime);
        flameState.currentElapse = 0f;

        flameState.flameColor = flameGradient.Evaluate(UnityEngine.Random.value);
        flameState.flameStateMaterial = material;
        flameState.flameStateMaterial.color = flameGradient.Evaluate(UnityEngine.Random.value);
    }


}
//   public bool Tick(ref Matrix4x4 matrix, Matrix4x4 parentMatrix, AnimationCurve flameRiseCurve, AnimationCurve flameScaleCurve)
//     {
//         if (currentElapse >= elapse) return true;
//         float elapseT = currentElapse / elapse;

//         // rise the flame
//         float riseT = flameRiseCurve.Evaluate(elapseT);
//         flamePosition.y = flameHeight * riseT;

//         // rise the scale
//         float scaleT = flameScaleCurve.Evaluate(elapseT);
//         Vector3 scaleMatrix = Vector3.one * scaleT * flameScale * 0.1f; 

//         Matrix4x4 flameMatrix = Matrix4x4.identity;
//         flameMatrix.SetTRS(flamePosition, Quaternion.identity, scaleMatrix);
//         matrix = flameMatrix;

//         currentElapse += Time.deltaTime;
//         return false;
//     }



//   void Awake()
//     {
//         this.spawnRadius = UnityEngine.Random.Range(this.minRadius, this.maxRadius);
//         BoxCollider fireCollider = this.transform.GetComponent<BoxCollider>();
//         fireCollider.size = new Vector3( this.spawnRadius/2, this.spawnRadius,  this.spawnRadius/2);
//         this.LoadFirePosition();
//         flames = new FlameStateV2[this.flameNumber];
//         matrices = new Matrix4x4[this.flameNumber];
//         for (int i = 0; i < this.flameNumber; i++)
//         {
//             flames[i] = instanstiateFlame();
//             matrices[i] = Matrix4x4.identity;
//         }
//     }

//     private void LoadFirePosition()
//     {
//         MRUKRoom currentRoom = MRUK.Instance.GetCurrentRoom();
//         bool success = currentRoom.GenerateRandomPositionOnSurface(MRUK.SurfaceType.FACING_UP, 0.1f, LabelFilter.Included(MRUKAnchor.SceneLabels.FLOOR), out Vector3 position, out Vector3 normal);
//         if (success)
//         {
//             this.transform.position = position;
//             this.transform.rotation = Quaternion.identity;
//         }

//     }

    
//     void FixedUpdate()
//     {
//         if (!mesh || !material || matrices.Count() <= 0 || flames.Count() <= 0) return;
//         for (int i = 0; i < this.flames.Count(); i++)
//         {
//             bool isNewFlame = flames[i].Tick(ref matrices[i], this.transform.localToWorldMatrix, riseCurve, scaleCurve);
//             if (isNewFlame)
//             {
//                 flames[i] = instanstiateFlame();
//             }
//         }
//     }

//     void Update()
//     {
//         if (!mesh || !material || matrices.Count() <= 0) return;
//         Graphics.DrawMeshInstanced(mesh, 0, material, matrices);
//     }

//     public FlameStateV2 instanstiateFlame()
//     {
//         FlameStateV2 flameState;
//         flameState.elapse = UnityEngine.Random.Range(this.minElapse, this.maxElapse);
//         flameState.flameHeight = UnityEngine.Random.Range(this.minHeight, this.maxHeight);
//         flameState.flameScale = UnityEngine.Random.Range(this.minScale, this.maxScale);
//         flameState.currentElapse = 0f;

//         float x = this.transform.position.x + UnityEngine.Random.Range(-this.spawnRadius / 2, this.spawnRadius / 2);
//         float z = this.transform.position.z + UnityEngine.Random.Range(-this.spawnRadius / 2, this.spawnRadius / 2);
//         flameState.flamePosition = new Vector3(x, 0, z);
//         return flameState;
//     }
