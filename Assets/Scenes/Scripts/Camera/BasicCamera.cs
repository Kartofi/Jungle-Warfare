using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BasicCamera : MonoBehaviour
{
    public static BasicCamera main;
    // Start is called before the first frame update
    void Awake()
    {
        main = this;
    }
    [Header("Fov Effect")]
    public float StartFOV = 70;
    public float FOVChange = 5;
    [Header("Fov Aim")]
    public float zoomFOV = -20;


    float timeLook = 0;
    Vector3 posLookAt;

    long lookAtId = 0;
    GameObject objLookAt;
    

    private void Start()
    {
        Camera camera = GetComponent<Camera>();
        float[] distances = new float[32];
        distances[10] = 15;
        camera.layerCullDistances = distances;
        Camera.main.fieldOfView = StartFOV;
    }
    public void LookAtFor(Vector3 pos,float time)
    {
        this.posLookAt = pos;
        this.timeLook = time;
    }
    public void LookAtFor(long id, float time)
    {
        if (!Udp.client.idsObjs.ContainsKey(id) && id != Udp.client.id)
        {
            return;
        }
        lookAtId = id;
        this.timeLook = time;
    }
    public void FOVEffect(float time)
    {
        float startFOV = StartFOV;

        float endFOV = startFOV + FOVChange * Player.instance.weaponRules.recoilMultiplier;
        
        
        DOTween.To(() => Camera.main.fieldOfView, fov => Camera.main.fieldOfView = fov, endFOV, time/2).SetEase(Ease.OutCirc).onComplete = () => 
        {
            DOTween.To(() => Camera.main.fieldOfView, fov => Camera.main.fieldOfView = fov, startFOV, time / 2);
        };
    }
    bool shaking = false;
    public void ShakeEffect(float time,float magnitude)
    {
        if (shaking == true)
        {
            return;
        }
        shaking = true;
        magnitude = Mathf.Clamp(magnitude, 0.75f, float.MaxValue);
        DOTween.To(() => Player.instance.cameraRotation.z, rotation => Player.instance.cameraRotation.z = rotation, magnitude, time / 3f).onComplete = () =>
        {
            DOTween.To(() => Player.instance.cameraRotation.z, rotation => Player.instance.cameraRotation.z = rotation, -magnitude, time / 3f).onComplete = () =>
            {
                DOTween.To(() => Player.instance.cameraRotation.z, rotation => Player.instance.cameraRotation.z = rotation, 0, time / 3f).onComplete = () =>
                {
                    shaking = false;
                };
            };
        };
    }
    
    Vector3 oldLookAt;
    
    void Update()
    {
        if (timeLook > 0)
        {
            timeLook -= Time.deltaTime;
            if (timeLook <= 0)
            {
                objLookAt = null;
                timeLook = 0;
                Player.instance.canCameraRotate = true;
                
                gameObject.transform.GetChild(0).gameObject.SetActive(true);
                Player.instance.gameObject.transform.GetChild(0).gameObject.SetActive(true);
                WeaponEffects.main.mainRenderer.enabled = true;
            }
            else
            {
                Player.instance.canCameraRotate = false;
                
                if (WeaponEffects.main.gameObject.activeSelf == true)
                {
                    WeaponEffects.main.gameObject.SetActive(false);
                }
                if (WeaponEffects.main.mainRenderer.enabled == true)
                {
                    WeaponEffects.main.mainRenderer.enabled = false;
                }

                if (Player.instance.gameObject.transform.GetChild(0).gameObject.activeSelf == true)
                {
                    Player.instance.gameObject.transform.GetChild(0).gameObject.SetActive(false);
                }
            }
            if (objLookAt != null)
            {
                if (objLookAt != null && objLookAt.CompareTag("Player") == true)
                {
                    posLookAt = objLookAt.transform.position;
                }
            }else
            {
                OtherPlayer data = Udp.client.idsObjs.GetValueOrDefault(lookAtId);
                if (data != null)
                {
                    this.objLookAt = Udp.client.idsObjs.GetValueOrDefault(lookAtId).gameObject;
                }
                
            }
            gameObject.transform.LookAt(Vector3.Lerp(oldLookAt, posLookAt, Time.deltaTime));
            oldLookAt = posLookAt;
        }
        
    }
}
