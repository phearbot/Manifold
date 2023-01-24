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
            return Color.white;
            
     }
}
