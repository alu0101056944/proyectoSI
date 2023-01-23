using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleLogic : MonoBehaviour
{
    private Collider boxCollider;

    [SerializeField]
    private float secondsUntilActivate;
    private float currentSeconds;

    private Renderer meshRenderer;

    [SerializeField]
    private Material materialBefore;
    [SerializeField]
    private Material materialAfter;

    private bool changed;

    // Start is called before the first frame update
    void Start()
    {
        boxCollider = GetComponent<Collider>();
        boxCollider.enabled = false;
        meshRenderer = GetComponent<Renderer>();
        meshRenderer.material = materialBefore;
        changed = false;
        currentSeconds = secondsUntilActivate;
    }

    // Update is called once per frame
    void Update()
    {
        currentSeconds -= Time.deltaTime;
        if (!changed && currentSeconds <= 0) {
          meshRenderer.material = materialAfter;
          boxCollider.enabled = true;
          currentSeconds = secondsUntilActivate;
          changed = true;
        }
    }
}
