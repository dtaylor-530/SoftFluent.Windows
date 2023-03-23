using Abstractions;
using Jellyfish;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Security;

namespace SoftFluent.Windows.Samples
{
    public class Customer : ViewModel, IGuid
    {
        private readonly string[] arrayOfStrings;
        private byte[] byteArray1;
        private DateTime creationDateAndTime;
        private DateTime dateOfBirth;
        private DaysOfWeek daysOfWeek;
        private string description;
        private string firstName;
        private Gender gender;
        private string lastName;
        private List<string> listOfStrings;
        private string multiEnumString;
        private string multiEnumStringWithDisplay;
        private string notBrowsable;
        private SecureString password;
        private int? nullableInt32;
        private double percentageOfSatisfaction;
        private int percentageOfSatisfactionInt;
        private Point point;
        private string preferredColorName;
        //private FontFamily preferredFont;
        private bool sampleBoolean;
        private bool sampleBooleanDropDownList;
        private bool? sampleNullableBoolean;
        private bool? sampleNullableBooleanDropDownList;
        private Status status;
        private Address subObject;
        private TimeSpan timeSpan;
        private string webSite;

        public Customer()
        {

            ListOfStrings = new List<string>
            {
                "string 1",
                "string 2"
            };

            //ArrayOfStrings = ListOfStrings.ToArray();
            CreationDateAndTime = DateTime.Now;
            Description = "press button to edit...";
            ByteArray1 = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };
            WebSite = "http://www.softfluent.com";
            Status = Status.Valid;
         
            DaysOfWeek = DaysOfWeek.WeekDays;
            PercentageOfSatisfaction = 50;
            PreferredColorName = "DodgerBlue";
            //PreferredFont = Fonts.SystemFontFamilies.FirstOrDefault(f => string.Equals(f.Source, "Consolas", StringComparison.OrdinalIgnoreCase));
            SampleNullableBooleanDropDownList = false;
            SampleBooleanDropDownList = true;
            MultiEnumString = "First, Second";
            SubObject = Address.Parse("1600 Amphitheatre Pkwy, Mountain View, CA 94043, USA");
        }

        public Guid Guid => Guid.Parse("901f3c6d-1424-4771-8672-0b77d7c44342");





        [Category("Collections")]
        public string[] ArrayOfStrings
        {
            get => arrayOfStrings;

        }

        [PropertyGridOptions(EditorDataTemplateResourceKey = "FormatTextEditor")]
        //[PropertyGrid(Name = "Format", Value = "0x{0}")]
        [ReadOnly(true)]
        [DisplayName("Byte Array (hex format)")]
        public byte[] ByteArray1
        {
            get => byteArray1;
            set => this.Set(ref byteArray1, value);
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
            get => creationDateAndTime;
            set => this.Set(ref creationDateAndTime, value);
        }

        [Category("Dates and Times")]
        [PropertyGridOptions(SortOrder = 40)]
        public DateTime DateOfBirth
        {
            get => dateOfBirth;
            set => this.Set(ref dateOfBirth, value);
        }

        [DisplayName("Days Of Week (multi-valued enum)")]
        [Category("Enums")]
        public DaysOfWeek DaysOfWeek
        {
            get => daysOfWeek;
            set => this.Set(ref daysOfWeek, value);
        }

        [DisplayName("Description (multi-line)")]
        //[PropertyGrid(Name = "Foreground", Value = "White")]
        //[PropertyGrid(Name = "Background", Value = "Black")]
        [PropertyGridOptions(EditorDataTemplateResourceKey = "BigTextEditor")]
        public string Description
        {
            get => description;
            set => this.Set(ref description, value);
        }

        [PropertyGridOptions(SortOrder = 10)]
        public string FirstName
        {
            get => firstName;
            set => this.Set(ref firstName, value);
        }

        [Category("Enums")]
        [PropertyGridOptions(SortOrder = 30)]
        public Gender Gender
        {
            get => gender;
            set => this.Set(ref gender, value);
        }


        [PropertyGridOptions(SortOrder = 20)]
        public string LastName
        {
            get => lastName;
            set => this.Set(ref lastName, value);
        }

        [Category("Collections")]
        public List<string> ListOfStrings
        {
            get => listOfStrings;
            set => this.Set(ref listOfStrings, value);
        }

        //[PropertyGridOptions(IsEnum = true, IsFlagsEnum = true, EnumNames = new string[] { "First", "Second", "Third" })]
        [Category("Enums")]
        public string MultiEnumString
        {
            get => multiEnumString;
            set => this.Set(ref multiEnumString, value);
        }

        //[PropertyGridOptions(IsEnum = true, IsFlagsEnum = true, EnumNames = new string[] { "None", "My First", "My Second", "My Third" }, EnumValues = new object[] { 0, 1, 2, 4 })]
        [Category("Enums")]
        public string MultiEnumStringWithDisplay
        {
            get => multiEnumStringWithDisplay;
            set => this.Set(ref multiEnumStringWithDisplay, value);
        }

        [Browsable(false)]
        public string NotBrowsable
        {
            get => notBrowsable;
            set => this.Set(ref notBrowsable, value);
        }

        [DisplayName("Nullable Int32 (supports empty string)")]
        public int? NullableInt32
        {
            get => nullableInt32;
            set => this.Set(ref nullableInt32, value);
        }

        [Category("Security")]
        [PropertyGridOptions(EditorDataTemplateResourceKey = "PasswordEditor")]
        [DisplayName("Password (SecureString)")]
        public SecureString Password
        {
            get => password;
            set
            {
                this.Set(ref password, value);
            }
        }

        //[Category("Security")]
        //[DisplayName("Password (String)")]
        //public string PasswordString
        //{
        //    get
        //    {
        //        if (Password == null)
        //        {
        //            return null;
        //        }

        //        return Password.ConvertToUnsecureString();
        //    }
        //}

        [PropertyGridOptions(EditorDataTemplateResourceKey = "PercentEditor")]
        [DisplayName("Percentage Of Satisfaction (double)")]
        public double PercentageOfSatisfaction
        {
            get => percentageOfSatisfaction;
            set
            {
                this.Set(ref percentageOfSatisfaction, value);
            }
        }

        [PropertyGridOptions(EditorDataTemplateResourceKey = "PercentEditor")]
        [DisplayName("Percentage Of Satisfaction (int)")]
        public int PercentageOfSatisfactionInt
        {
            get => percentageOfSatisfactionInt;
            set => this.Set(ref percentageOfSatisfactionInt, value);
        }

        [DisplayName("Point (auto type converter)")]
        public Point Point
        {
            get => point;
            set => this.Set(ref point, value);
        }

        [PropertyGridOptions(EditorDataTemplateResourceKey = "ColorEditor")]
        [DisplayName("Preferred Color Name (custom editor)")]
        public string PreferredColorName
        {
            get => preferredColorName;
            set => this.Set(ref preferredColorName, value);
        }

        //[PropertyGridOptions(EditorDataTemplateResourceKey = "FontEditor")]
        //[DisplayName("Preferred Font (custom editor)")]
        //public FontFamily PreferredFont
        //{
        //    get => preferredFont;
        //    set => this.Set(ref preferredFont, value);
        //}

        [DisplayName("Boolean (Checkbox)")]
        [Category("Booleans")]
        public bool SampleBoolean
        {
            get => sampleBoolean;
            set => this.Set(ref sampleBoolean, value);
        }

        [DisplayName("Boolean (DropDownList)")]
        [Category("Booleans")]
        [PropertyGridOptions(EditorDataTemplateResourceKey = "BooleanDropDownListEditor")]
        public bool SampleBooleanDropDownList
        {
            get => sampleBooleanDropDownList;
            set => this.Set(ref sampleBooleanDropDownList, value);
        }

        [DisplayName("Boolean (Checkbox three states)")]
        [Category("Booleans")]
        public bool? SampleNullableBoolean
        {
            get => sampleNullableBoolean;
            set => this.Set(ref sampleNullableBoolean, value);
        }

        [DisplayName("Boolean (DropDownList 3 states)")]
        [Category("Booleans")]
        [PropertyGridOptions(EditorDataTemplateResourceKey = "NullableBooleanDropDownListEditor")]
        public bool? SampleNullableBooleanDropDownList
        {
            get => sampleNullableBooleanDropDownList;
            set => this.Set(ref sampleNullableBooleanDropDownList, value);
        }

        [Category("Enums")]
        public Status Status
        {
            get => status;
            set
            {
                this.Set(ref status, value);
            }
        }

        [PropertyGridOptions(EditorDataTemplateResourceKey = "ColorEnumEditor")]
        [DisplayName("Status (colored enum)")]
        [ReadOnly(true)]
        [Category("Enums")]
        public Status StatusColor
        {
            get => Status;
            set => Status = value;
        }

        //       [PropertyGridOptions(IsEnum = true, EnumNames = new string[] { "1N\\/AL1D", "\\/AL1D", "UNKN0WN" }, EnumValues = new object[] { Status.Invalid, Status.Valid, Status.Unknown })]
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
            get => subObject;
            set => this.Set(ref subObject, value);
        }

        //[DisplayName("Sub Object (Address as Object)")]
        //[PropertyGridOptions(EditorDataTemplateResourceKey = "ObjectEditor", ForcePropertyChanged = true)]
        //public Address SubObjectAsObject
        //{
        //    get => SubObject;
        //    set => SubObject = value;
        //}

        [Category("Dates and Times")]
        public TimeSpan TimeSpan
        {
            get => timeSpan;
            set => this.Set(ref timeSpan, value);
        }

        [PropertyGridOptions(EditorDataTemplateResourceKey = "CustomEditor", SortOrder = -10)]
        [DisplayName("Web Site (custom sort order)")]
        public string WebSite
        {
            get => webSite;
            set => this.Set(ref webSite, value);
        }

        private void OnSubObjectPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //OnPropertyChanged(nameof(SubObject), false, true);
            //OnPropertyChanged(nameof(SubObjectAsObject), false, true);
        }
    }
}