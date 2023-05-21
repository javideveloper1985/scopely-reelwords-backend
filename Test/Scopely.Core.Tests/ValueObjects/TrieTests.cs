using Scopely.Core.Tests.Mocks;

namespace Scopely.Core.Tests.ValueObjects;

public class TrieTests
{
    [Fact]
    public void Insert_WhenInsertEmptyWord_ShouldNotGenerateNodes()
    {
        var trie = new TrieMock();

        trie.Insert(string.Empty);

        Assert.Equal(0, trie.GetRootNode().GetTotalChildNodesCount());
    }

    [Fact]
    public void Insert_WhenInsertNonEmptyWord_ShouldGenerateNodes()
    {
        var trie = new TrieMock();

        trie.Insert("cat");

        Assert.Equal(3, trie.GetRootNode().GetTotalChildNodesCount());
    }

    [Fact]
    public void Insert_WhenInsertSimilarWords_ShouldReuseNodes()
    {
        var word1 = "up";
        var word2 = "upload";
        var expectedNumberOfChildNodes = 6; //Longest word

        var trie = new TrieMock();
        trie.Insert(word1);
        trie.Insert(word2);

        var currentNode = trie.GetRootNode();

        Assert.Equal(expectedNumberOfChildNodes, currentNode.GetTotalChildNodesCount());
    }

    [Fact]
    public void Search_WhenWordsExist_ShouldReturnTrue()
    {
        var word = "hello";
        var trie = new TrieMock();
        trie.Insert(word);
        Assert.True(trie.Search(word));
    }

    [Fact]
    public void Search_WhenWordDoesNotExist_ShouldReturnFalse()
    {
        var trie = new TrieMock();
        trie.Insert("hello");
        Assert.False(trie.Search("goodbye"));
    }

    [Fact]
    public void Delete_WhenWordIsEmpty_ShouldReturnFalse()
    {
        var trie = new TrieMock();
        trie.Insert("hello");
        Assert.False(trie.Delete(string.Empty));
    }

    [Fact]
    public void Delete_WhenIsTheLastWord_ShouldReturnTrueAndRemoveAllNodes()
    {
        var trie = new TrieMock();
        trie.Insert("car");

        var hasBeenDeleted = trie.Delete("car");

        Assert.True(hasBeenDeleted);
        Assert.Equal(0, trie.GetRootNode().GetTotalChildNodesCount());
        Assert.False(trie.Search("car"));
    }

    [Fact]
    public void Delete_WhenWordExistsAndThereAreSimilarWords_ShouldReturnTrueAndRemoveNodes()
    {
        var trie = new TrieMock();
        trie.Insert("car");
        trie.Insert("can");

        var hasBeenDeleted = trie.Delete("car");

        Assert.True(hasBeenDeleted);
        Assert.Equal(3, trie.GetRootNode().GetTotalChildNodesCount());
        Assert.False(trie.Search("car"));
        Assert.True(trie.Search("can"));
    }

    [Fact]
    public void Delete_WhenWordExistsAndThereIsOneSmallerSimilarWord_ShouldReturnTrueAndDontRemoveNodes()
    {
        var trie = new TrieMock();
        trie.Insert("car");
        trie.Insert("cartoon");

        var hasBeenDeleted = trie.Delete("cartoon");

        Assert.True(hasBeenDeleted);
        Assert.Equal(3, trie.GetRootNode().GetTotalChildNodesCount());
        Assert.True(trie.Search("car"));
        Assert.False(trie.Search("cartoon"));
    }

    [Fact]
    public void Delete_WhenWordExistsAndThereIsOneLongerSimilarWord_ShouldReturnTrueAndDontRemoveNodes()
    {
        var trie = new TrieMock();
        trie.Insert("car");
        trie.Insert("cartoon");

        var hasBeenDeleted = trie.Delete("car");

        Assert.True(hasBeenDeleted);
        Assert.Equal(7, trie.GetRootNode().GetTotalChildNodesCount());
        Assert.False(trie.Search("car"));
        Assert.True(trie.Search("cartoon"));
    }

    [Fact]
    public void Delete_WhenWordDoesNotExist_ShouldReturnFalse()
    {
        var trie = new TrieMock();
        trie.Insert("car");
        Assert.False(trie.Delete("plane"));
    }
}