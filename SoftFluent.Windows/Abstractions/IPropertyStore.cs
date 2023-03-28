namespace Abstractions
{

    public interface IKey : IEquatable<IKey>
    {

    }

    public interface IObserver : IKey
    {
        public void OnNext(IPropertyChange propertyResult);
    }

    public interface IPropertyChange
    {
        //public IEnumerable<IPropertyResult> Results { get; set; }
        public string Name { get; }
        public object Value { get; }
    }

    //public interface IPropertyResult
    //{
    //    public Guid Guid { get; }
    //}

    //public interface IError : IPropertyResult
    //{
    //    public string Description { get; }
    //}

    //public interface IValidation
    //{
    //    public bool IsValid { get; }

    //    public string Description { get; }
    //}

    public interface IPropertyStore
    {
        public T GetValue<T>(IKey key);
        public void SetValue<T>(IKey key, T value);

        public object GetValue(IKey key, Type type);
        public void SetValue(IKey key, object value, Type type);

        public Task<Guid> GetGuidByParent(Guid guid, string Name, Type type);

        IDisposable Subscribe(IObserver observer);
        string Validate(string memberName);
    }
}
