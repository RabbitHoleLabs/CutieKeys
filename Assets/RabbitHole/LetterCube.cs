using TrieImplementation;
using UnityEngine;
using UnityEngine.UI;

public class LetterCube : MonoBehaviour {

    // these things are children of the LetterNode object gotten by drag/drop reference in the prefab
    public Transform stick;
    public Text cubeText;
    public Node trieNode;
    public bool isTruncated;

    public void setStickLength(float newLength) { 
        stick.transform.localScale = new Vector3(stick.transform.localScale.x, newLength, stick.transform.localScale.z);
        stick.transform.localPosition = new Vector3(0, -newLength, 0);
    }

    public void makeInvisible() {
        setStickLength(0);
        transform.GetComponent<MeshRenderer>().enabled = false;
        transform.Find("Label").gameObject.SetActive(false);
    }

    public void assignNode(Node newNode) {
        trieNode = newNode;
        cubeText.text = trieNode.Value.ToString();
    }
}
