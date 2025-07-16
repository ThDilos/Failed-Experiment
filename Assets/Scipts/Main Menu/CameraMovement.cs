using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] CameraShots[] cameraShots;

    private int index = 0;
    private int length;
    private float currentDistance, speed;
    private void Start()
    {
        length = cameraShots.Length;
        currentDistance = cameraShots[0].distance;
        speed = cameraShots[0].speed;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentDistance > 0)
        {
            transform.position = transform.position + transform.forward * speed * Time.deltaTime;
            currentDistance -= speed * Time.deltaTime;
        }
        else
        {
            nextCameraShot();
        }
    }

    private void nextCameraShot()
    {
        Debug.Log(index);
        index = index < length - 1 ? index + 1 : 0;
        transform.position = cameraShots[index].position;
        transform.rotation = Quaternion.Euler(cameraShots[index].rotation);
        currentDistance = cameraShots[index].distance;
        speed = cameraShots[index].speed;
    }

    [System.Serializable]
    private class CameraShots
    {
        [Header("Starting Position of Camera")]
        public Vector3 position;
        [Header("Starting Rotation of Camera")]
        public Vector3 rotation;
        [Header("Max Distance Travelled by Camera")]
        public float distance;
        [Header("Speed of Travelling")]
        public float speed;
    }
}
