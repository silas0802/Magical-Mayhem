using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private GameObject tile;
    [SerializeField] private BorderWallScript borderWall;
    [SerializeField] private GameObject lavaTile;
    [SerializeField] private GameObject mapWall;

    
    int mapSize = 41;
    int lavaTileCounter = 0;
    [SerializeField ]private float seed = 3.5f;
    float wallHieght = 5f;
    [SerializeField] float lavaSpawnTime = 1f;
    [SerializeField]float nextLavaSpawn;
    [SerializeField] float worlyCutOff = 0.5f;
    GameObject[,] tileArray;
    GameObject[,] wallArray;
    
    // Start is called before the first frame update
    void Start()
    {   
        
    }
    public void GenerateMap(int genType){
        //save the floortiles
        tileArray = new GameObject[mapSize,mapSize];
        TileSpawner();
        //set invisible border walls
        SetWalls();
        //generate walls on the map
        WallGen(genType);
    }

    // Update is called once per frame
    void Update()
    {
        Lava();
    }

    //This function transforms the walls to fit the egde of the gamemap and stops the player from falling off
    private void SetWalls(){
        //init borderwalls
        BorderWallScript southWall = Instantiate(borderWall, new Vector3(mapSize/2-20, wallHieght/2, 0-20), Quaternion.identity, transform);
        BorderWallScript northWall = Instantiate(borderWall, new Vector3(mapSize/2-20, wallHieght/2, mapSize-1-20), Quaternion.identity, transform);
        BorderWallScript eastWall = Instantiate(borderWall, new Vector3(mapSize-1-20, wallHieght/2, mapSize/2-20), Quaternion.identity, transform);
        BorderWallScript westWall = Instantiate(borderWall, new Vector3(0-20, wallHieght/2, mapSize/2-20), Quaternion.identity, transform);
        southWall.transform.localScale = new Vector3(mapSize,wallHieght,1);
        northWall.transform.localScale = new Vector3(mapSize,wallHieght,1);
        eastWall.transform.localScale = new Vector3(1,wallHieght,mapSize);
        westWall.transform.localScale = new Vector3(1,wallHieght,mapSize);

    }

    //instanciates the floortiles and saves them in an array
    private void TileSpawner(){
        for (int i = 0; i < mapSize; i++)
        {
            for (int j = 0; j < mapSize; j++)
            {
                tileArray[i,j] = Instantiate(tile, new Vector3(i-20, 0, j-20), Quaternion.identity, transform);
            }
        }
    }

    public void WallGen(int choose){
        wallArray = new GameObject[mapSize, mapSize];
        switch (choose)
        {
            case 1: PerlinWallGen();
                break;
            default: WorlyWallGen();
                break;
        }
    }
    private void PerlinWallGen(){
        Vector3 coords;
        for (int i = 0; i < mapSize; i++){
            for (int j = 0; j < mapSize; j++){
                float perlin = noise.snoise(new float2(i+seed,j+seed*2));
                if(perlin > worlyCutOff){
                    coords = tileArray[i,j].transform.position;
                    wallArray[i,j] = Instantiate(mapWall, coords, Quaternion.identity, transform);
                } 
                
            }
        }
    }
    //Generate wall placement from worly noise (the cellular function returns a distence to a point in the noise map)
    //if the magnitude of the returend distence exeeds the cutoff a wall is placed at the coresponding location
    private void WorlyWallGen(){
        Vector3 coords;
        Vector2 worly;
        for (int i = 0; i < mapSize; i++){
            for (int j = 0; j < mapSize; j++){
                worly = noise.cellular(new float2(i+seed,j+seed));
                if(worly.magnitude > worlyCutOff){
                    coords = tileArray[i,j].transform.position;
                    wallArray[i,j] = Instantiate(mapWall, coords, Quaternion.identity, transform);
                } 
                
            }
        }
    }

    //Resets the map
    public void ResetMap(){
        Transform[] mapChildren = transform.GetComponentsInChildren<Transform>();
        for(int i = 1; i < mapChildren.Length; i++){
            DestroyImmediate(mapChildren[i].gameObject);
        }
    }

    //This function converts the outemost layer of the map to lavaTiles at a preset rate(nextlavaSpawn)
    private void Lava(){
        nextLavaSpawn -= Time.deltaTime;
        Vector3 coords;
        if (lavaTileCounter < mapSize-1 && 0 > nextLavaSpawn){
            for (int i = lavaTileCounter; i < mapSize-lavaTileCounter; i++)
            {
                //bottom row
                if(tileArray[i,lavaTileCounter]){
                    coords = tileArray[i,lavaTileCounter].transform.position;
                    Destroy(tileArray[i,lavaTileCounter].gameObject);
                    tileArray[i,lavaTileCounter] = Instantiate(lavaTile, coords, Quaternion.identity, transform);
                }
                //left col
                if(tileArray[lavaTileCounter,i]){
                    coords = tileArray[lavaTileCounter,i].transform.position;
                    Destroy(tileArray[lavaTileCounter,i].gameObject);    
                    tileArray[lavaTileCounter,i] = Instantiate(lavaTile, coords, Quaternion.identity, transform);
                }
                //right col
                if(tileArray[mapSize-1-lavaTileCounter,i]){
                    coords = tileArray[mapSize-1-lavaTileCounter,i].transform.position;
                    Destroy(tileArray[mapSize-1-lavaTileCounter,i].gameObject);
                    tileArray[mapSize-1-lavaTileCounter,i] = Instantiate(lavaTile, coords, Quaternion.identity, transform);
                }
                //top row
                if(tileArray[i,mapSize-1-lavaTileCounter]){
                    coords = tileArray[i,mapSize-1-lavaTileCounter].transform.position;
                    Destroy(tileArray[i,mapSize-1-lavaTileCounter].gameObject);
                    tileArray[i,mapSize-1-lavaTileCounter] = Instantiate(lavaTile, coords, Quaternion.identity, transform);
                }
            }
            nextLavaSpawn = lavaSpawnTime;
            lavaTileCounter++;
        }
    }
}
