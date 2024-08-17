using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomEntry : MonoBehaviour
{
    [Header("通道左边的相机")] public GameObject leftCamera;
    [Header("通道右边的相机")] public GameObject rightCamera;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        CinemachineVirtualCameraBase mainCam = (CinemachineVirtualCameraBase)Camera.main.GetComponent<CinemachineBrain>().ActiveVirtualCamera;
        mainCam.gameObject.SetActive(false);
        if (collision.transform.position.x<transform.position.x)
        {
            rightCamera.SetActive(true);
            rightCamera.GetComponent<CinemachineVirtualCamera>().Follow = collision.transform;
        }
        else
        {
            leftCamera.SetActive(true);
            leftCamera.GetComponent<CinemachineVirtualCamera>().Follow = collision.transform;
        }
    }
}
