using UnityEngine;

/*
 * This class is to mark objects with touch input
 * So we do not need an update() to detect touching objects
 * Methods:
 *		- OnMouseDown
 */
public class MarkController : MonoBehaviour
{
	// to select objects and switch modes
	public SwitchMode sw;
	public UI_Manipulation_Script ui_Manipulation_Script;

	private bool marked;

    private void Start()
    {
		marked = sw.getListOfMarkedObjects().Contains(this.gameObject);
    }

    // this method will be called after every input
    private void OnMouseDown()
    {
		if (ui_Manipulation_Script.currentState != UI_Manipulation_Script.manipulationStates.Select)
		{
			// do NOT detect a gameobject in the scene
			return;
		}


		if (!marked)
        {
			// mark the selected object
			sw.markObjects(this.gameObject);
			marked = !marked;
        }
		else
        {
			// demark the selected object
			sw.demarkObjects(this.gameObject);
			marked = !marked;
		}
	}
}
