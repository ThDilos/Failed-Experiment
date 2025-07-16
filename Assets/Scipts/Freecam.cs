using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Freecam : MonoBehaviour
{
    private Transform tf;
    [SerializeField]
    private KeyCode hideCursorKey = KeyCode.V;
    private bool hideCursor = false;
    [SerializeField]
    private float cameraSpeed = 1.0f;
    private float currentCameraSpeed;
    [SerializeField]
    private float speedMultiplier = 2.0f;
    [SerializeField]
    private float cameraRotationSpeed = 10.0f;
    private float mouseX;
    private float mouseY;

    private bool isForward, isBackward, isUp, isDown, isLeft, isRight, isSpeeding;
    // Start is called before the first frame update
    void Awake()
    {
        tf = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        isForward = Input.GetKey(KeyCode.W);
        isBackward = Input.GetKey(KeyCode.S);
        isLeft = Input.GetKey(KeyCode.A);
        isRight = Input.GetKey(KeyCode.D);
        isUp = Input.GetKey(KeyCode.Space);
        isDown = Input.GetKey(KeyCode.LeftControl);
        isSpeeding = Input.GetKey(KeyCode.LeftShift);

        mouseX += Input.GetAxis("Mouse X") * cameraRotationSpeed;
        mouseY -= Input.GetAxis("Mouse Y") * cameraRotationSpeed;
        mouseY = Mathf.Clamp(mouseY, -70f, 70f);

        tf.rotation = Quaternion.Euler(mouseY, mouseX, 0);
        if (Input.GetKeyDown(hideCursorKey))
        {
            hideCursor = !hideCursor;
        }
        Cursor.visible = hideCursor;
        Cursor.lockState = hideCursor ? CursorLockMode.None : CursorLockMode.Locked;
    }

    private void FixedUpdate()
    {
        if (isForward)
        {
            tf.position += tf.forward * currentCameraSpeed * Time.fixedDeltaTime;
        }
        else if (isBackward)
        {
            tf.position -= tf.forward * currentCameraSpeed * Time.fixedDeltaTime;
        }
        if (isRight)
        {
            tf.position += tf.right * currentCameraSpeed * Time.fixedDeltaTime;
        }
        else if (isLeft)
        {
            tf.position -= tf.right * currentCameraSpeed * Time.fixedDeltaTime;
        }

        currentCameraSpeed = isSpeeding ? cameraSpeed * speedMultiplier : cameraSpeed;
    }
}
