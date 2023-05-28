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
    int actionPoints = 3;
    GameObject playerRef;
    bool activeCharSelect = false;

    private void Start()
    {
        playerRef = GameObject.FindGameObjectWithTag("MainCharacter");
        actionPointText.text = "Action points: " + actionPoints;
        dayCountText.text = "Days: " + playerRef.GetComponent<MainCharacter>().dayCount;
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
            actionPoints--;
            levelUpPanel.SetActive(true);
            StartCoroutine(FillXpBar(startingVal, (startingVal + changeVal)));
            playerRef.GetComponent<MainCharacter>().IncreaseProf(whichChar, changeVal);
        }
    }
    #endregion
}
