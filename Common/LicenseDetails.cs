using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    /// <summary>
    /// This class holds the license information
    /// </summary>
    [Serializable]
    public class LicenseDetails
    {
        public int DemoDays
        {
            get
            {
                return _demoDays;
            }
            set
            {
                _demoDays = value;
            }
        }

        public string CustomerName
        {
            get { return _customerName; }
            set { _customerName = value; }
        }

        public string ContactEmail
        {
            get { return _email; }
            set { _email = value; }
        }

        public int NumberOfSeats
        {
            get { return _numberOfSeats; }
            set { _numberOfSeats = value; }
        }

        public LicenseUsage Usage
        {
            get { return _usage; }
            set { _usage = value; }
        }

        public bool IsSiteLicense
        {
            get { return _siteLicense; }
            set { _siteLicense = value; }
        }

        public override string ToString()
        {
            return _demoDays+DELIM+_customerName + DELIM + _usage.ToString() + DELIM + _numberOfSeats.ToString() + DELIM + _siteLicense.ToString()+DELIM+_email;
        }

        public static LicenseDetails Parse(string str)
        {
            string[] parts = str.Split(new string[] { "!+-!" }, StringSplitOptions.None);
            if (parts.Length != 6)
                throw new ArgumentException("illegal license string");

            LicenseDetails res = new LicenseDetails();
            res._demoDays = int.Parse(parts[0]);
            res._customerName = parts[1];
            res._usage = (LicenseUsage)Enum.Parse(typeof(LicenseUsage), parts[2]);
            res._numberOfSeats = int.Parse(parts[3]);
            res._siteLicense = bool.Parse(parts[4]);
            res._email = parts[5];
            return res;
        }

        private const string DELIM = "!+-!";

        private bool _siteLicense;
        private LicenseUsage _usage;
        private int _numberOfSeats;
        private string _customerName;
        private int _demoDays;
        private string _email;
    }

    public enum LicenseUsage
    {
        Demo = 0,

        Academic = 1,

        Personal = 2,

        Commercial = 3,
    }
}
