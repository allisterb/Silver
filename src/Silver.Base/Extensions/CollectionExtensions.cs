namespace Silver;
public static class CollectionExtensions
{
    public static T? PeekIfNotEmpty<T>(this Stack<T> q) => q.Count > 0 ? q.Peek() : default(T);

    public static T? PopIfNotEmpty<T>(this Stack<T> q) => q.Count > 0 ? q.Pop() : default(T);

    public static void Pop<T>(this Stack<T> stack, int n)
    {
        for (int i = 1; i <= n; i++)
        {
            stack.Pop();
        }
    }

}

public static class CollectionUtils
{
    public static string JoinWithSpaces(this IEnumerable<string> s) => s.Aggregate((a, b) => a + " " + b);
}
