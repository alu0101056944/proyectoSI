using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Random = UnityEngine.Random;

public class FoodCollectorAgent : Agent
{
    [SerializeField]
    private string targetFlag = "food";
    [SerializeField]
    private List<string> badFlags;

    private float totalScore;

    public GameObject area;
    FoodCollectorArea m_MyArea;
    bool m_Satiated;
    float m_EffectTime;
    Rigidbody m_AgentRb;

    // Speed of agent rotation.
    public float turnSpeed = 300;

    // Speed of agent movement.
    public float moveSpeed = 2;
    public Material normalMaterial;
    public Material badMaterial;
    public Material goodMaterial;

    EnvironmentParameters m_ResetParams;

    public override void Initialize()
    {
        m_AgentRb = GetComponent<Rigidbody>();
        m_MyArea = area.GetComponent<FoodCollectorArea>();
        m_ResetParams = Academy.Instance.EnvironmentParameters;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        var localVelocity = transform.InverseTransformDirection(m_AgentRb.velocity);
        sensor.AddObservation(localVelocity.x);
        sensor.AddObservation(localVelocity.z);
    }

    public Color32 ToColor(int hexVal)
    {
        var r = (byte)((hexVal >> 16) & 0xFF);
        var g = (byte)((hexVal >> 8) & 0xFF);
        var b = (byte)(hexVal & 0xFF);
        return new Color32(r, g, b, 255);
    }

    public void MoveAgent(ActionBuffers actionBuffers)
    {
        if (Time.time > m_EffectTime + 0.5f)
        {
            if (m_Satiated)
            {
                Unsatiate();
            }
        }

        var dirToGo = Vector3.zero;
        var rotateDir = Vector3.zero;

        var continuousActions = actionBuffers.ContinuousActions;
        var discreteActions = actionBuffers.DiscreteActions;
          
        var forward = Mathf.Clamp(continuousActions[0], -1f, 1f);
        var right = Mathf.Clamp(continuousActions[1], -1f, 1f);
        var rotate = Mathf.Clamp(continuousActions[2], -1f, 1f);

        dirToGo = transform.forward * forward;
        dirToGo += transform.right * right;
        rotateDir = -transform.up * rotate;

        m_AgentRb.AddForce(dirToGo * moveSpeed, ForceMode.VelocityChange);
        transform.Rotate(rotateDir, Time.fixedDeltaTime * turnSpeed);
    

        if (m_AgentRb.velocity.sqrMagnitude > 25f) // slow it down
        {
            m_AgentRb.velocity *= 0.95f;
        }
    }

    void Satiate()
    {
        m_Satiated = true;
        m_EffectTime = Time.time;
        gameObject.GetComponentInChildren<Renderer>().material = goodMaterial;
    }

    void Unsatiate()
    {
        m_Satiated = false;
        gameObject.GetComponentInChildren<Renderer>().material = normalMaterial;
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        MoveAgent(actionBuffers);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        if (Input.GetKey(KeyCode.D))
        {
            continuousActionsOut[2] = 1;
        }
        if (Input.GetKey(KeyCode.W))
        {
            continuousActionsOut[0] = 1;
        }
        if (Input.GetKey(KeyCode.A))
        {
            continuousActionsOut[2] = -1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            continuousActionsOut[0] = -1;
        }
        var discreteActionsOut = actionsOut.DiscreteActions;
        discreteActionsOut[0] = Input.GetKey(KeyCode.Space) ? 1 : 0;
    }

    public override void OnEpisodeBegin()
    {
        totalScore = 0f;
        Unsatiate();
        m_AgentRb.velocity = Vector3.zero;
        transform.position = new Vector3(Random.Range(-m_MyArea.range, m_MyArea.range),
            2f, Random.Range(-m_MyArea.range, m_MyArea.range))
            + area.transform.position;
        transform.rotation = Quaternion.Euler(new Vector3(0f, Random.Range(0, 360)));
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(targetFlag))
        {
            Satiate();
            collision.gameObject.GetComponent<FoodLogic>().OnEaten();
            ++totalScore;
            AddReward(1f);
        }
        for (int i = 0; i < badFlags.Count; ++i) {
            if (collision.gameObject.CompareTag(badFlags[i]))
            {
                collision.gameObject.GetComponent<FoodLogic>().OnEaten();
                --totalScore;
                AddReward(-1f);
            }
        }
        if (collision.gameObject.CompareTag("wall"))
        {
            --totalScore;
            AddReward(-1f);
        }
    }

    public void penalize(int delta) {
      totalScore += delta;
      AddReward(delta);
    }

    public float getScore() {
      return totalScore;
    }
}
