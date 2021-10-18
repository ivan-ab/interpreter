using System;
using System.Collections.Generic;
using System.IO;
using Interpreter.Lexer;
using Interpreter.Nodes;

namespace Interpreter
{
    class Program
    {
        static ProgramNode Parse(string code)
        {
            var tokens = Tokenizer.GetTokens(code);
            var parser = new Parser(tokens);
            var programNode = parser.ParseProgram();
            return programNode;
        }

        static ProgramNode CheckedParse(string code)
        {
            var programNode = Parse(code);
            var code2 = programNode.FormattedString;
            var programNode2 = Parse(code2);
            var code3 = programNode2.FormattedString;
            if (code2 != code3)
            {
                Console.WriteLine(code2);
                Console.WriteLine(code3);
                throw new Exception("Кривой парсер или ToString у узлов");
            }

            return programNode;
        }

        static void Run(ProgramNode node, Dictionary<string, object> variables)
        {
            new Interpreter(variables).RunProgram(node);
        }

        static void Main(string[] args)
        {
            string code = File.ReadAllText(@"..\..\code.txt");
            var programCode = CheckedParse(code);
            var variables = new Dictionary<string, object>
            {
                { "x", 2 },
                { "y", 10 },
                { "z", 4 }
            };
            Run(programCode, variables);
        }
    }
}