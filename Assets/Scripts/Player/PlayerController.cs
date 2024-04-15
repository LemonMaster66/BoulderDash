using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class PlayerController : NetworkBehaviour
{
    public Transform Camera;
    
    [Header("Rolling Physics")]
    public float speed = 50;
    public float maxSpeed = 80;
    public float JumpForce = 8;
    public float Gravity = 100;

    public bool Grounded;

    private Rigidbody rb;

    [Header("Turning Physics")]
    [Tooltip("Turning Speeds Increase With Forward Momentum (Recomended)")]
    public bool DynamicTurningSpeeds = true;

    private float movementX;
    private float movementY;

    public float turnSpeedFactor;


    [Header("Boulder Audio")]
    public AudioSource RollingAudio;
    public AudioSource JumpAudio;
    public AudioSource LandAudio;

    private float XVolume;
    private float ZVolume;

    [Tooltip("How Gradualy the Rolling Volume Increases *60 Default*")]
    public int VolumeDivide = 60;

    [Header("Debug Stats")]
    public float PlayerVelocity;
    public float ForwardVelocityMagnitude;
    public float turnSpeed;
    public float FallingVelocity;



    void Start()
    {
        rb = GetComponent<Rigidbody>();
        GetComponent<Rigidbody>().useGravity = false;

        Gravity = Gravity / 35;
    }

    void Awake()
    {
        GameObject cameraObject = GameObject.Find("Camera");
        Camera = cameraObject.transform;
    }

    public void OnMove(InputAction.CallbackContext movementValue)
    {  
        Vector2 inputVector = movementValue.ReadValue<Vector2>();

        movementX = inputVector.x;
        movementY = inputVector.y;

    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            Jump();
        }
    }

    private void FixedUpdate()
    {   
        Vector3 CamF = Camera.forward;
        Vector3 CamR = Camera.right;

        CamF.y = 0;
        CamR.y = 0;

        CamF = CamF.normalized;
        CamR = CamR.normalized;

        //Extra Gravity
        rb.AddForce(Physics.gravity * Gravity * Gravity);

        PlayerVelocity = rb.velocity.magnitude;

        // Calculate the forward velocity magnitude
        Vector3 ForwardVelocity = Vector3.Project(rb.velocity, CamF);
        ForwardVelocityMagnitude = ForwardVelocity.magnitude;

        int decimals = 2;
        PlayerVelocity = (float)System.Math.Round(PlayerVelocity, decimals);
        ForwardVelocityMagnitude = (float)System.Math.Round(ForwardVelocityMagnitude, decimals);
        turnSpeed = (float)System.Math.Round(turnSpeed, decimals);


        //If Dynamic Turning Speeds Should Be Used
        if (DynamicTurningSpeeds) //improved turning
        {
            Vector3 movement = (CamF * movementY + CamR * movementX).normalized;
            rb.AddForce(movement * speed);

            turnSpeed = turnSpeedFactor * ForwardVelocityMagnitude;
            rb.AddForce(CamR * movementX * turnSpeed);
        }

        else //original style movement
        {
            Vector3 movement = (CamF * movementY + CamR * movementX);
            rb.AddForce(movement * speed);
        }


        //max speed
        if (rb.velocity.magnitude > maxSpeed)
        {
            // Get the velocity direction
            Vector3 newVelocity = rb.velocity;
            newVelocity.y = 0f;
            newVelocity = Vector3.ClampMagnitude(newVelocity, maxSpeed);
            newVelocity.y = rb.velocity.y;
            rb.velocity = newVelocity;
        }

        
        //Rolling Audio
        if (Grounded)
        {
            XVolume = Mathf.Sqrt(rb.velocity.x * rb.velocity.x);
            ZVolume = Mathf.Sqrt(rb.velocity.z * rb.velocity.z);
            RollingAudio.volume = XVolume/VolumeDivide + ZVolume/VolumeDivide;
            
        }
        else
        {
            RollingAudio.volume=0;

            FallingVelocity = rb.velocity.y*-1 / VolumeDivide;
            LandAudio.volume = FallingVelocity;
        }
        
    }

    void Jump()
    {
        Vector3 JumpForces = Vector3.zero;

        if (Grounded)
        {
            JumpForces = Vector3.up * JumpForce;
            JumpAudio.Play();
        }

        rb.AddForce(JumpForces, ForceMode.VelocityChange);
    }

    public void SetGrounded(bool state)
    {
        Grounded = state;
    }
}