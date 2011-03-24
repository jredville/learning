
interface IRecord<out T1, out T2>
{
    T1 First { get; }
    T2 Second { get; }
}