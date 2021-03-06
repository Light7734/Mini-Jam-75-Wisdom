using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// TODO: Tidy up / rename stuff
[RequireComponent(typeof(PlayerController))]
public class Player : MonoBehaviour
{
    #region Movement
    [SerializeField] private PlayerController controller;

    // General Movement
    [SerializeField] [Range(1f, 10f)] private float moveSpeed = 6f;
    [SerializeField] [Range(0f, .5f)] private float stepHeight = 0f;

    float velXSmoothing;

    // Slope movement
    [SerializeField] [Range(0f, 90f)] private float maxClimbAngle = 45f;
    [SerializeField] [Range(0f, 90f)] private float maxDecendAngle = 45f;

    // Air movement
    [SerializeField] [Range(0f, 1f)] private float accelerationAirborne = .3f;
    [SerializeField] [Range(0f, 1f)] private float accelerationGrounded = .1f;

    // Jumping
    [SerializeField] [Range(0.01f, 10f)] private float jumpHeight = 5f;
    [SerializeField] [Range(0f, 1f)] private float timeToJumpApex = .1f;

    private float gravity;
    private float jumpVelocity;

    // Current input
    private Vector2 desiredVelocity = Vector2.zero;

    private float moveDir = 0f;
    private bool isJumping = false;
    #endregion // __MOVEMENT__ //

    #region GameLogic

    private List<GameObject> crystals = new List<GameObject>();
    private List<GameObject> crystalsInRange = new List<GameObject>();

    private bool shiftDown = false;

    private int weightIndex = 2;


    #endregion // __GAME_LOGIC__

    [Serializable]
    public struct PlayerWeightState
    {
        public float speed;
        public float velocity;

        public float jumpHeight;
        public float timeToJumpApex;
    }

    [SerializeField] private List<PlayerWeightState> weightStates = new List<PlayerWeightState>();

    private InputAction shiftAction;

    void Start()
    {
        shiftAction = new InputAction(binding: "<Keyboard>/leftShift");
        shiftAction.Enable();

        shiftAction.performed += context => shiftDown = true;
        shiftAction.canceled += context => shiftDown = false;

        // Controller attributes
        PlayerController.Attributes controllerAttributes;

        controllerAttributes.maxSlopeAscendAngle = maxClimbAngle;
        controllerAttributes.maxSlopeDecendAngle = maxDecendAngle;
        controllerAttributes.stepHeight = stepHeight;

        controller.SetAttributes(controllerAttributes);

        // Jump / Gravity
        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        jumpVelocity = Mathf.Abs(gravity * timeToJumpApex);


        crystals = new List<GameObject>();
    }

    void Update()
    {
        if (controller.collisionState.above)
            Debug.Log("Collided above!!!");

        #region Movement


        // COLLISIONS //
        if (controller.collisionState.below && desiredVelocity.y < 0f)
            desiredVelocity.y = 0f;
        if (controller.collisionState.above && desiredVelocity.y > 0f)
            desiredVelocity.y = 0f;

        // MOVEMENT //
        if (controller.collisionState.below) 
            desiredVelocity.x = Mathf.SmoothDamp(desiredVelocity.x, moveDir * moveSpeed, ref velXSmoothing, accelerationGrounded);
        else
            desiredVelocity.x = Mathf.SmoothDamp(desiredVelocity.x, moveDir * moveSpeed, ref velXSmoothing, accelerationAirborne);

        // GRAVITY //
        desiredVelocity.y += gravity * Time.deltaTime;

        // JUMP
        if(isJumping)
        {
            desiredVelocity.y = jumpVelocity;
            isJumping = false;
        }

        controller.Move(desiredVelocity * Time.deltaTime, ForceDir.Self);

        #endregion // __MOVEMENT__ //

    }

    public void OnInteract(InputValue value)
    {
        if (!controller.collisionState.below)
            return;

        if (!shiftDown && crystalsInRange.Count != 0)
        {
            crystals.Add(crystalsInRange[crystalsInRange.Count - 1].gameObject);

            if (crystalsInRange[crystalsInRange.Count - 1].tag == "blueCrystal")
                AssignNewWeightStatus(++weightIndex);

            crystalsInRange[crystalsInRange.Count - 1].SetActive(false);
            crystalsInRange.RemoveAt(crystalsInRange.Count - 1);
        }
        else if(shiftDown && crystals.Count != 0)
        {
            crystals[0].SetActive(true);
            crystals[0].transform.position = transform.position + new Vector3(0f, .6f);

            if (crystals[0].tag == "blueCrystal")
                AssignNewWeightStatus(--weightIndex);

            crystals.RemoveAt(0);
        }

    }

    private void AssignNewWeightStatus(int index)
    {
        PlayerWeightState newState = weightStates[index];

        jumpHeight = newState.jumpHeight;
        timeToJumpApex = newState.timeToJumpApex;

        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        jumpVelocity = Mathf.Abs(gravity * timeToJumpApex);
    }

    public void OnJump(InputValue value)
    {
        if (controller.collisionState.below)
            isJumping = true;
    }

    public void OnMovement(InputValue value)
    {
        moveDir = value.Get<float>();    
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "blueCrystal")
        {
            collision.transform.position = new Vector2(collision.transform.position.x, collision.transform.position.y + .15f);
            collision.transform.GetChild(0).gameObject.SetActive(true);

            crystalsInRange.Add(collision.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "blueCrystal")
        {
            collision.transform.position = new Vector2(collision.transform.position.x, collision.transform.position.y - .15f);
            collision.transform.GetChild(0).gameObject.SetActive(false);

            crystalsInRange.Remove(collision.gameObject);
        }
    }


}
