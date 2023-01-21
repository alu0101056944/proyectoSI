using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventSystemManager : MonoBehaviour
{

    [SerializeField]
    private float secondsBetweenReset;
    private float currentSeconds;

    [SerializeField]
    private int weightOfObstacleSpawn;
    [SerializeField]
    private int maximumObstaclesToSpawn;

    [SerializeField]
    private GameObject automata;
    private SistemaEventos eventSystem;

    // Start is called before the first frame update
    void Start()
    {
      eventSystem = automata.GetComponent<SistemaEventos>();
      currentSeconds = secondsBetweenReset;
    }

    // Update is called once per frame
    void Update()
    {
        currentSeconds -= Time.deltaTime;
        if (currentSeconds <= 0) {
          Debug.Log(" alive : " + eventSystem.getAmountOfAlive());
          if (eventSystem.getAmountOfAlive() == 0) {
            eventSystem.randomize(maximumObstaclesToSpawn, weightOfObstacleSpawn);
          }
          currentSeconds = secondsBetweenReset;
        }
    }
}
