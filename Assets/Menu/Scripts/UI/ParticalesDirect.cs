using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class ParticalesDirect : MonoBehaviour {

    public delegate void ParticlesEmitFinished();
    public static event ParticlesEmitFinished OnParticlesEmitFinished;

    public float wantedEmitionTime = 0f;
    public float velocity = 0.2f;
    public ParticleSystem particles;

    private RectTransform targetTransform;
    private ParticleSystem.Particle[] particleList;
    private ParticleSystem.EmissionModule emissionSystem;
    private List<Vector3> velocities = new List<Vector3>();
    private float emitionTime = 0f;

    void Update()
    {
        MoveParticles();
    }

    void OnDisable()
    {
        emiting = false;
        particleList = null;
        velocities.Clear();
    }

    public void Move(RectTransform target)
    {
        targetTransform = target;
        emitionTime = wantedEmitionTime;
        emissionSystem = particles.emission;
        emissionSystem.enabled = true;

        velocities.Clear();
        for (int i = 0; i < 20; i++)
        {
            velocities.Add(new Vector3(Random.Range(-10f, 10f), 0f, 0f));
        }

      //  StartCoroutine(ParticleFinishedEvent());
    }


    private IEnumerator ParticleFinishedEvent()
    {
        yield return new WaitForSeconds(3f);
        //Debug.Log("particales finished");
        if (OnParticlesEmitFinished != null)
            OnParticlesEmitFinished();
    }

    bool emiting = false;
    private void MoveParticles()
    {
        if (emitionTime > 0f)
        {
            emitionTime -= Time.deltaTime;
            emiting = true;
        }
        else
        {
            emissionSystem = particles.emission;
            emissionSystem.enabled = false;
        }

        particleList = new ParticleSystem.Particle[particles.particleCount];
        particles.GetParticles(particleList);

        for (int i = 0; i < particleList.Length; i++)
        {
            Vector3 velocity = velocities[i];

            particleList[i].position = Vector3.SmoothDamp(particleList[i].position, targetTransform.GetWorldBounds().center, ref velocity, 0.6f);
            velocities[i] = velocity;
        }

        particles.SetParticles(particleList, particles.particleCount);

        if(emiting && particles.particleCount == 0)
        {
            emiting = false;
            StartCoroutine(ParticleFinishedEvent());
        }
    }

}

