using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [SerializeField]Transform player;
    Vector3 offset;
    // Start is called before the first frame update
    void Awake()
    {
        offset = transform.position-player.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = offset+player.position;
    }
}
