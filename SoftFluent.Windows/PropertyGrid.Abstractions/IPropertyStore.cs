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
        public IKey Key { get; }
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
        public void GetValue(IKey key);
        public void SetValue(IKey key, object value);

        public Task<Guid> GetGuidByParent(IKey key);

        IDisposable Subscribe(IObserver observer);
        string Validate(string memberName);
    }
}
