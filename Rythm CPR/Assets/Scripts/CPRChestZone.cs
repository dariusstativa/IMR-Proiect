using UnityEngine;

public class CPRChestZone : MonoBehaviour
{
    public ChestCompressionDepth compressionDepth;
    private int handsInside = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Hand")) return;

        handsInside++;

        if (handsInside == 1)
        {
            compressionDepth.HandEntered();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Hand")) return;

        handsInside = Mathf.Max(0, handsInside - 1);

        if (handsInside == 0)
        {
            compressionDepth.HandExited();
        }
    }
}
