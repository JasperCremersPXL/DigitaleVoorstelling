using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Url : MonoBehaviour
{
    public string UrlString;

    public void OpenUrl() {
        Application.OpenURL(UrlString);
    }
}
