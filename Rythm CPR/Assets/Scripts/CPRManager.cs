using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class CPRManager : MonoBehaviour
{
    public ScreenFader screenFader;
    public RhythmIndicator rhythmIndicator;

    public int minScoreToSurvive = 25;
    public int compressionsToFinish = 100;

    private int compressionCount = 0;
    private bool cprEnded = false;

    private void Start()
    {
        ResetSession();
    }

    public void RegisterCompression()
    {
        if (cprEnded) return;

        compressionCount++;

       
        if (rhythmIndicator != null)
            rhythmIndicator.RegisterCompression();

        
        if (compressionCount >= compressionsToFinish)
            EndCPR();
    }

    private void ResetSession()
    {
        compressionCount = 0;
        cprEnded = false;

        rhythmIndicator?.ResetIndicator();
    }

    private void EndCPR()
    {
        cprEnded = true;

        int finalScore = rhythmIndicator != null ? rhythmIndicator.score : 0;

        string nextScene = finalScore >= minScoreToSurvive
            ? "saved_scene"
            : "died_scene";

        StartCoroutine(EndFlow(nextScene));
    }

    private IEnumerator EndFlow(string sceneName)
    {
        if (screenFader != null)
            yield return screenFader.FadeOut(1f);

        SceneManager.LoadScene(sceneName);

        yield return new WaitForEndOfFrame();

        if (screenFader != null)
            yield return screenFader.FadeIn(1f);
    }
}
