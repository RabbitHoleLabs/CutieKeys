using TrieImplementation;
using UnityEngine;
using UnityEngine.UI;

public class LetterCube : MonoBehaviour {

    // these things are children of the LetterNode object gotten by drag/drop reference in the prefab
    public Transform stick;
    public Text nodeText;

    public Node trieNode;

    public void setStickLength(float newLength) { 
        stick.transform.localScale = new Vector3(stick.transform.localScale.x, newLength, stick.transform.localScale.z);
        stick.transform.localPosition = new Vector3(0, -newLength, 0);
    }

    public void assignNode(Node newNode) {
        trieNode = newNode;
        nodeText.text = trieNode.Value.ToString();
    }
}
