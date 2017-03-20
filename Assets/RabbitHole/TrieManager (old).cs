using System;
using UnityEngine;
using UnityEngine.UI;
using TrieImplementation;
using System.IO;

public class TrieManager : MonoBehaviour {

    public string wordList;
    private Trie trie;
    private string currInputPrefix;
    private Node currNode;

    public GameObject lprimary;
    public GameObject lsecondary;
    public GameObject rprimary;
    public GameObject rsecondary;


    // Use this for initialization
    void Start () {

        StreamReader stream = new StreamReader(wordList);
        trie = new Trie();

        while (!stream.EndOfStream)
        {
            string[] wordAndFreq = new string[2];
            wordAndFreq = stream.ReadLine().Split(',');
            /*
            for (int i = 0; i < word.Length; i++)
            {
                Debug.Log(word[i]);
            }
            */

            int wordWeight = 0;
            try
            {
                //convert weight string from file into int
                wordWeight = Int32.Parse(wordAndFreq[0]);
            }
            catch (FormatException e)
            {
                Debug.Log(e.Message);
            }
            trie.Insert(wordAndFreq[1], wordWeight);
        }
        Debug.Log("testing with th");
        Node prefix = trie.Prefix("th");
        Debug.Log(prefix.Weight);
        Debug.Log(prefix.Depth);
        foreach (var child in prefix.Children) {
            Debug.Log("child " + child.Value + " weight " + child.Weight);
        }
        /*
        string currInput;
        string currWord = "";
        while ((currInput = Console.ReadLine()) != "")
        {
            if (currInput == " ")
            {
                currWord = "";
            }
            else
            {
                currWord += currInput;
                Debug.Log(currWord);
                Node prefix = trie.Prefix(currWord);
                Debug.Log(prefix.Weight);
                Debug.Log(prefix.Depth);
                foreach (var child in prefix.Children)
                {
                    Debug.Log("child " + child.Value + " weight " + child.Weight);
                }
            }
        }
        Console.Read();
        */
    }

    public void updateTrie(string letter) {
        if (letter == " ") {
            currInputPrefix = "";
        }
        else {
            currInputPrefix += letter;
        }
        currNode = trie.Prefix(currInputPrefix);
        rprimary.GetComponent<Text>().text = currNode.Children[0].Value.ToString();
        rsecondary.GetComponent<Text>().text = currNode.Children[0].Children[0].Value.ToString();
        lprimary.GetComponent<Text>().text = currNode.Children[1].Value.ToString();
        lsecondary.GetComponent<Text>().text = currNode.Children[1].Children[0].Value.ToString();
        Debug.Log("current prefix: " + currInputPrefix);
        Debug.Log("right primary: " + currNode.Children[0].Value + ", secondary: " + currNode.Children[0].Children[0].Value);
        Debug.Log("left primary: " + currNode.Children[1].Value + ", secondary: " + currNode.Children[1].Children[0].Value);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
