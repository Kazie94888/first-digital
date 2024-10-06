namespace SmartCoinOS.Domain.Exceptions;

public class NullEntityException<T> : Exception
{
    public NullEntityException()
        : base($"{typeof(T).Name} was expected to be not null but was found to be null.")
    {
    }

    public static T Throw()
    {
        throw new NullEntityException<T>();
    }
}