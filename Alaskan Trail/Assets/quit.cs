using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class quit : MonoBehaviour
{
    public void doquit() {
        Application.Quit();
        Debug.Log("has quit game");
    }
}
