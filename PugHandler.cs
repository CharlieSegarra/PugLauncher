using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//add unity's input system
using UnityEngine.InputSystem;


public class PugHandler : MonoBehaviour
{
    [SerializeField] private float detatchDelay;
    [SerializeField] private GameObject pugPrefab;
    [SerializeField] private Rigidbody2D pivot;
    [SerializeField] private float respawnDelay;

    //will be attatched to pug handler in editor
   private Rigidbody2D currentPugRigidbody;

   //references spring joint connected to pug
   private SpringJoint2D currentPugSpringJoint;
   

    //global camera
    private Camera mainCamera;

    //determine if finger is dragging
    private bool isDragging;

    // Start is called before the first frame update
    void Start()
    {
        //references the main camera in Unity and starts it
        mainCamera = Camera.main;

        SpawnNewPug();
    }

    // Update is called once per frame
    void Update()
    {
        //due to update being called every frame
        //having touch position may cause issues since finger may not be on phone every frame
        //determines if finger even pressed down with bool isPresseed
        //if not pressed return;
        if(!Touchscreen.current.primaryTouch.press.isPressed)
        {
            //prevents error when null in launch method
            if(currentPugRigidbody == null) 
            {
                return;
            }

            if(isDragging)
            {
                LaunchBall();
            }

            isDragging = false;
            
            return;
        }

        isDragging = true;
        //taken out of phsyics control
        currentPugRigidbody.isKinematic = true;

        //created variable Vector2 touchPositon
        //use touch screen class/ current is touch screen on phone
        //ReadValue() will return a Vector2 which is x and y coordinate
        Vector2 touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();

        //convert to worldspace using a camera by passing through touchPositoin
        Vector3 worldposition = mainCamera.ScreenToWorldPoint(touchPosition);

        //writes to console
        //Debug.Log(worldposition);

        //when touching the screen it will set the rigid body to that positon
        currentPugRigidbody.position = worldposition;
    }


    private void SpawnNewPug()
    {
        //unity specfic to respawn prefabs
        //quaternion identy is default (x,y,z) axis
        GameObject pugInstance = Instantiate(pugPrefab, pivot.position, Quaternion.identity);

        currentPugRigidbody = pugInstance.GetComponent<Rigidbody2D>();
        currentPugSpringJoint = pugInstance.GetComponent<SpringJoint2D>();

        //attatches pug to pivot
        currentPugSpringJoint.connectedBody = pivot;
    }

    private void LaunchBall()
    {
        //makes pug reacts to phsyics again
        currentPugRigidbody.isKinematic = false;

        //prevents pug from snapping back into place after being launched
        currentPugRigidbody = null;

        //Invoke method Unity specific will call a method after a set time
        //parameters "Method", float delay
        Invoke(nameof(DetatchPug), detatchDelay);
    }

    private void DetatchPug()
    {
           //spring joint starts at rest
        currentPugSpringJoint.enabled = false;
        currentPugSpringJoint = null;

        Invoke(nameof(SpawnNewPug), respawnDelay);
    }

}
