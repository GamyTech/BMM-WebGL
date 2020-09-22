using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Scheduler
{
    public bool debug;

    private MonoBehaviour mono;
    private Queue<IJob> queue;

    private IEnumerator runner;

    private IJob currentJob;

    public Scheduler(MonoBehaviour o)
    {
        mono = o;
        queue = new Queue<IJob>();
    }

    public void AddJob(IJob job)
    {
        queue.Enqueue(job);
        StartRunner();
    }

    public void StopAllJobs()
    {
        StopRunner();
        queue.Clear();
    }

    private void StartRunner()
    {
        if (runner == null)
        {
            runner = runJobs();
            mono.StartCoroutine(runner);
        }
    }

    private void StopRunner()
    {
        if (runner != null)
        {
            mono.StopCoroutine(runner);
            runner = null;

            if(currentJob != null && currentJob is EnumeratorJob)
            {
                mono.StopCoroutine((currentJob as EnumeratorJob).JobRoutine);
            }
            currentJob = null;
        }
    }

    private IEnumerator runJobs()
    {
        while(queue.Count > 0)
        {
            currentJob = queue.Dequeue();
            if (debug) Debug.Log("Job Running: " + currentJob);

            if (currentJob is EnumeratorJob)
            {
                EnumeratorJob ej = currentJob as EnumeratorJob;
                yield return mono.StartCoroutine(ej.JobRoutine);
                if (ej.FinishedCallback != null)
                {
                    ej.FinishedCallback();
                }
            }
            else if(currentJob is WaitThenActionJob)
            {
                yield return wait((currentJob as WaitThenActionJob).waitTime);
                (currentJob as WaitThenActionJob).JobAction();
            }
            else if (currentJob is ActionThenWaitJob)
            {
                (currentJob as ActionThenWaitJob).JobAction();
                yield return wait((currentJob as ActionThenWaitJob).waitTime);
            }
            else if(currentJob is ActionJob)
            {
                (currentJob as ActionJob).JobAction();
            }
        }
        currentJob = null;
        runner = null;
    }

    private IEnumerator wait(float time)
    {
        yield return new WaitForSeconds(time);
    }
}
