using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIDeadPanel : MonoBehaviour
{
    [SerializeField]private GameObject deadPanel;
    private void OnEnable()
    {
        EventManager.StartListening(On.TookDamage, OnTookDamage);
    }

    private void OnDisable()
    {
        EventManager.StopListening(On.TookDamage, OnTookDamage);
    }

    private void OnTookDamage()
    {
        deadPanel.SetActive(true);
    }

    public void OnClickButtonRetry()
    {
        deadPanel.SetActive(false);
        EventManager.Instance.Reset();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void ApplicationQuit()
    {
        Application.Quit();
    }
}
