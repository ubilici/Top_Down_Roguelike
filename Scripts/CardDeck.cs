using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CardDeck : MonoBehaviour
{

    public List<GameObject> cardList = new List<GameObject>();
    public List<GameObject> currentCards = new List<GameObject>();
    public List<Transform> positionList = new List<Transform>();

    public int totalCards;
    int randomValue;
    Quaternion cardRotation;

    void Start()
    {
        totalCards = cardList.Count;
        cardRotation.eulerAngles = new Vector3(0, 180, 0);
    }

    public void OnMouseDown()
    {
        //PlaceCards();
    }

    public void PlaceCards()
    {
        if (DrawCards())
        {
            for (int i = 0; i < 3; i++)
            {
                StartCoroutine(MoveNewCard(currentCards[i], positionList[i].position));
            }
        }
    }

    bool DrawCards()
    {
        foreach (GameObject tempCard in currentCards)
        {
            GameObject.Destroy(tempCard);
        }
        if (totalCards >= 3)
        {
            for (int i = 0; i < 3; i++)
            {
                randomValue = Random.Range(0, (totalCards - 1));
                currentCards[i] = Instantiate(cardList[randomValue], transform.position, cardRotation) as GameObject;
                cardList[randomValue] = cardList[totalCards - 1];
                totalCards--;
            }
            return true;
        }
        return false;
    }

    IEnumerator MoveNewCard(GameObject newCard, Vector3 cardPosition)
    {
        float movePercent = 0;
        float speed = 0.6f;

        while (movePercent >= 0)
        {
            movePercent += Time.deltaTime * speed;
            newCard.transform.position = new Vector3(Mathf.Lerp(transform.position.x, cardPosition.x, movePercent), newCard.transform.position.y, newCard.transform.position.z);
            yield return null;
        }
    }
}
