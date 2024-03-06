using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu instance;
    [SerializeField] private Button resume;
    [SerializeField] private Button leave;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        resume.onClick.AddListener(Resume);
        leave.onClick.AddListener(Leave);



        gameObject.SetActive(false);
    }
    
    public void OpenPauseMenu()
    {
        gameObject.SetActive(true);
    }

    private void Resume()
    {
        gameObject.SetActive(false);
    }
    private void Leave()
    {
        NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene(1);
    }
    
}
