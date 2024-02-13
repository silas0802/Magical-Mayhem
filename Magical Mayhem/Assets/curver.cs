using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class curver : MonoBehaviour
{
    public float curveSpeed = 5f;
    private bool goingLeft = true;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if (goingLeft)
        {
            transform.Rotate(new Vector3(curveSpeed, 0, curveSpeed) * Time.deltaTime);
            if (transform.rotation.eulerAngles.z > 15 && transform.rotation.eulerAngles.z < 20 &&transform.rotation.eulerAngles.x > 15 && transform.rotation.eulerAngles.x < 20)
            {
                goingLeft = false;
            }
        }
        else
        {
            transform.Rotate(new Vector3(-curveSpeed, 0, -curveSpeed) * Time.deltaTime);
            Debug.Log(transform.rotation.eulerAngles.z +"    "+ transform.rotation.eulerAngles.x);
            if (transform.rotation.eulerAngles.z < 360-15 &&transform.rotation.eulerAngles.z > 340 && transform.rotation.eulerAngles.x < 360-5 && transform.rotation.eulerAngles.x > 340)
            {
                goingLeft = true;
            }
        }

        // if (goingLeft)
        // {
        //     if (transform.rotation.ToEuler().z > 15)
        //     {
        //         transform.Rotate(new Vector3(0, 0, curveSpeed) * Time.deltaTime);
        //         goingLeft = false;
        //     }
        //     else
        //     {
        //         transform.Rotate(new Vector3(0, 0, -curveSpeed) * Time.deltaTime);
        //     }
        // }
        // else
        // {
        //     if (transform.rotation.ToEuler().z > -15)
        //     {
        //         transform.Rotate(new Vector3(0, 0, curveSpeed) * Time.deltaTime);
        //         goingLeft = true;

        //     }
        //     else
        //     {
        //         transform.Rotate(new Vector3(0, 0, curveSpeed) * Time.deltaTime);
        //     }

        // }
    }
}
