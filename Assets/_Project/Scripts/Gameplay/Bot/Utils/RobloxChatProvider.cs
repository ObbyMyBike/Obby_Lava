using System.Collections.Generic;
using UnityEngine;

public static class RobloxChatProvider
{
    private static readonly string[] allPhrases = new string[]
    {
        "OMG!", "Noob!", "LOL", "Help me!", "GG", "BRB", "Haha", "WTF?", "Nice!", "Oof!",
        "I’m rich!", "Stop!", "Wait!", "LOL XD", "Nice game!", "What?", "No way!", "Try harder", "Wow!", "Yay!",
        "Let’s go!", "I got this!", "Hurry!", "Epic!", "Fail!", "Haha lol", "Come on!", "So close!", "What the?", "Lol noob",
        "GGWP", "Amazing!", "Lolol", "Help!", "Run!", "Cool!", "Thanks!", "Bruh", "Yikes!", "Yes!",
        "Nooooo!", "OMG LOL", "Haha nice", "Stop it!", "I win!", "Lmao", "Nice try", "Wow lol", "What a noob", "Bruh moment",
        "I’m stuck!", "Wait up!", "You got this!", "Epic win!", "Lol brb", "Too funny", "Haha wtf", "GG noob", "Omg lol", "Yayyy!",
        "No way lol", "Lol epic", "Help pls", "Wow amazing", "Stop trolling", "Nice play", "Lol bruh", "Haha omg", "GG EZ", "Try again",
        "I lost!", "Wow lol", "Haha nice try", "OMG wtf", "Run noob", "Lol wow", "Epic fail", "Haha bruh", "GG haha", "Lol gg",
        "Wow nice", "Stop noob", "Haha ez", "Come on lol", "OMG stop", "Lol what", "No way haha", "Haha omg lol", "GG noobs", "Wow bruh",
        "Haha nice!", "Lol brb", "Epic lol", "GG EZ noob", "Stop trolling lol", "Haha amazing", "OMG epic", "Lol nice!", "Yay GG", "Haha wow"
    };

    private static List<string> shuffledPhrases = new List<string>(allPhrases);
    private static int currentIndex = 0;

    public static string GetNextPhrase()
    {
        if (shuffledPhrases.Count == 0)
            return null;

        if (currentIndex >= shuffledPhrases.Count)
        {
            Shuffle();
            currentIndex = 0;
        }

        string nextPhrase = shuffledPhrases[currentIndex];
        currentIndex++;
        return nextPhrase;
    }

    private static void Shuffle()
    {
        for (int i = shuffledPhrases.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            var temp = shuffledPhrases[i];
            shuffledPhrases[i] = shuffledPhrases[j];
            shuffledPhrases[j] = temp;
        }
    }
}
