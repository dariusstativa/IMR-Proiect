using UnityEngine;

public class ChestCompressionDepth : MonoBehaviour
{
    public Transform chestBone;
    public Transform chestAnchor;
    public Transform handPoint;
    public CPRManager manager;

    public Vector3 relaxedLocalPos;
    public Vector3 compressedLocalPos;

    public float maxDepth = 0.06f;
    public float triggerDepth = 0.01f;
    public float smooth = 12f;

    private float baseHandY;
    private bool baseSet = false;
    private bool isDown = false;

    private bool handInside = false;   // 🔴 NOU

    private void Start()
    {
        if (chestBone != null)
            relaxedLocalPos = chestBone.localPosition;
    }

    private void Update()
    {
        if (!handInside || !baseSet) return;   // 🔴 CONDIȚIE NOUĂ

        Vector3 localHand = chestAnchor.InverseTransformPoint(handPoint.position);

        float delta = (baseHandY - localHand.y) * 30f;
        float depth = Mathf.Clamp(delta, 0f, maxDepth);
        float t = depth / maxDepth;

        Vector3 targetLocal =
            Vector3.Lerp(relaxedLocalPos, compressedLocalPos, t);

        chestBone.localPosition = Vector3.Lerp(
            chestBone.localPosition,
            targetLocal,
            Time.deltaTime * smooth
        );

        if (!isDown && depth >= triggerDepth)
        {
            isDown = true;
            manager?.RegisterCompression();
        }

        if (isDown && depth < triggerDepth * 0.5f)
        {
            isDown = false;
        }
    }

    // 🔴 APELATE DIN CPRChestZone
    public void HandEntered()
    {
        Vector3 localHand = chestAnchor.InverseTransformPoint(handPoint.position);
        baseHandY = localHand.y;
        baseSet = true;
        handInside = true;
    }

    public void HandExited()
    {
        handInside = false;
        baseSet = false;
    }
}