using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    public bool lookInUpdate = true;

    public bool scaleWithDistance = false;

    public bool scaleInUpdate = false;

    public float perfectDistance = 0;

    public float maxSize = 1;
    public float minSize = 1;
    private void Start()
    {
        Vector3 lookAtPos = Player.instance.gameObject.transform.position;
        lookAtPos.y = transform.position.y;
        transform.LookAt(lookAtPos);

        if (scaleWithDistance == true)
        {
            float distance = (gameObject.transform.position - Camera.main.transform.position).magnitude;
            transform.localScale = new Vector3(1, 1, 1) * Mathf.Clamp(Mathf.Pow(distance, 2) / (Mathf.Sqrt(distance) * 100) + minSize, minSize, maxSize);
        }
    }
    void Update()
    {
       if (lookInUpdate == true)
        {
            Vector3 lookAtPos = Player.instance.gameObject.transform.position;
            lookAtPos.y = transform.position.y;
            transform.LookAt(lookAtPos);
        }
       if (scaleWithDistance == true && scaleInUpdate == true)
        {
            float distance = (gameObject.transform.position - Camera.main.transform.position).magnitude;
            transform.localScale = new Vector3(1, 1, 1) * Mathf.Clamp(Mathf.Pow(distance,2) / (Mathf.Sqrt(distance) * 100) + minSize, minSize, maxSize);
        }
    }
}
