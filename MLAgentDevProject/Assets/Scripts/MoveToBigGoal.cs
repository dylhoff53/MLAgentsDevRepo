using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class MoveToBigGoal : Agent
{
    [SerializeField] private Transform bigGoalTransform;
    [SerializeField] private Material winMaterial;
    [SerializeField] private Material loseMaterial;
    [SerializeField] private MeshRenderer[] floorMeshRendererArray;
    [SerializeField] private GameObject[] breadCrumbs;

    public override void OnEpisodeBegin()
    {
        transform.localPosition = new Vector3(Random.Range(-4f, 4f), 0, Random.Range(-3f, 1f));
        bigGoalTransform.localPosition = new Vector3(Random.Range(-4f, 4f), 0, Random.Range(17f, 24f));
        foreach(GameObject bread in breadCrumbs)
        {
            bread.SetActive(true);
        }
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(bigGoalTransform.localPosition);
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];

        float moveSpeed = 3f;
        transform.localPosition += new Vector3(moveX, 0, moveZ) * Time.deltaTime * moveSpeed;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousAction = actionsOut.ContinuousActions;
        continuousAction[0] = Input.GetAxisRaw("Horizontal");
        continuousAction[1] = Input.GetAxisRaw("Vertical");
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Goal>(out Goal goal))
        {
            SetReward(6f);
            other.gameObject.SetActive(false);

        }
        else if (other.TryGetComponent<Wall>(out Wall wall))
        {
            SetReward(-1f);
            foreach (MeshRenderer meshRend in floorMeshRendererArray)
            {
                meshRend.material = loseMaterial;
            }
            EndEpisode();
        }
        else if (other.TryGetComponent<BigGoal>(out BigGoal bigGoal))
        {
            SetReward(50f);
            foreach (MeshRenderer meshRend in floorMeshRendererArray)
            {
                meshRend.material = winMaterial;
            }
            EndEpisode();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.TryGetComponent<Ground>(out Ground ground))
        {
        }
    }
}
