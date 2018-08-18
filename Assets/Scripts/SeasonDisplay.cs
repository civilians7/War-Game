﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SeasonDisplay : MonoBehaviour {

    public int startYear = 2010; 

    private Text seasonText; 
    private int currentYear; 
    private int currentSeasonNum = 0; 
    private string currentSeason = "Winter"; 

	// Use this for initialization
	void Start () {
        seasonText = GetComponent<Text>();
		currentYear = startYear;
        seasonText.text = currentSeason + " " + currentYear;
	}

    public void SeasonCounter() { //refactor to season display
        if (currentSeasonNum == 3) {
            currentSeasonNum = 0;
        } else {
            currentSeasonNum++;
        }

        if (currentSeasonNum == 0) {
            currentSeason = "Winter";
            currentYear++;
        } else if (currentSeasonNum == 1) {
            currentSeason = "Spring";
        } else if (currentSeasonNum == 2) {
            currentSeason = "Summer";
        } else if (currentSeasonNum == 3) {
            currentSeason = "Autumn";
        }

        seasonText.text = currentSeason + " " + currentYear;
    }

}
