using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/*
 * This class implements the function to switch between
 * manipulationmode and selectionmode.
 * Methods:
 *		- switch to selectionmode
 *		- switch to manipulationmode
 *		- mark objects
 *		- delete several or all stages
 *		- acitvate one certain UI element
 */
public class SwitchMode : MonoBehaviour
{
	// all UI GameObjects
	public GameObject listStagesPositioners;
	public GameObject uiSelectionmode;
	public GameObject uiManipulationmode;
	public GameObject uiRotation;
	public GameObject uiControlPad;
	public GameObject switchMoveControl;
	public GameObject uiColourSlider;
	public GameObject uiStretchButtons;
	public GameObject moveUI;

	// ArrayList of all UI GameObjects
	private ArrayList listUI = new ArrayList();

	public UI_Manipulation_Script ui_Manipulation_Script;

	// Feedback after deleting objects
	public GameObject particleSystem;

	// a variable to save the current marked object
	//private GameObject baseObject;

	private HashSet<GameObject> listOfMarkedObjects = new HashSet<GameObject>();

	// a dictionary of a marked object and their Mid Air Stage
	private Dictionary<GameObject, Transform> dictObjectStage = new Dictionary<GameObject, Transform>();


	void Start()
    {
		// add every UI GameObject to the ArrayList
		listUI.AddRange(new List<GameObject>
		{
			uiSelectionmode,
			uiManipulationmode,
			uiRotation,
			uiControlPad,
			switchMoveControl,
			uiColourSlider,
			uiStretchButtons,
			moveUI
		});
    }


    /*
     *  A method to mark the objects with a Raycast.
     *  save the object and stage in the HashSet and Dictionary.
     *  Every marked object gets an outline
     */
    public void markObjects(GameObject baseObject)
    {

		// Deactivate all Stages and Positioners
		// Your are not able to place an object anymore
		switchToManipulationmode();

            
		var outline = baseObject.GetComponent<Outline>();


		// get the complementary colour as outline of this object
		float r = 1 - baseObject.GetComponent<MeshRenderer>().material.color.r;
		float g = 1 - baseObject.GetComponent<MeshRenderer>().material.color.g;
		float b = 1 - baseObject.GetComponent<MeshRenderer>().material.color.b;

		// set the new outline
		outline.OutlineMode = Outline.Mode.OutlineAll;
		outline.OutlineColor = new Color(r, g, b, 1);
		outline.OutlineWidth = 7f;

		// save the object in HashSet
		listOfMarkedObjects.Add(baseObject);

		// save object and stage in the dictionary
		try
		{
			dictObjectStage.Add(baseObject, baseObject.transform.parent.parent);
		}
		catch
		{
			//ignore
		}
    }

	public void demarkObjects(GameObject baseObject)
    {
		
		var outline = baseObject.GetComponent<Outline>();


		// set the new outline
		outline.OutlineMode = Outline.Mode.OutlineHidden;
		outline.OutlineColor = new Color(0, 0, 0, 0);
		outline.OutlineWidth = 0f;

		// save the object in HashSet
		listOfMarkedObjects.Remove(baseObject);

		// save object and stage in the dictionary
		try
		{
			dictObjectStage.Remove(baseObject);
		}
		catch
		{
			//ignore
		}

		// if nothing is marked, switch mode
		if (listOfMarkedObjects.Count == 0)
        {
			// we need some time, because without it, it will detect a touch
			// and this application places an object
			Invoke("switchToSelectionmode", 0.01f);
        }
	}

    // deactivate all UI elements except the parameter
    private void activateGameObjects(GameObject gameObject)
    {
		foreach (GameObject ui in listUI)
		{
			if (ui == gameObject)
			{
				ui.SetActive(true);
				continue;
			}

			ui.SetActive(false);
		}
	}


	// the x-button in the manipulation mode calls this function
	public void switchToSelectionmode()
    {
		// activate stages and positioner and the ui of the selectionmode
		listStagesPositioners.SetActive(true);

		// disable the manipulationmode and the UIs of the manipulationtools
		activateGameObjects(uiSelectionmode);


		// there is no outline after you enter the selectionmode
		foreach (GameObject item in listOfMarkedObjects)
        {
			var outline = item.GetComponent<Outline>();

			outline.OutlineMode = Outline.Mode.OutlineHidden;
			outline.OutlineColor = new Color(0, 0, 0, 0);
			outline.OutlineWidth = 0f;

		}


		// if the user moved the objects around before
		ui_Manipulation_Script.removeObjectsFromCamera();
		
		// reset Icon Highlighting
		ui_Manipulation_Script.resetIconHighlighting();

		// clear the HashSet and the Dictionary
		listOfMarkedObjects.Clear();
		dictObjectStage.Clear();

		// no current state of a manipualtion mode
		ui_Manipulation_Script.currentState = UI_Manipulation_Script.manipulationStates.Select;
	}

	// A method to switch to the manipulation mode
	public void switchToManipulationmode()
	{
		// deactivate stages and positioner and the ui of the selectionmode
		listStagesPositioners.SetActive(false);
		activateGameObjects(uiManipulationmode);

		// setup the manipulationmode
		ui_Manipulation_Script.setControlPadBoolsOnFalse();
		ui_Manipulation_Script.highlightIcon();
		ui_Manipulation_Script.ButtonSelect_Click();
	}

	// a method for the delete button
	// It destroys the stages from the HashSet listOfMarkedObjects
	public void deleteButton_OnClick()
    {
		// if the user moved the objects around before
		ui_Manipulation_Script.removeObjectsFromCamera();

		// a teporary HashSet
		HashSet<GameObject> listIndex = new HashSet<GameObject>();
		foreach (GameObject item in listOfMarkedObjects)
		{
			GameObject temp = item.transform.parent.parent.gameObject;

			// get the same position and colour
			item.GetComponent<MeshRenderer>().enabled = false;
			item.GetComponent<Collider>().enabled = false;

			InstantiateParticleSystem(item.transform);

			Destroy(temp);
			

			listIndex.Add(item);
		}

		foreach (GameObject item in listIndex)
        {
			listOfMarkedObjects.Remove(item);
        }

		listIndex.Clear();

		// reset Icon Highlighting
		ui_Manipulation_Script.resetIconHighlighting();
		
		// switch to the selectionmode
		switchToSelectionmode();
    }

	// a method for the delete ALL button
	// It destroys the stages with a tag
	public void clearAllButton_OnClick()
	{
		// if the user moved the objects around before
		ui_Manipulation_Script.removeObjectsFromCamera();

		// destroy all placed objects and their Mid Air Stages
		// Every Stage has a Tag "MidAirStage"
		foreach (GameObject item in GameObject.FindGameObjectsWithTag("MidAirStage"))
		{
			// get the same position and colour
			Transform mainObject = item.transform.GetChild(0).GetChild(0);
			mainObject.GetComponent<MeshRenderer>().enabled = false;
			mainObject.GetComponent<Collider>().enabled = false;

			InstantiateParticleSystem(mainObject);
			
			Destroy(item);
		}

		// clear the listOfMarkedObjects
		listOfMarkedObjects.Clear();

		// reset Icon Highlighting
		ui_Manipulation_Script.resetIconHighlighting();
		
		// switch to the selectionmode
		switchToSelectionmode();
	}

	private void InstantiateParticleSystem(Transform mainObject)
    {
		GameObject instantiatedParticleSystem = Instantiate(particleSystem);

		// get the same position, colour and the mesh
		instantiatedParticleSystem.transform.position = mainObject.parent.position;
		instantiatedParticleSystem.GetComponent<ParticleSystem>().startColor = mainObject.GetComponent<MeshRenderer>().material.color;
		instantiatedParticleSystem.GetComponent<ParticleSystem>().GetComponent<ParticleSystemRenderer>().mesh = mainObject.GetComponent<MeshFilter>().mesh;

		// play ParticleSystem
		instantiatedParticleSystem.GetComponent<ParticleSystem>().Play();

		// destroy ParticleSystem and Mid Air Stage
		Destroy(instantiatedParticleSystem, 1);
	}

    // Getter
    public HashSet<GameObject> getListOfMarkedObjects(){
		return listOfMarkedObjects;
	}

	public Dictionary<GameObject, Transform> getDictionaryObjectStage()
    {
		return dictObjectStage;
    }
}
