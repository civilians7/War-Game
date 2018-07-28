using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Attack : MonoBehaviour {

    public static bool isMoving = true;
    Text buttonText;

    private void Start() {
        buttonText = GameObject.Find("Mode Button").GetComponent<Text>();
    }

    public void SwitchMode() {
        if (isMoving) {
            isMoving = false;
            buttonText.text = "Attacking";
        } else {
            isMoving = true;
            buttonText.text = "Moving";
        }
    }
}



