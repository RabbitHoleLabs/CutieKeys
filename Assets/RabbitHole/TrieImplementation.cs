using System.Collections.Generic;

/* C# Trie implementation courtesy of Arnaldo Pérez Castaño - https://visualstudiomagazine.com/Articles/2015/10/20/Text-Pattern-Search-Trie-Class-NET.aspx?Page=1 */

namespace TrieImplementation
{
    public class Node
    {
        public char Value { get; set; }
        public List<Node> Children { get; set; }
        public Node Parent { get; set; }
        public int Depth { get; set; }
        public int Weight { get; set; }

        public Node(char value, int depth, int weight, Node parent)
        {
            Value = value;
            Children = new List<Node>();
            Depth = depth;
            Weight = weight;
            Parent = parent;
        }

        public bool IsLeaf()
        {
            return Children.Count == 0;
        }

        public Node FindChildNode(char c)
        {
            foreach (Node child in Children)
                if (child.Value == c)
                    return child;

            return null;
        }

        public void DeleteChildNode(char c)
        {
            for (int i = 0; i < Children.Count; i++)
                if (Children[i].Value == c)
                    Children.RemoveAt(i);
        }
    }

    public class Trie
    {
        private readonly Node _root;

        public Trie()
        {
            _root = new Node('^', 0, 0, null);
        }

        public Node Prefix(string s)
        {
            Node currentNode = _root;
            Node result = currentNode;

            foreach (char c in s)
            {
                currentNode = currentNode.FindChildNode(c);
                if (currentNode == null)
                    break;
                result = currentNode;
            }

            return result;
        }

        public bool Search(string s)
        {
            Node prefix = Prefix(s);
            return prefix.Depth == s.Length && prefix.FindChildNode('$') != null;
        }

        public void Insert(string s, int weight)
        {
            Node commonPrefix = Prefix(s);
            Node current = commonPrefix;

            for (int i = current.Depth; i < s.Length; i++)
            {
                Node newNode = new Node(s[i], current.Depth + 1, weight, current);
                current.Children.Add(newNode);
                current = newNode;
            }

            current.Children.Add(new Node('$', current.Depth + 1, weight, current));
        }

        public void Delete(string s)
        {
            if (Search(s))
            {
                Node node = Prefix(s).FindChildNode('$');

                while (node.IsLeaf())
                {
                    Node parent = node.Parent;
                    parent.DeleteChildNode(node.Value);
                    node = parent;
                }
            }
        }

    }


}
