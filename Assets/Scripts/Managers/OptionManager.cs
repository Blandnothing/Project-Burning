using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OptionManager : MonoBehaviour
{
    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    public void EnterScene(int index)
    {
        StartCoroutine(LoadGame(index));
    }

    public void AddScene(int index)
    {
        StartCoroutine(LoadGame(index, true));
    }
    static IEnumerator LoadGame(int index, bool add = false)
    {
        var async = add ? SceneManager.LoadSceneAsync(index, LoadSceneMode.Additive) : SceneManager.LoadSceneAsync(index);
        while (!async.isDone)
        {
            yield return null;
        }
    }
}
