using System;
using System.Linq;

namespace Interpreter.Values
{
    class DumpFunction : ICallable
    {
        public object Call(params object[] args)
        {
            Console.WriteLine(string.Join(" ", args.Select(ValueToString)));
            return this;
        }

        public override string ToString()
        {
            return "dump";
        }

        static string ValueToString(object value)
        {
            if (value == null)
            {
                return "null";
            }

            if (value is bool)
            {
                return (bool)value ? "true" : "false";
            }

            if (value is int)
            {
                return ((int)value).ToString();
            }

            if (value is ICallable)
            {
                return value.ToString();
            }

            throw new Exception($"Неизвестный тип значения {value}");
        }
    }
}