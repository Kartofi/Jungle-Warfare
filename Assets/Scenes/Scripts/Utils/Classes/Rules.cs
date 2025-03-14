using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AntiCheatRules
{
    public static readonly float maxMoveDistance = 2.5f;
    public static readonly float headRadius = 0.62f;
    public static readonly string versionHash = "1237637816a1ef8e3a33c1191d9dea66";
    public static readonly float jumpCooldown = 1f;

}

[Serializable]
public static class RulesStatic
{
    
    public static int lobbySize = 20;

    public static float maxHealth = 100f;

    public static int respawnTime = 5000;

    public static float jumpPowerMultiplier = 1f;

    public static Rules rulesObj {
        get {
            return new Rules(lobbySize, maxHealth, respawnTime, new List<WeaponRules>());
        } 
    }
}
[Serializable]
public class LobbyRules
{
    public int lobbySize = 20;
    public float maxHealth = 100f;
    public int respawnTime = 5000;

    public float damageMultiplier = 1;
    public float walkspeedMultiplier = 1;
    public float jumpPowerMultiplier = 1;
    public float reloadTimeMultiplier = 1;
    public float reloadWalkspeedMultiplier = 1;
    public float headShotMultiplier = 1;

    public float rifleShootCooldown = 0.1f;
    public float rifleDamage = 10;
    public float rifleReloadTime = 1;
    public int rifleBulletsMax = 30;
    public float rifleHeadShotMultiplier = 1.5f;

    public float revolverShootCooldown = 0.5f;
    public float revolverDamage = 34;
    public float revolverReloadTime = 1;
    public int revolverBulletsMax = 6;
    public float revolverHeadShotMultiplier = 2;


    public LobbyRules()
    {

    }

}

[Serializable]
public class Rules
{
    public int lobbySize = 20;

    public float maxHealth = 100f;
    public float jumpPowerMultiplier = 1f;

    public int respawnTime = 5000;

    public List<WeaponRules> weaponsRules;

    public Rules(int lobbySize,float maxHealth,int respawnTime, List<WeaponRules> weaponsRules)
    {
        this.maxHealth = maxHealth;

        this.lobbySize = lobbySize;

        this.weaponsRules = weaponsRules;
        this.respawnTime = respawnTime;
    }
}


[Serializable]
public class WeaponRules
{
    public string WeaponName;

    public float shootCooldown = 0.1f;
    public float reloadWalkSpeed = 2.5f;
    public float walkSpeed = 5;
    public float recoilMultiplier = 1f;
    public float shootMaxDistance = 20f;
    public float shootIndicatorDistance = 20f;
    public int bulletsMax = 30;
    public float reloadTime = 0.5f;
    public float damage = 20f;
    public float headShotMultiplier = 1.75f;

    public WeaponRules(float shootCooldown,float shootMaxDistance, float shootIndicatorDistance, int bulletsMax, float reloadTime, float damage, float headShotMultiplier, float recoilMultiplier, float reloadWalkSpeed,float walkSpeed)
    {
        this.shootCooldown = shootCooldown;
        this.shootMaxDistance = shootMaxDistance;
        this.shootIndicatorDistance = shootIndicatorDistance;
        this.bulletsMax = bulletsMax;
        this.reloadTime = reloadTime;
        this.damage = damage;
        this.recoilMultiplier = recoilMultiplier;
        this.headShotMultiplier = headShotMultiplier;
        this.reloadWalkSpeed = reloadWalkSpeed;
        this.walkSpeed = walkSpeed;
    }
}
