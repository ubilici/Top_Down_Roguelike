using UnityEngine;
using System.Collections;

public class Card : MonoBehaviour
{

    public int cardID;
    public string cardName;

    public string cardInfo;

    public string cardRarity;
    public string cardType;

    public bool isSelected;

    public GameObject cardHover;

    float rotateTime = 0;
    public float waitTime = 1f;

    void Start()
    {
        rotateTime = Time.time + waitTime;
    }

    public void OnMouseDown()
    {
        if (!isSelected) isSelected = true;
        else isSelected = false;

        cardHover.SetActive(isSelected);
    }

    void Update()
    {
        if (transform.rotation.eulerAngles.y > 100 && Time.time > rotateTime)
        {
            transform.Rotate(Vector3.up, 100 * Time.deltaTime);
        }
    }

}
