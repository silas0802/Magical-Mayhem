using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using System.Security.Cryptography;
using System;
using Unity.Netcode;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private NetworkObject tile;
    [SerializeField] private NetworkObject borderWall;
    [SerializeField] private NetworkObject lavaTile;
    [SerializeField] private NetworkObject mapWall;
    public static MapGenerator instance;
    
    private int mapSize;
    private int lavaTileCounter = 0;
    private float wallHieght = 5f;
    [SerializeField] private float lavaSpawnTime = 1f;
    [SerializeField] private float nextLavaSpawn;
    private NetworkObject[,] tileArray;
    [SerializeField] private float wallCutOff = 0.75f;
    [SerializeField] private float landCutOff = 0.55f;
    private NetworkObject[,] wallArray;
    
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
        tileArray = new NetworkObject[mapSize,mapSize];
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
            case 1: wallArray = new NetworkObject[mapSize, mapSize];
                SimplexWallGen(seed);
                break;
            default: break;
        }
        //SetSpawnPoints();
        SetBorderWalls();
    }

    public List<(int,int)> GetSpanPoints(){
        List<(int,int)> points = new();
        int sideLen = mapSize/2;
        points.Add((sideLen,sideLen));
        points.Add((sideLen,-sideLen));
        points.Add((-sideLen,sideLen));
        points.Add((-sideLen,-sideLen));
        points.Add((sideLen,0));
        points.Add((0,sideLen));
        points.Add((-sideLen,0));
        points.Add((0,-sideLen));

        return points;
    }
    private void SetSpawnPoints(){
        int sideLen = mapSize/2;
        PlaceSpawnTile(sideLen,sideLen);
        PlaceSpawnTile(-sideLen,sideLen);
        PlaceSpawnTile(-sideLen,-sideLen);
        PlaceSpawnTile(sideLen,-sideLen);
        PlaceSpawnTile(sideLen,0);
        PlaceSpawnTile(0,sideLen);
        PlaceSpawnTile(-sideLen,0);
        PlaceSpawnTile(0,-sideLen);

    }
    //if the floor is lava change it to floortile and if there is a wall delete it
    private void PlaceSpawnTile(int x, int y){
        if(tileArray[x,y].GetComponent<LavaTileScript>()){
            tileArray[x,y].Despawn();
            tileArray[x,y] = InstObj("tile",x, -0.05f, y);
        }
        if(wallArray[x,y]){
            wallArray[x,y].Despawn();
        }
    }
    

    // Update is called once per frame
    void Update()
    {
        Lava();
    }

    private NetworkObject InstObj(string type, float x, float y, float z){
        Vector3 vec = new(x,y,z);
        NetworkObject obj;
        switch (type)
        {
            case "tile": obj = Instantiate(tile, vec, Quaternion.identity, transform);
                obj.Spawn(true);
                return obj;
            case "borderWall": obj = Instantiate(borderWall, vec, Quaternion.identity, transform);
                obj.Spawn(true);
                return obj;
            case "mapWall": obj = Instantiate(mapWall, vec, Quaternion.identity, transform);
                obj.Spawn(true);
                return obj;
            case "lavaTile": obj = Instantiate(lavaTile, vec, Quaternion.identity, transform);
                obj.Spawn(true);
                return obj;
            default: obj = Instantiate(tile, vec, Quaternion.identity, transform);
                obj.Spawn(true);
                return obj;
        };
    }
    private NetworkObject InstObj(string type, Vector3 coords)
    {
        NetworkObject obj;
        switch (type)
        {
            case "tile": obj = Instantiate(tile, coords, Quaternion.identity, transform);
                obj.Spawn(true);
                return obj;
            case "borderWall": obj = Instantiate(borderWall, coords, Quaternion.identity, transform);
                obj.Spawn(true);
                return obj;
            case "mapWall": obj = Instantiate(mapWall, coords, Quaternion.identity, transform);
                obj.Spawn(true);
                return obj;
            case "lavaTile": obj = Instantiate(lavaTile, coords, Quaternion.identity, transform);
                obj.Spawn(true);
                return obj;
            default: obj = Instantiate(tile, coords, Quaternion.identity, transform);
                obj.Spawn(true);
                return obj;
        };
    }

    //This function transforms the walls to fit the egde of the gamemap and stops the player from falling off
    private void SetBorderWalls(){
        //init borderwalls
        NetworkObject southWall = InstObj("borderWall",0, wallHieght / 2, -mapSize/2-1);
        NetworkObject northWall = InstObj("borderWall",0, wallHieght / 2, mapSize/2);
        NetworkObject eastWall = InstObj("borderWall",mapSize/2, wallHieght / 2, 0);
        NetworkObject westWall = InstObj("borderWall",-mapSize/2-1, wallHieght/2, 0);
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
                tileArray[i,j] = InstObj("tile",i-mapSize/2, -0.05f, j-mapSize/2);
            }
        }
    }

    //generate a map with a lot of lava 
    private void BrokenWorldGen(float seed){
         for (int i = 0; i < mapSize; i++){
            for (int j = 0; j < mapSize; j++){
                float perlin  = Mathf.PerlinNoise(i/1.5f+seed, j/1.5f+seed);
                if(  landCutOff > perlin){
                    tileArray[i,j] = InstObj("tile",i-mapSize/2, -0.05f, j-mapSize/2);   
                } 
                else{
                    tileArray[i,j] = InstObj("lavaTile",i-mapSize/2, -0.05f,j-mapSize/2);
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
                    wallArray[i,j] = InstObj("mapWall", coords);
                    //wallArray[i,j].GetComponent<NetworkObject>().Spawn(true);
                } 
            }
        }
    }

    //Generate wall placement from worly noise (the cellular function returns a distence to a point in the noise map)
    //if the magnitude of the returend distence exeeds the cutoff a wall is placed at the coresponding location
    // private void WorlyWallGen(float seed){
    //     Vector3 coords;
    //     Vector2 worly;
    //     for (int i = 0; i < mapSize; i++){
    //         for (int j = 0; j < mapSize; j++){
    //             worly = noise.cellular(new float2(i+seed,j+seed));
    //             if(worly.magnitude > wallCutOff){
    //                 coords = tileArray[i,j].transform.position;
    //                 wallArray[i,j] = Instantiate(mapWall, coords, Quaternion.identity, transform);
    //             } 
                
    //         }'
    //     }
    // }

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
                    tileArray[i, lavaTileCounter].Despawn();
                    tileArray[i,lavaTileCounter] = InstObj("lavaTile", coords);
                    
                }
                //left col
                if(tileArray[lavaTileCounter,i]){
                    coords = tileArray[lavaTileCounter, i].transform.position;
                    tileArray[lavaTileCounter, i].Despawn();
                    tileArray[lavaTileCounter,i] = InstObj("lavaTile", coords);
                }
                //right col
                if(tileArray[mapSize-1-lavaTileCounter,i]){
                    coords = tileArray[mapSize-1-lavaTileCounter,i].transform.position;
                    tileArray[mapSize-1-lavaTileCounter,i].Despawn();
                    tileArray[mapSize-1-lavaTileCounter,i] = InstObj("lavaTile", coords);
                }
                //top row
                if(tileArray[i,mapSize-1-lavaTileCounter]){
                    coords = tileArray[i,mapSize-1-lavaTileCounter].transform.position;
                    tileArray[i,mapSize-1-lavaTileCounter].Despawn();
                    tileArray[i,mapSize-1-lavaTileCounter] = InstObj("lavaTile", coords);
                }
            }
            nextLavaSpawn = lavaSpawnTime;
            lavaTileCounter++;
        }
    }

    
}
