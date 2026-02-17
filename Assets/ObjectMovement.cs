using UnityEditor.ShaderKeywordFilter;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ObjectMovement : MonoBehaviour
{
    // moves an object when something tells it to
    // i.e., press a button and a wall moves down

    public bool triggeredByButton = false;
    public bool resetOnTriggerFalse = false;
    public bool triggerAnotherOnFinish = false;
    [Tooltip("if false it will trigger after it's finished going to the location")]
    public bool finishAfterReset;
    public ObjectMovement next;
    [Tooltip("leave it null if you don't need it to be triggeredbybutton")]
    public GameButton button;
    [Tooltip("copies rotate AND position")]
    public Transform moveToTransform; // will copy rotate and position
    [Tooltip("if you save the new transform to a gameobject attached to the object that is moving")]
    public bool ignoreLocal = true;
    [Tooltip("if you want it to move position")]
    public bool positionCopy = true;
    [Tooltip("if you want it to rotate")]
    public bool rotateCopy = true;
    [Tooltip("if you want it to copy scale")]
    public bool scaleCopy = false;
    [Tooltip("moves back to original position after finished")]
    public bool resets;
    [Tooltip("time delay before going back to reset position")]
    public float resetDelay;
    [Tooltip("if not null and resets, it will move backwards to this position instead of original position")]
    public Transform moveBackTransform;
    [Range(0.01f, 1000)]
    public float moveSpeed = 6;
    [Tooltip("leave as 0 to copy moveSpeed if you don't care. Don't set it to 0 to 'not rotate'.")]
    [Range(0, 1000)]
    public float rotateSpeed = 0;
    [Tooltip("leave as 0 to copy moveSpeed if you don't care. Don't set it to 0 to 'not scale'.")]
    [Range(0, 1000)]
    public float scaleSpeed = 0;
    [Tooltip("change the speed of going back for both rotate and move")]
    [Range(0.01f, 1000)]
    public float backSpeedMultiplier = 1;
    [Tooltip("you can follow the transform if it's moving")]
    public bool followTheTransform = false;

    public float snapAmount = 0.1f;

    Vector3 savedToTransformPosition;
    Vector3 savedToTransformScale;
    Quaternion savedToTransformRotation;
    Vector3 savedBackTransformPosition;
    Vector3 savedBackTransformScale;
    Quaternion savedBackTransformRotation;

    bool moving;
    bool reset;

    bool moved;
    bool rotated;
    bool scaled;

    [Header("Order")]
    public bool moveFirst = false;
    public bool rotateFirst = false;

    // You can change this if you want. I can't be bothered

    public bool scaleFirst = false;
    public bool scaleLast = false;
    public bool switchOrderOnReturn = true;

    // I'm not setting up any legit logic, this script doesn't matter

    bool resetmoveFirst;
    bool resetrotateFirst;

    bool resetscaleFirst;
    bool resetscaleLast;

    [Header("How many times you can do this")]
    [Header("(this won't do anything if reset is false)")]
    public bool repeatEternally = false;
    public bool allowPressAgain = false;
    bool hasBeenPressedOnce;

    private void Awake()
    {

        if (moveBackTransform == null)
        {
            moveBackTransform = transform;
        }

        if (!switchOrderOnReturn)
        {
            resetmoveFirst = moveFirst;
            resetrotateFirst = rotateFirst;
            resetscaleFirst = scaleFirst;
            resetscaleLast = scaleLast;
        }
        else
        {
            resetmoveFirst = !moveFirst;
            resetrotateFirst = !rotateFirst;
            resetscaleFirst = !scaleFirst;
            resetscaleLast = !scaleLast;
        }

        if (ignoreLocal)
        {
            // Ignore local is going to ignore the fact that you saved the new position to a gameobject attached to this one. If you don't do this, it will be using the position + whatever the new transform is supposed to be (I assume).
            // You can change the name of this bool if you want
            moveToTransform.position = moveToTransform.localPosition;
            moveToTransform.rotation = moveToTransform.localRotation;

            if (moveBackTransform != null)
            {
                moveBackTransform.position = moveBackTransform.localPosition;
                moveBackTransform.rotation = moveBackTransform.localRotation;
            }
        }

        // using saved values because it will change when you move it

        savedToTransformPosition = moveToTransform.position;
        savedToTransformScale = moveToTransform.localScale;
        savedToTransformRotation = moveToTransform.rotation;
        if (moveBackTransform != null)
        {
            savedBackTransformPosition = moveBackTransform.position;
            savedBackTransformScale = moveBackTransform.localScale;
            savedBackTransformRotation = moveBackTransform.rotation;
        }

        if(rotateSpeed <= 0)
        {
            rotateSpeed = moveSpeed;
        }
        if (scaleSpeed <= 0)
        {
            scaleSpeed = moveSpeed;
        }
    }

    void FixedUpdate()
    {
        // this code is awful but it just doesn't matter

        if (followTheTransform)
        {
            if (!ignoreLocal)
            {
                savedToTransformPosition = moveToTransform.position;
                savedToTransformScale = moveToTransform.localScale;
                savedToTransformRotation = moveToTransform.rotation;
                if (moveBackTransform != null)
                {
                    savedBackTransformPosition = moveBackTransform.position;
                    savedBackTransformScale = moveBackTransform.localScale;
                    savedBackTransformRotation = moveBackTransform.rotation;
                }
            }
            else
            {
                // We might not need to do this local position stuff but whatever

                moveToTransform.position = moveToTransform.localPosition;
                moveToTransform.rotation = moveToTransform.localRotation;

                savedToTransformPosition = moveToTransform.position;
                savedToTransformScale = moveToTransform.localScale;
                savedToTransformRotation = moveToTransform.rotation;
                if (moveBackTransform != null)
                {
                    moveBackTransform.position = moveBackTransform.localPosition;
                    moveBackTransform.rotation = moveBackTransform.localRotation;

                    savedBackTransformPosition = moveBackTransform.position;
                    savedBackTransformScale = moveBackTransform.localScale;
                    savedBackTransformRotation = moveBackTransform.rotation;
                }
            }
        }

        if (!reset)
        {
            if (resetOnTriggerFalse && triggeredByButton && button != null)
            {
                if (!button.triggered && hasBeenPressedOnce) // has been pressed once should be fine, does this if it's already been started before
                {
                    InstantReset();
                }
            }

            if (moving)
            {
                if (scaleCopy && !scaled)
                {
                    if (!scaleLast)
                    {
                        Scale(savedToTransformScale, out bool doneScale);
                        if (doneScale)
                        {
                            scaled = true;
                        }
                        else
                        {
                            if (scaleFirst)
                            {
                                return;
                            }
                            // nothing else has to consider this because it will never reach it on scaleFirst if it's not scaled.
                        }
                    }
                    else
                    {
                        if (rotateCopy && rotated && moved || !rotateCopy && moved)
                        {
                            Scale(savedToTransformScale, out bool doneScale);
                            if (doneScale)
                            {
                                scaled = true;
                            }
                        }
                    }
                }

                if (!rotateCopy && positionCopy)
                {
                    MovePosition(savedToTransformPosition, out bool doneMove);
                    if (doneMove)
                    {
                        // remove the reset garbage from the if statements
                        moved = true;
                    }
                }else if(!positionCopy && rotateCopy)
                {
                    Rotate(savedToTransformRotation, out bool doneRotate);
                    if (doneRotate)
                    {
                        rotated = true;
                    }
                }
                else
                {
                    if (!moveFirst && !rotateFirst || moveFirst && rotateFirst)
                    {
                        if (!moved)
                        {
                            MovePosition(savedToTransformPosition, out bool doneMove);
                            if(doneMove)
                            moved = true;
                        }
                        if (!rotated)
                        {
                            Rotate(savedToTransformRotation, out bool doneRotate);
                            if(doneRotate)
                            rotated = true;
                        }

                    }
                    else if (moveFirst && !rotateFirst)
                    {
                        if (!moved)
                        {
                            MovePosition(savedToTransformPosition, out bool doneMove);
                            if (doneMove)
                                moved = true;
                        }
                        else // this else should maintain the order
                        {
                            if (!rotated)
                            {
                                Rotate(savedToTransformRotation, out bool doneRotate);
                                if (doneRotate)
                                    rotated = true;
                            }
                        }

                    }
                    else if (rotateFirst && !moveFirst)
                    {
                        if (!rotated)
                        {
                            Rotate(savedToTransformRotation, out bool doneRotate);
                            if (doneRotate)
                                rotated = true;
                        }
                        else // else should keep it in order
                        {
                            if (!moved)
                            {
                                MovePosition(savedToTransformPosition, out bool doneMove);
                                if (doneMove)
                                    moved = true;
                            }
                        }
                    }
                }





                if ((moved || !positionCopy) && (!rotateCopy || rotated) && (!scaleCopy || scaled))
                {
                    moving = false;

                    if (resets)
                    {
                        if (resetDelay > 0)
                        {
                            Invoke("ResetDelayed", resetDelay);
                        }
                        else
                            reset = true;
                    }

                    if (triggerAnotherOnFinish && !finishAfterReset)
                    {
                        next.CallMove();
                    }

                    moved = false;
                    rotated = false;
                    scaled = false;
                }

            }
            else
            {   
                // no repeat eternal because that will loop over and not use press
                if (!hasBeenPressedOnce || (allowPressAgain && !repeatEternally && (resets || resetOnTriggerFalse))) // these variable names refer to buttons but it applies to everything
                {
                    if (triggeredByButton && button != null)
                    {
                        if (button.triggered)
                        {
                            moving = true;
                            if (!hasBeenPressedOnce)
                                hasBeenPressedOnce = true;
                        }
                    }
                }
            }
        }
        else
        {
            if (scaleCopy && !scaled)
            {
                if (!resetscaleLast)
                {
                    Scale(savedBackTransformScale, out bool doneScale, backSpeedMultiplier);
                    if (doneScale)
                    {
                        scaled = true;
                    }
                    else
                    {
                        if (resetscaleFirst)
                            return;
                        // nothing else has to consider this because it will never reach it on scaleFirst if it's not scaled.
                    }
                }
                else
                {
                    if (rotateCopy && rotated && moved || !rotateCopy && moved)
                    {
                        Scale(savedBackTransformScale, out bool doneScale, backSpeedMultiplier);
                        if (doneScale)
                        {
                            scaled = true;
                        }
                    }
                }
            }

            if (!rotateCopy && positionCopy)
            {
                MovePosition(savedBackTransformPosition, out bool doneMove, backSpeedMultiplier);
                if (doneMove)
                {
                    // eliminate bullshit

                    moved = true;
                }
            }
            else if (!positionCopy && rotateCopy)
            {
                Rotate(savedBackTransformRotation, out bool doneRotate);
                if (doneRotate)
                {
                    // fuck off the resetting garbage















                                                      
                    /// |||||||        |-------\  ____________                
                    /// O! oWo !       | ------/                                       
                    /// \_____/        | )
                    ///  \   \     




                    rotated = true;


                }
            }
            else
            {
                if (!resetmoveFirst && !resetrotateFirst || resetmoveFirst && resetrotateFirst)
                {
                    if (!moved)
                    {
                        MovePosition(savedBackTransformPosition, out bool doneMove, backSpeedMultiplier);
                        if (doneMove)
                            moved = true;
                    }
                    if (!rotated)
                    {
                        Rotate(savedBackTransformRotation, out bool doneRotate, backSpeedMultiplier);
                        if(doneRotate)
                            rotated = true;
                    }

                }
                else if (resetmoveFirst)
                {
                    if (!moved)
                    {
                        MovePosition(savedBackTransformPosition, out bool doneMove, backSpeedMultiplier);
                        if (doneMove)
                            moved = true;
                    }
                    else
                    {
                        if (!rotated)
                        {
                            Rotate(savedBackTransformRotation, out bool doneRotate, backSpeedMultiplier);
                            if (doneRotate)
                                rotated = true;
                        }
                    }
                }
                else if (resetrotateFirst)
                {
                    if (!rotated)
                    {
                        Rotate(savedBackTransformRotation, out bool doneRotate, backSpeedMultiplier);
                        if (doneRotate)
                            rotated = true;
                    }
                    else
                    {
                        if (!moved)
                        {
                            MovePosition(savedBackTransformPosition, out bool doneMove, backSpeedMultiplier);
                            if (doneMove)
                                moved = true;
                        }
                    }
                }
            }

            if ((moved || !positionCopy) && (!rotateCopy || rotated) && (!scaleCopy || scaled))
            {
                reset = false;
                moved = false;
                rotated = false;
                scaled = false;

                if (triggerAnotherOnFinish && finishAfterReset)
                {
                    next.CallMove();
                }
                if (repeatEternally)
                    moving = true; // make it go again
            }
        }
    }
    
    void ResetDelayed()
    {
        reset = true;
    }


    // pos
    // times ten because it's slow and changing everything would be annoying
    void MovePosition(Vector3 toPos, out bool done, float multiplier = 1)
    {
        done = false;

        transform.position = Vector3.MoveTowards(transform.position, toPos, Time.deltaTime * moveSpeed * multiplier * 10);
        if (Vector3.Distance(transform.position, toPos) < snapAmount)
        {
            transform.position = toPos;
            done = true;
        }
    }

    // ros
    void Rotate(Quaternion toROS, out bool done, float multiplier = 1)
    {
        done = false;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, toROS, rotateSpeed * Time.deltaTime * multiplier * 10);
        if (Quaternion.Angle(transform.rotation, toROS) < snapAmount)
        {
            transform.rotation = toROS;
            done = true;
        }
    }

    // scos
    void Scale(Vector3 toSCOS, out bool done, float multiplier = 1)
    {
        done = false;

        transform.localScale = Vector3.MoveTowards(transform.localScale,toSCOS,Time.deltaTime * scaleSpeed * multiplier * 10);

        if (Vector3.Distance(transform.localScale, toSCOS) < snapAmount)
        {
            transform.localScale = toSCOS;
            done = true;
        }
    }


    public void CallMove()
    {
        moving = true;
    }

    public void InstantReset()
    {
        moving = false;

        reset = true;

        moved = false;
        rotated = false;
        scaled = false;
    }
}
