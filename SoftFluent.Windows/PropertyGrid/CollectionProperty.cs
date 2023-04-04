using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using Abstractions;
using Extensions = Utilities.Extensions;

namespace SoftFluent.Windows
{
    public class CollectionProperty : PropertyBase
    {
        public CollectionProperty(Guid guid) : base(guid)
        {         
        }

        public int Index { get; set; }

        public override string Name => Index.ToString();
        public string DisplayName => Name;
        public override bool IsReadOnly => true;

        public virtual string? Category => "Collection-Item";            

        public virtual string Description { get => GetProperty<string>(); set => SetProperty(value); }

        public override object? Value
        {
            get => Data;
            set
            {
                throw new Exception("g 4sdffsd");
            }
        }

        public string TemplateKey { get => GetProperty<string>(); set => SetProperty(value); }


        public string EditorTemplateKey { get => GetProperty<string>(); set => SetProperty(value); }

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