using System;
using Meta.XR.MRUtilityKit;
using UnityEngine;

[Serializable]
public struct FlameStateV2
{
    public float elapse;
    public float currentElapse;
    public float flameHeight;
    public float flameScale;
    public Vector3 flamePosition;

    public bool TickFlame(ref Matrix4x4 matrix, Matrix4x4 worldMatrix, AnimationCurve riseCurveT, AnimationCurve scaleCurveT)
    {
        if (currentElapse > elapse) return true;
        float elapseT = currentElapse / elapse;

        // rise flame
        float riseT = riseCurveT.Evaluate(elapseT);
        float newFlameHeight = riseT * flameHeight;
        flamePosition.y = newFlameHeight;

        // scale down the flame
        float scaleT = scaleCurveT.Evaluate(elapseT);
        float newFlameScale = flameScale * scaleT * 0.1f;
        Vector3 scale = Vector3.one * newFlameScale;

        // update matrix
        Matrix4x4 fireMatrix = Matrix4x4.zero;
        fireMatrix.SetTRS(flamePosition, Quaternion.identity, scale);
        matrix = fireMatrix * worldMatrix;

        currentElapse += Time.deltaTime;
        return false;
    }
}
public class TestFire : MonoBehaviour
{
    [SerializeField] public Material material;
    [SerializeField] public Mesh mesh;
    [SerializeField] public Gradient flameGradient;
    [SerializeField] public AnimationCurve riseCurve;
    [SerializeField] public AnimationCurve scaleCurve;
    [SerializeField] public float maxHeight;
    [SerializeField] public float minHeight;
    [SerializeField] public float maxScale;
    [SerializeField] public float minScale;

    [SerializeField] public float maxElapse;
    [SerializeField] public float minElapse;
    [SerializeField] public float minRadius;
    [SerializeField] public float maxRadius;

    [SerializeField] private Matrix4x4[] matrices;
    [SerializeField] private FlameStateV2[] flames;
    [SerializeField] public int flameNumber = 2;
    private float spawnRadius;
    private Vector3 firePosition;


    private void Awake()
    {
        this.LoadFirePosition();
        flames = new FlameStateV2[this.flameNumber];
        matrices = new Matrix4x4[this.flameNumber];
        spawnRadius = UnityEngine.Random.Range(this.maxRadius, this.minRadius);
        BoxCollider fireCollider = transform.GetComponent<BoxCollider>();
        fireCollider.size = new Vector3(spawnRadius, spawnRadius * 2, spawnRadius);
        for (int i = 0; i < this.flameNumber; i++)
        {
            this.flames[i] = InstanstiateFlame();
            this.matrices[i] = Matrix4x4.zero;
        }

    }

    private void LoadFirePosition()
    {
        MRUKRoom currentRoom = MRUK.Instance.GetCurrentRoom();
        bool success = false;
        while (!success)
        {
            success = currentRoom.GenerateRandomPositionOnSurface(MRUK.SurfaceType.FACING_UP, 0.1f, LabelFilter.Included(MRUKAnchor.SceneLabels.FLOOR), out Vector3 position, out Vector3 normal);
            if (!success) continue;

            // assign random position  to this fire object on floor
            this.transform.position = new Vector3(position.x, position.y, position.z);

        }
    }


    void Update()
    {
        if (!material || !mesh) return;
        Graphics.DrawMeshInstanced(mesh, 0, material, matrices);
    }

    private void FixedUpdate()
    {
        for (int i = 0; i < this.flameNumber; i++)
        {
            bool isNewFlame = flames[i].TickFlame(ref matrices[i], transform.localToWorldMatrix, riseCurve, scaleCurve);
            if (isNewFlame)
            {
                flames[i] = InstanstiateFlame();
            }
        }
    }


    public FlameStateV2 InstanstiateFlame()
    {
        FlameStateV2 flame;
        flame.currentElapse = 0;
        flame.elapse = UnityEngine.Random.Range(this.maxElapse, this.minElapse);
        flame.flameHeight = UnityEngine.Random.Range(this.maxHeight, this.minHeight);
        flame.flameScale = UnityEngine.Random.Range(this.maxScale, this.minScale);
        float positionX = transform.position.x + UnityEngine.Random.Range(-spawnRadius / 2, spawnRadius / 2);
        float positionZ = transform.position.z + UnityEngine.Random.Range(-spawnRadius / 2, spawnRadius / 2);
        flame.flamePosition = new Vector3(positionX, 0, positionZ);
        return flame;
    }
}
