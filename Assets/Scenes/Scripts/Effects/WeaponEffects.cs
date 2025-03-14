using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using DG.Tweening.Core;

public class WeaponEffects : MonoBehaviour
{
    public static WeaponEffects main;

    [Header("Weapon")]
    public GameObject Hands;
    public SkinnedMeshRenderer mainRenderer;

    [Header("Recoil Weapon")]
    public float RecoilZ = -0.1f;
    public float RecoilRotate = -5f;

    [Header("Idle Move Weapon")]
    public float idleY = -0.26f;
    public float idleMoveTime = 0.3f;

    [Header("Jump Move Weapon")]
    public float jumpY = -0.37f;
    public float fallingY = -0.1f;
    public float jumpMoveTime = 0.1f;

    [Header("Sway Weapon")]
    public float swayPower = 5f;
    public float rotatePower = 1.25f;
    public float swaySpeed = 30f;
    public Ease easeSpaceOut;
    public Ease easeGoBack;

    private Vector3 startRotation;
    private float startZ;
    private float startY;
    private float deltaTime = 0.008f;

    private void Awake()
    {
        main = this;
    }
    private void OnEnable()
    {
        main = this;
    }
    private void Start()
    {
        startRotation = transform.rotation.eulerAngles;
        startZ = transform.localPosition.z;
        startY = transform.localPosition.y;
    }
    [Header("Background")]
    public bool canMoveIdle = true;
    public bool canMoveIdleInside = true;

    private Quaternion _targetRotation = Quaternion.identity;

    public bool jumpEffectRunning = false;

    public void ShootEffect()
    {
        canMoveIdle = false;

        float tweenTime = Player.instance.weaponRules.shootCooldown / 3;

        Vector3 rotation = new Vector3(startRotation.x + RecoilRotate * Player.instance.weaponRules.recoilMultiplier, startRotation.y, startRotation.z);

        transform.DOLocalRotate(rotation, tweenTime).onComplete = ()=>
        {
            transform.DOLocalRotate(startRotation, tweenTime);
        };
        transform.DOLocalRotate(startRotation, tweenTime).SetDelay(tweenTime);

        transform.DOLocalMoveZ(startZ + RecoilZ * Player.instance.weaponRules.recoilMultiplier, tweenTime).onComplete = () => {
            transform.DOLocalMoveZ(startZ, tweenTime).onComplete = () => { 
                canMoveIdle = true;
                canMoveIdleInside = true;
            };
        };
        
    }

    public void JumpEffect()
    {
        jumpEffectRunning = true;
        canMoveIdle = false;

        transform.DOLocalMoveY(jumpY, jumpMoveTime/4).onComplete = () => {
            transform.DOLocalMoveY(fallingY, jumpMoveTime / 2).SetDelay(jumpMoveTime / 4);
        };
        
    }
    public void LandEffect()
    {
        canMoveIdle = false;

        transform.DOLocalMoveY(jumpY, jumpMoveTime / 3).onComplete = () => {
            transform.DOLocalMoveY(startY, jumpMoveTime / 2).onComplete = () => {
                if (Player.instance.state == (int)PlayerStates.falling)
                {
                    canMoveIdle = false;
                    canMoveIdleInside = false;
                }
                else
                {
                    canMoveIdle = true;
                    canMoveIdleInside = true;
                    jumpEffectRunning = false;
                }
            };
        };
    }

    public void FallingWeapon()
    {
        jumpEffectRunning = true;
        canMoveIdle = false;

        transform.DOLocalMoveY(fallingY, jumpMoveTime / 1);
    }
    public void WeaponSway()
    {
        float x = Input.GetAxisRaw("Mouse X") * swayPower;
        float y = Input.GetAxisRaw("Mouse Y") * swayPower;

        float right = Input.GetAxisRaw("Horizontal");
        float front = Input.GetAxisRaw("Vertical");

        Vector3 rotation = new Vector3(y, x, -rotatePower * right);

        if (front < 0)
        {
            rotation.x += rotatePower * front;
            startRotation.x = rotatePower * front;
        }
        else
        {
            startRotation.x = 0;
        }
        
        _targetRotation = Quaternion.Euler(rotation);
    }
    public void IdleMove()
    {
        Tween spaceOut = transform.DOLocalMoveY(idleY, idleMoveTime);
        spaceOut.SetEase(easeSpaceOut);

        spaceOut.onUpdate = () =>
        {
            if (canMoveIdle == false)
            {
                spaceOut.Kill();
            }
        };
        spaceOut.onComplete = () => {
            Tween goBack = transform.DOLocalMoveY(startY, idleMoveTime);
            goBack.SetEase(easeGoBack);

            goBack.onComplete = () => canMoveIdleInside = true;

            goBack.onUpdate = () => {
                if (canMoveIdle == false)
                {
                    goBack.Kill();
                }
            };

            canMoveIdleInside = false;
        };
    }
    public void RunMove()
    {
        Tween spaceOut = transform.DOLocalMoveY(idleY, idleMoveTime/2);
        spaceOut.SetEase(easeSpaceOut);

        spaceOut.onUpdate = () =>
        {
            if (canMoveIdle == false)
            {
                spaceOut.Kill();
            }
        };
        spaceOut.onComplete = () => {
            Tween goBack = transform.DOLocalMoveY(startY, idleMoveTime/2);
            goBack.SetEase(easeGoBack);

            goBack.onComplete = () => canMoveIdleInside = true;

            goBack.onUpdate = () => {
                if (canMoveIdle == false)
                {
                    goBack.Kill();
                }
            };

            canMoveIdleInside = false;
        };
    }
    public void Update()
    {
        transform.localRotation = Quaternion.RotateTowards(transform.localRotation, _targetRotation, swaySpeed * deltaTime);
        if (canMoveIdle == true && jumpEffectRunning == false && canMoveIdleInside == true)
        {
            if (Player.instance.state == (int)PlayerStates.idle)
            {
                IdleMove();
            }
            else
            {
                RunMove();
            }
        }
        if (Player.instance.isDead == false && Player.instance.canMove == true && Player.instance.canCameraRotate == true && Player.instance.reloading == false)
        {
            WeaponSway();
        }
    }
}
