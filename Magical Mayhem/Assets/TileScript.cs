using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileScript : MonoBehaviour
{
    // Start is called before the first frame update
    public float timer = 1f;
    private bool isTouched;
    private float gameStart =3f;
    Renderer render;
    Collider col;
    void Start()
    {
        render= GetComponent<Renderer>();
        col = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        gameStart-=Time.deltaTime;
        if (isTouched)
        {
            
        
            timer-=Time.deltaTime;
            if (timer<=0)
            {
                col.enabled=false;
                render.enabled=false;
            }
        }
    }
    void OnCollisionEnter(){
        if (gameStart<=0)
        {
            isTouched=true;    
        }
        
    }
}
