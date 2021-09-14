using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using Vuforia;


/* This class contains all the functions for the Selectionmode
 * Variables:
 *      - the UI of Selection- and Manipulationmode
 *      - stages and positioners
 *  Methods:
 *      - set initial stages
 *      - set and reset highlights of the icons
 *      - take screenshots
 *      - methods for the buttons to change the stages
 */
public class UI_Selection_Script : MonoBehaviour, IPointerDownHandler
{ 

    // two different uis
    public GameObject uiSelectionmode;
    public GameObject uiManipulationmode;

    public GameObject listStagesPositioners;


    // Video Button  
    public Button buttonVideocapture;
    public Sprite stop_sprite;
    public Sprite record_sprite;

    private bool helpIsActivated;
    public GameObject TextContainer;
    public Text helpfulInformations;
    public GameObject TapToPlace;

    // List of stages and positioners
    public int size = 10;
    public GameObject[] stageAndPositioners = new GameObject[10];
    public AnchorBehaviour[] stages = new AnchorBehaviour[10];
    public ContentPositioningBehaviour[] positioner = new ContentPositioningBehaviour[10];

    // List of Buttons
    public Button[] formIcons = new Button[11];


    // enumeration of all modes
    private enum Objects
    {
        Cube,
        Cylinder,
        Sphere,
        Capsule,
        Pyramid,
        Cone,
        Hemisphere,
        Tube,
        Ring,
        Prism,
        None
    }

    private Objects enumObjects;


    // Start is called before the first frame update
    void Start()
    {
        loadAndSetHelp();
        
        // enable cube stage and positioner as default
        setStageAndPositioner(0);

        //Select Select mode at beginning
        uiSelectionmode.SetActive(true);
        // and disable ui of manipulationmode
        uiManipulationmode.SetActive(false);

        enumObjects = Objects.Cube;

        // Highlight Cube Icon
        ColorUtility.TryParseHtmlString("#B3F2E5", out Color myColor);
        formIcons[0].image.color = myColor;
    }


    private void Update()
    {

        if ((Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began))
        {
            setInitialStage();
        }


#if UNITY_EDITOR
        if (Input.GetMouseButton(0))
        {
            setInitialStage();
        }
#endif
    }

    /* 
     * This method will be called, after every touch on the touchscreen.
     * The idea:
     * Without this method, Vuforia just places the last objects.
     * 
     * For example: If you coloured a cube and then you want to place a cube again.
     * This cube will be blue again.
     * Another example: If you delete the last placed cube, you are not able to place a cube again, 
     * because the last one is destroyed.
     * 
     * This method assign the initial stage to the positioner.
     */
    private void setInitialStage()
    {
        if (enumObjects == Objects.None)
        {
            return;
        }

        positioner[(int) enumObjects].AnchorStage = stages[(int) enumObjects];
    }
    
    // Highlight selected Icon
    public void highlightIcon()
    {
        resetIconHighlighting();
        
        ColorUtility.TryParseHtmlString("#B3F2E5", out Color myColor);

        formIcons[(int) enumObjects].image.color = myColor;
    }

    public void resetIconHighlighting()
    {
        foreach (Button i in formIcons)
        {
            i.image.color = Color.white;
        }
    }


    /*
    * Basic Idea is that you disable all other objects and enable
    * the mid air stage AND the positioner 
    * of the related object
    */
    private void setStageAndPositioner(int index)
    {
        for (int i = 0; i < stageAndPositioners.Length; i++)
        {
            if (i == index )
            {
                stageAndPositioners[i].SetActive(true);
            }
            else
            {
                stageAndPositioners[i].SetActive(false);
            }
        }
    }

    /* The method for the screenshot button:
     * It starts a coroutine takeScreenshot()
     */
    public void ButtonScreenshot_Click()
    {
        // call function
        StartCoroutine("takeScreenshot");
    }

    private IEnumerator takeScreenshot()
    {
        
        // disable the uiSelectionmode
        uiSelectionmode.SetActive(false);
        listStagesPositioners.SetActive(false);


        // capture a screenshot
        string timeStamp = System.DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss");
        string fileName = "CreARtion Screenshot" + timeStamp + ".png";
        string pathToSave = fileName;
        ScreenCapture.CaptureScreenshot(pathToSave);


        // enable the uiSelectionmode
        Invoke("enableSelectionmode", 0.1f);

        // new text
        helpfulInformations.text = "Screenshot was taken.";

        // call function after 1 second
        Invoke("changeInformationOnText", 1);

        yield return new WaitForEndOfFrame();
        
    }

    // enable the selectionmode ui
    private void enableSelectionmode()
    {
        uiSelectionmode.SetActive(true);
        // enable stages and positioners
        listStagesPositioners.SetActive(true);
    }


    // Buttons in the scrollable list

    // In this method you enable cube stage and positioner and
    // disable all other stages and positioners
    public void ButtonCube_Click()
    {
        setStageAndPositioner(0);

        enumObjects = Objects.Cube;
        changeInformationOnText();
        
        highlightIcon();
    }

    // In this method you enable cylinder stage and positioner and
    // disable all other stages and positioners
    public void ButtonCylinder_Click()
    {
        setStageAndPositioner(1);

        enumObjects = Objects.Cylinder;
        changeInformationOnText();
        
        highlightIcon();
    }

    // In this method you enable sphere stage and positioner and
    // disable all other stages and positioners
    public void ButtonSphere_Click()
    {
        setStageAndPositioner(2);

        enumObjects = Objects.Sphere;
        changeInformationOnText();
        
        highlightIcon();
    }

    // In this method you enable capsule stage and positioner and
    // disable all other stages and positioners
    public void ButtonCapusle_Click()
    {
        setStageAndPositioner(3);

        enumObjects = Objects.Capsule;
        changeInformationOnText();
        
        highlightIcon();
    }

    // In this method you enable pyramid stage and positioner and
    // disable all other stages and positioners
    public void ButtonPyramid_Click()
    {
        setStageAndPositioner(4);

        enumObjects = Objects.Pyramid;
        changeInformationOnText();
        
        highlightIcon();
    }

    // In this method you enable cone stage and positioner and
    // disable all other stages and positioners
    public void ButtonCone_Click()
    {
        setStageAndPositioner(5);

        enumObjects = Objects.Cone;
        changeInformationOnText();
        
        highlightIcon();
    }
    
    // In this method you enable hemisphere stage and positioner and
    // disable all other stages and positioners
    public void ButtonHemisphere_Click()
    {
        setStageAndPositioner(6);

        enumObjects = Objects.Hemisphere;
        changeInformationOnText();
        
        highlightIcon();
    }
    
    // In this method you enable tube stage and positioner and
    // disable all other stages and positioners
    public void ButtonTube_Click()
    {
        setStageAndPositioner(7);

        enumObjects = Objects.Tube;
        changeInformationOnText();
        
        highlightIcon();
    }

    // In this method you enable ring stage and positioner and
    // disable all other stages and positioners
    public void ButtonRing_Click()
    {
        setStageAndPositioner(8);

        enumObjects = Objects.Ring;
        changeInformationOnText();
        
        highlightIcon();
    }
    
    // In this method you enable prism stage and positioner and
    // disable all other stages and positioners
    public void ButtonPrism_Click()
    {
        setStageAndPositioner(9);

        enumObjects = Objects.Prism;
        changeInformationOnText();
        
        highlightIcon();
    }

    // deactivate all stages to view the sculpture
    public void ButtonDeactivate_Click()
    {
        setStageAndPositioner(10);

        enumObjects = Objects.None;
        changeInformationOnText();

        highlightIcon();
    }


    public void ButtonMainMenu_Click()
    {
        SceneManager.LoadScene("MainMenu");
    }

    // this method change the text of information
    // this method will be called in each click() method
    private void changeInformationOnText()
    {
        if (enumObjects == Objects.None)
        {
            helpfulInformations.text = "Move your device to the sculpture to see all the details.";
            return;
        }

        // change the text to the information how to place objects
        helpfulInformations.text = "Tap to place " + enumObjects.ToString() + ".\nOr tap on object to manipulate.";
    }
    
    // this method enables or disables help and hints
    private void loadAndSetHelp()
    {
        helpIsActivated = (PlayerPrefs.GetInt("help") != 0);

        TextContainer.SetActive(helpIsActivated);
        TapToPlace.SetActive(helpIsActivated);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Something was clicked!!");
    }
}