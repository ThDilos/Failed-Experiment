using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    private bool isFowardInput, isBackwardInput, isRightInput, isLeftInput;
    private bool isJumpInput, isRunningInput, isDashInput, isSitInput;
    private bool showCursor = true;
    private bool isSitting = false;

    // When true, lock all movement keys
    private bool lockMovement = false; // For sit, in air(?)
    private bool lockPosition = false; // For dialogue, cutscene n stuff

    [Header("Misc Keybind")]
    [SerializeField] private KeyCode dashKey = KeyCode.G;
    [SerializeField] private KeyCode sitKey = KeyCode.C;

    [Header("Camera Settings")]
    [SerializeField] private GameObject thirdPersonCamera;
    [SerializeField] private float cameraRotationSpeed = 10.0f;
    [SerializeField] private float cameraOffset = 0.0f;
    [SerializeField] private float cameraDistanceOffset = 10.0f;

    [Header("Player Stats")]
    // Walking
    [SerializeField] private float walkSpeed = 100.0f;
    // Running
    [SerializeField] private float runningSpeed = 300.0f;
    private float currentSpeed = 0.0f;
    // Rotating
    [SerializeField] private float rotationSpeed = 10.0f;
    // Jumping
    private bool grounded = false;
    private CapsuleCollider capsuleCollider;
    public LayerMask groundLayerMask;
    [SerializeField] private float jumpForce = 80.0f;
    // Dashing
    [SerializeField] float dashCD = 1.5f;
    private bool isDashing = false;
    private bool canDash = true;
    [SerializeField] private float dashForce = 100.0f;
    [SerializeField] private float dashDuration = 0.5f;
    // When depleted, trigger die()"

    [Header("Health")]
    [SerializeField] private int maxHealth = 100;
    public int currentHealth;
    public HealthBar healthBar;
    [HideInInspector] public bool died = false;

    // Ground lock is to prevent randomly floating away if you go against a slope fast
    private bool groundLocked = true;
    private float groundLockCD = 1.0f; // Allow jump to work, too low will prevent player from jumping completely

    [Header("Speed Control")]
    [SerializeField]
    private float artificialGravity = 10.0f;
    [SerializeField]
    private float terminalVelocity = 10.0f;

    //The amount of change the mouse has move in the x and y position
    private float mouseX;
    private float mouseY;

    [Header("SoundFX")]
    [SerializeField] private AudioClip[] walkClips;
    [SerializeField] private AudioClip[] runClips;
    [SerializeField] private AudioClip jumpClip;
    [SerializeField] private AudioClip dashClip;
    [SerializeField] private AudioClip dieClip;

    // The Interval of which footstep is played, to match up with player walking animation
    private float footstepTimer = 0.0f;
    [SerializeField] private float footstepInterval = 0.5f;

    [Header("Misc. [Check code comments in Player.cs for their uses]")]
    // Inside the Unity Editor, you need to switch to Game Tab and left click the game window to see this effect. It is functioning normally
    [SerializeField] private bool legacyStartingCameraMode = true; // Which player control method should start from? It's mode 1 where you have cursor freedom. And the tutorial tells you to switch to mode 2. We can have mode 2 since the start.

    // Other Run Time References
    private Rigidbody rb;
    private Animator animator;
    private GameController gameController;
    private SkinnedMeshRenderer foxRenderer;
    private Material originalMat;

    void Awake()
    {
        died = false;
        gameController = GetComponent<GameController>();
        rb = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        animator = GetComponent<Animator>();
        thirdPersonCamera = GameObject.Find("3rd Person Camera");
        mouseX = 0; mouseY = 30; // Adjust starting camera angle to not sunk into ground
        foxRenderer = GameObject.Find("fox").GetComponent<SkinnedMeshRenderer>(); // For setting fox transparency when cam too close
        originalMat = foxRenderer.material;

        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);

        // Switch Camera Mode based on Editor Setting
        if (!legacyStartingCameraMode) SwitchCameraMode();
    }

    // Update is called once per frame
    void Update()
    {
        if(!PauseMenu.isPaused)
        {
            HandleCamera();

            //Check the button input
            isFowardInput = Input.GetKey(KeyCode.W);
            isBackwardInput = Input.GetKey(KeyCode.S);
            isRightInput = Input.GetKey(KeyCode.D);
            isLeftInput = Input.GetKey(KeyCode.A);
            isJumpInput = Input.GetKeyDown(KeyCode.Space);
            isRunningInput = Input.GetKey(KeyCode.LeftShift);
            isDashInput = Input.GetKeyDown(dashKey);
            isSitInput = Input.GetKeyDown(sitKey);

            //Check if the user is on the ground, puts a sphere check under the player collider
            grounded = Physics.CheckSphere(new Vector3(transform.position.x, transform.position.y + capsuleCollider.height / 4.0f, transform.position.z),
                0.4f, // Radius of the checking sphere
                groundLayerMask); // Layermast to check for ground
            if (grounded && groundLockCD <= 0)
            {
                groundLocked = true;
            }
            else if (groundLockCD > 0)
            {
                groundLockCD -= Time.deltaTime;
            }
            /*Debug.DrawRay(transform.position,
                Vector3.down * capsuleCollider.height / 2.0f,
                Color.red);*/


            // If Jump is pressed we jump
            if (isJumpInput && grounded)
            {
                if (rb.velocity.magnitude == 0) // These coroutines are for better synchronization btw animation and jump time.
                {
                    StartCoroutine(Jump(0.3f));
                }
                else
                {
                    StartCoroutine(isRunningInput ? Jump(0.2f) : Jump(0.25f));
                }
            }

            if (!grounded) // The jump keeps triggering for some reason
            {
                animator.ResetTrigger("Jump");
            }

            // Artificial Gravity to make the character less floaty
            // Terminal Velocity to prevent player from falling too fast and phase through the ground
            if (!lockPosition && !isDashing)
            {
                if (groundLocked)
                {
                    rb.AddForce(artificialGravity  * -rb.transform.up, ForceMode.Impulse);
                }
                else
                {
                    rb.AddForce(artificialGravity / 10 * -rb.transform.up, ForceMode.Impulse);
                }
                rb.velocity = new Vector3(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -terminalVelocity, Mathf.Infinity), rb.velocity.z);
            }

            // Zoom
            if (Input.GetAxis("Mouse ScrollWheel") > 0f)
            {
                cameraDistanceOffset -= 1.0f;
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
            {
                cameraDistanceOffset += 1.0f;
            }
            // Clamp Distance Offset
            cameraDistanceOffset = Mathf.Clamp(cameraDistanceOffset, 3.0f, 16.0f);

            // Dash
            if (isDashInput && !isDashing && canDash && !lockMovement && !lockPosition)
            {
                isDashing = true;
                canDash = false;
                StartCoroutine(Dash());
            }

            // Sit Animation
            if (isSitInput && !(isFowardInput || isBackwardInput || isLeftInput || isRightInput || isJumpInput || isDashInput || isRunningInput))
            {
                animator.SetBool("Sitting", !isSitting);
                isSitting = !isSitting;
                lockMovement = isSitting;
            }
            if (isSitting && (isFowardInput || isBackwardInput || isLeftInput || isRightInput || isJumpInput || isDashInput || isRunningInput))
            {
                animator.SetBool("Sitting", false);
                isSitting = false;

                StartCoroutine(UnlockMovementFromSitting(0.1f)); // Unlock movement after the unsit animation played for 0.1s
            }
        }
    }

    IEnumerator UnlockMovementFromSitting(float delay)
    {
        yield return new WaitForSeconds(delay);
        lockMovement = false;
    }

    IEnumerator Dash()
    {
        // Trigger Animation
        animator.SetFloat("DashSpeedMultiplier", 0.8f / dashDuration);
        animator.SetTrigger("Dash");

        // play sound FX
        SoundFXManager.instance.PlaySoundFXClip(dashClip, transform, 0.3f);

        Debug.Log("Dash");
        // Dash Forward
        rb.AddForce(rb.transform.forward * dashForce, ForceMode.Impulse);
        yield return new WaitForSeconds(dashDuration); // Repeat the above commands for dashDuration seconds
        isDashing = false;
        animator.SetFloat("DashSpeedMultiplier", 1.0f);
        yield return new WaitForSeconds(dashCD);
        canDash = true;
    }
    IEnumerator Jump(float delay) // Delay to match animation
    {
        groundLockCD = 1.0f;
        animator.SetTrigger("Jump");
        // play sound FX
        SoundFXManager.instance.PlaySoundFXClip(jumpClip, transform, 1f);
        yield return new WaitForSeconds(delay);
        Debug.Log("Jump");
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z); // Reset y velocity
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        grounded = false;
        groundLocked = false;
    }

    private void FixedUpdate()
    {
        Vector3 direction = Vector3.zero;

        //we are running
        if (isRunningInput && ((showCursor && !isBackwardInput) || (!showCursor)))
        {
            currentSpeed = runningSpeed;
        }
        else
        {
            if (currentSpeed > walkSpeed) // A stupid way to gradually scale down the speed when you change from run to walk
            {
                currentSpeed *= 0.9f;
            }
            else
            {
                currentSpeed = walkSpeed;
            }
        }
        var velocity = direction * (isBackwardInput ? -currentSpeed : currentSpeed) * Time.fixedDeltaTime; // This line exists because remove it will error.

        // ShowCursor Mode:
        // Freely Rotate, Camera does not change player rotation at all.
        // When stationary: Rotate only change Character rotation.
        // Your only 2 ways of moving is Going Forward or Going Backward.
        // Good for senaric viewing.
        // Also frees up the cursor to click on screen / interactive stuff

        // !ShowCursor Mode:
        // Player's movement is relative to the camera.
        // Rotation is always updated via the camera when Moving.
        // When stationary: Directional Input will cause the Character to Turn to that direction AND move forward
        // !ShowCursor Mode does not have the "walking back" animation
        // Good for full immersive gameplay
        // Player is relatively agile, precise in controlling moving direction
        // Good for parkour, action-related gameplay.

        // ShowCursor Mode
        Quaternion rotateAngle;
        if (!lockPosition)
        {
            if (showCursor)
            {
                if (isFowardInput || isBackwardInput) // You only go forwards or backwards
                {
                    direction = transform.forward;
                }
                velocity = direction * (isBackwardInput ? -currentSpeed : currentSpeed) * Time.fixedDeltaTime; // Go forwards, or go backwards

                // Rotate Player by a fixed rate.
                if (isLeftInput)
                {
                    rb.MoveRotation(rb.rotation * (Quaternion.Euler(0, -rotationSpeed * 80 * Time.fixedDeltaTime, 0)));
                }
                else if (isRightInput)
                {
                    rb.MoveRotation(rb.rotation * (Quaternion.Euler(0, rotationSpeed * 80 * Time.fixedDeltaTime, 0)));
                }
                // Anime
                // Update Turning Animation
                animator.SetBool("Left", isLeftInput && !isRightInput);
                animator.SetBool("Right", isRightInput && !isLeftInput);
                // Walking Anime
                animator.SetBool("Walking", !isRunningInput && velocity.magnitude > 0.0f);
            }
            // !ShowCursor Mode
            else
            {
                if (isLeftInput || isRightInput || isFowardInput || isBackwardInput) // Start movin' if any of these keys are pressed
                {
                    direction = transform.forward; // You only go forward, there is no going back.
                }
                velocity = direction * currentSpeed * Time.fixedDeltaTime; // Move

                if (isLeftInput) // Rotate Left
                {
                    rotateAngle = Quaternion.Euler(0, thirdPersonCamera.transform.rotation.eulerAngles.y - ((isFowardInput) ? 45.0f : (isBackwardInput) ? 135.0f : 90.0f), 0);
                }
                else if (isRightInput) // Rotate Right
                {
                    rotateAngle = Quaternion.Euler(0, thirdPersonCamera.transform.rotation.eulerAngles.y + ((isFowardInput) ? 45.0f : (isBackwardInput) ? 135.0f : 90.0f), 0);
                }
                else if (velocity.magnitude > 0.0f)// Rotate to the cam forward or backward
                {
                    rotateAngle = Quaternion.Euler(0, thirdPersonCamera.transform.rotation.eulerAngles.y + ((isBackwardInput) ? 180 : 0), 0);
                }
                else // No Rotate
                {
                    rotateAngle = rb.rotation;
                }
                // Ro-ta-te
                rb.rotation = Quaternion.Lerp(rb.rotation, rotateAngle, rotationSpeed * Time.fixedDeltaTime);

                // Dash
                if (isDashInput)
                {
                    for (float currentTime = 0.0f; currentTime < dashDuration; currentTime += Time.fixedDeltaTime)
                    {
                        velocity = direction * currentSpeed * Time.fixedDeltaTime;
                    }
                }

                // Input based Animations
                // Turning Animation
                Vector3 crossProduct = Vector3.Cross(rb.transform.forward, rotateAngle * Vector3.forward); // CrossProduct method by ChadGPT
                                                                                                           // Basically if the crossProduct is +ve, your rotateAngle is on your right. -ve is on your left.
                                                                                                           // There seems to be no 0.0f, so after the && calculates the angle between your rotateAngle and currentAngle
                                                                                                           // And stop animation if the angle is less than the threshold (In this case 25.0f Degree)
                animator.SetBool("Left", crossProduct.y < 0.0f && Quaternion.Angle(rb.transform.rotation, rotateAngle) > 25.0f);
                animator.SetBool("Right", crossProduct.y > 0.0f && Quaternion.Angle(rb.transform.rotation, rotateAngle) > 25.0f);
                // Walking Animation
                // You always move in this !showcursor mode, so any directional inputs will trigger.
                animator.SetBool("Walking", (isLeftInput || isRightInput || isFowardInput || isBackwardInput) && !isRunningInput);
                // Prevent a lag into other animations caused by repeatedly transitioning into falling in air from Anystate
            }
        }



        if (!isDashing && !lockMovement && !lockPosition)
        {
            rb.velocity = new Vector3(velocity.x, rb.velocity.y, velocity.z);
        }
        if (lockMovement || lockPosition)
        {
            rb.velocity = Vector3.zero;
        }

        // Animations
        animator.SetBool("Running", isRunningInput && velocity.magnitude > 0.01f && ((!isBackwardInput && showCursor) || !showCursor));
        animator.SetBool("WalkingBack", isBackwardInput && showCursor);
        animator.SetBool("Falling", !grounded);
        animator.SetBool("InAirIsPlaying", animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Fox_Jump_InAir");

        // Walking Sound
        if ((isFowardInput || isBackwardInput || isLeftInput || isRightInput) && grounded && !isDashing)
        {
            footstepTimer -= Time.fixedDeltaTime;
            if (footstepTimer <= 0)
            {
                // play sound FX
                if (isRunningInput)
                {
                    SoundFXManager.instance.PlayRandomSoundFXClip(runClips, transform, 1f);
                }
                else
                {
                    SoundFXManager.instance.PlayRandomSoundFXClip(walkClips, transform, 1f);

                }
                footstepTimer = footstepInterval;
            }
        }
        else
        {
            footstepTimer = 0.0f;
        }
    }

    private void HandleCamera()
    {
        // When !showCursor, always update the cam direction
        // When showCursor, hold Left Click to drag the view around
        if (!showCursor || (Input.GetMouseButton(0) && showCursor))
        {
            mouseX += Input.GetAxis("Mouse X") * cameraRotationSpeed;
            mouseY -= Input.GetAxis("Mouse Y") * cameraRotationSpeed;
            mouseY = Mathf.Clamp(mouseY, -45f, 60f);
        }

        // If Right Click we toggle showCursor
        if (Input.GetMouseButtonDown(1))
        {
            SwitchCameraMode();
        }

        // Camera rotation
        thirdPersonCamera.transform.rotation = Quaternion.Euler(mouseY, mouseX, 0);

        // Camera position
        RaycastHit camRayHit; // camRayHit store the coordinate of ray impact

        //Putting a sphere check on camera, and put the camera offset with the sphere radius, in this case is 0.5f
        if (Physics.SphereCast(transform.position + Vector3.up * cameraOffset, 0.35f, -thirdPersonCamera.transform.forward, out camRayHit, cameraDistanceOffset, groundLayerMask))
        {
            thirdPersonCamera.transform.position = camRayHit.point + camRayHit.normal * 0.35f;
        }
        else
        {
            thirdPersonCamera.transform.position = (new Vector3(transform.position.x, transform.position.y + cameraOffset, transform.position.z) - thirdPersonCamera.transform.forward
                                        * cameraDistanceOffset);
        }

        // Camera Stays Focused on the Player, scaled up by "cameraOffset"
        thirdPersonCamera.transform.LookAt(transform.position + Vector3.up * cameraOffset);

        // Camera hide Player when near
        float camCloseness = Vector3.Distance(thirdPersonCamera.transform.position, transform.position);
        Material m = foxRenderer.material;
        // These are from https://discussions.unity.com/t/standard-material-shader-ignoring-setfloat-property-_mode/144845/2
        // You need all lines to make this work somehow :( Changing alpha directly wont work
        m.SetFloat("_Mode", (camCloseness < 2.0f) ? 3 : 0);
        m.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        m.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        m.DisableKeyword("_ALPHATEST_ON");
        m.EnableKeyword("_ALPHABLEND_ON");
        m.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        if (camCloseness < 2.0f)
        {
            m = originalMat;
        }
        m.renderQueue = (camCloseness < 2.0f) ? 3000 : 2000;
        Color color = m.color;
        color.a = (camCloseness < 2.0f) ? 0.3f : 1;
        m.color = color;
        foxRenderer.material = m;
    }

    private void SwitchCameraMode()
    {
        showCursor = !showCursor;
        Cursor.visible = showCursor;
        Cursor.lockState = showCursor ? CursorLockMode.None : CursorLockMode.Locked;
        Debug.Log("Switch to Camera Control Mode " + (showCursor ? "1" : "2"));
    }

    public void Die()
    {
        died = true;
        Debug.Log("awsl");
        lockPosition = true;
        animator.SetBool("die", true);
        // play sound FX
        SoundFXManager.instance.PlaySoundFXClip(dieClip, transform, 1f);
        StartCoroutine(respawn());
    }

    IEnumerator respawn()
    {
        yield return new WaitForSeconds(3);
        gameController.respawn();
    }

    public void Damage(int dmg)
    {
        Debug.Log("Ouch!");

        if (this.currentHealth < dmg) Die();
        else this.currentHealth -= dmg;
        healthBar.SetHealth(currentHealth);
    }
}
