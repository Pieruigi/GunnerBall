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

    public float Stamina { get; private set; } = 100;
    public float StaminaMax { get; private set; } = 100;

    public bool Sprinting { get { return sprinting; } }

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        CheckTestTarget();

        if(hasDestination)
        {
            Vector3 v = destination - transform.position;
            if(v.sqrMagnitude < minDistSqr)
            {
                hasDestination = false;
                Move(false, Vector2.zero);

            }
            else
            {
                moveDir = new Vector2(v.x, v.z);
                Move(true, moveDir.normalized);
            }
            

            
        }

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
        Debug.Log("Moving to " + moveDir);
    }

    /// <summary>
    /// Sprint event in the player controller should call this method
    /// </summary>
    public void Sprint(bool value)
    {
        sprintInput = value;
    }

    public void MoveTo(Vector3 destination)
    {
        // Get distance
        Debug.Log("Move to " + destination);
        float sqrDist = Vector3.SqrMagnitude(transform.position - destination);

            hasDestination = true;
            this.destination = destination;
        
    }

    public void LookAtTheBall()
    {
        Transform ball = GameObject.FindGameObjectWithTag(Tag.Ball).transform;


        // Rotate towards the ball
        Vector3 aiToBallV = ball.position - transform.position;
        transform.forward = new Vector3(aiToBallV.x, 0, aiToBallV.z);
    }

    #endregion
}
