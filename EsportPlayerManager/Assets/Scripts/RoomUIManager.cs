using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RoomUIManager : MonoBehaviour
{
    [SerializeField] GameObject characterSelectPanel;
    [SerializeField] GameObject levelUpPanel;
    [SerializeField] Slider xpSlider;
    GameObject playerRef;
    bool activeCharSelect = false;

    private void Start()
    {
        playerRef = GameObject.FindGameObjectWithTag("MainCharacter");
    }
    public void StartGame()
    {
        SceneManager.LoadScene("CombatScene");
    }
    public void OpenCloseCharSelect()
    {
        activeCharSelect = !activeCharSelect;
        characterSelectPanel.SetActive(activeCharSelect);
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
        activeCharSelect = false;
        characterSelectPanel.SetActive(false);
        levelUpPanel.SetActive(false);
    }

    #region characterSelectButtons
    public void CharacterSelect(int whichChar)
    {
        float startingVal = playerRef.GetComponent<MainCharacter>().returnCharProf(whichChar);
        //print("starting val: " + startingVal);
        float changeVal = Random.Range(.10f, .20f);
        levelUpPanel.SetActive(true);
        StartCoroutine(FillXpBar(startingVal, (startingVal + changeVal)));
        playerRef.GetComponent<MainCharacter>().IncreaseProf(whichChar, changeVal);
    }
    #endregion
}
