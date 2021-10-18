using System;

namespace Interpreter.Lexer
{
    class Token
    {
        public readonly TokenType Type;
        public readonly string Lexeme;

        public Token(TokenType type, string lexeme)
        {
            var regex = Regexes.Instance.GetTokenRegex(type);
            if (!regex.IsMatch(lexeme))
            {
                throw new Exception($"Лексема {lexeme} не подходит под регулярку {regex}");
            }

            Type = type;
            Lexeme = lexeme;
        }

        public override string ToString() => $"{Type} \"{Lexeme}\"";
    }
}