using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalColorMapper : MonoBehaviour
{
    public Color xPos;
    public Color yPos;
    public Color zPos;
    public Color xNeg;
    public Color yNeg;
    public Color zNeg;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Color MapNormalToColor(Vector3 normal)
    {
        if (normal == Vector3.right)
            return xPos;
        else if (normal == Vector3.up)
            return yPos;
        else if (normal == Vector3.forward)
            return zPos;
        else if (normal == Vector3.left)
            return xNeg;
        else if (normal == Vector3.down)
            return yNeg;
        else if (normal == Vector3.back)
            return zNeg;
        else
            //print(normal + " didn't map to anything");
            //print((normal == Vector3.forward));
            return Color.white;
            
     }

    public string MapNormalToSFX(Vector3 normal)
    {
        if (normal == Vector3.right)
            return "Grav1";
        else if (normal == Vector3.up)
            return "Grav2";
        else if (normal == Vector3.forward)
            return "Grav3";
        else if (normal == Vector3.left)
            return "Grav4";
        else if (normal == Vector3.down)
            return "Grav5";
        else if (normal == Vector3.back)
            return "Grav6";
        else
            return "Grav1";

	}

    public void MapMaterialColors(Material mat)
    {
        mat.SetColor("_xPosColor", xPos);
        mat.SetColor("_xNegColor", xNeg);
		mat.SetColor("_yPosColor", yPos);
		mat.SetColor("_yNegColor", yNeg); 
        mat.SetColor("_zPosColor", zPos);
		mat.SetColor("_zNegColor", zNeg);
	}
}
