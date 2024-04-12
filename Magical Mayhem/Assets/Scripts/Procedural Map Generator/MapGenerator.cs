using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Unity.Netcode;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private NetworkObject tile;
    [SerializeField] private NetworkObject borderWall;
    [SerializeField] private NetworkObject lavaTile;
    [SerializeField] private NetworkObject mapWall;
    [SerializeField] private NetworkObject healtCube;
    [SerializeField] private NetworkObject speedCube;
    public static MapGenerator instance;
    
    private int mapSize;
    private int mapType;
    private bool buffs;
    private int lavaTileCounter = 0;
    private readonly float wallHieght = 5f;
    [SerializeField] private float lavaSpawnTime = 10f;
    [SerializeField] private float nextLavaSpawn = 20f;
    private NetworkObject[,] tileArray;
    private NetworkObject[,] buffArray;
    [SerializeField] private float wallCutOff = 0.7f;
    [SerializeField] private float landCutOff = 0.55f;
    [SerializeField] private float upperBufs = 0.6f;
    [SerializeField] private float lowerBufs = 0.56f;
    private NetworkObject[,] wallArray;
    
    // Start is called before the first frame update
    void Start()
    {   
        mapSize = LobbySystem.mapSize;
        mapType = LobbySystem.mapType;
        buffs = LobbySystem.buffs;
    }

    void Awake(){
        if(instance == null){
            instance = this;
        }
        else{
            Destroy(gameObject);
        }
    }

    public void GenerateMap(){
        mapSize = mapSize switch
        {
            1 => 20,
            2 => 30,
            3 => 40,
            _ => 30,
        };
        
        //save the floortiles
        tileArray = new NetworkObject[mapSize,mapSize];
        SeedGen generator = new();
        float seed = generator.Seed();

        //dropdown menu
        // 1: barren
        // 2: volcano
        // 3: Ruins
        // 4: broken world
        switch (mapType)
        {
            case 1: TileSpawner();
                break;
            case 2: BrokenWorldGen(seed);
                break;
            case 3: TileSpawner();
                wallArray = new NetworkObject[mapSize, mapSize];
                SimplexWallGen(seed);
                break;
            case 4: BrokenWorldGen(seed);
                wallArray = new NetworkObject[mapSize, mapSize];
                SimplexWallGen(seed);
                break;
            default: TileSpawner();
                break;
        };
        if(buffs){
            buffArray = new NetworkObject[mapSize, mapSize];
            SimplexBuff(seed);
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
        // Lava();
     
    }

    private NetworkObject InstObj(string type, float x, float y, float z){
        Vector3 vec = new(x,y,z);
        NetworkObject obj = type switch
        {
            "tile" => Instantiate(tile, vec, Quaternion.identity, transform),
            "borderWall" => Instantiate(borderWall, vec, Quaternion.identity, transform),
            "mapWall" => Instantiate(mapWall, vec, Quaternion.identity, transform),
            "lavaTile" => Instantiate(lavaTile, vec, Quaternion.identity, transform),
            "healthBuff" => Instantiate(healtCube, vec, Quaternion.identity, transform),
            "speedBuff" => Instantiate(speedCube, vec, Quaternion.identity, transform),
            _ => Instantiate(tile, vec, Quaternion.identity, transform),
        };

        obj.Spawn(true);
        return obj;
    }
    private NetworkObject InstObj(string type, Vector3 coords)
    {
        NetworkObject obj = type switch
        {
            "tile" => Instantiate(tile, coords, Quaternion.identity, transform),
            "borderWall" => Instantiate(borderWall, coords, Quaternion.identity, transform),
            "mapWall" => Instantiate(mapWall, coords, Quaternion.identity, transform),
            "lavaTile" => Instantiate(lavaTile, coords, Quaternion.identity, transform),
            "healthBuff" => Instantiate(healtCube, coords, Quaternion.identity, transform),
            "speedBuff" => Instantiate(speedCube, coords, Quaternion.identity, transform),
            _ => Instantiate(tile, coords, Quaternion.identity, transform),
        };

        obj.Spawn(true);
        return obj;
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
                float perlin  = Mathf.PerlinNoise(i/2.5f+seed, j/2.5f+seed);
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

    private void SimplexBuff(float seed){
        Vector3 coords;
        for (int i = 0; i < mapSize; i++){
            for (int j = 0; j < mapSize; j++){
                float Simplex  = noise.snoise(new float2(i+seed,j+seed));
                if (tileArray[i, j].GetComponent("TileScript") != null )
                {
                    coords = tileArray[i,j].transform.position;
                    if(Simplex > lowerBufs && Simplex < upperBufs){
                        buffArray[i,j] = InstObj("healthBuff", coords);
                    }
                    else if(Simplex > upperBufs && Simplex < wallCutOff){
                        buffArray[i,j] = InstObj("speedBuff", coords);
                    }
                    //wallArray[i,j].GetComponent<NetworkObject>().Spawn(true);
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