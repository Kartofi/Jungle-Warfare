using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherPlayersAnimations : MonoBehaviour
{
    public static OtherPlayersAnimations main;

    private void Awake()
    {
        main = this;
    }
    Queue<long> reloadQueue = new Queue<long>();

    public void Reload(long id)
    {
        reloadQueue.Enqueue(id);
    }

    public void Update()
    {
        if (reloadQueue.Count > 0)
        {
            for(int i = 0; i < reloadQueue.Count; i++)
            {
                OtherPlayer playerData = Udp.client.idsObjs.GetValueOrDefault(reloadQueue.Dequeue());
                if (playerData == null){
                    continue;
                }
                GameObject obj = playerData.gameObject;
                if (obj != null && obj.CompareTag("Player") == true)
                {
                    obj.GetComponent<OtherPlayer>().torsoInvisible.GetComponent<WeaponAnimations>().Reload();
                }
            }
        }
    }
}
