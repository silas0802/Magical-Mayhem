using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.Mathematics;
using UnityEditor.ShaderGraph;
using UnityEngine;

public class SpawnTile : MonoBehaviour
{
    [SerializeField] private TileScript floor;
    [SerializeField] private WallScript wall;
    
    int mapSize = 41;
    int lavaTileCounter = 0;
    float wallHieght = 5f;
    float lavaSpawnTime = 10f;
    float nextLavaSpawn;
    TileScript[,] tileArray;
    
    // Start is called before the first frame update
    void Start()
    {   
        //set the time until map gets smaller
        nextLavaSpawn = Time.time + nextLavaSpawn;
        //save the floortiles
        tileArray = new TileScript[mapSize,mapSize];
        for (int i = 0; i < mapSize; i++)
        {
            for (int j = 0; j < mapSize; j++)
            {
                TileSpawner(j, i);
            }
        }
        //init borderwalls
        WallScript southWall = Instantiate(wall, new Vector3(mapSize/2, wallHieght/2, 0), quaternion.identity);
        WallScript northWall = Instantiate(wall, new Vector3(mapSize/2, wallHieght/2, mapSize-1), quaternion.identity);
        WallScript eastWall = Instantiate(wall, new Vector3(mapSize-1, wallHieght/2, mapSize/2), quaternion.identity);
        WallScript westWall = Instantiate(wall, new Vector3(0, wallHieght/2, mapSize/2), quaternion.identity);
        SetWalls(southWall, northWall, eastWall, westWall);

    }

    // Update is called once per frame
    void Update()
    {
        lava(tileArray);
    }

    //This function transforms the walls to fit the egde of the gamemap and stops the player from falling off
    void SetWalls(WallScript southWall, WallScript northWall, WallScript eastWall, WallScript westWall){
        southWall.transform.localScale = new Vector3(mapSize,wallHieght,1);
        northWall.transform.localScale = new Vector3(mapSize,wallHieght,1);
        eastWall.transform.localScale = new Vector3(1,wallHieght,mapSize);
        westWall.transform.localScale = new Vector3(1,wallHieght,mapSize);

    }

    //instanciates the floortiles and saves them in an array
    void TileSpawner(int xCoord, int yCoord){
        tileArray[xCoord,yCoord] = Instantiate(floor, new Vector3(xCoord, 0, yCoord), quaternion.identity);
    }

    void lava(TileScript[,] map){
        if (Time.time > nextLavaSpawn){
            for (int i = lavaTileCounter; i < mapSize-lavaTileCounter; i++)
            {
                // map[i,0] = set to now be lava tile;
                // map[0,i]    
                // map[mapSize-1,i]
                // map[i,mapSize-1]
            }
            nextLavaSpawn = Time.time + lavaSpawnTime;
            lavaTileCounter++;
        }
    }
}
