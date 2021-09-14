using UnityEngine;

/*
 * This class is the solution of our copying problem.
 * After copying, the MeshRenderer and the Collider of the objects were disabled.
 * 
 * But now, we enable them manually.
 */
public class DefaultObjectsParameters : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // copied objects appear on the screen
        this.GetComponent<MeshRenderer>().enabled = true;
        this.GetComponent<Collider>().enabled = true;
    }

}
