using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : MonoBehaviour
{
    [SerializeField] private GameObject firingParticle;
    [SerializeField] private GameObject nuzzle;
    [SerializeField] private AudioClip firingSfx;
    [SerializeField] private AudioClip hallFiringSfx;

    public bool firing = false;
    public float fireRate = 0.3f;
    [SerializeField] int damage = 1;
    public Transform target;
    private AudioSource audioSource;
    private float currentTimer = 0.0f;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (firing && target != null)
        {
            transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, target.position - transform.position, Time.deltaTime * fireRate, 0.0f));
            if (currentTimer < fireRate) currentTimer += Time.deltaTime;
            else Fire();
        }
        else
        {
            currentTimer = 0.0f;
        }
    }

    void Fire()
    {
        currentTimer = 0.0f;
        if (gameObject.scene.isLoaded) // Prevent closing scene error
        {
            Instantiate(firingParticle, nuzzle.transform.position, transform.rotation); // Spawn Explosion
            if (target.GetComponent<Player>() != null) 
            {
                handleSound(Vector3.Distance(transform.position, target.transform.position));
                target.GetComponent<Player>().Damage(damage);
            }
        }
    }

    void handleSound(float targetDistance)
    {
        if (targetDistance > 10.0f)
            audioSource.PlayOneShot(hallFiringSfx, targetDistance);
        else 
            audioSource.PlayOneShot(firingSfx, targetDistance);
    }
}
