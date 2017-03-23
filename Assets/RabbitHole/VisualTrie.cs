using System;
using UnityEngine;
using UnityEngine.UI;
using TrieImplementation;
using System.IO;

public class VisualTrie : MonoBehaviour {

    public string wordList;
    public Transform LetterCube;

    public int branchesToDisplay;
    public int depth;

    public float branchConeWidth;
    public float branchLength;
    public float narrowingFactor;

    private Trie trie;
    private string currInputPrefix;
    private Node rootNode;

    // Load trie data from file
    void Start() {

        trie = new Trie();
        StreamReader stream = new StreamReader(wordList);

        while (!stream.EndOfStream) {
            string[] wordAndFreq = new string[2];
            //file formatted like: "frequency,word\n" on each line
            wordAndFreq = stream.ReadLine().Split(',');
            int wordWeight = 0;
            try {
                //convert weight string from file into int
                wordWeight = Int32.Parse(wordAndFreq[0]);
            }
            catch (FormatException e) {
                Debug.Log(e.Message);
            }
            trie.Insert(wordAndFreq[1], wordWeight);
        }

        /* DEBUG
        Debug.Log("testing with th");
        Node prefix = trie.Prefix("th");
        Debug.Log(prefix.Weight);
        Debug.Log(prefix.Depth);
        foreach (var child in prefix.Children) {
            Debug.Log("child " + child.Value + " weight " + child.Weight);
        }
        */

        currInputPrefix = "";
        rootNode = trie.Prefix("");

        updateTrie("t");
    }

    public void updateTrie(string letter) {
        if (letter == " ") {
            currInputPrefix = "";
        }
        else {
            currInputPrefix += letter;
        }
        rootNode = trie.Prefix(currInputPrefix);
        renderTrie();
    }

    private void renderTrie() {
        Transform rootCube = Instantiate(LetterCube, transform.position, transform.rotation, transform);
        rootCube.GetComponentInChildren<Text>().text = currInputPrefix[currInputPrefix.Length-1].ToString();
        renderBranches(rootCube, rootNode, 0);
    }

    private void renderBranches(Transform currRootCube, Node currTrieNode, int currDepth) {
        // base case
        if (currDepth >= depth) return;
        // each branch divides the total branch cone width into sectors
        // we've chosen not to allow branches on the edges of the branch cone, so number of sectors will be branches + 1
        float currConeWidth = branchConeWidth - (currDepth * narrowingFactor);
        float sectorAngle = currConeWidth / (branchesToDisplay + 1);
        float currAngle = -(currConeWidth/2f) + sectorAngle; // starting angle is half of the branch cone width from vertical + one sector
        for (int i = 0; i < branchesToDisplay && i < currTrieNode.Children.Count; i++) {
            Quaternion angle = Quaternion.Euler(0f,0f,currAngle);
            // instantiate at the same position, but at an angle
            Transform currBranch = Instantiate(LetterCube, currRootCube.transform.position, currRootCube.transform.rotation * angle, transform);
            // next move the new cube in the right direction scaled by branchLength
            currBranch.position += angle * currRootCube.transform.up.normalized * (branchLength);
            currBranch.GetComponentInChildren<Text>().text = currTrieNode.Children[i].Value.ToString();
            if (!currTrieNode.Children[i].IsLeaf()) {
                renderBranches(currBranch, currTrieNode.Children[i], currDepth + 1);
            }
            currAngle += sectorAngle;
        }
    }

    // Update is called once per frame
    void Update() {

    }
}
