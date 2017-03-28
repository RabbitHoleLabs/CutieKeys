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
    private LetterCube rootCube;
    private string currInputPrefix;


    // Load trie data from file
    void Start() {
        currInputPrefix = "";
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
    }

    public void updateTrie() {
        clearTrie();
        rootCube = Instantiate(LetterCube, transform.position, transform.rotation, transform).GetComponent<LetterCube>();
        rootCube.assignNode(trie.Prefix(currInputPrefix));
        rootCube.setStickLength(0);
        renderBranches(rootCube, 0);
    }

    private void renderBranches(LetterCube rootCube, int currDepth) {
        // base case
        if (currDepth >= depth || rootCube.trieNode.IsLeaf()) return;
        // scale the cone width down by narrowingFactor at each level
        float currConeWidth = branchConeWidth - (currDepth * narrowingFactor);
        // each branch divides the total available cone width into sectors
        // we've chosen not to allow branches on the edges of this cone, so number of sectors will be branches + 1
        float sectorWidth = currConeWidth / (branchesToDisplay + 1);
        // starting angle is half of the branch cone width from vertical + one sector width
        float currAngle = -(currConeWidth/2f) + sectorWidth;
        for (int i = 0; i < branchesToDisplay && i < rootCube.trieNode.Children.Count; i++) {
            // instantiate and orient new letterCube, parented to current cube
            LetterCube newCube = Instantiate(LetterCube, rootCube.transform.position, rootCube.transform.rotation, rootCube.transform).GetComponent<LetterCube>();
            newCube.transform.Rotate(0f, 0f, currAngle);
            newCube.transform.position += newCube.transform.up.normalized * branchLength*2f; // not entirely sure why we need to multiply by 2 there to get the right distance.
            newCube.setStickLength(branchLength);
            newCube.assignNode(rootCube.trieNode.Children[i]);
            //recurse the newly made cube, then continue with this set of branches
            renderBranches(newCube, currDepth + 1);
            currAngle += sectorWidth;
        }
    }

    private void clearTrie() {
        if (rootCube != null) {
            DestroyImmediate(rootCube.transform.gameObject);
        }
    }

    // Update is called once per frame
    void Update() {
        foreach (char c in Input.inputString) {
            if (c == "\b"[0]) {
                if (currInputPrefix.Length != 0) {
                    currInputPrefix = currInputPrefix.Substring(0, currInputPrefix.Length - 1);
                }
            } else {
                if (c == " "[0]) {
                    currInputPrefix = "";
                } else {
                    currInputPrefix += c;
                }
            }
        }
        updateTrie();
    }
}
