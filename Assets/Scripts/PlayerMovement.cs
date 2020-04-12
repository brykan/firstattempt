using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private float jumpSpeed = 18.0f;
    private float gravity = 32.0f;
    private float runSpeed = 15.0f;
    private float walkSpeed = 45.0f;
    private float rotateSpeed = 150.0f;
 
    private bool grounded = false;
    private Vector3 moveDirection = Vector3.zero;
    private bool isWalking = false;
    private string moveStatus = "idle";
 
    static bool dead  = false;

    Vector3 velocity;
    private bool _isGrounded = true;
    CharacterController controller;
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {

        if (dead == false)
        {





            // Only allow movement and jumps while grounded
            if (grounded)
            {
                moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

                // if moving forward and to the side at the same time, compensate for distance
                // TODO: may be better way to do this?
                //if (Input.GetAxis("Horizontal") && Input.GetAxis("Vertical"))
                //{
                //    moveDirection *= .7;

                //}

                moveDirection = transform.TransformDirection(moveDirection);
                moveDirection *= isWalking ? walkSpeed : runSpeed;

                moveStatus = "idle";
                if (moveDirection != Vector3.zero)
                    moveStatus = isWalking ? "walking" : "running";

                // Jump!
                //if(Input.GetButton("Jump"))

                if (Input.GetKeyDown(KeyCode.Space))

                    moveDirection.y = jumpSpeed;
            }

            // Allow turning at anytime. Keep the character facing in the same direction as the Camera if the right mouse button is down.
           
                transform.rotation = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0);
       
                //transform.Rotate(0, Input.GetAxis("Horizontal") * rotateSpeed * Time.deltaTime, 0);






            // Toggle walking/running with the T key
            //if(Input.GetKeyDown("t"))
            //isWalking = !isWalking;




            //Apply gravity
            moveDirection.y -= gravity * Time.deltaTime;


            //Move controller
            
            CollisionFlags flags = controller.Move(moveDirection * Time.deltaTime);
            grounded = (flags & CollisionFlags.Below) != 0;


        }


        if (Input.GetMouseButton(1) || Input.GetMouseButton(0))
        {
            //Screen.lockCursor = true;

            Cursor.visible = false;


            //var mouse1 = Input.mousePosition.y;
            //var mouse2 = Input.mousePosition.x;

        }

        //Vector3 mousePos = Input.mousePosition;
        else
        {
            //Screen.lockCursor = false;
            Cursor.visible = false;

            //Input.mousePosition.y = mouse1;
            //Input.mousePosition.x = mouse2;

            //Input.mousePosition = mousePos;

        }


    }
}
