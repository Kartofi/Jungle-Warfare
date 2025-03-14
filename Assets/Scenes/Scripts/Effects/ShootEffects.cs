using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

class ShootEffectsQueue
{
    public long start;
    public Vector3 end;

    public Material hitMaterial;
    public Color hitColor;
    public bool playerHit;
    public float emissionIntensity;

    public ShootEffectsQueue(long start, Vector3 end, Material hitMaterial = null, Color hitColor = new Color(), bool playerHit = false, float emissionIntensity = 1)
    {
        this.start = start;
        this.end = end;
        this.hitMaterial = hitMaterial;

        this.hitColor = hitColor;
        this.playerHit = playerHit;
        this.emissionIntensity = emissionIntensity;
}
}
public class ShootEffects : MonoBehaviour
{
    public static ShootEffects main;
    private void Awake()
    {
        
        main = this;
    }
    [Header("Prefabs")]
    public GameObject BulletLine;

    public GameObject HitEffectObj;
    public GameObject BloodEffectObj;
    public GameObject DamageIndicatorHud;
    [Header("Settings")]
    public float maxDistance = 0.5f;
    public float minDistance = 0.1f;
    [Header("Local Settings")]
    public Camera weaponCamera;

    private Queue<ShootEffectsQueue> queue = new Queue<ShootEffectsQueue>();
    

    public void Create(long start, Vector3 end, Color hitColor = new Color(),bool playerHit = false,float emissionIntensity = 1)
    {
        queue.Enqueue(new ShootEffectsQueue(start, end,null, hitColor, playerHit, emissionIntensity));
    }
    private void Update()
    {
        if (queue.Count >= 1)
        {
            for(int i = 0; i < queue.Count; i++)
            {
                ShootEffectsQueue item = queue.Dequeue();

               
                if (item.start.Equals(Udp.client.id))
                {
                    GameObject obj = Instantiate(BulletLine);
                    Vector3 viewPort = weaponCamera.WorldToScreenPoint(WeaponData.local.GunTip.transform.position);
                    Vector3 start = Camera.main.ScreenToWorldPoint(viewPort);
                    if (WeaponData.local.Name == "Revolver")
                    {
                        SoundFXManager.main.SpawnSoundAt(start, SoundFX.shoot,1);
                    }
                    else
                    {

                        SoundFXManager.main.SpawnSoundAt(start, SoundFX.shoot);
                    }

                    obj.transform.position = start;
                    obj.transform.LookAt(item.end, Vector3.up);
                    obj.GetComponent<LineRenderer>().SetPosition(1, new Vector3(0, 0, (item.end - start).magnitude));
                }
                else
                {
                    OtherPlayer playerData = Udp.client.idsObjs.GetValueOrDefault(item.start);
                    GameObject playerObj = playerData == null ? null : playerData.gameObject;

                    if (playerObj == null)
                    {
                        continue;
                    }

                    GameObject obj = Instantiate(BulletLine);
                    Vector3 start = playerObj.GetComponent<OtherPlayer>().weapon.GunTip.transform.position;
                    if (playerObj.GetComponent<OtherPlayer>().weapon.Name == "Revolver")
                    {
                        SoundFXManager.main.SpawnSoundAt(start, SoundFX.shoot, 1);
                    }
                    else
                    {

                        SoundFXManager.main.SpawnSoundAt(start, SoundFX.shoot);
                    }
                   
                    obj.transform.position = start;
                    obj.transform.LookAt(item.end, Vector3.up);
                    obj.GetComponent<LineRenderer>().SetPosition(1, new Vector3(0, 0, (item.end - start).magnitude));
                    foreach (ParticleSystem system in playerObj.GetComponent<OtherPlayer>().weapon.particles)
                    {
                        system.Emit(2);
                    }
                    HitEffect(item.end, item.playerHit, false, null, item.hitColor, item.emissionIntensity);
                }
            }
        }
    }

    public void HitEffect(Vector3 position, bool playerHit = false, bool headShot = false, Material hitMaterial = null, Color colorHit = new Color(), float emissionIntensity = 1, bool isLocal = false, Vector3 playerPos = new Vector3())
    {
        if (playerHit == false)
        {
            GameObject obj = Instantiate(HitEffectObj);
            obj.transform.position = position;

            Renderer renderer = obj.GetComponent<ParticleSystemRenderer>();
            
            if (hitMaterial == null)
            {
                Color BaseColor = colorHit * emissionIntensity;
                renderer.material.SetColor("_BaseColor", BaseColor);
                renderer.material.SetColor("1st_ShadeColor", new Color(BaseColor.r - 0.05f, BaseColor.g - 0.05f, BaseColor.b - 0.05f));
            }
            else
            {
                Color emissionColor = hitMaterial.GetColor("_EmissionColor");
                float intensity = (emissionColor.r + emissionColor.g + emissionColor.b) / 3f + 1;

                Color BaseColor = hitMaterial.color * intensity;

                renderer.material.SetColor("_BaseColor", BaseColor);
                renderer.material.SetColor("1st_ShadeColor", BaseColor);

            }

            obj.GetComponent<ParticleSystem>().Emit(5);
            Destroy(obj, 1f);
        }
        else
        {
            GameObject obj = Instantiate(BloodEffectObj);
            obj.transform.position = position;
            obj.GetComponent<ParticleSystem>().Emit(5);

            if (isLocal == true)
            {
                GameObject damageIndicator = Instantiate(DamageIndicatorHud);

                damageIndicator.transform.position = playerPos + Vector3.up * 2.2f + new Vector3(Random.Range(-0.1f,0.1f), Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f));
                if (headShot == true)
                {
                    damageIndicator.transform.GetChild(0).GetComponent<TMP_Text>().color = Color.yellow;
                    damageIndicator.transform.GetChild(0).GetComponent<TMP_Text>().text = (Player.instance.weaponRules.damage * Player.instance.weaponRules.headShotMultiplier).ToString();
                }
                else
                {
                    damageIndicator.transform.GetChild(0).GetComponent<TMP_Text>().text = (Player.instance.weaponRules.damage).ToString();
                }
                
            }
            
            Destroy(obj, 1f);
        }
    }

}
