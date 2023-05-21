using ReelWords.Domain.ValueObjects;
using Xunit;

namespace ReelWords.Domain.Tests.ValueObjects;

public class ReelPanelTests
{
    public static IEnumerable<object[]> GetReelsForScrollTest()
    {
        return new List<object[]>()
        {
            new object[]
            {
                new char[4] { 'u', 'm', 'b', 'n' }, //row1
                new char[4] { 'y', 'a', 'y', 'w' }, //row2
                new char[4] { 'u', 'a', 'y', 'w' }, //expectedRow1
                new char[4] { 'y', 'm', 'b', 'n' }, //expectedRow2
                "yaw" //word
            },
            new object[]
            {
                new char[4] { 'a', 'm', 'c', 'a' }, //row1
                new char[4] { 'e', 'w', 'r', 'e' }, //row2
                new char[4] { 'e', 'w', 'c', 'e' }, //expectedRow1
                new char[4] { 'a', 'm', 'r', 'a' }, //expectedRow2
                "wee" //word
            }
        };
    }

    [Theory]
    [MemberData(nameof(GetReelsForScrollTest))]
    public void ScrollLetters_WhenWordMatchesWithReelCharacters_ScrollLastColumns(
        char[] row1, char[] row2,
        char[] expectedRow1, char[] expectedRow2,
        string word)
    {
        var reelPanel = ReelPanel.CreateEmpty(2, 4);
        reelPanel.AddReel(0, row1);
        reelPanel.AddReel(1, row2);

        reelPanel.ScrollLetters(word);

        Assert.Equal(expectedRow1, reelPanel.GetReelByRow(0));
        Assert.Equal(expectedRow2, reelPanel.GetReelByRow(1));
    }
}
