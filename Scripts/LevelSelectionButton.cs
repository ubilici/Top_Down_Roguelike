using UnityEngine;
using System.Collections;

public class LevelSelectionButton : MonoBehaviour {

    public string levelTitle;
    public string levelDescription;
    public string levelReward;
    public bool isUnlocked;
    public bool isCompleted;
    public bool isSelected;

    public MeshRenderer innerRenderer;
    public GameObject selectedBorder;

    public int levelID;

    LevelSelection levelSelection;

    void Start()
    {
        levelSelection = FindObjectOfType<LevelSelection>();
    }

    void OnMouseDown()
    {
        levelSelection.SetLevelCharacteristics(levelTitle, levelDescription, levelReward, isUnlocked, isCompleted);
        levelSelection.SetSelectedLevel(levelID);
        OnSelected();
    }

    public void OnSelected()
    {
        isSelected = true;
        selectedBorder.SetActive(true);
    }

    public void OnUnselected()
    {
        isSelected = false;
        selectedBorder.SetActive(false);
    }

    void Update()
    {
        if (isSelected)
        {
            selectedBorder.transform.Rotate(Vector3.up, 100 * Time.deltaTime);
        }
    }


}
