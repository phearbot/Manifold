using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeSpawner : MonoBehaviour
{
    [SerializeField] Cube spawnedCube;
    GameObject cubePrefab;


    // Start is called before the first frame update
    void Start()
    {
        cubePrefab = Resources.Load("Prefabs/Cube") as GameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Cube SpawnNewCube()
    {
        // call the dustruction method on the old cube (should fade over time)
        Destroy(spawnedCube.gameObject);
        spawnedCube = Instantiate(cubePrefab, transform.position, Quaternion.Euler(transform.up)).GetComponent<Cube>();
        spawnedCube.transform.rotation = transform.rotation;
		return spawnedCube;
    }
}
