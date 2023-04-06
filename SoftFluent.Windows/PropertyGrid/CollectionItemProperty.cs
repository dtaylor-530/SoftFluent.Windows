namespace SoftFluent.Windows
{
    public class CollectionItemProperty : PropertyBase
    {
        public CollectionItemProperty(Guid guid) : base(guid)
        {         
        }

        public int Index { get; set; }

        public override string Name => Index.ToString();
        public string DisplayName => Name;
        public override bool IsReadOnly => true;

        public virtual string? Category => "Collection-Item";            

        public override object? Value
        {
            get => Data;
            set
            {
                throw new Exception("g 4sdffsd");
            }
        }

        public bool IsValueType => PropertyType.IsValueType;

        public bool IsFlagsEnum => throw new NotImplementedException();



        public override string ToString()
        {
            return Name;
        }

        protected virtual bool TryChangeType(object value, Type type, IFormatProvider provider, out object changedValue)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            return ConversionHelper.TryChangeType(value, type, provider, out changedValue);
        }
    }
}