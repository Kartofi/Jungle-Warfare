using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using System.Text;
using System.Linq;

public class Player : MonoBehaviour
{
    public static Player instance;
    private void Awake()
    {
        instance = this;
    }

    Rigidbody rb;
    [Header("Animations")]
    public Animator animator;
    public Animator targetsAnimator;
    [Header("Camera")]
    GameObject cameraObj;

    [Header("Weapon")]
    public string weapon = "Rifle";
    public WeaponRules weaponRules;
    public WeaponData weaponData;
    public WeaponData[] weapons;


    public GameObject[] weaponsShadows;
    [Header("Stats")]
    public float health = RulesStatic.maxHealth;
    public int state = (int)PlayerStates.idle;
    public float kills = 0;

    public int bullets;
    [Header("States")]
    public bool isDead = false;
    public bool canMove = true;
    public bool canCameraMove = true;
    public bool canCameraRotate = true;
    public bool canShoot = true;
    public bool reloading = false;

    [Header("Settings")]
    public float speed = 5;
    public float sensitivity = 5;
    public float recoilSpeed = 1.5f;

    public AudioSource audioSource;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        rb = gameObject.GetComponent<Rigidbody>();
        cameraObj = Camera.main.gameObject;
    }
    int count = 0;
    Vector3 lastPos;
    Vector3 lastCameraRotation;
    private void FixedUpdate()
    {
        WeaponSystem.main.HandleCursorEffects();
        count++;
        if (count >= 2)
        {
            if (lastPos != transform.position || lastCameraRotation != cameraRotation)
            {
                Vector3 rotationCamera = cameraRotation;
                rotationCamera.x = Math.Clamp(rotationCamera.x, -60, 60);
                Udp.client.UpdatePos(gameObject.transform.localPosition, gameObject.transform.eulerAngles, new CameraData(cameraObj.transform.position, rotationCamera));
            }
            else
            {
                Udp.client.EmptyPacket();
            }
            lastPos = transform.position;
            lastCameraRotation = cameraRotation;
            count = 0;
        }

        if (isDead == false)
        {
            if (cameraRecoilApply == true)
            {
                cameraRotation.x -= recoilSpeed;
                applied -= recoilSpeed;
                if (applied <= maxApplied)
                {
                    cameraRecoilApply = false;
                    applied = 0;
                }
            }
        }
    }  

    private float shootCooldownValue = 0;

    bool cameraRecoilApply = false;
    float applied = 0;
    float maxApplied = 0;

    void Update()
    {
        if (Udp.client.Rules.weaponsRules.Count <= 0)
        {
            return;
        }
        if (this.weaponRules == null)
        {
            return;
        }
        if (shootCooldownValue < weaponRules.shootCooldown)
        {
            shootCooldownValue += Time.deltaTime;
        }
        if (isDead == false)
        {
            HandleCamera();
            HandleMovement();
        }
        if (isDead == false && canMove == true)
        {
            
            if (Input.GetKeyDown(KeyCode.R))
            {
                WeaponSystem.main.HandleReload();
            }
            if (Input.GetMouseButton(0) && shootCooldownValue >= weaponRules.shootCooldown && canShoot == true && reloading == false)
            {
                if (bullets <= 0)
                {
                    WeaponSystem.main.HandleReload();
                    return;
                }
                shootCooldownValue = 0;
                WeaponSystem.main.HandleShooting();
                cameraRecoilApply = true;
                maxApplied = -weaponRules.recoilMultiplier / 2;
            }

        }
        else
        {
            bool onGround = OnGround(1.3f);
            HandleAnimations(onGround, 0, 0);
        }
    }
    public Vector3 cameraRotation = new Vector3(0,0,0);
   

    float timeFreeFall = 0;
    float lastJump = float.MaxValue;

    void HandleCamera()
    {
        if (canCameraRotate == true)
        {
            float MouseX = Input.GetAxisRaw("Mouse X") * sensitivity;
            float MouseY = Input.GetAxisRaw("Mouse Y") * sensitivity;
            cameraRotation += new Vector3(-MouseY, MouseX, 0);


            cameraRotation.x = Mathf.Clamp(cameraRotation.x, -90f, 90);
            cameraObj.transform.localEulerAngles = cameraRotation;
            gameObject.transform.localEulerAngles = new Vector3(0, cameraRotation.y, 0);
        }
        
        if (canCameraMove == true)
        {
            cameraObj.transform.DOMove(gameObject.transform.position + new Vector3(0, 1f,0), 0.1f);
        }
    }
    void HandleMovement()
    {
        lastJump += Time.deltaTime;
        float vertical = 0;
        float horizontal = 0;
        if (canMove == true)
        {
            vertical = Input.GetAxisRaw("Vertical");
            horizontal = Input.GetAxisRaw("Horizontal");
        }
        bool onGround = OnGround(1.3f);
        bool freeFall = !OnGround(3f);

        if (Input.GetKey(KeyCode.Space) && onGround == true && canMove == true && lastJump >= AntiCheatRules.jumpCooldown)
        {
            lastJump = 0;
            rb.velocity = new Vector3(rb.velocity.x, 5 * Udp.client.Rules.jumpPowerMultiplier, rb.velocity.z);

            WeaponEffects.main.JumpEffect();
        }
        Vector3 moveVel = (transform.forward * vertical + transform.right * horizontal).normalized * speed;
        if (freeFall)
        {
            timeFreeFall += 2 * Time.deltaTime;
            rb.velocity = new Vector3(moveVel.x / Mathf.Floor(timeFreeFall + 1), rb.velocity.y, moveVel.z / Mathf.Floor(timeFreeFall + 1));
        }
        else
        {
            if (timeFreeFall != 0)
            {
                timeFreeFall = 0;
            }
            rb.velocity = new Vector3(moveVel.x, rb.velocity.y, moveVel.z);
        }
        HandleAnimations(onGround, vertical, horizontal);
    }
    void HandleAnimations(bool onGround, float vertical,float horizontal)
    {
        void triggerLandEffect()
        {
            if (state == (int)PlayerStates.falling)
            {
                WeaponEffects.main.LandEffect();
            }
        }
        
        if (onGround == false)
        {
            animator.SetBool("IsWalking", false);
            animator.SetBool("IsFalling", true);
            if (state != (int)PlayerStates.falling)
            {
                WeaponEffects.main.FallingWeapon();
            }
            state = (int)PlayerStates.falling;
        }
        else if ((vertical != 0 || horizontal != 0) && onGround == true)
        {
            animator.SetBool("IsWalking", true);
            animator.SetBool("IsFalling", false);

            triggerLandEffect();

            state = (int)PlayerStates.walking;
        }
        else
        {
            animator.SetBool("IsWalking", false);
            animator.SetBool("IsFalling", false);

            triggerLandEffect();

            state = (int)PlayerStates.idle;
        }
        if (animator.GetBool("IsWalking") == true)
        {
            if (audioSource.isPlaying == false)
            {
                audioSource.Play();
            }
        }
        else
        {
            if (audioSource.isPlaying == true)
            {
                audioSource.Stop();
            }
        }
    }
    bool OnGround(float distance)
    {
        bool ray = Physics.Raycast(gameObject.transform.position, new Vector3(0, -10f, 0), distance);
        return ray;
    }

    public void UpdateWeapon(string weapon)
    {
        foreach (GameObject item in weaponsShadows)
        {
            if (item.name != "Gun" + weapon )
            {
                item.SetActive(false);
            }
            else
            {
                item.SetActive(true);
            }
        }
        foreach (WeaponData weaponInstance in weapons)
        {
            if (weaponInstance.Name != weapon)
            {
                weaponInstance.gameObject.SetActive(false);
                targetsAnimator.SetBool(weaponInstance.Name, false);
            }
            else
            {
                targetsAnimator.SetBool(weaponInstance.Name, true);
                weaponInstance.SetLocalThis();
                weaponInstance.gameObject.SetActive(true);

                WeaponRules weaponRules = Udp.client.Rules.weaponsRules.Find(item => item.WeaponName == weapon);
                if (weaponRules == null)
                {
                    return;
                }
                this.weaponRules = weaponRules;

                speed = weaponRules.walkSpeed;

                WeaponData weaponData = Player.instance.weapons.FirstOrDefault(item => item.Name == weaponRules.WeaponName);
                if (weaponRules == null)
                {
                    return;
                }
                this.weaponData = weaponData;
            }
        }

        
    }
}
