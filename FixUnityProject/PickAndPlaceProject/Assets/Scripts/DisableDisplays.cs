using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DisableDisplays : MonoBehaviour
{
    private List<GameObject> Displays = new List<GameObject>();

    void Start() {
        foreach (Transform display in gameObject.transform) {
            Displays.Add(display.gameObject);
        }
    }

    public void DisableAllDisplays() {
        foreach(GameObject display in Displays) {
            display.SetActive(false);
        }
    }
}
