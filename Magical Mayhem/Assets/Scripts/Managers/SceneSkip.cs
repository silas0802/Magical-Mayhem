using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneSkip : MonoBehaviour
{
    private void Start()
    {
        SceneHelper.instance.LoadScene(1);
    }
}
