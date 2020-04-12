using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{

    public Transform target;                      // Target to follow
    float targetHeight = 1.7f;                         // Vertical offset adjustment
    float distance = 12.0f;                            // Default Distance
    float offsetFromWall = 0.1f ;                       // Bring camera away from any colliding objects
    float maxDistance = 20f;                       // Maximum zoom Distance
    float  minDistance = 0.6f;                      // Minimum zoom Distance
    float  xSpeed = 200.0f;                             // Orbit speed (Left/Right)
    float  ySpeed = 200.0f;                             // Orbit speed (Up/Down)
    float yMinLimit = -80f;                            // Looking up limit
    float  yMaxLimit = 80f;                             // Looking down limit
    float zoomRate = 40f;                          // Zoom Speed
    float rotationDampening = 3.0f;                // Auto Rotation speed (higher = faster)
    float zoomDampening = 5.0f;                    // Auto Zoom speed (Higher = faster)
    LayerMask collisionLayers = -1;     // What the camera will collide with
    bool lockToRearOfTarget = false;             // Lock camera to rear of target
    bool allowMouseInputX = true;                // Allow player to control camera angle on the X axis (Left/Right)
    bool allowMouseInputY = true;                // Allow player to control camera angle on the Y axis (Up/Down)

    private float xDeg = 0.0f;
    private float yDeg = 0.0f;
    private float currentDistance;
    private float desiredDistance;
    private float correctedDistance;
    private bool rotateBehind = false;
    // Start is called before the first frame update
    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        xDeg = angles.x;
        yDeg = angles.y;
        currentDistance = distance;
        desiredDistance = distance;
        correctedDistance = distance;

        // Make the rigid body not change rotation
        if (GetComponent<Rigidbody>())
            GetComponent<Rigidbody>().freezeRotation = true;

        if (lockToRearOfTarget)
            rotateBehind = true;
    }

    // Update is called once per frame
    void LateUpdate()
    {



        // Don't do anything if target is not defined
        if (!target)
            return;

        Vector3 vTargetOffset;


        // If either mouse buttons are down, let the mouse govern camera position
        //if (GUIUtility.hotControl == 0)
        //{
          //  if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
           // {
                //Check to see if mouse input is allowed on the axis
                if (allowMouseInputX)
                    xDeg += (float)(Input.GetAxis("Mouse X") * xSpeed * 0.02);
                else
                    RotateBehindTarget();
                if (allowMouseInputY)
                    yDeg -= (float) (Input.GetAxis("Mouse Y") * ySpeed * 0.02);

                //Interrupt rotating behind if mouse wants to control rotation
                if (!lockToRearOfTarget)
                    rotateBehind = false;
            //}

            // otherwise, ease behind the target if any of the directional keys are pressed
            //else if (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0 || rotateBehind)
            //{
                //RotateBehindTarget();
            //}
        //}
        yDeg = ClampAngle(yDeg, yMinLimit, yMaxLimit);

        // Set camera rotation
        Quaternion rotation = Quaternion.Euler(yDeg, xDeg, 0);

        // Calculate the desired distance
        desiredDistance -= Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * zoomRate * Mathf.Abs(desiredDistance);
        desiredDistance = Mathf.Clamp(desiredDistance, minDistance, maxDistance);
        correctedDistance = desiredDistance;

        // Calculate desired camera position
        vTargetOffset = new Vector3(0, -targetHeight, 0);
        Vector3 position = target.position - (rotation * Vector3.forward * desiredDistance + vTargetOffset);

        // Check for collision using the true target's desired registration point as set by user using height
        RaycastHit collisionHit;
        Vector3 trueTargetPosition = new Vector3(target.position.x, target.position.y + targetHeight, target.position.z);

        // If there was a collision, correct the camera position and calculate the corrected distance
        var isCorrected = false;
        if (Physics.Linecast(trueTargetPosition, position, out collisionHit, collisionLayers,QueryTriggerInteraction.Ignore))
        {
            // Calculate the distance from the original estimated position to the collision location,
            // subtracting out a safety "offset" distance from the object we hit.  The offset will help
            // keep the camera from being right on top of the surface we hit, which usually shows up as
            // the surface geometry getting partially clipped by the camera's front clipping plane.
            correctedDistance = Vector3.Distance(trueTargetPosition, collisionHit.point) - offsetFromWall;
            isCorrected = true;
        }

        // For smoothing, lerp distance only if either distance wasn't corrected, or correctedDistance is more than currentDistance
        currentDistance = !isCorrected || correctedDistance > currentDistance ? Mathf.Lerp(currentDistance, correctedDistance, Time.deltaTime * zoomDampening) : correctedDistance;

        // Keep within limits
        currentDistance = Mathf.Clamp(currentDistance, minDistance, maxDistance);

        // Recalculate position based on the new currentDistance
        position = target.position - (rotation * Vector3.forward * currentDistance + vTargetOffset);

        //Finally Set rotation and position of camera
        transform.rotation = rotation;
        transform.position = position;
    }
    void RotateBehindTarget()
    {
        float targetRotationAngle = target.eulerAngles.y;
        float currentRotationAngle = transform.eulerAngles.y;
        xDeg = Mathf.LerpAngle(currentRotationAngle, targetRotationAngle, rotationDampening * Time.deltaTime);

        // Stop rotating behind if not completed
        if (targetRotationAngle == currentRotationAngle)
        {
            if (!lockToRearOfTarget)
                rotateBehind = false;
        }
        else
            rotateBehind = true;

    }
    float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360)
            angle += 360;
        if (angle > 360)
            angle -= 360;
        return Mathf.Clamp(angle, min, max);
    }

}
