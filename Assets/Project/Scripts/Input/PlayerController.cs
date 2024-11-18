using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Platformer
{
    public class PlayerController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] Rigidbody rb;
        [SerializeField] Animator animator;
        [SerializeField] CinemachineFreeLook freeLookVcam;
        [SerializeField] InputReader input;
        [SerializeField] GroundChecker groundChecker;

        [Header("Settings")]
        [SerializeField] float moveSpeed = 6f;
        [SerializeField] float rotationSpeed = 15f;
        [SerializeField] float smoothTime = 0.2f;

        [Header("Jump Settings")]
        [SerializeField] float jumpForce = 10f;
        [SerializeField] float jumpDuration = 0.5f;
        [SerializeField] float jumpCooldown = 0;
        [SerializeField] float jumpMaxHeight = 2f;
        [SerializeField] float gravityMultiplier = 3f;

        const float ZeroF = 0f;

        Transform mainCam;

        float currentSpeed;
        float velocity;
        float jumpVelocity;

        Vector3 movement;

        List<Timer> timers;
        CountDownTimer jumpTimer;
        CountDownTimer jumpCooldownTimer;

        //Animator parameters
        static readonly int Speed = Animator.StringToHash("Speed");

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            animator = GetComponent<Animator>();
            groundChecker = GetComponent<GroundChecker>();

            mainCam = Camera.main.transform;
            freeLookVcam.Follow = transform;
            freeLookVcam.LookAt = transform;
            //Invoke event when observered transform is teleported, adjusting freeLookVCam's position accordingly
            freeLookVcam.OnTargetObjectWarped(transform, transform.position - freeLookVcam.transform.position - Vector3.forward);

            rb.freezeRotation = true;

            //Setup timers
            jumpTimer = new CountDownTimer(jumpDuration);
            jumpCooldownTimer = new CountDownTimer(jumpCooldown);
            timers = new List<Timer>(2) { jumpTimer, jumpCooldownTimer};

            jumpTimer.OnTimerStop += () => jumpCooldownTimer.Start();
        }
        private void Start() => input.EnablePlayerActions();

        private void OnEnable()
        {
            input.Jump += OnJump;
        }
        private void OnDisable()
        {
            input.Jump -= OnJump;
        }

        private void Update()
        {
            movement = new Vector3(input.Direction.x, 0f, input.Direction.y);
            HandleAnimator();
            HandleTimers();
        }
        private void FixedUpdate()
        {
            HandleJump();
            HandleMovement();
        }
        void HandleTimers() {
            foreach(var timer in timers){
                timer.Tick(Time.deltaTime);
            }
        }

        void HandleJump() {
            //If not jumping and is grounded, keep jump velocity at 0
            if(!jumpTimer.IsRunning && groundChecker.IsGround) {
                jumpVelocity = ZeroF;
                jumpTimer.Stop();
                return;
            }

            //If jumping or falling calculate velocity
            if(jumpTimer.IsRunning) 
            {
                //Progress point for initial burst of velocity
                float launchPoint = 0.9f;
                if(jumpTimer.Progress > launchPoint) {
                    //Calculate the velocity require to reach the jump height using physics equation v = sqrt(2gh) with g = gravity, h = height, v = velocity
                    jumpVelocity = Mathf.Sqrt(2 * jumpMaxHeight * Mathf.Abs(Physics.gravity.y));
                }
                else {
                    //Gradually apply less velocity as the jump progresses
                    jumpVelocity += (1 - jumpTimer.Progress) * jumpForce * Time.fixedDeltaTime;
                }
            }
            else {
                //Gravity take over
                jumpVelocity += Physics.gravity.y * gravityMultiplier * Time.deltaTime;
            }

            //Apply velocity
            rb.velocity = new Vector3(rb.velocity.x, jumpVelocity, rb.velocity.z);
        }
        void HandleAnimator()
        {
            animator.SetFloat(Speed, currentSpeed);
        }
        void HandleMovement()
        {
            //Rotate movement direction to match camera rotation
            var adjustedDirection = Quaternion.AngleAxis(mainCam.eulerAngles.y, Vector3.up) * movement;
            if(adjustedDirection.magnitude > ZeroF)
            {

                HandleRotation(adjustedDirection);
                HandleHorizontalMovement(adjustedDirection);
                SmoothSpeed(adjustedDirection.magnitude);
            }
            else
            {
                SmoothSpeed(ZeroF);

                //Reset the horizontal velocity for a snappy stop
                rb.velocity = new Vector3(ZeroF, rb.velocity.y, ZeroF);
            }
        }
        void HandleRotation(Vector3 adjustedDirection)
        {
            //Adjust rotation to match movement direction
            var targetRotation = Quaternion.LookRotation(adjustedDirection);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);  //Smooth out the rotation
            //transform.LookAt(transform.position + adjustedDirection);   //Instantly rotate direction
        }
        void HandleHorizontalMovement(Vector3 adjustedDirection)
        {
            //Move the player
            Vector3 velocity = adjustedDirection * moveSpeed * Time.deltaTime;
            rb.velocity = new Vector3(velocity.x, rb.velocity.y, velocity.z);
        }
        void SmoothSpeed(float value)
        {
            currentSpeed = Mathf.SmoothDamp(currentSpeed, value, ref velocity, smoothTime);
        }

        void OnJump(bool performed)
        {
            if (performed && !jumpTimer.IsRunning && !jumpCooldownTimer.IsRunning && groundChecker.IsGround) { 
                jumpTimer.Start();
            }
            else if (!performed && jumpTimer.IsRunning) {
                jumpTimer.Stop();
            }
        }
    }
}
