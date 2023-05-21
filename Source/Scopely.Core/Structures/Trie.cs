namespace Scopely.Core.Structures;

public class Trie
{
    private readonly CharNode _rootNode = new();

    public static Trie CreateFromListOfWords(List<string> words)
    {
        var trie = new Trie();
        words.ForEach(word => trie.Insert(word.ToLower()));
        return trie;
    }

    public void Insert(string word)
    {
        if (string.IsNullOrEmpty(word))
            return;

        var currentNode = _rootNode;
        for (int i = 0; i < word.Length; i++)
        {
            //We create new nodes except when a new node already exists
            var childNode = currentNode.FindChild(word[i]);
            if (childNode is null)
                currentNode = currentNode.CreateChild(word[i]);
            else
                currentNode = childNode;
        }

        //Finally, since we are inserting we always mark the last letter as 'end of word'
        currentNode.SetIsEndOfWord(true);
    }

    public bool Search(string? word)
    {
        //TODO: Unit test this branch
        if (string.IsNullOrEmpty(word))
            return false;

        var currentNode = _rootNode;
        foreach (var @char in word)
        {
            currentNode = currentNode.FindChild(@char);
            if (currentNode == null)
                return false;
        }
        return currentNode.IsEndOfWord;
    }

    public bool Delete(string word)
    {
        if (string.IsNullOrEmpty(word))
            return false;

        var currentNode = _rootNode;
        var charNodes = new List<CharNode>() { _rootNode };
        var idxLastKeyNode = 0;
        foreach (var @char in word)
        {
            //Store the index of the last key node because we may not need to remove the whole word
            if (currentNode.ChildNodesCount > 1 || currentNode.IsEndOfWord)
                idxLastKeyNode = charNodes.Count - 1;

            currentNode = currentNode.FindChild(@char);
            if (currentNode == null)
                return false;

            charNodes.Add(currentNode);
        }

        //Mark last node as not end of word
        currentNode.SetIsEndOfWord(false);

        //If last char node has not child nodes, we can remove the following node to the last key node
        if (currentNode.ChildNodesCount == 0)
            charNodes[idxLastKeyNode].RemoveChildNode(word[idxLastKeyNode]);

        return true;
    }

    protected CharNode GetRoot() => _rootNode;
}