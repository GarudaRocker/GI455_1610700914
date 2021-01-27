using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchingManager : MonoBehaviour
{

    private string word;

    public Image findButton;

    public GameObject inputWord;
    public Text isFoundText;

    private bool isFound = false;

    //public string[] matchingNameList = new string[] { "Mouse", "Keyboard", "Laptop", "Computer", "Table" };

    public void IsFoundName()
    {
        word = inputWord.GetComponent<Text>().text;

        if (word == "Mouse")
        {
            isFound = true;
        }
        else if (word == "Keyboard")
        {
            isFound = true;
        }
        else if (word == "Laptop")
        {
            isFound = true;
        }
        else if (word == "Computer")
        {
            isFound = true;
        }
        else if (word == "Table")
        {
            isFound = true;
        }
        else
        {
            isFound = false;
        }

        DisplayText();
    }

    private void DisplayText()
    {
        if (isFound)
        {
            isFoundText.color = Color.green;

            isFoundText.text = "<color=white>[</color>" + word + "<color=white>] is found.</color>";
        }


        else
        {
            isFoundText.color = Color.red;

            isFoundText.text = "<color=white>[</color>" + word + "<color=white>] is not found.</color>";
        }
    }








}
