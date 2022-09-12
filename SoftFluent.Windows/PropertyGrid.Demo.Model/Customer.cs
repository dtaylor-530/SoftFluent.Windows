using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Security;
//using System.Windows.Media;
using Utilities;

namespace SoftFluent.Windows.Samples
{
    public class Customer : AutoObject
    {
        public Customer()
        {
            Id = Guid.NewGuid();
            ListOfStrings = new List<string>
            {
                "string 1",
                "string 2"
            };

            ArrayOfStrings = ListOfStrings.ToArray();
            CreationDateAndTime = DateTime.Now;
            Description = "press button to edit...";
            ByteArray1 = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };
            WebSite = "http://www.softfluent.com";
            Status = Status.Valid;
            Addresses = new ObservableCollection<Address> { new Address { Line1 = "2018 156th Avenue NE", City = "Bellevue", State = "WA", ZipCode = 98007, Country = "USA" } };
            DaysOfWeek = DaysOfWeek.WeekDays;
            PercentageOfSatisfaction = 50;
            PreferredColorName = "DodgerBlue";
           // PreferredFont = Fonts.SystemFontFamilies.FirstOrDefault(f => string.Equals(f.Source, "Consolas", StringComparison.OrdinalIgnoreCase));
            SampleNullableBooleanDropDownList = false;
            SampleBooleanDropDownList = true;
            MultiEnumString = "First, Second";
            SubObject = Address.Parse("1600 Amphitheatre Pkwy, Mountain View, CA 94043, USA");
        }

        [PropertyGridOptions(EditorDataTemplateResourceKey = "AddressListEditor", SortOrder = 10)]
        [DisplayName("Addresses (custom editor)")]
        [Category("Collections")]
        public ObservableCollection<Address> Addresses
        {
            get => GetProperty<ObservableCollection<Address>>();
            set => SetProperty(value);
        }

        [Category("Collections")]
        public string[] ArrayOfStrings
        {
            get => GetProperty<string[]>();
            set => SetProperty(value);
        }

        [PropertyGridOptions(EditorDataTemplateResourceKey = "FormatTextEditor")]
        [PropertyGrid(Name = "Format", Value = "0x{0}")]
        [ReadOnly(true)]
        [DisplayName("Byte Array (hex format)")]
        public byte[] ByteArray1
        {
            get => GetProperty<byte[]>();
            set => SetProperty(value);
        }

        [ReadOnly(true)]
        [DisplayName("Byte Array (press button for hex dump)")]
        public byte[] ByteArray2
        {
            get => ByteArray1;
            set => ByteArray1 = value;
        }

        //[ReadOnly(true)]
        [Category("Dates and Times")]
        [PropertyGridOptions(EditorDataTemplateResourceKey = "DateTimePicker")]
        public DateTime CreationDateAndTime
        {
            get => GetProperty<DateTime>();
            set => SetProperty(value);
        }

        [Category("Dates and Times")]
        [PropertyGridOptions(SortOrder = 40)]
        public DateTime DateOfBirth
        {
            get => GetProperty<DateTime>();
            set => SetProperty(value);
        }

        [DisplayName("Days Of Week (multi-valued enum)")]
        [Category("Enums")]
        public DaysOfWeek DaysOfWeek
        {
            get => GetProperty<DaysOfWeek>();
            set => SetProperty(value);
        }

        [DisplayName("Description (multi-line)")]
        [PropertyGrid(Name = "Foreground", Value = "White")]
        [PropertyGrid(Name = "Background", Value = "Black")]
        [PropertyGridOptions(EditorDataTemplateResourceKey = "BigTextEditor")]
        public string Description
        {
            get => GetProperty<string>();
            set => SetProperty(value);
        }

        [PropertyGridOptions(SortOrder = 10)]
        public string FirstName
        {
            get => GetProperty<string>();
            set => SetProperty(value);
        }

        [Category("Enums")]
        [PropertyGridOptions(SortOrder = 30)]
        public Gender Gender
        {
            get => GetProperty<Gender>();
            set => SetProperty(value);
        }

        [DisplayName("Guid (see menu on right-click)")]
        public Guid Id
        {
            get => GetProperty<Guid>();
            set => SetProperty(value);
        }

        [PropertyGridOptions(SortOrder = 20)]
        public string LastName
        {
            get => GetProperty<string>();
            set => SetProperty(value);
        }

        [Category("Collections")]
        public List<string> ListOfStrings
        {
            get => GetProperty<List<string>>();
            set => SetProperty(value);
        }

        [PropertyGridOptions(IsEnum = true, IsFlagsEnum = true, EnumNames = new string[] { "First", "Second", "Third" })]
        [Category("Enums")]
        public string MultiEnumString
        {
            get => GetProperty<string>();
            set => SetProperty(value);
        }

        [PropertyGridOptions(IsEnum = true, IsFlagsEnum = true, EnumNames = new string[] { "None", "My First", "My Second", "My Third" }, EnumValues = new object[] { 0, 1, 2, 4 })]
        [Category("Enums")]
        public string MultiEnumStringWithDisplay
        {
            get => GetProperty<string>();
            set => SetProperty(value);
        }

        [Browsable(false)]
        public string NotBrowsable
        {
            get => GetProperty<string>();
            set => SetProperty(value);
        }

        [DisplayName("Nullable Int32 (supports empty string)")]
        public int? NullableInt32
        {
            get => GetProperty<int?>();
            set => SetProperty(value);
        }

        [Category("Security")]
        [PropertyGridOptions(EditorDataTemplateResourceKey = "PasswordEditor")]
        [DisplayName("Password (SecureString)")]
        public SecureString Password
        {
            get => GetProperty<SecureString>();
            set
            {
                if (SetProperty(value))
                {
                    OnPropertyChanged(nameof(PasswordString));
                }
            }
        }

        [Category("Security")]
        [DisplayName("Password (String)")]
        public string PasswordString
        {
            get
            {
                if (Password == null)
                {
                    return null;
                }

                return Password.ConvertToUnsecureString();
            }
        }

        [PropertyGridOptions(EditorDataTemplateResourceKey = "PercentEditor")]
        [DisplayName("Percentage Of Satisfaction (double)")]
        public double PercentageOfSatisfaction
        {
            get => GetProperty<double>();
            set
            {
                if (SetProperty(value))
                {
                    OnPropertyChanged(nameof(PercentageOfSatisfactionInt));
                }
            }
        }

        [PropertyGridOptions(EditorDataTemplateResourceKey = "PercentEditor")]
        [DisplayName("Percentage Of Satisfaction (int)")]
        public int PercentageOfSatisfactionInt
        {
            get => GetProperty<int>(0, "PercentageOfSatisfaction");
            set => SetProperty("PercentageOfSatisfaction", value);
        }

        [DisplayName("Point (auto type converter)")]
        public Point Point
        {
            get => GetProperty<Point>();
            set => SetProperty(value);
        }

        [PropertyGridOptions(EditorDataTemplateResourceKey = "ColorEditor")]
        [DisplayName("Preferred Color Name (custom editor)")]
        public string PreferredColorName
        {
            get => GetProperty<string>();
            set => SetProperty(value);
        }

        //[PropertyGridOptions(EditorDataTemplateResourceKey = "FontEditor")]
        //[DisplayName("Preferred Font (custom editor)")]
        //public FontFamily PreferredFont
        //{
        //    get => GetProperty<FontFamily>();
        //    set => SetProperty(value);
        //}

        [DisplayName("Boolean (Checkbox)")]
        [Category("Booleans")]
        public bool SampleBoolean
        {
            get => GetProperty<bool>();
            set => SetProperty(value);
        }

        [DisplayName("Boolean (DropDownList)")]
        [Category("Booleans")]
        [PropertyGridOptions(EditorDataTemplateResourceKey = "BooleanDropDownListEditor")]
        public bool SampleBooleanDropDownList
        {
            get => GetProperty<bool>();
            set => SetProperty(value);
        }

        [DisplayName("Boolean (Checkbox three states)")]
        [Category("Booleans")]
        public bool? SampleNullableBoolean
        {
            get => GetProperty<bool?>();
            set => SetProperty(value);
        }

        [DisplayName("Boolean (DropDownList 3 states)")]
        [Category("Booleans")]
        [PropertyGridOptions(EditorDataTemplateResourceKey = "NullableBooleanDropDownListEditor")]
        public bool? SampleNullableBooleanDropDownList
        {
            get => GetProperty<bool?>();
            set => SetProperty(value);
        }

        [Category("Enums")]
        public Status Status
        {
            get => GetProperty<Status>();
            set
            {
                if (SetProperty(value))
                {
                    OnPropertyChanged(nameof(StatusColor));
                    OnPropertyChanged(nameof(StatusColorString));
                }
            }
        }

        [PropertyGridOptions(EditorDataTemplateResourceKey = "ColorEnumEditor", PropertyType = typeof(PropertyGridEnumProperty))]
        [DisplayName("Status (colored enum)")]
        [ReadOnly(true)]
        [Category("Enums")]
        public Status StatusColor
        {
            get => Status;
            set => Status = value;
        }

        [PropertyGridOptions(IsEnum = true, EnumNames = new string[] { "1N\\/AL1D", "\\/AL1D", "UNKN0WN" }, EnumValues = new object[] { Status.Invalid, Status.Valid, Status.Unknown })]
        [DisplayName("Status (enum as string list)")]
        [Category("Enums")]
        public string StatusColorString
        {
            get => Status.ToString();
            set => Status = (Status)Enum.Parse(typeof(Status), value);
        }

        [DisplayName("Sub Object (Address)")]
        [PropertyGridOptions(ForcePropertyChanged = true)]
        public Address SubObject
        {
            get => GetProperty<Address>();
            set
            {
                // because it's a sub object we want to update the property grid
                // when inner properties change
                Address so = SubObject;
                if (so != null)
                {
                    so.PropertyChanged -= OnSubObjectPropertyChanged;
                }

                if (SetProperty(value))
                {
                    so = SubObject;
                    if (so != null)
                    {
                        so.PropertyChanged += OnSubObjectPropertyChanged;
                    }

                    // these two properties are coupled
                    OnPropertyChanged(nameof(SubObjectAsObject));
                }
            }
        }

        [DisplayName("Sub Object (Address as Object)")]
        [PropertyGridOptions(EditorDataTemplateResourceKey = "ObjectEditor", ForcePropertyChanged = true)]
        public Address SubObjectAsObject
        {
            get => SubObject;
            set => SubObject = value;
        }

        [Category("Dates and Times")]
        public TimeSpan TimeSpan
        {
            get => GetProperty<TimeSpan>();
            set => SetProperty(value);
        }

        [PropertyGridOptions(EditorDataTemplateResourceKey = "CustomEditor", SortOrder = -10)]
        [DisplayName("Web Site (custom sort order)")]
        public string WebSite
        {
            get => GetProperty<string>();
            set => SetProperty(value);
        }

        private void OnSubObjectPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(SubObject), false, true);
            OnPropertyChanged(nameof(SubObjectAsObject), false, true);
        }
    }
}