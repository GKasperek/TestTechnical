using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * Class MaxSaver use to save and load the best score
 */
public class MaxSaver : MonoBehaviour
{
    // Best score
    public int MaxToSave;
    // Text of the Home scene display the best score
    public Text MaxText;
    // Using Singleton to have only one instance of the MaxSaver to keep one score and one saver/loader
    private static MaxSaver _instance;

    /*
     * Initialize the score by loading with LoadGame() and display it in textzone
     */
    private void Start()
    {
        this.LoadGame();
        MaxText.text = MaxToSave.ToString();
    }

    /*
     * Save the score
     */
    public void SaveGame(int max)
    {
        this.MaxToSave = max;
        PlayerPrefs.SetInt("MaxSaved", this.MaxToSave);
        PlayerPrefs.Save();
    }

    /*
     * Load the score if it has already been saved else the best score is 0
     */
    public void LoadGame()
    {
        if (PlayerPrefs.HasKey("MaxSaved"))
        {
            MaxToSave = PlayerPrefs.GetInt("MaxSaved");
        }
        else
        {
            MaxToSave = 0;
        }
    }

    /* 
     * Singleton to have unique instance of MaxSaver
     */
    public static MaxSaver GetInstance()
    {
        if(_instance == null)
        {
            _instance = new MaxSaver();
        }
        return _instance;
    }


}
