using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class LevelButton : MonoBehaviour {

    public GameObject damageBar, accuracyBar, healthBar, speedBar, reloadBar;
    public GameObject point;
    public List<Button> buttonList;
    public Text skillPoints;
    private GameObject tempPoint;
    private int sp;

    void Start()
    {

    }

    public void ButtonClick(string skillName)
    {
        CheckSkillPoints();
        if (sp > 0)
        {
            switch (skillName)
            {
                case "Damage":
                    tempPoint = Instantiate(point);
                    tempPoint.transform.SetParent(damageBar.transform);
                    break;
                case "Accuracy":
                    tempPoint = Instantiate(point);
                    tempPoint.transform.SetParent(accuracyBar.transform);
                    break;
                case "Speed":
                    tempPoint = Instantiate(point);
                    tempPoint.transform.SetParent(speedBar.transform);
                    break;
                case "Reload":
                    tempPoint = Instantiate(point);
                    tempPoint.transform.SetParent(reloadBar.transform);
                    break;
                case "Health":
                    tempPoint = Instantiate(point);
                    tempPoint.transform.SetParent(healthBar.transform);
                    break;
            }
            sp--;
            SetSkillPoints();
            if (sp <= 0)
            {
                DisableButtons();
            }
        }
    }

    void CheckSkillPoints()
    {
        sp = int.Parse(skillPoints.text);
    }

    void SetSkillPoints()
    {
        skillPoints.text = sp.ToString();
    }

    void DisableButtons()
    {
        foreach (Button tempButton in buttonList)
        {
            tempButton.interactable = false;
        }
    }


}
