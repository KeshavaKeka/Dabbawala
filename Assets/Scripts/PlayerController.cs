using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Player Settings")]
    public float driftFactor = 0.95f;
    public float accelerationFactor = 30.0f;
    public float turnFactor = 3.5f;
    public float maxSpeed = 20f;

    float accelerationInput = 0;
    float steeringInput = 0;

    float rotationAngle = 0;

    float velocityVsUp = 0;

    Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        ApplyForce();
        KillOrthogonalVelocity();
        ApplySteering();
    }

    void ApplyForce()
    {
        velocityVsUp = Vector2.Dot(transform.up, rb.velocity);

        if (velocityVsUp > maxSpeed && accelerationInput > 0)
            return;

        if (velocityVsUp < -maxSpeed * 0.25f && accelerationInput < 0)
            return;

        if (rb.velocity.sqrMagnitude > maxSpeed * maxSpeed && accelerationInput > 0)
            return;

        if (accelerationInput == 0)
        {
            rb.drag = Mathf.Lerp(rb.drag, 10.0f, Time.fixedDeltaTime);
        }
        else
        {
            if ((velocityVsUp > 0 && accelerationInput < 0) || (velocityVsUp < 0 && accelerationInput > 0))
            {
                rb.drag = Mathf.Lerp(rb.drag, 10.0f, Time.fixedDeltaTime);
            }
            else
            {
                rb.drag = 0;
            }
        }

        Vector2 engineForceVector = transform.up * accelerationInput * accelerationFactor;
        rb.AddForce(engineForceVector, ForceMode2D.Force);
    }


    void ApplySteering()
    {
        float minSpeed = (rb.velocity.magnitude / 8);
        minSpeed = Mathf.Clamp01(minSpeed);
        rotationAngle -= steeringInput * turnFactor * minSpeed;
        rb.MoveRotation(rotationAngle);
    }

    void KillOrthogonalVelocity()
    {
        Vector2 forwordVel = transform.up * Vector2.Dot(rb.velocity, transform.up);
        Vector2 rightVel = transform.right * Vector2.Dot(rb.velocity, transform.right);

        rb.velocity = forwordVel + rightVel * driftFactor;
    }

    public void SetInputVector(Vector2 input)
    {
        steeringInput = input.x;
        accelerationInput = input.y;
    }
}
