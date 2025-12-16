using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneOnCollision : MonoBehaviour
{
    public string sceneName;

    private void OnCollisionEnter(Collision collision)
    {
        
        if (collision.collider.CompareTag("Hand"))
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}