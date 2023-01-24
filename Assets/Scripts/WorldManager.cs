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
    public GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        GenerateWorld();
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
					Instantiate(levelPrefab, new Vector3(transX, transY, transZ), Quaternion.identity);
				}

			}
		}

    }

    // Update is called once per frame
    void Update()
    {
        ClampPlayer();
    }

    void ClampPlayer()
    {
        float x = player.transform.position.x;
        float y = player.transform.position.y;
        float z = player.transform.position.z;

        bool clampX = false;
        bool clampY = false;
        bool clampZ = false;
        //print(player.transform.position);

        if (player.transform.position.x > worldRepeatDistanceX / 2f)
        {
			x = player.transform.position.x - worldRepeatDistanceX;
            clampX = true;
		}
        if (player.transform.position.y > worldRepeatDistanceY / 2f)
        {
			y = player.transform.position.y - worldRepeatDistanceY;
			clampY = true;
		}
		if (player.transform.position.z > (worldRepeatDistanceZ / 2f))
        {
			z = player.transform.position.z - worldRepeatDistanceZ;
			clampZ = true;
		}

		// Find a way to do this section better
		if (player.transform.position.x < -worldRepeatDistanceX / 2f)
		{
			x = player.transform.position.x + worldRepeatDistanceX;
			clampX = true;
		}
		if (player.transform.position.y < -worldRepeatDistanceY / 2f)
		{
			y = player.transform.position.y + worldRepeatDistanceY;
			clampY = true;
		}
		if (player.transform.position.z < -worldRepeatDistanceZ / 2f)
		{
			z = player.transform.position.z + worldRepeatDistanceZ;
			clampZ = true;
		}

        // If they didn't wrap this frame, apply respective offsets
        if (!clampX) x += worldOffsetX;
        if (!clampY) y += worldOffsetY;
        if (!clampZ) z += worldOffsetZ;

		if (clampX || clampY || clampZ)
        {
            Vector3 newPos = new Vector3(x, y, z);
            //print(player.transform.position + " clamping to " + newPos);
            player.transform.position = newPos;
		}



	}

    void ClampOffsets()
    {

    }

    
}
