using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TechTalk.SpecFlow.RuntimeTests.AssistTests.ExampleEntities
{
    public class CustomPerson
    {
        public string Name { get; set; }
        public PhoneNumber PhoneNumber { get; set; }
    }

    public class PhoneNumber
    {
        public int CountryCode { get; set; }
        public int AreaCode { get; set; }
        public string Phone { get; set; }

        public PhoneNumber()
        {
        }

        public PhoneNumber(int countryCode, int areaCode, string phone)
        {
            CountryCode = countryCode;
            AreaCode = areaCode;
            Phone = phone;
        }

        public override string ToString()
        {
            return string.Format("+{0} {1} {2}", CountryCode, AreaCode, Phone);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (PhoneNumber)) return false;
            return Equals((PhoneNumber) obj);
        }

        public bool Equals(PhoneNumber other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other.CountryCode == CountryCode && other.AreaCode == AreaCode && Equals(other.Phone, Phone);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = CountryCode;
                result = (result*397) ^ AreaCode;
                result = (result*397) ^ (Phone != null ? Phone.GetHashCode() : 0);
                return result;
            }
        }
    }
}
