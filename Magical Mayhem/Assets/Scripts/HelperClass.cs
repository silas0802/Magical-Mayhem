using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelperClass
{
    /// <summary>
    /// Casts a Ray from the mouse position on screen to the world and returns that position
    /// </summary>
    /// <param name="hasHit"></param>
    /// <returns></returns>
    public static Vector3 GetMousePosInWorld(out bool hasHit){
        Vector2 mousePos = Input.mousePosition;
        Vector3 target = new Vector3();
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(mousePos.x,mousePos.y,0));
        if (Physics.Raycast(ray, out hit)){
            hasHit = true;
            target = hit.point;
        }
        else{
            hasHit = false;
        }
        
        return target;
    }
    
}
