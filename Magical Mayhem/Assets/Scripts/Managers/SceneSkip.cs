using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSkip : MonoBehaviour
{
    private void Start()
    {
        SceneManager.LoadScene(1);
    }
}
