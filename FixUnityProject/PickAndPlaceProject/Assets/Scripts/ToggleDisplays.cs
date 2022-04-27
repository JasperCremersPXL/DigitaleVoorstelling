using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleDisplays : MonoBehaviour
{
    public GameObject DisplayOne;
    public GameObject DisplayTwo;
    public bool ToggleBool = false;
    public GameObject ProjectDisplayOne;
    public GameObject ProjectDisplayTwo;
    public GameObject ProjectDisplayThree;
    public GameObject ProjectDisplayFour;
    public GameObject OpenLinkButton;
    public GameObject VideoPlayer;
    public bool VideoBool = false;


    public void OnClick() {
        if (!ToggleBool) {
            // WIEBENIK & EXCELLENTIE DISPLAYS
            if (DisplayOne.activeSelf) {
                DisplayOne.SetActive(false);
                DisplayTwo.SetActive(true);
                if (VideoBool) {
                    VideoPlayer.SetActive(true);
                }
            }
            else if (DisplayTwo.activeSelf) {
                DisplayOne.SetActive(true);
                DisplayTwo.SetActive(false);
                if (VideoBool) {
                    VideoPlayer.SetActive(false);
                }
            }
        } else {
            // GITHUBPROJECTEN DISPLAYS
            if (ProjectDisplayOne.activeSelf) {
                ProjectDisplayOne.SetActive(false);
                ProjectDisplayTwo.SetActive(true);
                ProjectDisplayThree.SetActive(false);
                ProjectDisplayFour.SetActive(false);

                OpenLinkButton.SetActive(true);
                OpenLinkButton.GetComponent<Url>().UrlString = "https://take-five.netlify.app/";
            }
            else if (ProjectDisplayTwo.activeSelf) {
                ProjectDisplayOne.SetActive(false);
                ProjectDisplayTwo.SetActive(false);
                ProjectDisplayThree.SetActive(true);
                ProjectDisplayFour.SetActive(false);

                OpenLinkButton.SetActive(true);
                OpenLinkButton.GetComponent<Url>().UrlString = "https://github.com/JasperCremersPXL/6nimmtV2.0";
            }
            else if (ProjectDisplayThree.activeSelf) {
                ProjectDisplayOne.SetActive(false);
                ProjectDisplayTwo.SetActive(false);
                ProjectDisplayThree.SetActive(false);
                ProjectDisplayFour.SetActive(true);

                OpenLinkButton.SetActive(false);
            }
            else if (ProjectDisplayFour.activeSelf) {
                ProjectDisplayOne.SetActive(true);
                ProjectDisplayTwo.SetActive(false);
                ProjectDisplayThree.SetActive(false);
                ProjectDisplayFour.SetActive(false);

                OpenLinkButton.SetActive(false);
            }
        }
    }
}
