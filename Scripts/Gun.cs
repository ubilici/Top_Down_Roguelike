using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour
{

    public enum FireMode
    {
        Auto, Burst, Single
    };

    [Header("Weapon Settings")]
    public FireMode fireMode;
    public Transform[] projectileSpawn;
    public Projectile projectile;
    public float msBetweenShots = 100f;
    public float muzzleVelocity = 35f;
    public int burstCount;
    public int projectilesPerMag;
    public float reloadTime = 0.8f;

    [Header("Recoil")]
    public Vector2 kickMinMax = new Vector2(0.05f, 0.2f);
    public Vector2 recoilMinMax = new Vector2(3, 5);
    public float recoilMoveSettleTime = 0.1f;
    public float recoilRotationSettleTime = 0.1f;

    [Header("Effects")]
    public Transform shell;
    public Transform shellEjection;
    public AudioClip shootAudio;
    public AudioClip reloadAudio;
    MuzzleFlash muzzleFlash;

    bool triggerReleasedSinceLastShot;
    bool isReloading;
    int shotsRemainingInBurst;
    public int projectilesRemainingInMag;

    Vector3 listenerPos;
    float nextShotTime;

    Vector3 recoildSmoothDampVelocity;
    float recoilRotationSmoothDampVelocity;
    float recoilAngle;

    void Start()
    {
        muzzleFlash = GetComponent<MuzzleFlash>();
        shotsRemainingInBurst = burstCount;
        triggerReleasedSinceLastShot = true;
        projectilesRemainingInMag = projectilesPerMag;
        listenerPos = AudioManager.instance.transform.position;
    }

    void LateUpdate()
    {
        // Animate recoil
        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, Vector3.zero, ref recoildSmoothDampVelocity, recoilMoveSettleTime);
        recoilAngle = Mathf.SmoothDamp(recoilAngle, 0, ref recoilRotationSmoothDampVelocity, recoilRotationSettleTime);
        transform.localEulerAngles = transform.localEulerAngles + Vector3.left * recoilAngle;

        if (!isReloading && projectilesRemainingInMag == 0)
        {
            //Reload();
        }
    }

    public void Shoot()
    {
        if (!isReloading && Time.time > nextShotTime && projectilesRemainingInMag > 0)
        {

            if (fireMode == FireMode.Burst)
            {
                if (shotsRemainingInBurst == 0)
                {
                    return;
                }
                shotsRemainingInBurst--;
            }

            else if (fireMode == FireMode.Single)
            {
                if (!triggerReleasedSinceLastShot)
                {
                    return;
                }
            }

            for (int i = 0; i < projectileSpawn.Length; i++)
            {
                if (projectilesRemainingInMag == 0)
                {
                    break;
                }
                projectilesRemainingInMag--;
                GameObject.Find("UI").GetComponent<GameUI>().RefreshAmmo(projectilesRemainingInMag);
                nextShotTime = Time.time + msBetweenShots / 1000;
                Projectile newProjectile = Instantiate(projectile, projectileSpawn[i].position, projectileSpawn[i].rotation) as Projectile;
                newProjectile.SetSpeed(muzzleVelocity);
            }

            //Instantiate(shell, shellEjection.position, shellEjection.rotation);
            muzzleFlash.Activate();
            transform.localPosition -= Vector3.forward * Random.Range(kickMinMax.x, kickMinMax.y);
            recoilAngle += Random.Range(recoilMinMax.x, recoilMinMax.y);
            recoilAngle = Mathf.Clamp(recoilAngle, 0, 30);

            AudioManager.instance.PlaySound(shootAudio, listenerPos);
        }
        else if (!isReloading && projectilesRemainingInMag <= 0)
        {
            Reload();
        }
    }

    public void Reload()
    {
        if (!isReloading && projectilesRemainingInMag != projectilesPerMag)
        {
            AudioManager.instance.PlaySound(reloadAudio, listenerPos);
            StartCoroutine(AnimateReload());
        }
    }

    IEnumerator AnimateReload()
    {
        GameObject.Find("UI").GetComponent<GameUI>().RefreshAmmo(-1);
        isReloading = true;

        float reloadSpeed = 0.01f / reloadTime;
        float percent = 0;
        Vector3 initialRotation = transform.localEulerAngles;
        float maxReloadAngle = 30;

        while (percent < 1)
        {
            percent += Time.deltaTime + reloadSpeed;
            float interpolation = (-Mathf.Pow(percent, 2) + percent) * 4;
            float reloadAngle = Mathf.Lerp(0, maxReloadAngle, interpolation);
            transform.localEulerAngles = initialRotation + Vector3.left * reloadAngle;

            yield return null;
        }

        yield return new WaitForSeconds(reloadTime);


        isReloading = false;
        projectilesRemainingInMag = projectilesPerMag;
        GameObject.Find("UI").GetComponent<GameUI>().RefreshAmmo(projectilesRemainingInMag);
    }

    public void Aim(Vector3 aimPoint)
    {
        if (!isReloading)
        {
            transform.LookAt(aimPoint);
        }
    }

    public void OnTriggerHold()
    {
        Shoot();
        triggerReleasedSinceLastShot = false;
    }

    public void OnTriggerRelease()
    {
        triggerReleasedSinceLastShot = true;
        shotsRemainingInBurst = burstCount;
    }

}
