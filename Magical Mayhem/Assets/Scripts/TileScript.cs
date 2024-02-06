using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileScript : MonoBehaviour
{
    float timer;
    [SerializeField] Vector2 minMaxTimer = new Vector2(5, 30);
    Renderer visual;
    Collider col;
    // Start is called before the first frame update
    void Start()
    {
        visual = GetComponentInChildren<Renderer>();
        col = GetComponentInChildren<Collider>();
        ResetTile();
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0 && timer > -5)
        {
            visual.enabled = false;
            col.enabled = false;
        }
        else if (timer < -5)
        {
            ResetTile();
        }
    }

    void ResetTile()
    {
        visual.enabled = true;
        col.enabled = true;
        timer = Random.value*(minMaxTimer.y-minMaxTimer.x)+minMaxTimer.x;
    }
}
