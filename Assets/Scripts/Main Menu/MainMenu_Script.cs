using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


/*
 * This class contains the logic of the mainmenu
 * All methods change the scene.
 * Methods:
 *      - switch to CreARtion
 *      - switch to Gallery
 *      - open an url link to dropbox
 */
public class MainMenu_Script : MonoBehaviour
{
    // In this link are the images of the ImageTargets, so that everybody can download them.
    private string url = "https://www.dropbox.com/sh/4takb350i80cplm/AADP4ZQDYn7RJqRzyyGyeWnra?dl=0";
    public Text warn;
    
    // We want to check, if people activated the hints
    public GameObject toggler;

    private void Start()
    {
        // check if the user start this application for the first time
        if (PlayerPrefs.GetInt("StartTheFirstTime") == 0)
        {
            PlayerPrefs.SetInt("StartTheFirstTime", 1);
            PlayerPrefs.SetInt("help", 1);
            PlayerPrefs.Save();
        }

        // Check if user activated help in last session
        toggler.GetComponent<Toggle>().isOn = (PlayerPrefs.GetInt("help") != 0);
    }

    /*
     * These functions are for the buttons.
     */

    // If you tap on one button, a new scene will be loaded.
    public void FreeBuilding_Click()
    {
        SceneManager.LoadScene("CreARtion");
    }

    public void Gallery_Click()
    {
        SceneManager.LoadScene("Gallery");
    }


    // open the DropBox URL to downlaod the ImageTargets.
    public void OpenURL_Click()
    {
        // test if user is connected to the internet
        if (Application.internetReachability == 0)
        {
            warn.text = "You need internet connection!";
            return;
        }

        warn.text = "";
        Application.OpenURL(url);
        
    }
    
    // change if help and hints are activated
    public void toggleHelp(bool tog)
    {
        // save it to disk, to save it for next session
        PlayerPrefs.SetInt("help", (tog ? 1 : 0));
        PlayerPrefs.Save();
    }
}
