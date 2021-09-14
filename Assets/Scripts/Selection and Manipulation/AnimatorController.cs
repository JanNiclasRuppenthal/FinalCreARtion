using UnityEngine;

/*
 * This class controlls the animations of the imported models from the ImageTargets
 * It implements the logic, when the app should play the animation.
 */
public class AnimatorController : MonoBehaviour
{
    // public variables
    public GameObject modelledObject;
    public GameObject camera;
    public float minimalDistance;

    // private variables
    private Animator anim;
    private bool details = false;
    private float distance;

    // Start is called before the first frame update
    void Start()
    {
        anim = modelledObject.GetComponent<Animator>();
    }

    public void Update()
    {
        // compute the disctance between camera and model
        distance = Vector3.Distance(camera.transform.position, modelledObject.transform.position);

        // If it is below the minimal distance then play the animation forward
        if (!details && distance < minimalDistance)
        {
            anim.Play("Base Layer.Forwards");
            details = true;
        }
        // If it is over the minimal distance then play the animation rewind
        else if (distance > minimalDistance)
        {
            anim.Play("Base Layer.Rewind");
            details = false;
        }
    }
}
