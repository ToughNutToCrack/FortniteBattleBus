using UnityEngine;

public class BusController : MonoBehaviour
{
    public Terrain terr;
    [Range(0, 1000)]
    public float customHeight = 300;
    public float movementSpeed = 10;
    public Vector2 mapSize = new Vector2(1100,1100);
    public Vector2 maxOffsetFromCenter = new Vector2(50, 50);

    [Space]
    public GameEvent busIn, busOut;
    
    Vector3 terrSize, spawnPos, mapCenter, navDir;
    void Start()
    {
        //terrSize = terr.terrainData.size;
        terrSize = new Vector3(mapSize.x, 0, mapSize.y);
        SpawnBus();
        mapCenter = new Vector3(terrSize.x / 2 + Random.Range(0, maxOffsetFromCenter.x), 0, terrSize.z / 2 + Random.Range(0, maxOffsetFromCenter.y));
        navDir = mapCenter - spawnPos;
        transform.rotation = Quaternion.LookRotation(new Vector3(navDir.x, 0, navDir.z));
    }

    private void SpawnBus()
    {
        //0 Left, 1 Down, 2 Right, 3 Top
        var randomEdge = Random.Range(0, 4);
        switch (randomEdge)
        {
            case 0:
                spawnPos = new Vector3(0, customHeight, Random.Range(0, terrSize.z));
                break;
            case 1:
                spawnPos = new Vector3(Random.Range(0, terrSize.x), customHeight, 0);
                break;
            case 2:
                spawnPos = new Vector3(terrSize.x, customHeight, Random.Range(0, terrSize.z));
                break;
            case 3:
                spawnPos = new Vector3(Random.Range(0, terrSize.x), customHeight, terrSize.z);
                break;
        }
        
        transform.position = spawnPos;
    }

    void Update()
    {
        var newPos = transform.position + (navDir.normalized * movementSpeed * Time.deltaTime);
        transform.position = new Vector3(newPos.x, customHeight, newPos.z);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("MapArea"))
            busIn.Raise();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("MapArea"))
            busOut.Raise();
    }

}
