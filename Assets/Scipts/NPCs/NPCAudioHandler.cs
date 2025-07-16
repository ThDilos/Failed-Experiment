using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCAudioHandler : MonoBehaviour
{

    [SerializeField] private AudioClip walk;
    [SerializeField] private AudioClip running;

    private AudioSource audioSource;
    private Animator animator;
    private NavMeshAgent agent;

    private int step = 0;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        float vel = animator.GetFloat("vel");

        if (!audioSource.isPlaying && vel > 0)
        {
            if (vel <= 1)
            {
                audioSource.PlayOneShot(walk);
            }
            else
            {
                audioSource.PlayOneShot(running);
                StartCoroutine(cutSource(0.2f));
            }
            audioSource.pitch = (step == 0) ? Random.Range(0.8f, 0.9f) : Random.Range(1.1f, 1.2f);
            step = step == 0 ? 1 : 0;
        }
    }

    // Cutoff run audio for quick run audio
    IEnumerator cutSource(float delay)
    {
        yield return new WaitForSeconds(delay);
        audioSource.Stop();
    }
}
