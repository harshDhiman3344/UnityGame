using TMPro;
using UnityEngine;

#nullable disable
public class Car : MonoBehaviour
{
    [Header("Misc")]
    public Transform centerOfMass;
    public Suspension[] wheelPositions; // Array of Suspension objects
    public GameObject wheel; // Wheel prefab
    public TextMeshProUGUI text; // TextMeshPro for UI
    private Collider c;

    [Header("Suspension Variables")]
    public bool autoValues;
    public float suspensionLength;
    public float restHeight;
    public float suspensionForce;
    public float suspensionDamping;

    [Header("Car specs")]
    public float engineForce = 5000f;
    public float steerForce = 1f;
    public float antiRoll = 5000f;
    public float stability;

    public float turnSpeed = 100f;
    public float driftFactor = 10f;
    public float brakeForce = 5f;

    [Header("Drift specs")]
    public float driftMultiplier = 1f;
    public float driftThreshold = 0.5f;

    private float C_drag = 3.5f;
    private float C_rollFriction = 105f;
    private float C_breaking = 3000f;

    [Header("Audio Sources")]
    public AudioSource accelerate;
    public AudioSource deaccelerate;

    
    public Rigidbody rb { get; set; }

    public float steering { get; set; }

    public float throttle { get; set; }

    public bool breaking { get; set; }

    public float speed;

    public float steerAngle { get; set; }

    private float dir;
    private Vector3 lastVelocity;
    private bool grounded;
    private Vector3 CG;
    private float cgHeight;
    private float wheelBase;
    private float axleWeightRatioFront = 0.5f;
    private float axleWeightRatioRear = 0.5f;
    private float wheelRadius;
    private float yawRate;
    private float weightTransfer = 0.2f;
    private float cgToRearAxle;
    private float cgToFrontAxle;
    private float tireGrip = 2f;
    private float lockGrip = 0.7f;
    private float cornerStiffnessFront = 5f;
    private float cornerStiffnessRear = 5.2f;
    private float yawGripThreshold = 0.6f;
    private float yawGripMultiplier = 0.15f;
    public bool yes;


    private void Awake()
    {
        this.rb = this.GetComponent<Rigidbody>();
        if (this.autoValues)
        {
            this.suspensionLength = 0.3f;
            this.suspensionForce = 10f * this.rb.mass;
            this.suspensionDamping = 4f * this.rb.mass;
        }
        foreach (AntiRoll componentsInChild in this.gameObject.GetComponentsInChildren<AntiRoll>())
            componentsInChild.antiRoll = this.antiRoll;
        if ((bool)(Object)this.centerOfMass)
            this.rb.centerOfMass = this.centerOfMass.localPosition;

        this.c = this.GetComponentInChildren<Collider>();
        this.wheelBase = Vector3.Distance(this.wheelPositions[0].transform.position, this.wheelPositions[2].transform.position);
        this.CG = this.c.bounds.center;
        this.cgHeight = this.c.bounds.extents.y + this.suspensionLength;
        this.cgToFrontAxle = Vector3.Distance(this.wheelPositions[0].transform.position + (this.wheelPositions[1].transform.position - this.wheelPositions[0].transform.position) * 0.5f, this.CG);
        this.cgToRearAxle = Vector3.Distance(this.wheelPositions[2].transform.position + (this.wheelPositions[3].transform.position - this.wheelPositions[2].transform.position) * 0.5f, this.CG);
        this.wheelRadius = this.suspensionLength / 2f;
        this.InitWheels();
    }

    private void Update()
    {
        // this.MoveWheels();
        // this.Audio();
        this.CheckGrounded();
        this.Steering();
    }

    private void FixedUpdate() => this.HandleMovement();

    // private void Audio()
    // {
    //     this.accelerate.volume = Mathf.Lerp(this.accelerate.volume, Mathf.Abs(this.throttle) + Mathf.Abs(this.speed / 80f), Time.deltaTime * 6f);
    //     this.deaccelerate.volume = Mathf.Lerp(this.deaccelerate.volume, (float)((double)this.speed / 40.0 - (double)this.throttle * 0.5), Time.deltaTime * 4f);
    //     this.accelerate.pitch = Mathf.Lerp(this.accelerate.pitch, 0.65f + Mathf.Clamp(Mathf.Abs(this.speed / 160f), 0.0f, 1f), Time.deltaTime * 5f);
    //     if (!this.grounded)
    //         this.accelerate.pitch = Mathf.Lerp(this.accelerate.pitch, 1.5f, Time.deltaTime * 8f);
    //     this.deaccelerate.pitch = Mathf.Lerp(this.deaccelerate.pitch, (float)(0.5 + (double)this.speed / 40.0), Time.deltaTime * 2f);
    // }

    public Vector3 acceleration { get; private set; }

    // private void Movement()
    // {
    //     Vector3 direction = this.XZVector(this.rb.velocity);
    //     Vector3 vector3_1 = this.transform.InverseTransformDirection(this.XZVector(this.rb.velocity));
    //     this.acceleration = (this.lastVelocity - vector3_1) / Time.fixedDeltaTime;
    //     this.dir = Mathf.Sign(this.transform.InverseTransformDirection(direction).z);
    //     this.speed = direction.magnitude * 3.6f * this.dir;
    //     float num1 = Mathf.Abs(this.rb.angularVelocity.y);
    //     foreach (Suspension wheelPosition in this.wheelPositions)
    //     {
    //         if (wheelPosition.grounded)
    //         {
    //             Vector3 vector3_2 = this.XZVector(this.rb.GetPointVelocity(wheelPosition.hitPos));
    //             this.transform.InverseTransformDirection(vector3_2);
    //             Vector3 vector3_3 = Vector3.Project(vector3_2, wheelPosition.transform.right);
    //             float num2 = 1f;
    //             float num3 = 1f;
    //             if (wheelPosition.terrain)
    //             {
    //                 num3 = 0.6f;
    //                 num2 = 0.75f;
    //             }
    //             float f = Mathf.Atan2(vector3_1.x, vector3_1.z);
    //             if (this.breaking)
    //                 num3 -= 0.6f;
    //             float driftThreshold = this.driftThreshold;
    //             if ((double)num1 > 1.0)
    //                 driftThreshold -= 0.2f;
    //             bool flag = false;
    //             if ((double)Mathf.Abs(f) > (double)driftThreshold)
    //             {
    //                 float num4 = Mathf.Clamp(1f - Mathf.Clamp(Mathf.Abs(f) * 2.4f - driftThreshold, 0.0f, 1f), 0.05f, 1f);
    //                 float magnitude = this.rb.velocity.magnitude;
    //                 flag = true;
    //                 if ((double)magnitude < 8.0)
    //                     num4 += (float)((8.0 - (double)magnitude) / 8.0);
    //                 if ((double)num1 < (double)this.yawGripThreshold)
    //                 {
    //                     float num5 = (this.yawGripThreshold - num1) / this.yawGripThreshold;
    //                     num4 += num5 * this.yawGripMultiplier;
    //                 }
    //                 if ((double)Mathf.Abs(this.throttle) < 0.30000001192092896)
    //                     num4 += 0.1f;
    //                 num3 = Mathf.Clamp(num4, 0.0f, 1f);
    //             }
    //             float num6 = 1f;
    //             if (flag)
    //                 num6 = this.driftMultiplier;
    //             if (this.breaking)
    //                 this.rb.AddForceAtPosition(wheelPosition.transform.forward * this.C_breaking * Mathf.Sign(-this.speed) * num2, wheelPosition.hitPos);
    //             this.rb.AddForceAtPosition(wheelPosition.transform.forward * this.throttle * this.engineForce * num6 * num2, wheelPosition.hitPos);
    //             double mass = (double)this.rb.mass;
    //             Vector3 vector3_4 = vector3_3 * (float)mass * num2 * num3;
    //             this.rb.AddForceAtPosition(-vector3_4, wheelPosition.hitPos);
    //             this.rb.AddForceAtPosition(wheelPosition.transform.forward * vector3_4.magnitude * 0.25f, wheelPosition.hitPos);
    //             float num7 = Mathf.Clamp(1f - num3, 0.0f, 1f);
    //             if ((double)Mathf.Sign(this.dir) != (double)Mathf.Sign(this.throttle) && (double)this.speed > 2.0)
    //             {
    //                 float num8;
    //                 num7 = Mathf.Clamp(num8 = num7 + 0.5f, 0.0f, 1f);
    //             }
    //             wheelPosition.traction = num7;
    //             this.rb.AddForce(-this.C_drag * direction);
    //             this.rb.AddForce(-this.C_rollFriction * direction);
    //         }
    //     }
    //     this.StandStill();
    //     this.lastVelocity = vector3_1;
    // }

    // private void StandStill()
    // {
    //     if ((double)Mathf.Abs(this.speed) < 1.0 && this.grounded && (double)this.throttle == 0.0)
    //     {
    //         bool flag = true;
    //         foreach (Suspension wheelPosition in this.wheelPositions)
    //         {
    //             if ((double)Vector3.Angle(wheelPosition.hitNormal, Vector3.up) > 1.0)
    //             {
    //                 flag = false;
    //                 break;
    //             }
    //         }
    //         if (flag)
    //             this.rb.drag = (float)((1.0 - (double)Mathf.Abs(this.speed)) * 30.0);
    //         else
    //             this.rb.drag = 0.0f;
    //     }
    //     else
    //         this.rb.drag = 0.0f;
    // }


    private void HandleMovement()
{
    // Getting input for movement
    float moveInput = Input.GetAxis("Vertical"); // Forward and backward movement (W/S or Up/Down Arrow)
    float turnInput = Input.GetAxis("Horizontal"); // Left and right movement (A/D or Left/Right Arrow)

    // Calculate movement force (don't apply too much force when drifting)
    Vector3 moveForce = transform.forward * moveInput * speed;
    rb.AddForce(moveForce, ForceMode.Acceleration);

    // Apply turning (steering)
    float turnAmount = turnInput * turnSpeed * Time.deltaTime;
    transform.Rotate(Vector3.up, turnAmount);

    // Apply drifting effect, reduce velocity for smoother sliding
    if (moveInput != 0) // Apply drift only when moving
    {
        rb.velocity = Vector3.Lerp(rb.velocity, rb.velocity * driftFactor, Time.deltaTime);
    }

    // Limit maximum speed (optional, but useful for controlling fast movement)
    float maxSpeed = 30f; // Adjust the max speed as needed
    if (rb.velocity.magnitude > maxSpeed)
    {
        rb.velocity = rb.velocity.normalized * maxSpeed;
    }

    // Check for braking input
    if (Input.GetKey(KeyCode.Space)) // Spacebar for braking
    {
        ApplyBraking();
    }
}

// Apply braking force
private void ApplyBraking()
{
    // Apply downward force for braking, preventing the car from flying up or down too much
    rb.AddForce(Vector3.down * brakeForce, ForceMode.Acceleration);
}


// private void StandStill()
// {
//     // Check if the car is nearly stopped and apply drag
//     if (Mathf.Abs(this.speed) < 1f && this.grounded && this.throttle == 0f)
//     {
//         bool isFlat = true;

//         foreach (Suspension wheel in this.wheelPositions)
//         {
//             if (Vector3.Angle(wheel.hitNormal, Vector3.up) > 1f)
//             {
//                 isFlat = false;
//                 break;
//             }
//         }

//         this.rb.drag = isFlat ? (1f - Mathf.Abs(this.speed)) * 30f : 0f;
//     }
//     else
//     {
//         this.rb.drag = 0f;
//     }
// }

    private void Steering()
    {
        foreach (Suspension wheelPosition in this.wheelPositions)
        {
            if (!wheelPosition.rearWheel)
            {
                wheelPosition.steeringAngle = this.steering * (37f - Mathf.Clamp((float)((double)this.speed * 0.34999999403953552 - 2.0), 0.0f, 17f));
                this.steerAngle = wheelPosition.steeringAngle;
            }
        }
    }

    private Vector3 XZVector(Vector3 v) => new Vector3(v.x, 0.0f, v.z);

    private void InitWheels()
    {
        foreach (Suspension wheelPosition in this.wheelPositions)
        {
            wheelPosition.wheelObject = Object.Instantiate<GameObject>(this.wheel).transform;
            wheelPosition.wheelObject.parent = wheelPosition.transform;
            wheelPosition.wheelObject.transform.localPosition = Vector3.zero;
            wheelPosition.wheelObject.transform.localRotation = Quaternion.identity;
            wheelPosition.wheelObject.localScale = Vector3.one * this.suspensionLength * 2f;
        }
    }

    private void MoveWheels()
    {
        foreach (Suspension wheelPosition in this.wheelPositions)
        {
            float y = Mathf.Lerp(wheelPosition.wheelObject.transform.localPosition.y, -wheelPosition.hitHeight + this.suspensionLength, Time.deltaTime * 20f);
            wheelPosition.wheelObject.transform.localPosition = new Vector3(0.0f, y, 0.0f);
            wheelPosition.wheelObject.Rotate(Vector3.right, this.XZVector(this.rb.velocity).magnitude * 1f * this.dir);
            wheelPosition.wheelObject.localScale = Vector3.one * (this.suspensionLength * 2f);
            wheelPosition.transform.localScale = Vector3.one / this.transform.localScale.x;
        }
    }

    private void CheckGrounded()
    {
        this.grounded = false;
        foreach (Suspension wheelPosition in this.wheelPositions)
        {
            if (wheelPosition.grounded)
                this.grounded = true;
        }
    }
}

public class CarSuspension : MonoBehaviour
{
    public bool grounded;
    public Vector3 hitPos;
    public float hitHeight;
    public Transform wheelObject;
    public float steeringAngle;
    public bool rearWheel;
    public bool terrain; // Ensure this property is defined only once
    public float traction;
    public Vector3 hitNormal; // Added for ground normal reference

}