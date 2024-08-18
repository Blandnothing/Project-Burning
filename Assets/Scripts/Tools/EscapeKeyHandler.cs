using UnityEngine;

public class EscapeKeyHandler : MonoBehaviour
{
    public GameObject EscPanel;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            HandleEscapeKey();
        }
    }

    void HandleEscapeKey()
    {
        Debug.Log("Esc key pressed");

        if (EscPanel == null)
            EscPanel = GameObject.Find("EscPanel");
        EscPanel.SetActive(!EscPanel.activeSelf);
    }
}
