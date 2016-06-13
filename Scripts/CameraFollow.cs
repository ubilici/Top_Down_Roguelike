using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CameraFollow : MonoBehaviour {

    public GameObject nextLevelButton;
    public Button nextLevelB;
    public GameObject upgradeUI;

    public bool activateGizmos;

    public CapsuleCollider target;
    public Vector3 focusAreaSize;

    public float verticalOffset;

    public CanvasGroup upgradeGroup;
    public GameObject crosshair;

    public GameObject player;

    Vector2 cameraYZ;
    FocusArea focusArea;
    bool isNeeded;

    float timerUp;
    float timerDown;

    public bool upgradesFlag, gameFlag;

    bool goDown;

    void Awake()
    {
        //focusArea = new FocusArea(target.bounds, focusAreaSize);
        cameraYZ = new Vector2(transform.position.y, transform.position.z);
        player = FindObjectOfType<Player>().gameObject;
    }

    void LateUpdate()
    {

        if (upgradesFlag)
        {
            upgradesFlag = false;
            MoveToUpgrades();
        }
        if(gameFlag)
        {
            gameFlag = false;
            MoveToGame();
        }

        if (isNeeded && target != null)
        {
            focusArea.Update(target.bounds);

            Vector3 focusPosition = new Vector3(focusArea.centre.x, cameraYZ.x, cameraYZ.y + focusArea.centre.z); // + Vector3.up * verticalOffset;

            transform.position = focusPosition;
        }

        if (timerDown < Time.time && goDown)
        {
            goDown = false;
            //upgradeGroup.gameObject.SetActive(false);
            FindObjectOfType<Spawner>().NextWave();
        }
    }

    public void SwitchCameraFollow(bool value)
    {
        isNeeded = value;
    }

    public void MoveToUpgrades()
    {
        StopCoroutine("AnimateCamMovement");
        StartCoroutine(AnimateCamMovement("up"));
        crosshair.SetActive(false);
        Cursor.visible = true;
        player.SetActive(false);
        nextLevelButton.SetActive(true);
        upgradeUI.SetActive(true);
        nextLevelB.interactable = false;
        GetComponent<Camera>().orthographic = false;
        //upgradeGroup.gameObject.SetActive(true);
    }

    public void MoveToGame()
    {
        StopCoroutine("AnimateCamMovement");

        StartCoroutine(AnimateCamMovement("down"));
        crosshair.SetActive(true);
        Cursor.visible = false;
        player.SetActive(true);
        nextLevelButton.SetActive(false);
        upgradeUI.SetActive(false);
        GetComponent<Camera>().orthographic = true;    
        //StartCoroutine(FadeUpgrade(false));

        timerDown = Time.time + 0.5f;
        goDown = true;
    }

    void OnDrawGizmos()
    {
        if (activateGizmos)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawCube(focusArea.centre, focusAreaSize);
        }
    }

    struct FocusArea
    {
        public Vector3 centre;
        public Vector3 velocity;
        float left, right, top, bottom;

        public FocusArea(Bounds targetBounds, Vector3 size)
        {
            left = targetBounds.center.x - size.x / 2;
            right = targetBounds.center.x + size.x / 2;
            bottom = targetBounds.center.z - size.z / 2;
            top = targetBounds.center.z + size.z / 2;

            velocity = Vector3.zero;
            centre = new Vector3((left + right) / 2, 0, (top + bottom) / 2);
        }

        public void Update(Bounds targetBounds)
        {
            float shiftX = 0;
            if (targetBounds.min.x < left)
            {
                shiftX = targetBounds.min.x - left;
            }
            else if (targetBounds.max.x > right)
            {
                shiftX = targetBounds.max.x - right;
            }
            left += shiftX;
            right += shiftX;

            float shiftZ = 0;
            if (targetBounds.min.z < bottom)
            {
                shiftZ= targetBounds.min.z - bottom;
            }
            else if (targetBounds.max.z > top)
            {
                shiftZ = targetBounds.max.z - top;
            }
            top += shiftZ;
            bottom += shiftZ;

            centre = new Vector3((left + right) / 2, 0,(top + bottom) / 2);
            velocity = new Vector3(shiftX, 0, shiftZ);

        }
    }

    public IEnumerator AnimateCamMovement(string upOrDown)
    {
        float animatePercent = 0;
        float speed = 2f;
        float x, y, z, w;

        if (upOrDown == "up")
        {
            x = 15;
            y = 65;
            z = 45;
            w = 0;
        }
        else
        {
            x = 65;
            y = 15;
            z = 0;
            w = 45;
        }

        while (animatePercent >= 0)
        {
            animatePercent += Time.deltaTime * speed;

            transform.eulerAngles = new Vector3(Mathf.Lerp(z, w, animatePercent), transform.eulerAngles.y, transform.eulerAngles.z);
            transform.position = new Vector3(transform.position.x, Mathf.Lerp(x, y, animatePercent), transform.position.z);
            yield return null;
        }
    }

    IEnumerator FadeUpgrade(bool upOrDown)
    {
        float animatePercent = 0;
        float speed = 3f;
        float x, y;

        if (upOrDown)
        {
            x = 0;
            y = 1;
        }
        else
        {
            x = 1;
            y = 0;
        }

        while (animatePercent >= 0)
        {
            animatePercent += Time.deltaTime * speed;

            upgradeGroup.alpha = Mathf.Lerp(x, y, animatePercent);

            yield return null;
        }
    }

}
