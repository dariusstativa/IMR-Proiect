using UnityEngine;

public class QuitOnTouch : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger QUIT with: " + other.name + " | tag: " + other.tag);

        if (other.CompareTag("Hand"))
        {
            Debug.Log("Exit game!");
            Application.Quit();

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }
}