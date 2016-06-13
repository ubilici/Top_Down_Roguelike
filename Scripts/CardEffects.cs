using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class CardEffects : MonoBehaviour {

    MapGenerator mapGen;
    Spawner spawner;
    CameraFollow cameraFollower;
    GunController gunController;

    public List<Card> selectedCards;

    public GameObject cardPanel;
    public GameObject card1, card2, card3;

    public GameObject LightningBall;

    void Awake()
    {
        mapGen = FindObjectOfType<MapGenerator>();
        spawner = FindObjectOfType<Spawner>();
        cameraFollower = FindObjectOfType<CameraFollow>();
        gunController = FindObjectOfType<GunController>();
    }

    public void OnMouseDown()
    {
        selectedCards = new List<Card>();
        foreach (Card tempCard in FindObjectsOfType<Card>())
        {
            if (tempCard.isSelected)
            {
                selectedCards.Add(tempCard);
            }
        }

        SetCardEffects(selectedCards);

        //cameraFollower.MoveToGame();

        

        cameraFollower.gameFlag = true;
        /*
        if (selectedCards.Count > 0)
        {
            cardPanel.SetActive(true);
            if (selectedCards.Count == 1)
            {
                card1.SetActive(false);
                card2.SetActive(false);
            }
            else if (selectedCards.Count == 2)
            {
                card1.SetActive(false);
            }
        }
         */

    }

    public void SetCardEffects(List<Card> cardEffects)
    {
        foreach (Card thisCard in cardEffects)
        {
            switch (thisCard.cardName)
            {
                case "ball_lightning":
                    SetBallLightningOn();
                    break;
                case "fast_learner":
                    SetEnemyHealth(6);
                    SetEnemyCount(10);
                    break;
                case "lone_island":
                    SetMapSize(4);
                    SetGunType(2);
                    break;
                case "rusty_metal":
                    break;
                case "toxic_blood":
                    break;
                case "unstoppable_force":
                    break;
            }
        }
    }

    public void SetEnemyHealth(int hpValue)
    {
        spawner.waves[mapGen.mapIndex + 1].enemyHealth = hpValue;
    }

    public void SetEnemyCount(int enemyCount)
    {
        spawner.waves[mapGen.mapIndex + 1].enemyCount = enemyCount;
    }

    public void SetMapSize(int _mapSize)
    {
        mapGen.maps[mapGen.mapIndex + 1].mapSize.x = _mapSize;
        mapGen.maps[mapGen.mapIndex + 1].mapSize.y = _mapSize;
    }

    public void SetGunType(int _gunIndex)
    {
        gunController.EquipGunIndex(_gunIndex);
    }

    public void SetBallLightningOn()
    {
        LightningBall.SetActive(true);
    }

}
