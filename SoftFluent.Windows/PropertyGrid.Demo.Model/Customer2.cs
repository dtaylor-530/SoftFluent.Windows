using Abstractions;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace SoftFluent.Windows.Samples
{
    public class Customer2 : Customer
    {
        private ObservableCollection<Address> addresses;

        public Customer2() : base()
        {
            addresses = new ObservableCollection<Address> { new Address { Line1 = "2018 156th Avenue NE", City = "Bellevue", State = "WA", ZipCode = 98007, Country = "USA" } };
        }

        public int IntegerValue { get; set; }

        [PropertyGridOptions(EditorDataTemplateResourceKey = "AddressListEditor", SortOrder = 10)]
        [DisplayName("Addresses (custom editor)")]
        [Category("Collections")]
        public ObservableCollection<Address> Addresses
        {
            get => addresses;
        }
    }
}