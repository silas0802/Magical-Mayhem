using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using System.Security.Cryptography;
using System;
using Unity.Netcode;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private GameObject tile;
    [SerializeField] private BorderWallScript borderWall;
    [SerializeField] private GameObject lavaTile;
    [SerializeField] private MapWallScript mapWall;
    public static MapGenerator instance;
    
    private int mapSize;
    private int lavaTileCounter = 0;
    private float wallHieght = 5f;
    [SerializeField] private float lavaSpawnTime = 1f;
    [SerializeField] private float nextLavaSpawn;
    private GameObject[,] tileArray;
    [SerializeField] private float wallCutOff = 0.75f;
    [SerializeField] private float landCutOff = 0.4f;
    private MapWallScript[,] wallArray;
    
    // Start is called before the first frame update
    void Start()
    {   
        //GenerateMap(1, 1, "Medium");
    }

    void Awake(){
        if(instance == null){
            instance = this;
        }
        else{
            Destroy(gameObject);
        }
    }

    public void GenerateMap(int genType, int mapType, string Size){
        switch (Size)
        {
            case "Small": mapSize = 20;
                break;
            case "Medium": mapSize = 30;
                break;
            case "Large": mapSize = 40;
                break;
            default: mapSize = 30;
                break;
        }
        //save the floortiles
        tileArray = new GameObject[mapSize,mapSize];
        float seed = SeedGen();
        switch (mapType)
        {
            case 1: BrokenWorldGen(seed);
                break;
            default: TileSpawner();
                break;
        }
        //Obsticles on map
        switch (genType)
        {
            case 1: wallArray = new MapWallScript[mapSize, mapSize];
                SimplexWallGen(seed);
                break;
            default: break;
        }
        SetBorderWalls();
    }

    public List<(int,int)> GetUnitPlacement(){
        List<(int,int)> place = new();
        
        return place;
    }

    // Update is called once per frame
    void Update()
    {
        Lava();
    }

    //This function transforms the walls to fit the egde of the gamemap and stops the player from falling off
    private void SetBorderWalls(){
        //init borderwalls
        BorderWallScript southWall = Instantiate(borderWall, new Vector3(0, wallHieght/2, -mapSize/2-1), Quaternion.identity, transform);
        BorderWallScript northWall = Instantiate(borderWall, new Vector3(0, wallHieght/2, mapSize/2), Quaternion.identity, transform);
        BorderWallScript eastWall = Instantiate(borderWall, new Vector3(mapSize/2, wallHieght/2, 0), Quaternion.identity, transform);
        BorderWallScript westWall = Instantiate(borderWall, new Vector3(-mapSize/2-1, wallHieght/2, 0), Quaternion.identity, transform);
        southWall.transform.localScale = new Vector3(mapSize+1,wallHieght,1);
        northWall.transform.localScale = new Vector3(mapSize+1,wallHieght,1);
        eastWall.transform.localScale = new Vector3(1,wallHieght,mapSize+1);
        westWall.transform.localScale = new Vector3(1,wallHieght,mapSize+1);

    }

    //instanciates the floortiles and saves them in an array
    private void TileSpawner(){
        for (int i = 0; i < mapSize; i++)
        {
            for (int j = 0; j < mapSize; j++)
            {
                tileArray[i,j] = Instantiate(tile, new Vector3(i-mapSize/2, -0.05f, j-mapSize/2), Quaternion.identity, transform);
            }
        }
    }

    //generate a map with a lot of lava 
    private void BrokenWorldGen(float seed){
         for (int i = 0; i < mapSize; i++){
            for (int j = 0; j < mapSize; j++){
                float perlin  = Mathf.PerlinNoise(i/1.5f+seed, j/1.5f+seed);
                if(perlin  > landCutOff){
                    tileArray[i,j] = Instantiate(tile, new Vector3(i-mapSize/2, -0.05f, j-mapSize/2), Quaternion.identity, transform);   
                } 
                else{
                    tileArray[i,j] = Instantiate(lavaTile, new Vector3(i-mapSize/2, -0.05f,j-mapSize/2), Quaternion.identity, transform);
                }
            }
        }
    }

    //Generates random placement of walls based on simplex noise (it is uniformly random) it takes in 2 floats as coordinates and returns a value 
    //Then we have a cutoff value and if the noise map returns a value above this value a wall is placed
    private void SimplexWallGen(float seed){
        Vector3 coords;
        for (int i = 0; i < mapSize; i++){
            for (int j = 0; j < mapSize; j++){
                float Simplex  = noise.snoise(new float2(i+seed,j+seed));
                if (Simplex > wallCutOff && tileArray[i, j].GetComponent("TileScript") != null )
                {
                    coords = tileArray[i,j].transform.position;
                    coords.y += 1.5f;
                    wallArray[i,j] = Instantiate(mapWall, coords, Quaternion.identity, transform);
                    //wallArray[i,j].GetComponent<NetworkObject>().Spawn();
                } 
                
            }
        }
    }
    //Generate wall placement from worly noise (the cellular function returns a distence to a point in the noise map)
    //if the magnitude of the returend distence exeeds the cutoff a wall is placed at the coresponding location
    private void WorlyWallGen(float seed){
        Vector3 coords;
        Vector2 worly;
        for (int i = 0; i < mapSize; i++){
            for (int j = 0; j < mapSize; j++){
                worly = noise.cellular(new float2(i+seed,j+seed));
                if(worly.magnitude > wallCutOff){
                    coords = tileArray[i,j].transform.position;
                    wallArray[i,j] = Instantiate(mapWall, coords, Quaternion.identity, transform);
                } 
                
            }
        }
    }

    //generate a seed based on the privious one by hashing it and saving it
    private float SeedGen(){
        float oldSeed = PlayerPrefs.HasKey("Seed")? PlayerPrefs.GetFloat("Seed") : RandomFloat();
        MD5 hash = MD5.Create();
        float newSeed = BitConverter.ToInt16(hash.ComputeHash(BitConverter.GetBytes(oldSeed)));
        hash.Dispose();
        newSeed /= 1000;
        PlayerPrefs.SetFloat("Seed", newSeed);
        print(newSeed);
        //PlayerPrefs.DeleteAll();
        //newSeed = 180.26f;
        return newSeed;
    }

    private float RandomFloat(){
        System.Random random = new System.Random();
        double dub = random.NextDouble();
        double dub2 = Math.Pow(2, random.Next(0, 8));
        return (float)(dub*dub2);
        
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
                if(tileArray[i, lavaTileCounter]){
                    coords = tileArray[i, lavaTileCounter].transform.position;
                    Destroy(tileArray[i, lavaTileCounter]);
                    tileArray[i,lavaTileCounter] = Instantiate(lavaTile, coords, Quaternion.identity, transform);
                }
                //left col
                if(tileArray[lavaTileCounter,i]){
                    coords = tileArray[lavaTileCounter, i].transform.position;
                    Destroy(tileArray[lavaTileCounter, i]);
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
