using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeSpawner : MonoBehaviour
{
	[SerializeField]
	private float range;

	[SerializeField]
    private GameObject spawningObject;

	[SerializeField]
	private int maximumObjects = 3;

	private float secondsBetweenSpawn = 5f;

	private int amountOfObjects = 0;

	private List<GameObject> spawnedObjects;

	private int objectToMove = 0;

    // Start is called before the first frame update
    void Start()
    {
		spawnedObjects = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
		secondsBetweenSpawn -= Time.deltaTime;
		if (secondsBetweenSpawn <= 0) {
			Vector3 spawnPosition = new Vector3(Random.Range(-range, range), 0f,
					Random.Range(-range, range)) + transform.position;
			if (amountOfObjects < maximumObjects) {
				var objectSpawned = Instantiate(spawningObject, spawnPosition,
							Quaternion.Euler(0, 0, 0));
				objectSpawned.SetActive(true);
				spawnedObjects.Add(objectSpawned);
				amountOfObjects++;
			} else {
				objectToMove = ++objectToMove % spawnedObjects.Count;
				spawnedObjects[objectToMove].transform.position = spawnPosition;
			}
			secondsBetweenSpawn = 5f;
		}
    }
}
