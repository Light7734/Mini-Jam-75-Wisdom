using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(PlayerController))]
public class Player : MonoBehaviour
{
    #region Movement
    [SerializeField] private PlayerController controller;

    // General Movement
    [SerializeField] [Range(1f, 10f)] private float moveSpeed = 6f;

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

    [SerializeField] private SceneLoader sceneLoader;
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Animator animator;
    [SerializeField] private Gate gate = null;

    private List<GameObject> crystals = new List<GameObject>();
    private List<GameObject> crystalsInRange = new List<GameObject>();

    private bool gateInRange = false;
    private bool shiftDown = false;
    private bool dying = false;

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

    private static bool started = false;

    void Start()
    {
        if(!started)
        {
            SoundManager.i.PlaySound(SoundManager.Sound.GameBackground, Vector3.zero, true, true);
            started = true;
        }

        shiftAction = new InputAction(binding: "<Keyboard>/leftShift");
        shiftAction.Enable();

        shiftAction.performed += context => shiftDown = true;
        shiftAction.canceled += context => shiftDown = false;

        // Controller attributes
        PlayerController.Attributes controllerAttributes;

        controllerAttributes.maxSlopeAscendAngle = maxClimbAngle;
        controllerAttributes.maxSlopeDecendAngle = maxDecendAngle;

        controller.SetAttributes(controllerAttributes);

        // Jump / Gravity
        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        jumpVelocity = Mathf.Abs(gravity * timeToJumpApex);


        crystals = new List<GameObject>();
    }

    void Update()
    {
        if (dying)
            return;

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
            if (weightIndex == 2)
                SoundManager.i.PlaySound(SoundManager.Sound.normalJump);

            if (weightIndex > 2)
                SoundManager.i.PlaySound(SoundManager.Sound.highJump);

            if (weightIndex < 2)
                SoundManager.i.PlaySound(SoundManager.Sound.lowJump);

            desiredVelocity.y = jumpVelocity;
            isJumping = false;
        }

        controller.Move(desiredVelocity * Time.deltaTime, ForceDir.Self);

        #endregion // __MOVEMENT__ //

    }

    private void AssignNewWeightStatus(int index)
    {
        PlayerWeightState newState = weightStates[index];

        jumpHeight = newState.jumpHeight;
        timeToJumpApex = newState.timeToJumpApex;

        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        jumpVelocity = Mathf.Abs(gravity * timeToJumpApex);

        if (index == 4)
            sprite.color = new Color(43f / 255f, 136f / 255f, 255f / 255f);
        else if (index == 3)
            sprite.color = new Color(0f, 242f / 255f, 251f / 255f);
        else if (index == 2)
            sprite.color = new Color(0f, 255f / 255f, 0f, 255f);
        else if (index == 1)
            sprite.color = new Color(255f / 255f, 145f / 255f, 42f / 255f);
        else if (index == 0)
            sprite.color = new Color(255f / 255f, 55f / 255f, 10f / 255f);
    }

    public void OnInteract(InputValue value)
    {
        if (gate == null)
            return;

        if (!shiftDown && crystalsInRange.Count != 0)
        {
            crystals.Add(crystalsInRange[0].gameObject);

            if (crystalsInRange[0].tag == "blueCrystal")
            {
                SoundManager.i.PlaySound(SoundManager.Sound.CrystalPickupBlue);
                AssignNewWeightStatus(++weightIndex);
            }
            else if (crystalsInRange[0].tag == "redCrystal")
            {
                SoundManager.i.PlaySound(SoundManager.Sound.CrystalPickupRed);
                AssignNewWeightStatus(--weightIndex);
            }

            crystalsInRange[0].SetActive(false);

            return;
        }
        else if(shiftDown && crystals.Count != 0)
        {
            SoundManager.i.PlaySound(SoundManager.Sound.CrystalDrop);
            crystals[0].SetActive(true);
            crystals[0].transform.position = transform.position + new Vector3(0f, .6f);

            if (crystals[0].tag == "blueCrystal")
                AssignNewWeightStatus(--weightIndex);
            else if (crystals[0].tag == "redCrystal")
                AssignNewWeightStatus(++weightIndex);

            crystals.RemoveAt(0);

            return;
        }

        if(!shiftDown && crystals.Count != 0 && gateInRange && !gate.open)
        {
            gate.AddCrystal(crystals[crystals.Count - 1]);

            if(crystals[crystals.Count - 1].tag == "blueCrystal")
                AssignNewWeightStatus(--weightIndex);
            else if(crystals[crystals.Count - 1].tag == "redCrystal")
                AssignNewWeightStatus(++weightIndex);

            crystals.RemoveAt(crystals.Count - 1);

            return;
        }
        else if(shiftDown && gateInRange && !gate.open)
        {
            GameObject crystal = gate.RemoveCrystal();
            if(crystal != null)
            {
                crystals.Add(crystal);

                if (crystal.tag == "blueCrystal")
                    AssignNewWeightStatus(++weightIndex);
                else if (crystal.tag == "redCrystal")
                    AssignNewWeightStatus(--weightIndex);
            }

            return;
        }

        if (!shiftDown && gate.open && gateInRange)
            sceneLoader.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
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
        if(collision.tag == "blueCrystal" || collision.tag == "redCrystal")
        {
            collision.transform.position = new Vector2(collision.transform.position.x, collision.transform.position.y + .15f);
            collision.transform.GetChild(0).gameObject.SetActive(true);

            crystalsInRange.Add(collision.gameObject);
        }

        if(collision.tag == "gate")
        {
            collision.transform.GetChild(0).gameObject.SetActive(true);
            gateInRange = true;
        }

        if(collision.tag == "spike")
        {
            dying = true;
            animator.SetTrigger("dying");

            sceneLoader.LoadScene(SceneManager.GetActiveScene().buildIndex);
            SoundManager.i.PlaySound(SoundManager.Sound.Death);
        }

        if(collision.tag == "pressurePlate")
            collision.gameObject.GetComponent<PerssurePlate>().ApplyPressure(weightIndex);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "blueCrystal" || collision.tag == "redCrystal")
        {
            collision.transform.position = new Vector2(collision.transform.position.x, collision.transform.position.y - .15f);
            collision.transform.GetChild(0).gameObject.SetActive(false);

            crystalsInRange.Remove(collision.gameObject);
        }

        if (collision.tag == "gate")
        {
            collision.transform.GetChild(0).gameObject.SetActive(false);
            gateInRange = false;
        }
    }

}
