using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class RoomUIManager : MonoBehaviour
{
    [SerializeField] GameObject characterSelectPanel;
    [SerializeField] GameObject levelUpPanel;
    [SerializeField] Slider xpSlider;
    [SerializeField] TMP_Text actionPointText;
    [SerializeField] TMP_Text dayCountText;
    [SerializeField] int randomEventStartingDay = 1;
    [SerializeField] GameObject eventObj;
    int actionPoints = 3;
    GameObject playerRef;
    bool activeCharSelect = false;
    bool eventUpgrade = false;

    private void Start()
    {
        playerRef = GameObject.FindGameObjectWithTag("MainCharacter");
        actionPointText.text = "Action points: " + actionPoints;
        dayCountText.text = "Days: " + playerRef.GetComponent<MainCharacter>().dayCount;
        if(playerRef.GetComponent<MainCharacter>().dayCount >= randomEventStartingDay)
        {
            // check for random event
            int rando = Random.Range(0,100);
            if(rando < 30)
            {
                int whichCharRando = Random.Range(0, 9);
                string tempString = "You watched a video showing some spicy new tricks for " + playerRef.GetComponent<MainCharacter>().listOfClasses[whichCharRando].name;
                eventUpgrade = true;
                CharacterSelect(whichCharRando);
                StartCoroutine(EventCoroutine(tempString, 5));
                print("Upgraded Character");
            }
            else if(rando >= 30 && rando < 40)
            {
                string tempString = "You decided to mix things up today and play some video games for a change";
                StartCoroutine(EventCoroutine(tempString, 3));
            }
            else if (rando >= 40 && rando < 50)
            {
                string tempString = "Make sure to pet your pet";
                StartCoroutine(EventCoroutine(tempString, 3));
            }
            else if (rando >= 50 && rando < 55)
            {
                string tempString = "You got distracted by some pizza that appeared in your room and lost some valuable training time";
                actionPoints -= 2;
                actionPointText.text = "Action points: " + actionPoints;
                StartCoroutine(EventCoroutine(tempString, 5));
            }
            else if (rando >= 55 && rando < 65)
            {
                string tempString = "You decided to try and touch grass. It didn't go well";
                actionPoints--;
                actionPointText.text = "Action points: " + actionPoints;
                StartCoroutine(EventCoroutine(tempString, 5));
            }
            else if (rando >= 65 && rando < 70)
            {
                string tempString = "You found the golden duck! You feel extra motivated today";
                actionPoints += 5;
                actionPointText.text = "Action points: " + actionPoints;
                StartCoroutine(EventCoroutine(tempString, 5));
            }
        }
    }
    IEnumerator EventCoroutine(string eventString , int textTime)
    {
        eventObj.SetActive(true);
        eventObj.transform.GetChild(0).GetComponent<TMP_Text>().text = eventString;
        yield return new WaitForSeconds(textTime);
        eventObj.SetActive(false);
    }
    public void StartGame()
    {
        playerRef.GetComponent<MainCharacter>().dayCount++;
        SceneManager.LoadScene("CombatScene");
    }
    public void OpenCloseCharSelect()
    {
        if (actionPoints > 0)
        {
            activeCharSelect = !activeCharSelect;
            characterSelectPanel.SetActive(activeCharSelect);
            if (activeCharSelect)
            {
                UpdateUIImages(0, -1);
            }
        }
    }
    void UpdateUIImages(int whichMode, int whichSprite)
    {
        int i = 0;
        foreach (Transform childTransform in characterSelectPanel.transform)
        {
            // Access the child object
            GameObject childObject = childTransform.gameObject;
            print("Name: " + childObject.name);

            // Check if the child object has the specific component
            Image component = childObject.GetComponent<Image>();
            if (component != null && i < playerRef.GetComponent<MainCharacter>().listOfClasses.Count)
            {
                if (whichMode == 0)
                {
                    component.sprite = playerRef.GetComponent<MainCharacter>().listOfClasses[i].characterSprite;
                }
                if (whichMode == 1 && i == whichSprite)
                {
                    component.color = Color.gray;
                }
                if (whichMode == 2 && i == whichSprite)
                {
                    component.color = Color.red;
                }
                if (whichMode == 3)
                {
                    component.color = Color.white;
                }
                i++;
            }
        }
    }
    IEnumerator FillXpBar(float startingXp, float targetXp)
    {
        float progress = startingXp;
        //print("Progress: " + progress);
        while(progress <= targetXp)
        {
            progress += Time.deltaTime * .5f;
            xpSlider.value = Mathf.Lerp(0f, 1f, progress);
            yield return null;
        }
        yield return new WaitForSeconds(1);
        actionPointText.text = "Action points: " + actionPoints;
        activeCharSelect = false;
        characterSelectPanel.SetActive(false);
        levelUpPanel.SetActive(false);
    }

    #region characterSelectButtons
    public void CharacterSelect(int whichChar)
    {
        if(actionPoints > 0)
        {
            float startingVal = playerRef.GetComponent<MainCharacter>().returnCharProf(whichChar);
            //print("starting val: " + startingVal);
            float changeVal = Random.Range(.10f, .20f);
            if (!eventUpgrade)
            {
                actionPoints--;
                
            }
            eventUpgrade = false;
            levelUpPanel.SetActive(true);
            StartCoroutine(FillXpBar(startingVal, (startingVal + changeVal)));
            playerRef.GetComponent<MainCharacter>().IncreaseProf(whichChar, changeVal);
        }
    }
    #endregion
}
