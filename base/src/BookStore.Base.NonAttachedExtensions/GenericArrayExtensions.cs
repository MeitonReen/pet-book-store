namespace BookStore.Base.NonAttachedExtensions;

public static class GenericArrayExtensions
{
    public static void Deconstruct<T>(this T[] targetArray, out T tupleItem0, out T tupleItem1)
    {
        tupleItem0 = targetArray[0];
        tupleItem1 = targetArray[1];
    }
}