using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    public int worldRepeatSizeX = 1;
    public int worldRepeatSizeY = 1;
    public int worldRepeatSizeZ = 1;
    public float worldRepeatDistanceX = 100;
    public float worldRepeatDistanceY = 100;
    public float worldRepeatDistanceZ = 100;
    public float worldOffsetX = 0;
    public float worldOffsetY = 0;
    public float worldOffsetZ = 0;

    public GameObject levelPrefab;
    public GameObject levelItemsPrefab;
    public GameObject player;

    Cube[] cubes;

    // Start is called before the first frame update
    void Start()
    {
        GenerateWorld();
        FindAllCubes();
    }

    public void FindAllCubes()
    {
		cubes = FindObjectsOfType<Cube>();
	}

	void GenerateWorld()
    {

        for (int y = -worldRepeatSizeY / 2; y < worldRepeatSizeY / 2 + 1; y++)
        {
			float _offsetY = 0;

			for (int x = -worldRepeatSizeX / 2; x < worldRepeatSizeX / 2 + 1; x++)
			{
				float _offsetX = 0;
				for (int z = -worldRepeatSizeZ / 2; z < worldRepeatSizeZ / 2 + 1; z++)
				{
					float _offsetZ = worldOffsetZ * y;
					float transX = x * worldRepeatDistanceX + _offsetX;
                    float transY = y * worldRepeatDistanceY + _offsetY;
					float transZ = z * worldRepeatDistanceZ + _offsetZ;
                    //print(z * worldRepeatDistanceZ + " : " + (z * worldRepeatDistanceZ + _offsetZ));
					Instantiate(levelPrefab, new Vector3(transX, transY, transZ), Quaternion.identity, transform);
				}

			}
		}

        if (levelItemsPrefab != null)
        {
			Instantiate(levelItemsPrefab, Vector3.zero, Quaternion.identity, transform);

            // This check exists for the Main Menu that doesn't have a Game Manager
            if (GameManager.instance != null)
                GameManager.instance.AddPillarsToArray();
		}


    }

    // Update is called once per frame
    void Update()
    {
        //print(Time.time + " : " + player.transform.position);
    }

	private void FixedUpdate()
	{
		ClampObject(player);

        foreach (Cube cube in cubes)
        {
            if (cube != null)
                ClampObject(cube.gameObject);
        }
	}

	void ClampObject(GameObject objectToClamp)
    {
        float x = objectToClamp.transform.position.x;
        float y = objectToClamp.transform.position.y;
        float z = objectToClamp.transform.position.z;

        bool clampX = false;
        bool clampY = false;
        bool clampZ = false;

        if (objectToClamp.transform.position.x > worldRepeatDistanceX / 2f)
        {
			x = objectToClamp.transform.position.x - worldRepeatDistanceX;
            clampX = true;
		}
        if (objectToClamp.transform.position.y > worldRepeatDistanceY / 2f)
        {
			y = objectToClamp.transform.position.y - worldRepeatDistanceY;
			clampY = true;
		}
		if (objectToClamp.transform.position.z > (worldRepeatDistanceZ / 2f))
        {
			z = objectToClamp.transform.position.z - worldRepeatDistanceZ;
			clampZ = true;
		}

		// Find a way to do this section better
		if (objectToClamp.transform.position.x < -worldRepeatDistanceX / 2f)
		{
			x = objectToClamp.transform.position.x + worldRepeatDistanceX;
			clampX = true;
		}
		if (objectToClamp.transform.position.y < -worldRepeatDistanceY / 2f)
		{
			y = objectToClamp.transform.position.y + worldRepeatDistanceY;
			clampY = true;
		}
		if (objectToClamp.transform.position.z < -worldRepeatDistanceZ / 2f)
		{
			z = objectToClamp.transform.position.z + worldRepeatDistanceZ;
			clampZ = true;
		}

        // If they didn't wrap this frame, apply respective offsets
        if (!clampX) x += worldOffsetX;
        if (!clampY) y += worldOffsetY;
        if (!clampZ) z += worldOffsetZ;

		if (clampX || clampY || clampZ)
        {
            Vector3 newPos = new Vector3(x, y, z);
            objectToClamp.transform.position = newPos;
		}



	}

    void ClampOffsets()
    {

    }

    
}
