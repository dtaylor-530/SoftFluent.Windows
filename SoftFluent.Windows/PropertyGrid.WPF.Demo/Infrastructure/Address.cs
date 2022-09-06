using System;
using System.ComponentModel;
using System.Text;

namespace SoftFluent.Windows.Samples
{
    [TypeConverter(typeof(AddressConverter))]
    public class Address : AutoObject
    {
        [PropertyGridOptions(SortOrder = 40)]
        public string City
        {
            get => GetProperty<string>();
            set => SetProperty(value);
        }

        [PropertyGridOptions(SortOrder = 60)]
        public string Country
        {
            get => GetProperty<string>();
            set => SetProperty(value);
        }

        [PropertyGridOptions(SortOrder = 10)]
        public string Line1
        {
            get => GetProperty<string>();
            set => SetProperty(value);
        }

        [PropertyGridOptions(SortOrder = 20)]
        public string Line2
        {
            get => GetProperty<string>();
            set => SetProperty(value);
        }

        [PropertyGridOptions(SortOrder = 50)]
        public string State
        {
            get => GetProperty<string>();
            set => SetProperty(value);
        }

        [PropertyGridOptions(SortOrder = 30)]
        public int? ZipCode
        {
            get => GetProperty<int?>();
            set => SetProperty(value);
        }

        // poor man's one line comma separated USA postal address parser
        public static Address Parse(string text)
        {
            Address address = new Address();
            if (text != null)
            {
                string[] split = text.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (split.Length > 0)
                {
                    int zip = 0;
                    int index = -1;
                    string state = null;
                    for (int i = 0; i < split.Length; i++)
                    {
                        if (TryFindStateZip(split[i], out state, out zip))
                        {
                            index = i;
                            break;
                        }
                    }

                    if (index < 0)
                    {
                        address.DistributeOverProperties(split, 0, int.MaxValue, nameof(Line1), nameof(Line2), nameof(City), nameof(Country));
                    }
                    else
                    {
                        address.ZipCode = zip;
                        address.State = state;
                        address.DistributeOverProperties(split, 0, index, nameof(Line1), nameof(Line2), nameof(City));
                        if (string.IsNullOrWhiteSpace(address.City) && address.Line2 != null)
                        {
                            address.City = address.Line2;
                            address.Line2 = null;
                        }
                        address.DistributeOverProperties(split, index + 1, int.MaxValue, nameof(Country));
                    }
                }
            }
            return address;
        }

        public override string ToString()
        {
            const string sep = ", ";
            StringBuilder sb = new StringBuilder();
            AppendJoin(sb, Line1, string.Empty);
            AppendJoin(sb, Line2, sep);
            AppendJoin(sb, City, sep);
            AppendJoin(sb, State, sep);
            if (ZipCode.HasValue)
            {
                AppendJoin(sb, ZipCode.Value.ToString(), " ");
            }
            AppendJoin(sb, Country, sep);
            return sb.ToString();
        }

        protected override bool OnPropertyChanged(string name, bool setChanged, bool forceRaise)
        {
            return base.OnPropertyChanged(name, setChanged, forceRaise);
        }

        private static void AppendJoin(StringBuilder sb, string value, string sep)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return;
            }

            string s = sb.ToString();
            if (!s.EndsWith(" ") && !s.EndsWith(",") && !s.EndsWith(Environment.NewLine))
            {
                sb.Append(sep);
            }
            sb.Append(value);
        }

        private static bool TryFindStateZip(string text, out string state, out int zip)
        {
            zip = 0;
            state = null;
            string zipText = text;
            int pos = text.LastIndexOfAny(new[] { ' ' });
            if (pos >= 0)
            {
                zipText = text.Substring(pos + 1).Trim();
            }

            if (!int.TryParse(zipText, out zip) || zip <= 0)
            {
                return false;
            }

            state = text.Substring(0, pos).Trim();
            return true;
        }

        private void DistributeOverProperties(string[] split, int offset, int max, params string[] properties)
        {
            for (int i = 0; i < properties.Length; i++)
            {
                if ((offset + i) >= split.Length || (offset + i) >= max)
                {
                    return;
                }

                string s = split[offset + i].Trim();
                if (s.Length == 0)
                {
                    continue;
                }

                SetProperty(properties[i], (object)s);
            }
        }
    }
}