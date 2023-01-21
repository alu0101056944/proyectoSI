using UnityEngine;
using UnityEngine.UI;

public class UpdateSurvivolSatiety : MonoBehaviour
{
    private Text textLabel;
    private FoodCollectorAgent[] components;
    
    void Start() {
      textLabel = GetComponent<Text>();
      var survivorAgents = GameObject.FindGameObjectsWithTag("agent");
      components = new FoodCollectorAgent[survivorAgents.Length];
      for (int i = 0; i < survivorAgents.Length; ++i) {
        components[i] = survivorAgents[i].GetComponent<FoodCollectorAgent>();
      }
    }

    void Update() {
      textLabel.text = "";
      for (int i = 0; i < components.Length; ++i) {
        var score = components[i].getScore();
        textLabel.text = textLabel.text + "Survivor " + i + ": " +
            score + "\n";
      }
    }
}
