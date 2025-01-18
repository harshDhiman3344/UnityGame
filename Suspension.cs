using UnityEngine;

#nullable disable
public class Suspension : MonoBehaviour
{
    private Car car;
    private Rigidbody bodyRb;
    public Transform wheelObject;
    public bool rearWheel;
    [HideInInspector]
    public bool skidding;
    public bool showFx = true;
    public AudioSource skidSfx;
    public ParticleSystem smokeFx;
    public ParticleSystem spinFx;
    private ParticleSystem.EmissionModule smokeEmitting;
    private ParticleSystem.EmissionModule spinEmitting;
    private float wheelAngleVelocity;
    public float steeringAngle;
    public float traction;
    private float steerTime = 15f;
    public bool spinning;
    public LayerMask whatIsGround;
    private MeshRenderer mesh;
    public Vector3 hitPos;
    public Vector3 hitNormal;
    public float hitHeight;
    public bool grounded;
    public float lastCompression;
    private float raycastOffset;
    public float restLength;
    public float springTravel;
    public float springStiffness;
    public float damperStiffness;
    private float minLength;
    private float maxLength;
    private float lastLength;
    private float springLength;
    private float springVelocity;
    private float springForce;
    private float damperForce;

    private void Start()
    {
        this.car = this.transform.parent.GetComponent<Car>();
        this.bodyRb = this.car.GetComponent<Rigidbody>();
        this.raycastOffset = this.car.suspensionLength * 0.5f;
        this.smokeEmitting = this.smokeFx.emission;
        this.spinEmitting = this.spinFx.emission;
    }

    private void FixedUpdate() => this.NewSuspension();

    private void Update()
    {
        if (this.rearWheel)
            return;
        this.wheelAngleVelocity = Mathf.Lerp(this.wheelAngleVelocity, this.steeringAngle, this.steerTime * Time.deltaTime);
        this.transform.localRotation = Quaternion.Euler(Vector3.up * this.wheelAngleVelocity);
    }

    public bool terrain { get; set; }

    private void NewSuspension()
    {
        this.minLength = this.restLength - this.springTravel;
        this.maxLength = this.restLength + this.springTravel;
        float suspensionLength = this.car.suspensionLength;
        RaycastHit hitInfo;
        if (Physics.Raycast(this.transform.position, -this.transform.up, out hitInfo, this.maxLength + suspensionLength))
        {
            this.lastLength = this.springLength;
            this.springLength = hitInfo.distance - suspensionLength;
            this.springLength = Mathf.Clamp(this.springLength, this.minLength, this.maxLength);
            this.springVelocity = (this.lastLength - this.springLength) / Time.fixedDeltaTime;
            this.springForce = this.springStiffness * (this.restLength - this.springLength);
            this.damperForce = this.damperStiffness * this.springVelocity;
            this.bodyRb.AddForceAtPosition((this.springForce + this.damperForce) * this.transform.up, hitInfo.point);
            this.terrain = hitInfo.collider.gameObject.CompareTag("Terrain");
            this.hitPos = hitInfo.point;
            this.hitNormal = hitInfo.normal;
            this.hitHeight = hitInfo.distance;
            this.grounded = true;
        }
        else
        {
            this.grounded = false;
            this.hitHeight = this.car.suspensionLength + this.car.restHeight;
        }
    }

    private void LateUpdate()
    {
        if (!this.showFx)
            return;

        if (!this.rearWheel)
            return;

        if ((double)this.traction > 0.15000000596046448 && this.grounded)
        {
            // this.spinEmitting.enabled = true;
            // this.spinEmitting.rateOverTime = (ParticleSystem.MinMaxCurve)Mathf.Clamp(this.traction * 60f, 20f, 400f);
        }
        
    }
}
