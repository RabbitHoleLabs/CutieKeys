using System;
using UnityEngine;
using UnityEngine.UI;
using TrieImplementation;
using System.IO;

public class VisualTrie : MonoBehaviour {

    public string wordList;
    public Transform LetterCube;
    public Text displayText;

    // VisualTrie display modifiers
    public int branchesToDisplay;
    public int depth;
    public float branchConeWidth;
    public float branchLength;
    public float narrowingFactor;

    private Trie trie;
    private LetterCube rootCube;
    private bool movingRoot;


    // Load trie data from file
    void Awake() {
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
        
        movingRoot = false;
        reloadTrie();
        selectLetters(transform.GetChild(0).GetChild(2).GetChild(2));
    }

    private void clearTrie() {
        if (rootCube != null) {
            Destroy(rootCube.transform.gameObject);
        }
    }

    public void reloadTrie() {
        clearTrie();
        rootCube = Instantiate(LetterCube, transform.position, transform.rotation, transform).GetComponent<LetterCube>();
        string wordInProgress = displayText.text.Substring(displayText.text.LastIndexOf(" ") + 1);
        //if the word being typed isn't found in the trie, trie.Prefix returns the root of the trie, aka the most common starting characters
        rootCube.assignTrieNode(trie.Prefix(wordInProgress));
        rootCube.makeInvisible();
        renderBranches(rootCube, 0);
    }

    public void selectLetters(Transform selectedCube) {
        Transform currentCube = selectedCube;
        string selection = "";
        while (currentCube.parent.name == "LetterCube(Clone)") {
            selection = selection.Insert(0, currentCube.GetComponent<LetterCube>().trieNode.Value.ToString());
            currentCube = currentCube.parent;
        }
        displayText.text += selection;
        reloadTrie();
    }

    /*
    public void selectLetter(int selection) {
        // get letterCube from selection number
        LetterCube selectedCube = rootCube.transform.GetChild(selection + 2).GetComponent<LetterCube>(); // +2 to get past stick and label
        //add the selected letter to what's typed out
        displayText.text += selectedCube.trieNode.Value;
        if (selectedCube.trieNode.Value == ' ') {
            typeLetter(" ");
            return;
        }
        currInputPrefix += selectedCube.trieNode.Value;
        // de-parent selected node from rootcube and parent it to master visual trie object
        selectedCube.transform.SetParent(transform);
        // next delete current root, this removes anything not in selected branch
        DestroyImmediate(rootCube.transform.gameObject);
        //set up new root cube and start moving it towards where the root cube should be
        rootCube = selectedCube;
        rootCube.makeInvisible();
        movingRoot = true;
        //extend any truncated branchesat the end of the new remaining branch
        GameObject[] truncatedBranches = GameObject.FindGameObjectsWithTag("truncatedBranch");
        foreach (GameObject branch in truncatedBranches) {
            renderBranches(branch.transform.GetComponent<LetterCube>(), depth - 1);
            branch.tag = "Untagged";
        }

    }
    */

    private void renderBranches(LetterCube rootCube, int currDepth) {
        // base case
        if (currDepth >= depth) {
            if (!rootCube.trieNode.IsLeaf()) {
                rootCube.tag = "truncatedBranch";
            }
            return;
        }
        // scale the cone width down by narrowingFactor at each level
        float currConeWidth = branchConeWidth - (currDepth * narrowingFactor);
        // each branch divides the total available cone width into sectors
        // we've chosen not to allow branches on the edges of this cone, so number of sectors will be branches + 1
        float sectorWidth = currConeWidth / (branchesToDisplay + 1);
        // starting angle is half of the branch cone width from vertical + one sector width
        float currAngle = (currConeWidth / 2f) - sectorWidth;
        for (int i = 0; i < branchesToDisplay && i < rootCube.trieNode.Children.Count; i++) {
            // instantiate new letterCube parented to current cube
            LetterCube newCube = Instantiate(LetterCube, rootCube.transform.position, rootCube.transform.rotation, rootCube.transform).GetComponent<LetterCube>();
            //rotate it then move it "up" relative to it's rotation, away from the root cube
            newCube.transform.Rotate(0f, 0f, currAngle);
            // distance moved also needs to be scaled relative to the size of this visualTrie and the requested branch length. The 2... makes it work. I think because of the way cylinder length is measured?
            newCube.transform.position += newCube.transform.up.normalized * transform.lossyScale.y * branchLength * 2f;
            newCube.setStickLength(branchLength);
            newCube.assignTrieNode(rootCube.trieNode.Children[i]);
            //recurse on the newly made cube, then continue with this set of branches
            renderBranches(newCube, currDepth + 1);
            currAngle -= sectorWidth;
        }
    }

    private void FixedUpdate() {
        //TODO: make this a coroutine
        if (movingRoot) {
            Vector3 prevPosition = rootCube.transform.position;
            Quaternion prevRotation = rootCube.transform.rotation;
            rootCube.transform.position = Vector3.Lerp(rootCube.transform.position, transform.position, 8f * Time.deltaTime);
            rootCube.transform.rotation = Quaternion.Slerp(rootCube.transform.rotation, transform.rotation, 10f * Time.deltaTime);
            if (rootCube.transform.position == prevPosition && rootCube.transform.rotation == prevRotation) {
                //movingRoot = false;
            }

            if (rootCube.transform.position == transform.position && rootCube.transform.rotation == transform.rotation) {
                //movingRoot = false;
            }
        }
    }

    private void Update() {
        //reloadTrie();
    }
}
