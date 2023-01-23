/**
 * Celullar automata whose matrix starts from the transform of the gameobject
 * that contains this script.
 *
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SistemaEventos : MonoBehaviour
{
    [SerializeField]
    private int cellSize = 10;
    [SerializeField]
    private int matrixSize = 10;

    [SerializeField]
    private int[] matrixArray;
    private int[,] matrix; // cant be serialized

    private List<GameObject> instantiated;

    [SerializeField]
    private GameObject[] stateToGameObjectArray;
    private Dictionary<int, GameObject> stateToGameObject; // cant be serialized

    [SerializeField]
    private float secondsBetweenUpdate = 5;
    private float secondsPassed = 0;

    [SerializeField]
    private bool showIdentifier;
    [SerializeField]
    private bool showIJ;

    void Awake()
    {
        matrix = new int[matrixSize, matrixSize];
        for (int i = 0; i < matrix.GetLength(0); ++i) {
        	for (int j = 0; j < matrix.GetLength(1); ++j) {
            	matrix[i, j] = matrixArray[(i * matrixSize) + j];
          	}
        }
        
        stateToGameObject = new Dictionary<int, GameObject>();
        for (int i = 0; i < stateToGameObjectArray.Length; ++i) {
            stateToGameObject.Add(i, stateToGameObjectArray[i]);
        }
        instantiated = new List<GameObject>();
    }

    private int transitionFunction(int cell, int i, int j)
    {
      var neighbours = new List<int>();
      if (i == 0 && j == 0) {
        neighbours.Add(matrix[i, j + 1]);
        neighbours.Add(matrix[i + 1, j]);
        neighbours.Add(matrix[i + 1, j + 1]);
      } else if (i == 0 && j == matrixSize - 1) {
        neighbours.Add(matrix[i, j - 1]);
        neighbours.Add(matrix[i + 1, j]);
        neighbours.Add(matrix[i + 1, j - 1]);
      } else if (i == matrixSize - 1 && j == 0) {
        neighbours.Add(matrix[i, j + 1]);
        neighbours.Add(matrix[i - 1, j]);
        neighbours.Add(matrix[i - 1, j + 1]);
      } else if (i == matrixSize - 1 && j == matrixSize - 1) {
        neighbours.Add(matrix[i, j - 1]);
        neighbours.Add(matrix[i - 1, j]);
        neighbours.Add(matrix[i - 1, j - 1]);
      } else if (i == 0 && j == 0) {
        neighbours.Add(matrix[i, j + 1]);
        neighbours.Add(matrix[i + 1, j]);
        neighbours.Add(matrix[i + 1, j + 1]);
      } else if (i > 0 && i < matrixSize - 1 && j == 0) { // left border
        neighbours.Add(matrix[i, j + 1]);
        neighbours.Add(matrix[i + 1, j]);
        neighbours.Add(matrix[i + 1, j + 1]);
        neighbours.Add(matrix[i - 1, j]);
        neighbours.Add(matrix[i - 1, j + 1]);
      } else if (i > 0 && i < matrixSize - 1 && j == matrixSize - 1) { // right border
        neighbours.Add(matrix[i, j - 1]);
        neighbours.Add(matrix[i + 1, j]);
        neighbours.Add(matrix[i + 1, j - 1]);
        neighbours.Add(matrix[i - 1, j]);
        neighbours.Add(matrix[i - 1, j - 1]);
      } else if (j > 0 && j < matrixSize - 1 && i == 0) { // top border
        neighbours.Add(matrix[i, j + 1]);
        neighbours.Add(matrix[i + 1, j]);
        neighbours.Add(matrix[i + 1, j + 1]);
        neighbours.Add(matrix[i, j -1 ]);
        neighbours.Add(matrix[i + 1, j - 1]);
      } else if (j > 0 && j < matrixSize - 1 && i == matrixSize - 1) { // bottom border
        neighbours.Add(matrix[i, j - 1]);
        neighbours.Add(matrix[i, j + 1]);
        neighbours.Add(matrix[i - 1, j]);
        neighbours.Add(matrix[i - 1, j + 1]);
        neighbours.Add(matrix[i - 1, j - 1]);
      } else {
        //Debug.Log("Inside at : " + i + ", " + j + ".");
        neighbours.Add(matrix[i, j - 1]);
        neighbours.Add(matrix[i, j + 1]);
        neighbours.Add(matrix[i - 1, j]);
        neighbours.Add(matrix[i - 1, j + 1]);
        neighbours.Add(matrix[i - 1, j - 1]);
        neighbours.Add(matrix[i + 1, j]);
        neighbours.Add(matrix[i + 1, j - 1]);
        neighbours.Add(matrix[i + 1, j + 1]);
      }

      var amountOfAlive = 0;
      for (int u = 0; u < neighbours.Count; ++u) {
        if (neighbours[u] >= 1) {
          ++amountOfAlive;
        }
      }

      //Debug.Log("i: " + i + "j: " + j + ", amount: " + neighbours.Count + "alive: " + amountOfAlive);

      switch (cell) {
        case 0:
          if (amountOfAlive >= 3) {
            return 3;
          }
          return 0;
        case 1:
        if (amountOfAlive >= 3) {
            return 0;
          }
          return 0;
        case 2:
        if (amountOfAlive >= 3) {
            return 0;
          }
          return 1;
        case 3:
        if (amountOfAlive >= 3) {
            return 0;
          }
          return 2;
        case 4:
        if (amountOfAlive >= 3) {
            return 0;
          }
          return 3;
        case 5:
          if (amountOfAlive >= 3) {
            return 0;
          }
          return 4;
        default:
          Debug.Log("Current cell id has no transition.");
          break;
        }
      // var rand = new Random(); cannot instance Ramdom() (static class)
      // var indice = rand.Next(neighbours.Count);
      return 0;
    }

    // Update is called once per frame
    void Update()
    {
    	secondsPassed -= Time.deltaTime;
        if (secondsPassed <= 0) {
            for (int i = 0; i < instantiated.Count; ++i) {
                Destroy(instantiated[i]);
            }
            var newMatrix = new int[matrixSize, matrixSize];
            for (int i = 0; i < matrix.GetLength(0); ++i) {
                for (int j = 0; j < matrix.GetLength(1); ++j) {
                    newMatrix[i, j] = transitionFunction(matrix[i, j], i, j);

                    var type = stateToGameObject[matrix[i, j]];
                    var renderer = type.GetComponent<Renderer>();
                    var sizeX = renderer.bounds.size.x;
                    var sizeZ = renderer.bounds.size.z;
					          var sizeY = renderer.bounds.size.y;
                    var positionX = transform.position.x + (j * cellSize) + cellSize / 2;
                    var positionY = transform.position.y + (sizeY / 2);
                    var positionZ = transform.position.z + (i * cellSize) + cellSize / 2;
                    var position = new Vector3(positionX, positionY, positionZ);
                    instantiated.Add(
                      Instantiate(type, position, Quaternion.identity)
                    );
                }
            }
            matrix = newMatrix;
            secondsPassed = secondsBetweenUpdate;
        }
    }

	void OnDrawGizmosSelected() {
		var currentPosition = transform.position;
		for (int i = 0; i < matrixSize + 1; ++i) {
			var end = currentPosition + new Vector3(0, 0, cellSize * matrixSize);
			var direction = end - currentPosition;
			Gizmos.DrawRay(currentPosition, direction);
			currentPosition = currentPosition + new Vector3(cellSize, 0, 0);
		}
		currentPosition = transform.position;
		for (int i = 0; i < matrixSize + 1; ++i) {
			var end = currentPosition + new Vector3(matrixSize * cellSize, 0, 0);
			var direction = end - currentPosition;
			Gizmos.DrawRay(currentPosition, direction);
			currentPosition = currentPosition + new Vector3(0, 0, cellSize);
		}
    if (showIdentifier) {
      var ourMatrix = new int[matrixSize, matrixSize];
      for (int i = 0; i < ourMatrix.GetLength(0); ++i) {
                  for (int j = 0; j < ourMatrix.GetLength(1); ++j) {
                      var identifier = (i * matrixSize) + j;

                      var positionX = (j * cellSize) + cellSize / 2;
                      var positionZ = (i * cellSize) + cellSize / 2;
                      var position = new Vector3(positionX, 0, positionZ);
                      //Handles.Label(position, "" + identifier);
                  }
      }
    }
    if (showIJ) {
      var ourMatrix = new int[matrixSize, matrixSize];
      for (int i = 0; i < ourMatrix.GetLength(0); ++i) {
                  for (int j = 0; j < ourMatrix.GetLength(1); ++j) {
                      var positionX = (j * cellSize) + cellSize / 2;
                      var positionZ = (i * cellSize) + cellSize / 2;
                      var position = new Vector3(positionX, 0, positionZ);
                      //Handles.Label(position, "" + i + ", " + j);
                  }
      }
    }
	}

  public int getAmountOfAlive() {
    var amount = 0;
    for (int i = 0; i < matrix.GetLength(0); ++i) {
        for (int j = 0; j < matrix.GetLength(1); ++j) {
          if (matrix[i , j] > 0) {
            amount++;
          }
        }
    }
    return amount;
  }

  public void randomize(int maximumAlive, int weight) {
      while (maximumAlive > 0) {
          int randomI = Random.Range(0, matrix.GetLength(0));
          int randomJ = Random.Range(0, matrix.GetLength(1));
          int randomNumber = Random.Range(1, weight);
          if (randomNumber == 1 && maximumAlive > 0) {
            matrix[randomI, randomJ] = 3;
           --maximumAlive;
          }
      }
  }
}
