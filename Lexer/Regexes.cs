using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Interpreter.Lexer
{
    class Regexes : IEnumerable
    {
        static Regexes CreateInstance()
        {
            var regexes = new Regexes()
            {
                { TokenType.Whitespaces, @"\s+" },
                { TokenType.SingleLineComment, @"//[^\r\n]*" },
                { TokenType.MultiLineComment, @"/\*.*?\*/" },
                { TokenType.Identifier, @"[\w_-[\d]][\w_]*" },
                { TokenType.NumberLiteral, @"\d+" },
                { TokenType.OperatorOrPunctuator, @"==|!=|<=|>=|\|\||&&|[-+*/%?:!<>,=;(){}[\]]" },
            };
            regexes.Build();
            return regexes;
        }

        List<Tuple<TokenType, string>> tokenPatterns = new List<Tuple<TokenType, string>>();
        Dictionary<TokenType, Regex> tokenRegexes = new Dictionary<TokenType, Regex>();
        Regex combinedRegex;
        IEnumerable<Tuple<TokenType, string>> tokenGroupNames;
        static Regexes instance;
        public static Regexes Instance => instance ?? (instance = CreateInstance());
        public Regex CombinedRegex => combinedRegex;
        public IEnumerable<Tuple<TokenType, string>> TokenGroupNames => tokenGroupNames;
        static readonly Regex emptyRegex = MakeRegex(@"\A\z");

        public Regex GetTokenRegex(TokenType type)
        {
            Regex regex;
            if (tokenRegexes.TryGetValue(type, out regex))
            {
                return regex;
            }

            return emptyRegex;
        }

        void Add(TokenType type, string pattern)
        {
            var regex = MakeRegex(@"\A(" + pattern + @")\z");
            tokenRegexes.Add(type, regex);
            tokenPatterns.Add(Tuple.Create(type, pattern));
        }

        void Build()
        {
            var pattern = string.Join("|", tokenPatterns.Select(x => $"(?<{x.Item1}>{x.Item2})"));
            combinedRegex = MakeRegex(pattern);
            tokenGroupNames = tokenPatterns.Select(x => Tuple.Create(x.Item1, x.Item1.ToString())).ToArray();
        }

        static Regex MakeRegex(string pattern)
        {
            return new Regex(pattern,
                RegexOptions.Compiled |
                RegexOptions.CultureInvariant |
                RegexOptions.ExplicitCapture |
                RegexOptions.IgnorePatternWhitespace |
                RegexOptions.Singleline |
                RegexOptions.Multiline |
                RegexOptions.None
            );
        }

        public IEnumerator GetEnumerator()
        {
            throw new NotSupportedException();
        }
    }
}