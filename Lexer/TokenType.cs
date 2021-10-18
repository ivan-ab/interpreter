namespace Interpreter.Lexer
{
    enum TokenType
    {
        Whitespaces,
        SingleLineComment,
        MultiLineComment,
        Identifier,
        NumberLiteral,
        OperatorOrPunctuator,
        EOF
    }
}