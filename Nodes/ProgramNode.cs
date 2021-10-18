using System.Collections.Generic;
using System.Linq;

namespace Interpreter.Nodes
{
    class ProgramNode : INode
    {
        public readonly IReadOnlyList<IStatement> Statements;

        public ProgramNode(IReadOnlyList<IStatement> statements)
        {
            Statements = statements;
        }

        public string FormattedString => string.Join("", Statements.Select(x => x.FormattedString));
    }
}