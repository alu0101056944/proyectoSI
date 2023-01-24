using UnityEngine;
using UnityEngine.UI;

public class UpdatePredatorSatiety : MonoBehaviour
{
    private Text textLabel;
    private PredatorAgent[] components;
    
    void Start() {
      textLabel = GetComponent<Text>();
      var predatorAgents = GameObject.FindGameObjectsWithTag("agentPredator");
      components = new PredatorAgent[predatorAgents.Length];
      for (int i = 0; i < predatorAgents.Length; ++i) {
        components[i] = predatorAgents[i].GetComponent<PredatorAgent>();
      }
    }

    void Update() {
      textLabel.text = "";
      for (int i = 0; i < components.Length; ++i) {
        var score = components[i].getScore();
        textLabel.text = textLabel.text + "Predator " + i + ": " +
            score + "\n";
      }
    }
}
