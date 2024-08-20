using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class PuzzleSceneButton : MonoBehaviour
{
    public int sceneIndex = 3;
    bool active = false;
    public List<GameObject> HideObjects = new List<GameObject>();
    [SerializeField]  GameObject eventSystem;
    
    public void OnClick()
    {
        if (!active)
        {
            eventSystem.SetActive(false);
            StartCoroutine(AddScene(sceneIndex));
            foreach (var obj in HideObjects)
                obj.SetActive(false);
        }
        else
        {
            StartCoroutine(RemoveScene());
            foreach (var obj in HideObjects)
                obj.SetActive(true);
            eventSystem.SetActive(true);
        }
        active = !active;
    }

    IEnumerator AddScene(int index)
    {
        var async = SceneManager.LoadSceneAsync(index, LoadSceneMode.Additive);
        while (!async.isDone)
        {
            yield return null;
        }
    }

    IEnumerator RemoveScene()
    {
        var async = SceneManager.UnloadSceneAsync(sceneIndex);
        while (!async.isDone)
        {
            yield return null;
        }
    }
}