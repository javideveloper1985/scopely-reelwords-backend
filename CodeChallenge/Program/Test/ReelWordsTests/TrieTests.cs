using ReelWords;
using Xunit;

namespace ReelWordsTests
{
    public class TrieTests
    {
        private const string TEST_WORD = "parallel";

        [Fact]
        public void TrieInsertTest()
        {
            Trie trie = new Trie();
            trie.Insert(TEST_WORD);
            Assert.True(trie.Search(TEST_WORD));
        }

        [Fact]
        public void TrieDeleteTest()
        {
            Trie trie = new Trie();
            trie.Insert(TEST_WORD);
            Assert.True(trie.Search(TEST_WORD));
            trie.Delete(TEST_WORD);
            Assert.False(trie.Search(TEST_WORD));
        }
    }
}