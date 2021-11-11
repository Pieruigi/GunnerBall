using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zoca;


public class FakePlayerController : MonoBehaviour
{

    [SerializeField]
    float maxSpeed = 0;

    [SerializeField]
    float sprintMul = 2;

    [SerializeField]
    FireWeapon fireWeapon;
    public FireWeapon FireWeapon
    {
        get { return fireWeapon; }
    }

    [SerializeField]
    Transform testTarget;

    #region private fields
    float acc = 30;
    float speed = 0;
    bool moving = false;
    bool sprinting = false;
    bool sprintInput = false;
    Vector2 moveDir;
    Vector3 velocity;
    Vector3 destination;
    bool hasDestination = false;
    float minDistSqr = 4f;
    Rigidbody ballRB;

    public float Stamina { get; private set; } = 100;
    public float StaminaMax { get; private set; } = 100;

    public bool Sprinting { get { return sprinting; } }

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        ballRB = GameObject.FindGameObjectWithTag(Tag.Ball).GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

        CheckTestTarget();

        //if (hasDestination)
        //{
        //    Vector3 v = destination - transform.position;
        //    if (v.sqrMagnitude < minDistSqr)
        //    {
        //        hasDestination = false;
        //        Move(false, Vector2.zero);

        //    }
        //    else
        //    {
        //        moveDir = new Vector2(v.x, v.z);
        //        Move(true, moveDir.normalized);
        //    }



        //}

        CheckMovement();
      
    }

    void CheckTestTarget()
    {
        if (testTarget)
        {
            Vector3 dir = testTarget.position - transform.position;

            if (dir.magnitude > 3)
                Move(true, new Vector2(dir.x, dir.z).normalized);
            else
                Move(false, Vector2.zero);
        }
    }

    void CheckMovement()
    {
        // Compute target velocity and target direction
        float targetSpeed = maxSpeed;
        Vector3 targetDir = new Vector3(moveDir.x, 0, moveDir.y);

        sprinting = sprintInput;

        if (sprinting)
        {
            if(Stamina > 0)
            {
                Stamina -= 10 * Time.deltaTime;
                targetSpeed *= 2;
            }
            else
            {
                sprinting = false;
                targetSpeed = maxSpeed;
            }
        }
        
    
        Vector3 targetVelocity = targetSpeed * targetDir;

        // Adjust current velocity
        velocity = Vector3.MoveTowards(velocity, targetVelocity, Time.deltaTime * acc);

        if (sprinting)
        {
            transform.forward = Vector3.MoveTowards(transform.forward, velocity.normalized, Time.deltaTime * 10);
        }

        // Move player
        transform.position += velocity * Time.deltaTime;
    }

    

    #region public
    /// <summary>
    /// The input event in the player controller could call this method; 
    /// ex: 
    /// public void OnMove(InputAction.CallbackContext context)
    /// {
    ///     Move(context.performed, context.ReadValue<Vector2>());
    /// }
    /// </summary>
    /// <param name="move"></param>
    /// <param name="direction"></param>
    public void Move(bool move, Vector2 direction)
    {
        if (move)
        {
            moving = true;
            
        }
        else
        {
            moving = false;
            
        }
        moveDir = direction.normalized;
        
    }

    public void MoveTo(Vector3 destination)
    {
        hasDestination = true;
        this.destination = destination;
    }

    /// <summary>
    /// Sprint event in the player controller should call this method
    /// </summary>
    public void Sprint(bool value)
    {
        sprintInput = value;
    }

   

    public void LookAt(Vector3 target)
    {
        // Yaw
        Vector3 targetFwd = target - transform.position;
        transform.forward = Vector3.MoveTowards(transform.forward, new Vector3(targetFwd.x, 0, targetFwd.z), Time.deltaTime*720);
        //transform.forward = targetFwd;
        // Pitch
        Vector3 pToT = target - transform.position;
        Vector3 fwd = transform.forward;
        //pToT.y = 0;
        //fwd.y = 0;
        float angle = Vector3.SignedAngle(fwd, pToT, Vector3.up);
  
    }

    public void Shoot(bool value)
    {
        //transform.Rotate(new Vector3(0, 3, 0), Space.Self);
        Debug.DrawRay(transform.position, transform.forward * fireWeapon.FireRange*10, Color.red, 5);
        
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit info;
        bool hit = Physics.Raycast(ray, out info, fireWeapon.FireRange * 2);
        if(hit)
        {
            ballRB.AddForce(-info.normal * 80, ForceMode.VelocityChange);
        }
    }

    #endregion
}
