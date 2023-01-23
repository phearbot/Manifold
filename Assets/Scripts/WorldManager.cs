using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    public int worldRepeatSizeX = 1;
    public int worldRepeatSizeY = 1;
    public int worldRepeatSizeZ = 1;
    public float worldRepeatDistanceX = 100;
    public float worldRepeatDistanceY = 100;
    public float worldRepeatDistanceZ = 100;


    public GameObject levelPrefab;
    public GameObject player;
    CharacterController controller;

    // Start is called before the first frame update
    void Start()
    {
        controller = player.GetComponent<CharacterController>();
        GenerateWorld();
    }

    void GenerateWorld()
    {
        for (int y = -worldRepeatSizeY / 2; y < worldRepeatSizeY / 2 + 1; y++)
        {
			for (int x = -worldRepeatSizeX / 2; x < worldRepeatSizeX / 2 + 1; x++)
			{
				for (int z = -worldRepeatSizeZ / 2; z < worldRepeatSizeZ / 2 + 1; z++)
				{
					// -(worldRepeatSizeX / 2) + 
					float transX = x * worldRepeatDistanceX;
                    float transY = y * worldRepeatDistanceY;
					float transZ = z * worldRepeatDistanceZ;
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

        bool clamp = false;
        //print(player.transform.position);

        if (player.transform.position.x > worldRepeatDistanceX / 2f)
        {
            print("x");
			x = player.transform.position.x - worldRepeatDistanceX;
            clamp = true;
		}
        if (player.transform.position.y > worldRepeatDistanceY / 2f)
        {
            print("y");
			y = player.transform.position.y - worldRepeatDistanceY;
			clamp = true;
		}
		if (player.transform.position.z > (worldRepeatDistanceZ / 2f))
        {
            print(player.transform.position.z + " : " + worldRepeatDistanceZ / 2f);
			z = player.transform.position.z - worldRepeatDistanceZ;
			clamp = true;
		}

		// Find a way to do this section better
		if (player.transform.position.x < -worldRepeatDistanceX / 2f)
		{
			print("x");
			x = player.transform.position.x + worldRepeatDistanceX;
			clamp = true;
		}
		if (player.transform.position.y < -worldRepeatDistanceY / 2f)
		{
			print("y");
			y = player.transform.position.y + worldRepeatDistanceY;
			clamp = true;
		}
		if (player.transform.position.z < -worldRepeatDistanceZ / 2f)
		{
			print(player.transform.position.z + " : " + (float)worldRepeatDistanceZ / 2f);
			z = player.transform.position.z + worldRepeatDistanceZ;
			clamp = true;
		}


		if (clamp)
        {
            Vector3 newPos = new Vector3(x, y, z);
            print(player.transform.position + " clamping to " + newPos);
            player.transform.position = newPos;

		}


	}
}
