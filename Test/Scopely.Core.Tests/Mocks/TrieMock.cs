using Scopely.Core.Structures;

namespace Scopely.Core.Tests.Mocks;

public class TrieMock : Trie
{
    public CharNode GetRootNode() => GetRoot();
}