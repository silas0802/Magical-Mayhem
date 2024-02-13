using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSpawner : MonoBehaviour
{
    public GameObject tile;
    public int x;
    public int y;
    public Transform player;
    // Start is called before the first frame update
    void Start()
    {
      SpawnTiles();  
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SpawnTiles(){
        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {
               Instantiate(tile,new Vector3(i,0,j),Quaternion.identity); 
            }
        }
        player.position=new Vector3(x/2,3,y/2);
    }


    
}
