using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(GunController))]
public class Player : LivingEntity {

    public Crosshair crosshair;

    public float moveSpeed = 5f;
    
    PlayerController controller;
    Camera viewCamera;
    GunController gunController;

    public Gun[] guns;
    int gunNumber;

    public Transform tankBody;

    protected override void Start()
    {
        gunNumber = 0;
        base.Start();
        controller = GetComponent<PlayerController>();
        gunController = GetComponent<GunController>();
        viewCamera = Camera.main;
    }

    void Awake()
    {
        FindObjectOfType<Spawner>().OnNewWave += OnNewWave;
    }

    void OnNewWave(int waveNumber)
    {
        //health = startingHealth;
        //Debug.Log(waveNumber);
        //gunController.EquipGunIndex(waveNumber - 1);
    }

    void Update()
    {
        // Movement Input
        Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

        Vector3 moveVelocity = moveInput.normalized * moveSpeed;
        controller.Move(moveVelocity);

        if (moveInput.x == 1)
        {
            if (moveInput.z == 1)
            {
                tankBody.eulerAngles = new Vector3(0, 45, 0);
            }
            else if (moveInput.z == 0)
            {
                tankBody.eulerAngles = new Vector3(0, 90, 0);
            }
            else if (moveInput.z == -1)
            {
                tankBody.eulerAngles = new Vector3(0, 135, 0);
            }
        }
        else if (moveInput.x == 0)
        {
            if (moveInput.z == 1)
            {
                tankBody.eulerAngles = new Vector3(0, 0, 0);
            }
            else if (moveInput.z == -1)
            {
                tankBody.eulerAngles = new Vector3(0, 180, 0);
            }
        }
        else if (moveInput.x == -1)
        {
            if (moveInput.z == 1)
            {
                tankBody.eulerAngles = new Vector3(0, 315, 0);
            }
            else if (moveInput.z == 0)
            {
                tankBody.eulerAngles = new Vector3(0, 270, 0);
            }
            else if (moveInput.z == -1)
            {
                tankBody.eulerAngles = new Vector3(0, 225, 0);
            }
        }

        // Look Input
        Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.up * gunController.GunHeight);
        float rayDistance;

        if (groundPlane.Raycast(ray, out rayDistance))
        {
            Vector3 point = ray.GetPoint(rayDistance);
            //Debug.DrawLine(ray.origin, point, Color.red);
            controller.LookAt(point);
            crosshair.transform.position = point;
            crosshair.DetectTargets(ray);
            if ((new Vector2(point.x, point.z) - new Vector2(transform.position.x, transform.position.z)).sqrMagnitude > 2)
            {
                gunController.Aim(point);
            }
        }

        // Weapon Input
        if (Input.GetMouseButton(0))
        {
            gunController.OnTriggerHold();
        }

        if (Input.GetMouseButtonUp(0))
        {
            gunController.OnTriggerRelease();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            gunController.Reload();
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            gunNumber++;
            if (gunNumber >= guns.Length)
            {
                gunNumber = 0;
            }
            gunController.EquipGun(guns[gunNumber]);
        }

        if (transform.position.y < -5)
        {
            TakeDamage(health);
        }
    }

    public override void Die()
    {
        AudioManager.instance.PlaySound("Player Death");
        base.Die();
    }
}
