using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Interpreter.Lexer
{
    class Tokenizer
    {
        public static IEnumerable<Token> GetTokens(string text)
        {
            var regex = Regexes.Instance.CombinedRegex;
            var groupNames = Regexes.Instance.TokenGroupNames;
            int lastPos = 0;
            var matches = regex.Matches(text);
            foreach (Match m in matches)
            {
                if (lastPos < m.Index)
                {
                    throw new Exception($"Пропустили {text.Substring(lastPos, m.Index - lastPos)}");
                }

                bool found = false;
                foreach (var kv in groupNames)
                {
                    if (m.Groups[kv.Item2].Success)
                    {
                        if (found)
                        {
                            throw new Exception("Кривая регулярка нашла несколько вхождений");
                        }

                        found = true;
                        yield return new Token(kv.Item1, m.Value);
                    }
                }

                if (!found)
                {
                    throw new Exception("Кривая регулярка");
                }

                lastPos = m.Index + m.Length;
            }

            if (lastPos < text.Length)
            {
                throw new Exception($"Пропустили {text.Substring(lastPos)}");
            }
        }
    }
}