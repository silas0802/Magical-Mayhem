using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.Mathematics;
using UnityEngine;

public class SpawnTile : MonoBehaviour
{
    [SerializeField] private TileScript floor;
    [SerializeField] private BorderWallScript borderWall;
    [SerializeField] private LavaTileScript lavaTile;
    [SerializeField] private MapWallScript mapWall;

    int mapSize = 41;
    int lavaTileCounter = 0;
    float wallHieght = 5f;
    [SerializeField] float lavaSpawnTime = 1f;
    [SerializeField]float nextLavaSpawn;
    TileScript[,] tileArray;
    LavaTileScript[,] lavaTileArray;
    
    // Start is called before the first frame update
    void Start()
    {   
        //save the floortiles
        tileArray = new TileScript[mapSize,mapSize];
        lavaTileArray = new LavaTileScript[mapSize,mapSize];
        for (int i = 0; i < mapSize; i++)
        {
            for (int j = 0; j < mapSize; j++)
            {
                TileSpawner(j, i);
            }
        }
        //init borderwalls
        BorderWallScript southWall = Instantiate(borderWall, new Vector3(mapSize/2-20, wallHieght/2, 0-20), quaternion.identity);
        BorderWallScript northWall = Instantiate(borderWall, new Vector3(mapSize/2-20, wallHieght/2, mapSize-1-20), quaternion.identity);
        BorderWallScript eastWall = Instantiate(borderWall, new Vector3(mapSize-1-20, wallHieght/2, mapSize/2-20), quaternion.identity);
        BorderWallScript westWall = Instantiate(borderWall, new Vector3(0-20, wallHieght/2, mapSize/2-20), quaternion.identity);
        SetWalls(southWall, northWall, eastWall, westWall);

    }

    // Update is called once per frame
    void Update()
    {
        lava();
    }

    //This function transforms the walls to fit the egde of the gamemap and stops the player from falling off
    void SetWalls(BorderWallScript southWall, BorderWallScript northWall, BorderWallScript eastWall, BorderWallScript westWall){
        southWall.transform.localScale = new Vector3(mapSize,wallHieght,1);
        northWall.transform.localScale = new Vector3(mapSize,wallHieght,1);
        eastWall.transform.localScale = new Vector3(1,wallHieght,mapSize);
        westWall.transform.localScale = new Vector3(1,wallHieght,mapSize);

    }

    //instanciates the floortiles and saves them in an array
    void TileSpawner(int xCoord, int yCoord){
        tileArray[xCoord,yCoord] = Instantiate(floor, new Vector3(xCoord-20, 0, yCoord-20), quaternion.identity);
    }

    //This function converts the outemost layer of the map to lavaTiles at a preset rate(nextlavaSpawn)
    void lava(){
        nextLavaSpawn -= Time.deltaTime;
        (float, float, float) coords=(0,0,0);
        if (lavaTileCounter < mapSize-1 && 0 > nextLavaSpawn){
            for (int i = lavaTileCounter; i < mapSize-lavaTileCounter; i++)
            {
                //bottom row
                if(tileArray[i,lavaTileCounter]){
                    coords=(tileArray[i,lavaTileCounter].transform.position.x,0,tileArray[i,lavaTileCounter].transform.position.z);
                    Destroy(tileArray[i,lavaTileCounter].gameObject);
                    lavaTileArray[i,lavaTileCounter] = Instantiate(lavaTile, new Vector3(coords.Item1, coords.Item2, coords.Item3), quaternion.identity);
                }
                //left col
                if(tileArray[lavaTileCounter,i]){
                    coords=(tileArray[lavaTileCounter,i].transform.position.x,0,tileArray[lavaTileCounter,i].transform.position.z);
                    Destroy(tileArray[lavaTileCounter,i].gameObject);    
                    lavaTileArray[lavaTileCounter,i] = Instantiate(lavaTile, new Vector3(coords.Item1, coords.Item2, coords.Item3), quaternion.identity);
                }
                //right col
                if(tileArray[mapSize-1-lavaTileCounter,i]){
                    coords=(tileArray[mapSize-1-lavaTileCounter,i].transform.position.x,0,tileArray[mapSize-1-lavaTileCounter,i].transform.position.z);
                    Destroy(tileArray[mapSize-1-lavaTileCounter,i].gameObject);
                    lavaTileArray[mapSize-1-lavaTileCounter,i] = Instantiate(lavaTile, new Vector3(coords.Item1, coords.Item2, coords.Item3), quaternion.identity);
                }
                //top row
                if(tileArray[i,mapSize-1-lavaTileCounter]){
                    coords=(tileArray[i,mapSize-1-lavaTileCounter].transform.position.x,0,tileArray[i,mapSize-1-lavaTileCounter].transform.position.z);
                    Destroy(tileArray[i,mapSize-1-lavaTileCounter].gameObject);
                    lavaTileArray[i,mapSize-1-lavaTileCounter] = Instantiate(lavaTile, new Vector3(coords.Item1, coords.Item2, coords.Item3), quaternion.identity);
                }
            }
            nextLavaSpawn = lavaSpawnTime;
            lavaTileCounter++;
        }
    }
}
