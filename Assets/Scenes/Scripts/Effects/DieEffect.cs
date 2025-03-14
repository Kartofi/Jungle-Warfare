using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieEffect : MonoBehaviour
{
    public static DieEffect main;

    public GameObject particle;

    Queue<Vector3> dieEffectQueue = new Queue<Vector3>();

    public int emitCount;
    // Start is called before the first frame update
    void Awake()
    {
        main = this;
    }
    public void AddPosition(Vector3 pos)
    {
        dieEffectQueue.Enqueue(pos);
    }
    // Update is called once per frame
    void Update()
    {
        if (dieEffectQueue.Count > 0)
        {
            for(int i =0; i< dieEffectQueue.Count; i++)
            {
                Vector3 pos = dieEffectQueue.Dequeue();

                GameObject clone = Instantiate(particle);
                clone.transform.position = pos;

                clone.GetComponent<ParticleSystem>().Emit(emitCount);

            }
        }
    }
}
