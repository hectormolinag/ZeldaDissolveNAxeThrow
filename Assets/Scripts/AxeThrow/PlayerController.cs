using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cinemachine;

public class PlayerController : MonoBehaviour
{
    [Header("Axe Settings")]
    [SerializeField] Transform axe = null;
    [SerializeField] float axeThrowForce = 0f;
    [SerializeField] Transform curvePoint = null;
    [SerializeField] Transform hand = null;
    Rigidbody axeRB;
    Axe weaponScript;

    [Header("Shake Cinemachine")]
    public CinemachineFreeLook virtualCamera;
    public CinemachineImpulseSource impulseSource;

    public Vector3 desiredMoveDirection;

    public bool canPlayerMove = true;

    private Rigidbody rb;
    private Animator anim;
    private CharacterController controller;

    private Vector3 origLocPos;
    private Vector3 origLocRot;

    private Vector3 oldAxePos;

    float h, v;
    Camera cam;

    float time = 0.0f;

    bool hasWeapon = true;
    bool isAiming = false;

    bool isWalking = false;

    bool isReturning;
     
    void Start()
    {
        Cursor.visible = false;

        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();

        cam = Camera.main;

        controller = GetComponent<CharacterController>();

        axeRB = axe.gameObject.GetComponent<Rigidbody>();

        weaponScript = axe.gameObject.GetComponent<Axe>();

        origLocPos = axe.localPosition;
        origLocRot = axe.localEulerAngles;
    }

    void Update()
    {
        if (canPlayerMove)
        {
            CheckAnimations();
            Aim();

            ReturnAxe();
        }

    }

    void FixedUpdate()
    {
        if(canPlayerMove)
            Walk();
    }

    void Walk()
    {
        if (!isAiming)
        {
            h = Input.GetAxisRaw("Horizontal");
            v = Input.GetAxisRaw("Vertical");

            var forward = cam.transform.forward;
            var right = cam.transform.right;

            forward.y = 0f;
            right.y = 0f;

            forward.Normalize();
            right.Normalize();

            desiredMoveDirection = forward * v + right * h;

            Vector3 movement = new Vector3(h, 0f, v);

            if (h != 0 || v != 0)
                isWalking = true;
            else
                isWalking = false;

            controller.Move(desiredMoveDirection * Time.deltaTime * 4);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(desiredMoveDirection), 0.2f);
        }
    }

    void Aim()
    {
        if (Input.GetButton("Fire1") && hasWeapon && !isWalking)
        {
            isAiming = true;
            anim.SetBool("canAim", true);
            var camera = Camera.main;
            var forward = cam.transform.forward;
            var right = cam.transform.right;

            desiredMoveDirection = forward;

            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(desiredMoveDirection), 0.2f);

        }
        if (Input.GetButtonUp("Fire1") && hasWeapon && !isWalking)
        {
            anim.SetBool("canAim", false);
            anim.SetTrigger("throw");
        }
    }

    void Throw()
    {
        hasWeapon = false;
        weaponScript.activated = true;
        axeRB.isKinematic = false;
        axeRB.collisionDetectionMode = CollisionDetectionMode.Continuous;
        axe.parent = null;
        axe.eulerAngles = new Vector3(0, -90 + transform.eulerAngles.y, 0);
        axe.transform.position += transform.right / 5;
        axeRB.AddForce(Camera.main.transform.forward * axeThrowForce + transform.up * 2, ForceMode.Impulse);

        isAiming = false;
    }

    void ReturnAxe()
    {
        if (Input.GetButtonDown("Fire1") && !hasWeapon)
        {
            time = 0.0f;
            oldAxePos = axe.transform.position;
            axeRB.Sleep();
            axeRB.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            axe.DORotate(new Vector3(-90, -90, 0), .2f).SetEase(Ease.InOutSine);
            axe.DOBlendableLocalRotateBy(Vector3.right * 90, .5f);
            isReturning = true;
            axeRB.isKinematic = true;
            axeRB.velocity = Vector3.zero;
        }

        if(isReturning)
        {
            if (time < 1.0)
            {

                weaponScript.activated = true;
                axe.transform.position = GetBezierQuadraticCurvePoint(time, oldAxePos, curvePoint.position, hand.position);
                time += Time.deltaTime;
            }
            else
            {
                // Reset Axe
                isReturning = false;
                axeRB.isKinematic = true;
                weaponScript.activated = false;
                axe.transform.parent = hand.transform;

                axe.localPosition = origLocPos;
                axe.localEulerAngles = origLocRot;
                hasWeapon = true;

                //Shake
                impulseSource.GenerateImpulse(Vector3.right);

            }
        }
    }

    Vector3 GetBezierQuadraticCurvePoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        Vector3 p = (uu * p0) + (2 * u * t * p1) + (tt * p2);
        return p;
    }

    void CheckAnimations()
    {
        if(h != 0 || v != 0)
        {
            anim.SetBool("canWalk", true);
        }
        else
        {
            anim.SetBool("canWalk", false);
        }
    }

    public void DeactivatePlayerMovement()
    {
        canPlayerMove = false;
        virtualCamera.enabled = false;
        anim.SetBool("canWalk", false);
        h = 0;
        v = 0;
    }

    public void ReActivatePlayerMovement()
    {
        canPlayerMove = true;
        virtualCamera.enabled = true;
        anim.SetBool("canWalk", false);
    }
        
}
