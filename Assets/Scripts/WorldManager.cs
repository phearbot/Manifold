using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    public int worldRepeatSizeX = 1;
    public int worldRepeatSizeY = 1;
    public float worldRepeatDistanceX = 100;
    public float worldRepeatDistanceZ = 100;

    public GameObject levelPrefab;

    // Start is called before the first frame update
    void Start()
    {
        GenerateWorld();
    }

    void GenerateWorld()
    {
        for (int x = 0; x < worldRepeatSizeX; x++)
        {
            for (int z = 0; z < worldRepeatSizeY; z++)
            {
                float transX = x * worldRepeatDistanceX;
                float transZ = z * worldRepeatDistanceZ;
                Instantiate(levelPrefab, new Vector3(transX,0, transZ), Quaternion.identity);
            }

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
