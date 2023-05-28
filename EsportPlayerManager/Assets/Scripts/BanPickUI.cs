using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class BanPickUI : MonoBehaviour
{
    private MainCharacter playerRef;
    
    
    private int bpTimes = 0;
    private string bpPeriod = "Ban";

    private int[] isBPable = new int [10];



    private void Start()
    {
        playerRef = GameObject.FindGameObjectWithTag("MainCharacter").GetComponent<MainCharacter>();
    }



    public void BanPickChar(int number)
    {
        Debug.Log("BP times" + bpTimes);


        if (isBPable[number] == 0)
        {
            if (bpPeriod == "Ban")
            {
                Debug.Log("Ban: Char" + number);
                isBPable[number] = 1;
                bpTimes += 1;
            }
            else if (bpPeriod == "Pick")
            {
                Debug.Log("Pick: Char" + number);
                isBPable[number] = 1;
                bpTimes += 1;
                //playerRef.charactersOnTeam.Add(playerRef.listOfClasses[number]);
            }
        }
        else
        {
            Debug.Log("char:" + number + "is already b/p");
            
        }



        if (bpTimes >= 2)
        {
            bpPeriod = "Pick";
            Debug.Log("Char BP status" + isBPable);
            for (int i = 0; i < 9; i++)
            {
                Debug.Log("Char " + i + ": " + isBPable[i]);
            }
        }

        if (bpTimes >= 8)
        {
            Debug.Log("BP END! Char BP status" + isBPable);
            for (int i = 0; i < 9; i++)
            {
                Debug.Log("Char " + i + ": " + isBPable[i]);
            }
        }



    }



}
