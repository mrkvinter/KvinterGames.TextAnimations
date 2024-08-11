using System.Collections.Generic;
using TMPro;

namespace KvintessentialGames.TextAnimations
{
    public class AnimationTagTextPreprocessor : ITextPreprocessor
    {
        private readonly Dictionary<string, (int, int)> animationTags = new();

        public IReadOnlyDictionary<string, (int, int)> AnimationTags => animationTags;

        public string PreprocessText(string text)
        {
            var parts = text.Split('<', '>');
            var textLength = 0;
            animationTags.Clear();
            var stackInfo = new Stack<(string, int)>();

            for (var i = 0; i < parts.Length; i++)
            {
                var value = parts[i];
                if (i % 2 == 0)
                {
                    textLength += value.Length;
                }
                else if (!IsLinkTag(value.Replace(" ", "")))
                {
                    if (value == "br")
                    {
                        textLength++;
                    }
                }
                else
                {
                    if (value.StartsWith("/"))
                    {
                        var (id, start) = stackInfo.Pop();
                        animationTags[id] = (start, textLength);
                    }
                    else
                    {
                        var split = value.Split('=');
                        if (split.Length < 2)
                        {
                            continue;
                        }
                        var id = split[1].Trim();
                        stackInfo.Push((id, textLength));
                    }
                }
            }

            while (stackInfo.Count > 0)
            {
                var (id, start) = stackInfo.Pop();
                animationTags[id] = (start, textLength);
            }

            return text;
        }

        private bool IsLinkTag(string text)
        {
            return text.StartsWith("link") || text.StartsWith("/link");
        }
    }
}