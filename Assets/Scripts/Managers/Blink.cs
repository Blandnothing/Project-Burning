using Spine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Blink : MonoBehaviour
{
    [SerializeField]
    private Image _img;
    [SerializeField, Header("最后一组的duration无实际作用！")]
    private BlinkData[] _OpenEyeData;
    [SerializeField, Header("最后一组的duration无实际作用！")]
    private BlinkData[] _CloseEyeData;
    private IEnumerator _blinkCoroutine;

    private void OnDestroy()
    {
        SetValue(0);
    }
    private void Start()
    {
        PlayBlink(_OpenEyeData);
        EventCenter.Instance.AddEvent(EventName.dead, ()=> BlinkLoadScene(1));
    }
    public void PlayBlink(BlinkData[] _data)
    {
        _img.enabled = true;
        if (_blinkCoroutine != null)
        {
            StopCoroutine(_blinkCoroutine);
            _blinkCoroutine = null;
        }

        _blinkCoroutine = BlinkCoroutine(_data);
        StartCoroutine(_blinkCoroutine);
    }

    IEnumerator BlinkCoroutine(BlinkData[] _data)
    {
        SetValue(_data[0].Value);
        float end=0;
        for (int i = 0; i < _data.Length - 1; i++)
        {
            var begin = _data[i].Value;
            end = _data[i + 1].Value;
            var duration = _data[i].Duration;
            var timer = 0f;
            while (timer < duration)
            {
                SetValue(Mathf.Lerp(begin, end, timer / duration));
                timer += Time.deltaTime;
                yield return null;
            }
            SetValue(end);
        }

        if (end == 1)
        {
            //_img.enabled = false;
        }
    }

    public void SetValue(float y)
    {
        _img.material.SetVector("_Param", new Vector4(0.6f, y, 1, 1));
    }
    public void BlinkLoadScene(int sceneIndex)
    {
        StartCoroutine(LoadScene(sceneIndex));
        PlayBlink(_CloseEyeData);
    }
    IEnumerator LoadScene(int index)
    {
        var async = SceneManager.LoadSceneAsync(index);
        async.allowSceneActivation = false;
        float deadTime = 2;
        float deadTimer = 0;
        while (!async.isDone && deadTimer < deadTime)
        {
            deadTimer += Time.deltaTime;
            yield return null;
        }
        async.allowSceneActivation = true;
    }


    [System.Serializable]
    public class BlinkData
    {
        public float Value;
        public float Duration;
    }
}