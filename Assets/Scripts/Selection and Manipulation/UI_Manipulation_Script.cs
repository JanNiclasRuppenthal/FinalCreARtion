using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/*
 * This class implements all the function for the mainpulationtools.
 * Methods:
 *      - set and reset the highlighting of the icons
 *      - activate certain UI gameobjects
 *      - select the objects
 *      - move the objects with device / camera
 *      - move the objects with a control pad
 *      - remove the objects from the camera 
 *      - resize the objects with fingers
 *      - stretch the objects in any directions
 *      - rotate the objects with three circular slider
 *      - change the colour (and outline) of the objects
 *      - copy the objects
 */
public class UI_Manipulation_Script : MonoBehaviour
{

    // main UI of the manipulationmode
    public GameObject uiManipulationmode;
    private bool helpIsActivated;
    public GameObject StretchTextContainer;
    public GameObject TextContainer;
    public Text helpfulInformations;
    public GameObject scrollableListManipulations;

    // for copying: an array with the cloned stages
    public GameObject[] clonedStages = new GameObject[10];

    // ArrayList of all UI GameObjects
    private ArrayList listUI = new ArrayList();

    //Rotation-UI
    public GameObject uiRotation;
    
    //Color-UI
    public GameObject uiColor;

    // childs of Rotation UI
    public CircularRangeControlX circularX;
    public CircularRangeControlY circularY;
    public CircularRangeControlZ circularZ;

    // Move-UI and bool variable
    public GameObject moveUI;
    private bool movement;

    // arraylist of marked objects
    public SwitchMode sw;
    private HashSet<GameObject> listOfMarkedObjects;

    // hashtable with the parents of the objects and their stages.
    private Dictionary<GameObject, Transform> objectsStages;

    // Variables for stretching
    private float initialFingersDistance;
    private Vector3 initialScale;
    private float initialScaleDirX;
    private float initialScaleDirY;
    private float initialScaleDirZ;
    public GameObject uiListStretchButtons;
    public Button[] stretchButtons = new Button[3];
    private bool stretch_x = false;
    private bool stretch_y = false;
    private bool stretch_z = false;

    // private variables for the control pad
    private bool up_y = false;
    private bool down_y = false;
    private bool right_x = false;
    private bool left_x = false;
    private bool up_z = false;
    private bool down_z = false;
    private bool rotate_right = false;
    private bool rotate_left = false;
    private float speed = 0.005f;

    // control pad
    public GameObject controlPad;
    public GameObject switchMoveControl;
    private bool controlPadIsNotActive = false;
    public Sprite controlPad_sprite;
    public Sprite phone_sprite;

    public Camera camera;

    // all the different states for the manipulations
    public enum manipulationStates
    {
        Select,
        Move,
        Resize,
        Rotate,
        Stretch,
        Color,
        Copy
    }

    public manipulationStates currentState;
    
    // an array with the buttons for changing the colour of the icons
    public Button[] mIcons = new Button[6];


    // Start is called before the first frame update
    void Start()
    {
        // add all UI elemtents
        listUI.AddRange(new List<GameObject>{
            uiRotation,
            controlPad,
            switchMoveControl,
            moveUI,
            uiColor,
            uiListStretchButtons,
            TextContainer
        });

        // get the ArrayList
        listOfMarkedObjects = sw.getListOfMarkedObjects();

        // get the Hashtable
        objectsStages = sw.getDictionaryObjectStage();

        // Initial state
        currentState = manipulationStates.Select;

        // highlight icon after switching the mode
        highlightIcon();

        // set a helpfull text
        loadAndSetHelp();
        helpfulInformations.text = "Tap on the objects to select or deselect.";

    }

    // Update is called once per frame
    void Update()
    {
        // TODO: better writing
        switch (currentState)
        {
            case manipulationStates.Move:
                if (!controlPadIsNotActive)
                {
                    moveObjects(); 
                }
                break;
            case manipulationStates.Resize:
                resizeObjects();
                break;
            case manipulationStates.Stretch:
                stretchObjects();
                break;
            default: break;
        }

        // logic for the control pad to move the objects
        if (up_y)
        {
            foreach (GameObject objects in listOfMarkedObjects)
            {
                Vector3 pos = objects.transform.parent.position;
                objects.transform.parent.position = new Vector3(pos.x, pos.y + speed, pos.z);
            }
        }
        if(down_y)
        {
            foreach (GameObject objects in listOfMarkedObjects)
            {
                Vector3 pos = objects.transform.parent.position;
                objects.transform.parent.position = new Vector3(pos.x, pos.y - speed, pos.z);
            }
        }
        if (right_x)
        {
            foreach (GameObject objects in listOfMarkedObjects)
            {
                Vector3 pos = objects.transform.parent.position;
                objects.transform.parent.position = new Vector3(pos.x + speed, pos.y, pos.z);
            }
        }
        if (left_x)
        {
            foreach (GameObject objects in listOfMarkedObjects)
            {
                Vector3 pos = objects.transform.parent.position;
                objects.transform.parent.position = new Vector3(pos.x - speed, pos.y, pos.z);
            }
        }
        if (up_z)
        {
            foreach (GameObject objects in listOfMarkedObjects)
            {
                Vector3 pos = objects.transform.parent.position;
                objects.transform.parent.position = new Vector3(pos.x, pos.y, pos.z + speed);
            }
        }
        if (down_z)
        {
            foreach (GameObject objects in listOfMarkedObjects)
            {
                Vector3 pos = objects.transform.position;
                objects.transform.parent.position = new Vector3(pos.x, pos.y, pos.z - speed);
            }
        }
    }
    
    // this method enables or disables help and hints
    private void loadAndSetHelp()
    {
        helpIsActivated = (PlayerPrefs.GetInt("help") != 0);
        
        TextContainer.SetActive(helpIsActivated);
    }

    
    // Highlight selected Icon
    public void highlightIcon()
    {
        resetIconHighlighting();
        
        // Select a Color
        ColorUtility.TryParseHtmlString("#B3F2E5", out Color myColor);

        mIcons[(int) currentState].image.color = myColor;
    }

    public void resetIconHighlighting()
    {
        foreach (Button i in mIcons)
        {
            i.image.color = Color.white;
        }
    }

    // this method activates certain UI elements
    private void activateGameObjects(GameObject[] array)
    {
        bool skip = false;
        foreach(GameObject ui in listUI)
        {
            for(int i = 0; i < array.Length; i++)
            {
                // found the right UI element?
                if (ui == array[i])
                {
                    skip = true;
                    break;
                }
            }

            // check if TextContainer is necessary
            if (ui.name == "TextContainer" && !helpIsActivated)
            {
                skip = false;
                continue;
            }
            
            // activate, if we found the right element
            if (skip)
            {
                ui.SetActive(true);
                skip = false;
                continue;
            }

            // deactivate the unnecessary UI elements
            ui.SetActive(false);
        }
    }


    /*
     * All following methods implements the function of the buttons.
     * All public methods are called by the buttons.
     */

    public void ButtonSelect_Click()
    {
        // if the user moved the objects around before
        removeObjectsFromCamera();
        movement = false;

        // set state
        currentState = manipulationStates.Select;

        highlightIcon();

        // deactivate the controlpad
        controlPadIsNotActive = false;
        setControlPadBoolsOnFalse();

        // set a helpfull text
        activateGameObjects(new GameObject[] {TextContainer});
        helpfulInformations.text = "Tap on the objects to select them.";
    }

    public void ButtonMove_Click()
    {
        // set current state of the UI
        currentState = manipulationStates.Move;

        // deactivate the function of the control pad
        controlPadIsNotActive = false;
        switchMoveControl.GetComponent<Button>().image.overrideSprite = controlPad_sprite;
        //buttonSwitchMoveControl.image.overrideSprite = controlPad_sprite;
        setControlPadBoolsOnFalse();

        highlightIcon();

        // set a helpfull text
        activateGameObjects(new GameObject[] { switchMoveControl, moveUI, TextContainer });
        helpfulInformations.text = "Hold the button down and move your device to move your selected objects.";
    }

 

    private void moveObjects()
    {
        // move the objects only the user holds the button
        if (!movement)
        {
            removeObjectsFromCamera();
            return;
        }
        
        foreach (GameObject objects in listOfMarkedObjects)
        {
            // put the object as a child to camera
            objects.transform.parent.parent = camera.transform;
        }
    }

    public void holdMoveButton(bool move)
    {
        movement = move;
    }

    // switch to the control pad or to the movement with device
    public void changeMoveControl()
    {
        if (controlPadIsNotActive)
        {
            // change the UI
            activateGameObjects(new GameObject[] {moveUI, TextContainer, switchMoveControl});

            // change the image
            //buttonSwitchMoveControl.image.overrideSprite = controlPad_sprite;
            switchMoveControl.GetComponent<Button>().image.overrideSprite = controlPad_sprite;
        }
        else
        {
            // change the UI
            activateGameObjects(new GameObject[] { controlPad, switchMoveControl });

            // if the user moved the objects around before
            removeObjectsFromCamera();
            movement = false;

            // change the image
            //buttonSwitchMoveControl.image.overrideSprite = phone_sprite;
            switchMoveControl.GetComponent<Button>().image.overrideSprite = phone_sprite;
        }
        
        controlPadIsNotActive = !controlPadIsNotActive;
    }

    public void removeObjectsFromCamera()
    {
        if (currentState != manipulationStates.Move)
        {
            return;
        }

        foreach (GameObject objects in listOfMarkedObjects)
        {
            // marked objects get their stages back
            objects.transform.parent.parent = objectsStages[objects];
        }
    }



    public void ButtonResize_Click()
    {
        // if the user moved the objects around before
        removeObjectsFromCamera();
        movement = false;

        // deactivate the function of the control pad
        controlPadIsNotActive = false;
        setControlPadBoolsOnFalse();;

        currentState = manipulationStates.Resize; 
        
        highlightIcon();

        // set a helpfull text
        activateGameObjects(new GameObject[] {TextContainer});
        helpfulInformations.text = "Use the two finger gesture to resize the selected objects.";
    }

    private void resizeObjects()
    {

        int fingersOnScreen = 0;

        foreach (Touch touch in Input.touches)
        {
            fingersOnScreen++; //Count fingers (or rather touches) on screen as you iterate through all screen touches.

            //You need two fingers on screen to pinch.
            if (fingersOnScreen == 2)
            {
                foreach (GameObject objects in listOfMarkedObjects)
                {

                    //First set the initial distance between fingers so you can compare.
                    if (touch.phase == TouchPhase.Began)
                    {
                        initialFingersDistance = Vector2.Distance(Input.touches[0].position, Input.touches[1].position);
                        initialScale = objects.transform.parent.localScale;
                    }
                    else
                    {
                        var currentFingersDistance = Vector2.Distance(Input.touches[0].position, Input.touches[1].position);
                        var scaleFactor = currentFingersDistance / initialFingersDistance;

                        // scale the parent of the marked object
                        objects.transform.parent.localScale = initialScale * scaleFactor;
                    }
                }
            }
        }
    }

    public void ButtonRotate_Click()
    {
        // if the user moved the objects around before
        removeObjectsFromCamera();
        movement = false;

        // deactivate the functions of the control pad
        controlPadIsNotActive = false;
        setControlPadBoolsOnFalse();

        currentState = manipulationStates.Rotate;
        
        highlightIcon();

        activateGameObjects(new GameObject[] {uiRotation});

        // reset the circular slider
        circularX.Reset();
        circularY.Reset();
        circularZ.Reset();
    }


    public void ButtonStretch_Click()
    {
        // if the user moved the objects around before
        removeObjectsFromCamera();
        movement = false;

        // deactivate the functions of the control pad
        controlPadIsNotActive = false;
        setControlPadBoolsOnFalse();

        currentState = manipulationStates.Stretch;
        
        highlightIcon();

        // Instantiate the initial state
        StretchX_Click();
        stretch_y = false;
        stretch_z = false;
        stretchButtons[1].image.color = Color.white;
        stretchButtons[2].image.color = Color.white;

        activateGameObjects(new GameObject[] {uiListStretchButtons});
        
        // enable or disable help text
        StretchTextContainer.SetActive(helpIsActivated);
    }


   
    private void stretchObjects()
    {
        int fingersOnScreen = 0;

        foreach (Touch touch in Input.touches)
        {
            fingersOnScreen++; //Count fingers (or rather touches) on screen as you iterate through all screen touches.

            //You need two fingers on screen to stretch.
            if (fingersOnScreen == 2)
            {
                foreach (GameObject objects in listOfMarkedObjects)
                {
                   
                    //First set the initial distance between fingers so you can compare.
                    if (touch.phase == TouchPhase.Began)
                    {
                        initialFingersDistance = Vector2.Distance(Input.touches[0].position, Input.touches[1].position);

                        if (stretch_x)
                        {
                            initialScaleDirX = objects.transform.parent.localScale.x;
                        } 
                        if (stretch_y) 
                        {
                            initialScaleDirY = objects.transform.parent.localScale.y;
                        } 
                        if (stretch_z) 
                        {
                            initialScaleDirZ = objects.transform.parent.localScale.z;
                        }
                    }
                    else
                    {
                        var currentFingersDistance = Vector2.Distance(Input.touches[0].position, Input.touches[1].position);
                        var stretchFactor = currentFingersDistance / initialFingersDistance;

                        float x = objects.transform.parent.localScale.x;
                        float y = objects.transform.parent.localScale.y;
                        float z = objects.transform.parent.localScale.z;

                        if (stretch_x)
                        {
                            x = initialScaleDirX * stretchFactor;
                        }
                        if (stretch_y)
                        {
                            y = initialScaleDirY * stretchFactor;
                        }
                        if (stretch_z)
                        {
                            z = initialScaleDirZ * stretchFactor;
                        }

                        objects.transform.parent.localScale = new Vector3(x, y, z);
                    }
                }
            }
        }
    }

    // methods for the three different stretch buttons
    public void StretchX_Click()
    {
        stretch_x = !stretch_x;

        // change colour of the button
        ColorUtility.TryParseHtmlString("#B3F2E5", out Color myColor);

        if (stretch_x)
        {
            stretchButtons[0].image.color = myColor;
        }
        else
        {
            stretchButtons[0].image.color = Color.white;
        }

    }

    public void StretchY_Click()
    {
        stretch_y = !stretch_y;

        // change colour of the button
        ColorUtility.TryParseHtmlString("#B3F2E5", out Color myColor);

        if (stretch_y)
        {
            stretchButtons[1].image.color = myColor;
        }
        else
        {
            stretchButtons[1].image.color = Color.white;
        }
    }

    public void StretchZ_Click()
    {
        stretch_z = !stretch_z;

        // change colour of the button
        ColorUtility.TryParseHtmlString("#B3F2E5", out Color myColor);

        if (stretch_z)
        {
            stretchButtons[2].image.color = myColor;
        }
        else
        {
            stretchButtons[2].image.color = Color.white;
        }
    }

    public void Colour_Click()
    {
        // if the user moved the objects around before
        removeObjectsFromCamera();
        movement = false;

        // deactivate the function of the control pad
        controlPadIsNotActive = false;
        setControlPadBoolsOnFalse();

        currentState = manipulationStates.Color;
        
        highlightIcon();

        activateGameObjects(new GameObject[] {uiColor});
    }

    
    public void Copy_Click()
    {
        // if the user moved the objects around before
        removeObjectsFromCamera();
        movement = false;

        // deactivate the function of the control pad
        controlPadIsNotActive = false;
        setControlPadBoolsOnFalse();

        currentState = manipulationStates.Copy;

        highlightIcon();

        // deactivate everything
        activateGameObjects(new GameObject[] {null});

        /*
         * This foreach loop is the main copying function in this method
         */

        ArrayList copyObjects = new ArrayList();
        foreach (GameObject objects in listOfMarkedObjects)
        {
            // Instantiates a certain cloned Stage
            GameObject copiedStage = null;
            if (objects.name.Contains("Cube"))
            {
                copiedStage = Instantiate(clonedStages[0]);
            }
            else if (objects.name.Contains("Cylinder"))
            {
                copiedStage = Instantiate(clonedStages[1]);
            }
            else if (objects.name.Contains("Sphere"))
            {
                copiedStage = Instantiate(clonedStages[2]);
            }
            else if (objects.name.Contains("Capsule"))
            {
                copiedStage = Instantiate(clonedStages[3]);
            }
            else if (objects.name.Contains("Pyramid"))
            {
                copiedStage = Instantiate(clonedStages[4]);
            }
            else if (objects.name.Contains("Cone"))
            {
                copiedStage = Instantiate(clonedStages[5]);
            }
            else if (objects.name.Contains("Hemisphere"))
            {
                copiedStage = Instantiate(clonedStages[6]);
            }
            else if (objects.name.Contains("Tube"))
            {
                copiedStage = Instantiate(clonedStages[7]);
            }
            else if (objects.name.Contains("Ring"))
            {
                copiedStage = Instantiate(clonedStages[8]);
            }
            else if (objects.name.Contains("Prism"))
            {
                copiedStage = Instantiate(clonedStages[9]);
            }


            // copy the position for the child (the parent of the main object) of the copied stage
            copiedStage.transform.GetChild(0).position = new Vector3(objects.transform.parent.position.x, 
                                        objects.transform.parent.position.y, objects.transform.parent.position.z);

            // copy the rotation for the main object of the copied stage
            Vector3 rotation = objects.transform.eulerAngles;
            copiedStage.transform.GetChild(0).GetChild(0).transform.eulerAngles = rotation;


            // copy the scale for the child (the parent of the main object) of the copied stage
            copiedStage.transform.GetChild(0).transform.localScale = new Vector3(objects.transform.parent.localScale.x, objects.transform.parent.localScale.y, objects.transform.parent.localScale.z);

            // copy the colour for the main object of the copied stage
            float r = objects.GetComponent<MeshRenderer>().material.color.r;
            float g = objects.GetComponent<MeshRenderer>().material.color.g;
            float b = objects.GetComponent<MeshRenderer>().material.color.b;
            float a = objects.GetComponent<MeshRenderer>().material.color.a;

            copiedStage.transform.GetChild(0).GetChild(0).GetComponent<Renderer>().material.SetColor("_Color", new Color(r, g, b, a));


            // get and then set the outline
            var outline = copiedStage.transform.GetChild(0).GetChild(0).GetComponent<Outline>();

            outline.OutlineMode = Outline.Mode.OutlineAll;
            outline.OutlineColor = new Color(1-r, 1-g, 1-b, 1);
            outline.OutlineWidth = 7f;

            // add the main copied objects to an ArrayList
            copyObjects.Add(copiedStage.transform.GetChild(0).GetChild(0).gameObject);
        }

        // there is no outline after you enter the selectionmode
        foreach (GameObject item in listOfMarkedObjects)
        {
            var outline = item.GetComponent<Outline>();

            outline.OutlineMode = Outline.Mode.OutlineHidden;
            outline.OutlineColor = new Color(0, 0, 0, 0);
            outline.OutlineWidth = 0f;

        }

        // clear the dynamic Lists
        listOfMarkedObjects.Clear();
        objectsStages.Clear();

        // add the copied objects to marked objects
        foreach (GameObject copies in copyObjects)
        {
            listOfMarkedObjects.Add(copies);
            objectsStages.Add(copies, copies.transform.parent.parent);
            copies.transform.parent.parent = camera.transform;
        }
    
        // after copying: you can now move the copied object
        ButtonMove_Click();  
    }

    /*
     * All following methods are for changing the variables 
     * for the control pad.
     */

    public void setControlPadBoolsOnFalse()
    {
        up_y = false;
        down_y = false;
        right_x = false;
        left_x = false;
        up_z = false;
        down_z = false;
    }


    public void ButtonUP_Click(bool up)
    {
        up_y = up;
        
    }

    public void ButtonDOWN_Click(bool down)
    {
        down_y = down;
    }

    public void ButtonRIGHT_Click(bool right)
    {
        right_x = right;
    }

    public void ButtonLEFT_Click(bool left)
    {
        left_x = left;
    }

    public void ButtonUPZ_Click(bool up)
    {
        up_z = up;
    }

    public void ButtonDOWNZ_Click(bool down)
    {
        down_z = down;
    }

    public void ButtonRightROTATE_Click(bool right)
    {
        rotate_right = right;
    }

    public void ButtonLeftROTATE_Click(bool left)
    {
        rotate_left = left;
    }
}
