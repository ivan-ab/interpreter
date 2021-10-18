namespace Interpreter.Values
{
    interface ICallable
    {
        object Call(params object[] args);
    }
}