namespace Scopely.Core.Structures;

public class CharNode
{
    private readonly Dictionary<char, CharNode> _childNodes = new();

    public bool IsEndOfWord { get; private set; }

    public int ChildNodesCount => _childNodes.Count;

    public CharNode? FindChild(char @char)
        => _childNodes.ContainsKey(char.ToLower(@char))
            ? _childNodes[char.ToLower(@char)]
            : null;

    public CharNode CreateChild(char @char)
    {
        _childNodes.Add(char.ToLower(@char), new CharNode());
        return _childNodes[char.ToLower(@char)];
    }

    public void SetIsEndOfWord(bool isLast)
        => IsEndOfWord = isLast;

    public void RemoveChildNode(char @char)
        => _childNodes.Remove(char.ToLower(@char));

    public int GetTotalChildNodesCount()
    {
        var count = _childNodes.Count;
        foreach (var node in _childNodes)
            count += node.Value.GetTotalChildNodesCount();
        return count;
    }
}
