using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class SpawnTile : MonoBehaviour
{
    [SerializeField] private TileScript floor;
    [SerializeField] private WallScript wall;
    int mapSize = 20;
    TileScript[,] tilearray;
    
    // Start is called before the first frame update
    void Start()
    {
        tilearray = new TileScript[mapSize,mapSize];
        for (int i = 0; i < mapSize; i++)
        {
            for (int j = 0; j < mapSize; j++)
            {
                TileSpawner(j, i);
            }
        }
        WallScript southWall = Instantiate(wall, new Vector3(0, 0, 0), quaternion.identity);
        WallScript northWall = Instantiate(wall, new Vector3(mapSize-1, 0, mapSize-1), quaternion.identity);
        WallScript eastWall = Instantiate(wall, new Vector3(mapSize-1, 0, 0), quaternion.identity);
        WallScript westWall = Instantiate(wall, new Vector3(0, 0, mapSize-1), quaternion.identity);
        SetWalls(southWall, northWall, eastWall, westWall);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //This function transforms the walls to fit the egde of the gamemap and stops the player from falling off
    void SetWalls(WallScript southWall, WallScript northWall, WallScript eastWall, WallScript westWall){
        southWall.transform.localScale = new Vector3(mapSize,1,1);
        northWall.transform.localScale = new Vector3(mapSize,1,1);
        //eastWall.transform.localScale = new Vector3(1,)
    }

    //instanciates the floortiles and saves them in an array
    void TileSpawner(int xCoord, int yCoord){
        tilearray[xCoord,yCoord] = Instantiate(floor, new Vector3(xCoord, 0, yCoord), quaternion.identity);
    }
}
