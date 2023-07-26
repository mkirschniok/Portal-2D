using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float speed = 8f;
    [SerializeField] float jumpForce = 14f;
    [SerializeField] float interactDistance = 2f;
    [SerializeField] Transform holdPoint;

    Rigidbody2D rigidbody;
    eMovementState movementState;
    Animator       animator;
    float          dirX = 0f;
    float          dirXMultiplier = 1.0f;
    float          cubeSavedMass = 0;
    bool           isJumping = false;
    bool           isFacingRight = true;
    GameObject     currentCube;
    BoxCollider2D  boxCollider2D;

    public bool IsFacingRight
    {
        get => isFacingRight;
        set => isFacingRight = value;
    }

    enum eMovementState
    {
        IDLE,
        WALK,
        JUMP,
        FALL
    }

    public void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    public bool IsMoving()
    {
        return Mathf.Abs( dirX ) >= 0.1f;
    }
    public void InverseXMovementAxis()
    {
        dirXMultiplier = -1f;
    }

    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        boxCollider2D = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        dirX = Input.GetAxisRaw("Horizontal") * dirXMultiplier;
        if (dirX==0)
            dirXMultiplier = 1.0f;

        if ( IsTouchingGround() && Input.GetButtonDown("Jump") )
        {
            isJumping = true;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (currentCube == null)
            {
                TryPickUpCube();
            }
            else
            {
                DropCube();
            }
        }

        if (currentCube != null)
        {
            var cubePosition = currentCube.transform.position;
            var targetPos    = holdPoint.transform.position;
            var diff         = (targetPos - cubePosition)*10;
            currentCube.GetComponent<Rigidbody2D>().velocity = new Vector2(diff.x, diff.y);
        }
    }

    public bool IsTouchingGround()
    {
        var layerMask1 = LayerMask.GetMask("Units");
        var layerMask2 = LayerMask.GetMask("Terrain");
        RaycastHit2D hit = Physics2D.Raycast( transform.position - Vector3.down * 0.1f, Vector2.down, 0.3f , layerMask1 | layerMask2 );
        if (hit.collider != null)
        {
            //Debug.DrawLine(transform.position, hit.point, Color.green);
            return true;
        }
        //Debug.DrawLine(transform.position - Vector3.down * 0.3f, transform.position + Vector3.down );
        return false;
    }

    void FixedUpdate()
    {
        rigidbody.velocity = new Vector2(dirX * speed, rigidbody.velocity.y);
        if (isJumping)
        {
            rigidbody.velocity = new Vector2(rigidbody.velocity.x, jumpForce);
            isJumping = false;
        }
        AnimationUpdate(dirX);
    }

    void AnimationUpdate(float dirX)
    {
        if (dirX > 0 || dirX < 0)
        {
            movementState = eMovementState.WALK;
        }
        else
        {
            movementState = eMovementState.IDLE;
        }

        if (rigidbody.velocity.y > 0.1f)
        {
            movementState = eMovementState.JUMP;
        }
        else if (rigidbody.velocity.y < -0.1f)
        {
            movementState = eMovementState.FALL;
        }

        animator.SetInteger("state", (int)movementState);
    }

    #region CUBE
    void TryPickUpCube()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, interactDistance);

        if (colliders == null)
        {
            return;
        }

        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Cube") && currentCube == null)
            {
                currentCube = collider.gameObject;
                currentCube.GetComponent<Cube>().Take();
                break;
            }
        }
    }

    void DropCube()
    {
        if (currentCube == null) return;
        currentCube.GetComponent<Cube>().Drop();
        currentCube = null;
    }
    #endregion CUBE
}