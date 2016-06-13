using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class LevelSelection : MonoBehaviour {

    public Text levelTitle;
    public Text levelDescription;
    public Text levelReward;
    public Button nextLevel;

    public Material unlockedMaterial;
    public Material completedMaterial;

    CameraFollow cameraFollower;

    public List<LevelSelectionButton> levelList;

    LevelSelectionButton currentSelectedButton;

    void Awake()
    {
        cameraFollower = FindObjectOfType<CameraFollow>();
    }

    public void SetLevelCharacteristics(string _title, string _description, string _reward, bool _isUnlocked, bool _isCompleted)
    {
        levelTitle.text = _title;
        levelDescription.text = _description;
        levelReward.text = _reward;
        if (_isUnlocked && !_isCompleted)
        {
            nextLevel.interactable = true;
        }
        else
        {
            nextLevel.interactable = false;
        }
    }

    public void UnlockLevels()
    {
        if (levelList[7].isCompleted)
        {
            levelList[8].isUnlocked = true;
        }
        if (levelList[6].isCompleted)
        {
            levelList[8].isUnlocked = true;
        } 
        if (levelList[5].isCompleted)
        {
            levelList[7].isUnlocked = true;
        }
        if (levelList[4].isCompleted)
        {
            levelList[6].isUnlocked = true;
            levelList[7].isUnlocked = true;
        }
        if (levelList[3].isCompleted)
        {
            levelList[6].isUnlocked = true;
        }
        if (levelList[2].isCompleted)
        {
            levelList[4].isUnlocked = true;
            levelList[5].isUnlocked = true;
        }
        if (levelList[1].isCompleted)
        {
            levelList[3].isUnlocked = true;
            levelList[4].isUnlocked = true;
        }
        if (levelList[0].isCompleted)
        {
            levelList[1].isUnlocked = true;
            levelList[2].isUnlocked = true;
        }

        SetUnlockedLevelsColors();
        SetCompletedLevelsColors();
    }

    public void SetUnlockedLevelsColors()
    {
        foreach (LevelSelectionButton tempLevelSelectionButton in levelList)
        {
            if(tempLevelSelectionButton.isUnlocked)
                tempLevelSelectionButton.innerRenderer.material = unlockedMaterial;
        }
    }

    public void SetCompletedLevelsColors()
    {
        foreach (LevelSelectionButton tempLevelSelectionButton in levelList)
        {
            if (tempLevelSelectionButton.isCompleted)
                tempLevelSelectionButton.innerRenderer.material = completedMaterial;
        }
    }

    public void SetSelectedLevel(int _levelID)
    {
        foreach (LevelSelectionButton tempLevelSelectionButton in levelList)
        {
            tempLevelSelectionButton.OnUnselected();
        }
        levelList[_levelID - 1].isSelected = true;
        currentSelectedButton = levelList[_levelID - 1];
    }

    public void NextLevelButton()
    {
        //launch selected Level
        Debug.Log("Launch " + currentSelectedButton.levelID);

        //return
        levelList[currentSelectedButton.levelID - 1].isCompleted = true;
        UnlockLevels();
        cameraFollower.gameFlag = true;
    }

}
