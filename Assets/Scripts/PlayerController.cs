using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float defaultSlideSpeed = 2f;

    public float defaultWalkSpeed = 5f;
    public float defaultSprintSpeed = 10f;
    public float defaultAirMoveSpeed = 4f;

    public float mouseSensitivity = 2f;
    public float upDownRange = 90f;
    public float jumpSpeed = 4f;

    private Vector3 moveSpeed;
    private Vector2 axisInput;

    private Vector3 contactPointHitNormal;
    private RaycastHit rayCastHit;

    private bool grounded;
    private bool sliding;
    private bool playerControl = false;

    private bool falling;
    private float fallingStartLevel;

    private float slideLimit;
    private float nowSpeed;

    private float verticalRotation;
    private float horizontalRotation;

    private short jumpTimer;

    private CharacterController controller;

    private GameObject playerCamera;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        controller = GetComponent<CharacterController>();
        slideLimit = controller.slopeLimit - .1f;

        playerCamera = GameObject.Find("Personal Camera");

    }

    /**
     * 매 프레임마다 동작
     * 단순히 입력을 받거나
     */
    void Update()
    {
        Rotate();
        Move();
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        contactPointHitNormal = hit.point;
    }

    private void Sprint()
    {
        nowSpeed = Input.GetButton("Fire3") ? defaultSprintSpeed : defaultWalkSpeed;
    }

    private void Move()
    {
        GetAxisInput();

        if (grounded)
        {
            SlidingCheck();

            if (falling)
            {
                falling = false;
                if (20f < fallingStartLevel - transform.position.y)
                {
                    // 떨어진 높이에 따라 이벤트 발생
                }
            }

            Sprint();

            // Sliding 상태이거나 레이캐스트 hit 의 태그가 Slide일 경우
            if ((sliding) || (rayCastHit.collider.tag == "Slide"))
            {
                Vector3 hitNormal = rayCastHit.normal;
                moveSpeed = new Vector3(hitNormal.x, -hitNormal.y, hitNormal.z);
                Vector3.OrthoNormalize(ref hitNormal, ref moveSpeed);
                moveSpeed *= defaultSlideSpeed;
                playerControl = false;
            }
            else
            {
                moveSpeed = new Vector3(axisInput.x, 0, axisInput.y);
                moveSpeed = transform.TransformDirection(moveSpeed) * nowSpeed;
                playerControl = true;
            }

            JumpCheck();
        }
        else
        {
            // 캐릭터가 지상에 서있지 않고 떨어지는 중이 아닐 경우 
            // 떨어짐 처리하고 떨어지기 시작한 높이를 기록
            if (!falling)
            {
                falling = true;
                fallingStartLevel = transform.position.y;
            }

            // 공중에서 제어 가능하게 하기 위해선 이곳에 if (playerControl) 로 작성해줘야함

            /*if (playerControl)
            {
                moveSpeed.x = axisInput.x * nowSpeed;
                moveSpeed.z = axisInput.y * nowSpeed;
                moveSpeed = transform.TransformDirection(moveSpeed);
            }*/

        }
        SetGravity();

        grounded = (controller.Move(moveSpeed * Time.deltaTime) & CollisionFlags.Below) != 0;

    }

    private void SlidingCheck()
    {
        sliding = false;
        if ((Physics.Raycast(transform.position, Vector3.down, out rayCastHit, controller.height * .5f + controller.radius)
            || Physics.Raycast(contactPointHitNormal + Vector3.up, Vector3.down, out rayCastHit))
            && Vector3.Angle(rayCastHit.normal, Vector3.up) > slideLimit)
            sliding = true;
    }

    private void SetGravity()
    {
        moveSpeed.y += Physics.gravity.y * Time.deltaTime;
    }
    private void JumpCheck()
    {
        if (!Input.GetButton("Jump"))
        {
            if (jumpTimer < short.MaxValue)
                jumpTimer++;
        }
        else if (jumpTimer >= 1)
        {
            moveSpeed.y = jumpSpeed;
            jumpTimer = 0;
        }
    }

    private void GetAxisInput()
    {
        axisInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;
    }

    private void Rotate()
    {

        horizontalRotation = Input.GetAxis("Mouse X") * mouseSensitivity;
        transform.Rotate(0f, horizontalRotation, 0f);

        verticalRotation -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        verticalRotation = Mathf.Clamp(verticalRotation, -upDownRange, upDownRange);
        playerCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);

    }
}
