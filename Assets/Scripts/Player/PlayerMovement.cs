using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed;
    [SerializeField]
    private float turnRate;
    [SerializeField]
    private float gravityMultiplier;
    [SerializeField]
    private int maxCollisions;
    [SerializeField]
    private float sweepTestDistance;
    [SerializeField]
    private float groundCheckDistance;
    [SerializeField]
    private float slopeLimit;
    [SerializeField]
    private float jumpSpeed;
    [SerializeField]
    private float jumpHeight;
    [SerializeField]
    private float jumpCooldown;
    [SerializeField]
    private float airSpeed;


    [SerializeField]
    private Transform cameraTransform;
    [SerializeField]
    private CapsuleCollider playerCollider;
    [SerializeField]
    private Rigidbody playerRigidbody;

    private bool grounded;
    private bool tankMode;
    private Vector2 input;
    private Vector3 movementVector;
    private Vector3 rotationStepVector;
    private Vector3 penetrationDirection;
    private float penetrationDistance;
    private float xInput;
    private float yInput;

    private bool jumping;
    private float heightJumped;
    private float timeSinceJump;

    private Vector3 slideVectorDebug;
    private Vector3 hitNormalRayDebug;
    private Vector3 hitNormalPositionDebug;

    private void Start()
    {
        timeSinceJump = 0.0f;
        jumping = false;

        tankMode = false;
        grounded = true;
        playerRigidbody = GetComponent<Rigidbody>();
        playerCollider = GetComponent<CapsuleCollider>();
        cameraTransform = GetComponent<PlayerSetup>().cameraObject.transform;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(transform.position + playerCollider.center + Vector3.up * (playerCollider.height * 0.5f - playerCollider.radius), playerCollider.radius);
        Gizmos.DrawWireSphere(transform.position + playerCollider.center - Vector3.up * (playerCollider.height * 0.5f - playerCollider.radius), playerCollider.radius);
        Gizmos.color = Color.cyan;

        Gizmos.DrawLine(transform.position, transform.position + penetrationDirection);
        Gizmos.color = Color.blue;

        Gizmos.DrawLine(transform.position, transform.position + movementVector.normalized * moveSpeed * Time.deltaTime);

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, slideVectorDebug * 5.0f);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(hitNormalPositionDebug, hitNormalRayDebug * 8.0f);
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.LeftAlt))
        {
            tankMode = !tankMode;
        }
        xInput = Input.GetAxisRaw("Horizontal");
        yInput = Input.GetAxisRaw("Vertical");
        input = new Vector2(xInput, yInput).normalized;

        RaycastHit groundHit;
        grounded = CheckGround(out groundHit);

        if (grounded == false && jumping == false)
        {
            Translate(-transform.up * 9.8f * gravityMultiplier * Time.deltaTime);
            //transform.position -= transform.up * 9.8f * gravityMultiplier * Time.deltaTime;
        }

        HandleJump();
        HandleTranslation();
        HandleRotation();
        Depenetrate();
    }

    private void Depenetrate()
    {
        float capsuleHeight = playerCollider.height;
        Collider[] collisions = new Collider[maxCollisions];
        Physics.OverlapSphereNonAlloc(transform.position, 6.0f, collisions);

        Physics.OverlapCapsuleNonAlloc(
            transform.position + playerCollider.center + Vector3.up * (playerCollider.height * 0.5f - playerCollider.radius),
            transform.position + playerCollider.center - Vector3.up * (playerCollider.height * 0.5f - playerCollider.radius),
            playerCollider.radius, collisions);

        foreach (Collider collider in collisions)
        {
            if (collider == playerCollider)
            {
                continue;
            }
            if (collider == null)
            {
                continue;
            }

            bool collided = Physics.ComputePenetration(
                playerCollider, playerCollider.transform.position + new Vector3(0, 0.0f, 0), playerCollider.transform.rotation,
                collider, collider.transform.position, collider.transform.rotation,
                out penetrationDirection, out penetrationDistance);

            if (collided == true)
            {
                // Depenetrate
                transform.position += penetrationDirection * penetrationDistance;
                //Slide along normal
                RaycastHit hit;
                if (Physics.Raycast(transform.position, penetrationDirection, out hit, 5.0f) == true)
                {
                    Vector3 slideDirection = Vector3.ProjectOnPlane(penetrationDirection, hit.normal);
                    transform.position += slideDirection * penetrationDistance;
                }
            }
        }
    }

    private bool CheckGround(out RaycastHit hit)
    {
        if (jumping == false)
        {
            Vector3 offset = new Vector3(0.0f, 0.5f, 0.0f);
            // Sweeptest all?
            if (Physics.CapsuleCast(
                transform.position + playerCollider.center + transform.up + offset * (playerCollider.height * 0.5f - playerCollider.radius),
                transform.position + playerCollider.center - transform.up + offset * (playerCollider.height * 0.5f - playerCollider.radius),
                playerCollider.radius, -transform.up, out hit, groundCheckDistance + 0.5f))
            {
                if (Vector3.Angle(transform.up, hit.normal) >= slopeLimit)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }
        else
        {
            hit = new RaycastHit();
            return false;
        }
    }

    private void HandleJump()
    {
        if (Input.GetButtonDown("Jump") == true)
        {
            if (grounded == false)
            {
                Debug.Log("Not grounded");
                return;
            }

            if (timeSinceJump <= jumpCooldown)
            {
                grounded = false;
                jumping = true;
                heightJumped = 0.0f;
            }
        }
        if (jumping == true)
        {
            float jumpAmount = jumpSpeed * Time.deltaTime;
            if (jumpAmount + heightJumped > jumpHeight)
            {
                jumpAmount = Mathf.Clamp(jumpAmount, 0.0f, jumpHeight - heightJumped);
                transform.position += transform.up * jumpAmount;
                jumping = false;
                heightJumped = 0.0f;
            }
            else
            {
                transform.position += transform.up * jumpAmount;
                heightJumped += jumpAmount;
            }
        }
    }

    private void HandleTranslation()
    {
        if (tankMode == false)
        {
            // Move towards the direction the camera is looking
            movementVector = Vector3.zero;
            movementVector += input.x * Vector3.ProjectOnPlane(cameraTransform.right, Vector3.up);
            movementVector += input.y * Vector3.ProjectOnPlane(cameraTransform.forward, Vector3.up);

            movementVector *= moveSpeed * Time.deltaTime;

            Translate(movementVector);
        }
        else if (tankMode == true)
        {
            // Tank controls
            // Move forward relative to the hull
            Vector3 movementVector = new Vector3(0.0f, 0.0f, yInput).normalized;
            movementVector = transform.TransformDirection(movementVector);
            movementVector *= moveSpeed * Time.deltaTime;

            Translate(movementVector);
        }
    }

    private void Translate(Vector3 translationVector)
    {
        RaycastHit[] sweepHits = playerRigidbody.SweepTestAll(translationVector, sweepTestDistance);

        foreach (RaycastHit hit in sweepHits)
        {
            if (hit.collider == playerCollider)
            {
                continue;
            }

            hitNormalRayDebug = hit.normal;
            hitNormalPositionDebug = hit.point;


            if (Vector3.Angle(transform.up, hit.normal) >= slopeLimit)
            {
                float slideDistance = translationVector.magnitude - hit.distance;

                if (slideDistance > 0.0f)
                {
                    Vector3 sideways = Vector3.Cross(hit.normal, Vector3.up);
                    Vector3 slideVector = Vector3.ProjectOnPlane(translationVector, hit.normal);
                    //slideVector.z = 0.0f;
                    //slideVector.y = 0.0f;

                    slideVectorDebug = slideVector.normalized * slideDistance;
                    transform.position += translationVector.normalized * hit.distance + slideVector.normalized * slideDistance * 0.5f;
                }
                else
                {
                    Vector3 slideVector = Vector3.ProjectOnPlane(translationVector, hit.normal);
                    transform.position += slideVector.normalized * translationVector.magnitude;
                    //transform.position += translationVector;
                }

                // test steps
                //Debug.Log("Slope limit" + Vector3.Angle(Vector3.up, hit.normal));
                continue;
            }
            else
            {
                float slideDistance = translationVector.magnitude - hit.distance;

                if (slideDistance > 0.0f)
                {
                    Vector3 slideVector = Vector3.ProjectOnPlane(translationVector, hit.normal);

                    slideVectorDebug = slideVector.normalized * slideDistance;
                    transform.position += translationVector.normalized * hit.distance + slideVector.normalized * slideDistance;
                }
                else
                {
                    Vector3 slideVector = Vector3.ProjectOnPlane(translationVector, hit.normal);
                    transform.position += slideVector.normalized * translationVector.magnitude;
                }
            }
        }
        if (sweepHits.Length == 0)
        {
            //Debug.Log("No collisions");
            if (grounded == true)
            {
                RaycastHit hit;
                playerRigidbody.SweepTest(-transform.up, out hit, 5.0f);
                Vector3 slideVector = Vector3.ProjectOnPlane(translationVector, hit.normal);
                transform.position += slideVector.normalized * translationVector.magnitude;
            }
            else
            {
                transform.position += translationVector;
            }
        }
    }

    private void HandleRotation()
    {
        if (tankMode == false)
        {
            // Turn towards the direction of movement
            Vector3 lookDireciton = cameraTransform.forward;
            // ???
            float rotationY = Mathf.Atan2(lookDireciton.x, lookDireciton.z) * Mathf.Rad2Deg;
            // Profit
            Quaternion targetRotation = Quaternion.Euler(0.0f, rotationY, 0.0f);
            Quaternion rotationStep = Quaternion.RotateTowards(transform.rotation, targetRotation, turnRate * Time.deltaTime);
            transform.rotation = rotationStep;
        }
        else if (tankMode == true)
        {
            // Turn the hull in the direction of the horizontal input
            transform.Rotate(new Vector3(0, 1, 0), xInput * turnRate * Time.deltaTime);
        }
    }

}
