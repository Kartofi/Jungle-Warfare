using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Random = UnityEngine.Random;

public class WeaponSystem : MonoBehaviour
{
    public static WeaponSystem main;
    private void Awake()
    {
        main = this;
    }

    long lastShootTime = 0;

    public float precision = 1;

    public int shotsInRow = 0;



    public void HandleCursorEffects()
    {
        if (Player.instance.weaponRules == null)
        {
            return;
        }
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        bool hitPlayer = false;
        if (Physics.Raycast(ray, out hit, Player.instance.weaponRules.shootMaxDistance))
        {
            PlayerHitBoxData hitBox = hit.collider.gameObject.GetComponent<PlayerHitBoxData>();
            if (hitBox != null && hitBox.CompareTag("PlayerHitBox") == true)
            {
                if (CursorEffects.instance.cursorParts[0].color != Color.red)
                {
                    CursorEffects.instance.TurnRed();
                }
                hitPlayer = true;
            }
        }
        else
        {
            if (CursorEffects.instance.cursorParts[0].color != Color.white)
            {
                CursorEffects.instance.TurnWhite();
            }
        }
        if (hitPlayer == false)
        {
            if (CursorEffects.instance.cursorParts[0].color != Color.white)
            {
                CursorEffects.instance.TurnWhite();
            }
        }
    }

    public void HandleShooting()
    {
        if (Player.instance.weaponRules == null || Player.instance.reloading == true)
        {
            return;
        }
        
        Vector3 bloom = new Vector3((1f - precision) * RandomSign() * 0.025f, (1f - precision) * RandomSign() * 0.025f, 0);

        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0) + bloom);
        RaycastHit hit;

        BulletsDisplay.main.UpdateBulletsBar();
        foreach (ParticleSystem system in WeaponData.local.particles)
        {
            system.Emit(2);
        }
        WeaponEffects.main.ShootEffect();
        BasicCamera.main.FOVEffect(Player.instance.weaponRules.shootCooldown);
        BasicCamera.main.ShakeEffect(Player.instance.weaponRules.shootCooldown, Player.instance.weaponRules.recoilMultiplier/2f);

        if (Physics.Raycast(ray, out hit, Player.instance.weaponRules.shootMaxDistance))
        {
            PlayerHitBoxData hitBox = hit.collider.gameObject.GetComponent<PlayerHitBoxData>();
            
            TcpRequest shootIndicator = new TcpRequest("shoot", hit.point);

            if (hitBox != null && hitBox.CompareTag("PlayerHitBox") == true)
            {
                Debug.Log(hitBox.head);
                Event shootEvent = new Event("shoot", hit.point, hitBox.playerData.id, "damageHit", hitBox.head);

                Udp.client.SendData(shootEvent);

                shootIndicator.playerHit = true;

                
                ShootEffects.main.HitEffect(hit.point,true, hitBox.head, isLocal:true,playerPos: hitBox.playerData.position);
            }
            else
            {
                Renderer renderer = hit.collider.gameObject.GetComponent<Renderer>();
                if (renderer != null)
                {
                    Material material = renderer.material;

                    Color HitColor = material == null ? Color.white : material.color;
                    shootIndicator.hitColor = HitColor;
                    float emission = MaterialsHandle.GetEmissionMultiplier(material);
                    shootIndicator.emissionIntensity = emission;

                    ShootEffects.main.HitEffect(hit.point, false, false, material, HitColor, emission);
                }
            }
            
            shootIndicator.shootType = "shootIndicator";
            Udp.client.SendData(shootIndicator);

            ShootEffects.main.Create(Udp.client.id, hit.point);
        }
        else
        {
            TcpRequest shootIndicator = new TcpRequest("shoot", ray.origin + ray.direction * Player.instance.weaponRules.shootIndicatorDistance);
            shootIndicator.shootType = "shootIndicator";

            Udp.client.SendData(shootIndicator);

            ShootEffects.main.Create(Udp.client.id, ray.origin + ray.direction * Player.instance.weaponRules.shootIndicatorDistance);
        }

        long shootTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        if (shootTime - lastShootTime < Player.instance.weaponRules.shootCooldown * 1000 * 1.5f)
        {
            shotsInRow++;
        }
        else
        {
            shotsInRow = 0;
        }
        lastShootTime = shootTime;
    }

    public void HandleReload()
    {
        if (Player.instance.weaponRules.bulletsMax == 0)
        {
            return;
        }
        if (Player.instance.bullets < Player.instance.weaponRules.bulletsMax && Player.instance.reloading == false)
        {
            WeaponAnimations.local.Reload();
            Player.instance.speed = Player.instance.weaponRules.reloadWalkSpeed;

            TCP.main.SendData(new Event("reload"));
            
            Player.instance.reloading = true;

            WeaponEffects.main.canMoveIdle = false;

            Invoke("ResetPlayer", Player.instance.weaponRules.reloadTime);
        }
    }


    private void Update()
    {
        if (Udp.client.Rules.weaponsRules.Count <= 0)
        {
            return;
        }
        long now = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        
        if (now - lastShootTime > Player.instance.weaponRules.shootCooldown * 1000 * 1.5f && shotsInRow != 0) { 
            shotsInRow = 0;
        }

        float precision = CalculatePrecision();

        CursorEffects.instance.PrecisionSize(precision);
        this.precision = precision;

    }

    void ResetPlayer()
    {
        Player.instance.speed = Player.instance.weaponRules.walkSpeed;

        WeaponEffects.main.canMoveIdle = true;
        WeaponEffects.main.canMoveIdleInside = true;
    }

    float CalculatePrecision()
    {
        return 1 - Mathf.Clamp((float)Math.Pow(shotsInRow,1.21d) * 1 / 10f,0f,1f);
    }
    int RandomSign() {
        return Random.Range(0, 2) * 2 - 1;
    }
}
