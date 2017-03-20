using System;
using UnityEngine;
using UnityEngine.UI;
using TrieImplementation;
using System.IO;

public class VisualTrie : MonoBehaviour {

    public string wordList;
    public Transform LetterCube;

    public int branchesToDisplay;
    public int branchConeWidth;
    public int depth;
    public float branchLength;
    

    private Trie trie;
    private string currInputPrefix;
    private Node currNode;

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

        // DEBUG
        Debug.Log("testing with th");
        Node prefix = trie.Prefix("th");
        Debug.Log(prefix.Weight);
        Debug.Log(prefix.Depth);
        foreach (var child in prefix.Children) {
            Debug.Log("child " + child.Value + " weight " + child.Weight);
        }

        currNode = trie.Prefix("");
        renderTrie();
    }

    public void updateTrie(string letter) {
        if (letter == " ") {
            currInputPrefix = "";
        }
        else {
            currInputPrefix += letter;
        }
        currNode = trie.Prefix(currInputPrefix);
        renderTrie();
    }

    private void renderTrie() {
        Transform rootCube = Instantiate(LetterCube, transform.position, transform.rotation, transform);
        renderBranches(rootCube, 1);
    }

    private void renderBranches(Transform currRoot, int currDepth) {
        if (currDepth > depth) return;
        /*
        int sectorAngle = branchConeWidth/(branchesToDisplay + 1);
        int currAngle = sectorAngle;
        for (int i = 0; i < branchesToDisplay; i++) {
            //make cube at 60 degrees offset from root + currAngle
            Instantiate(LetterCube, currRoot.transform.position + new Vector3(, currRoot.transform.rotation, transform);
        }
        */

        //TODO: add rotation onto spawned branch cubes so that they actually branch away from eachother, this will fix current overlap
        Transform branch1 = Instantiate(LetterCube, currRoot.transform.position + new Vector3(0.05f, 0f, branchLength), currRoot.transform.rotation, transform);
        renderBranches(branch1, currDepth + 1);
        Transform branch2 = Instantiate(LetterCube, currRoot.transform.position + new Vector3(-0.05f, 0f, branchLength), currRoot.transform.rotation, transform);
        renderBranches(branch2, currDepth + 1);
    }

    // Update is called once per frame
    void Update() {

    }
}
