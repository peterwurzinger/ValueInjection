namespace ValueInjection
{
    public interface IValueObtainer
    {
        object ObtainValue(object key);
    }

    public interface IValueObtainer<out TLookupType> : IValueObtainer
    {
        new TLookupType ObtainValue(object key);
    }
}
