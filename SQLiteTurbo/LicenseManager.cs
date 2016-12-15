using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using Microsoft.Win32;
using Common;

namespace SQLiteTurbo
{
    /// <summary>
    /// This class is responsible to manage all license related information
    /// </summary>
    public class LicenseManager
    {
        static LicenseManager()
        {
            try
            {
                _license = DecodeLicense(Utils.GetLicenseFilePath());
            }
            catch (Exception ex)
            {
                // The license will not be loaded
            } // catch
        }

        public static LicenseDetails License
        {
            get { return _license; }
        }

        public static bool IsLicenseInstalled
        {
            get
            {
                return _license != null;
            }
        }

        public static bool HasValidLicense
        {
            get
            {
                if (_license == null)
                    return false;
                if (_license.Usage == LicenseUsage.Demo)
                    return OkForEvaluation(_license);
                else
                    return true;
            }
        }

        public static bool OkForEvaluation(LicenseDetails license)
        {
            if (license != null && DaysLeftForEvaluation(license) > 0)
                return true;
            else
                return false;
        }

        public static int DaysLeftForEvaluation(LicenseDetails license)
        {
            if (license == null)
                return 0;

            string fpath = Utils.GetInstallationDirectory() + "/conf.dat";
            if (!File.Exists(fpath))
                return 0;

            string before = File.ReadAllText(fpath);

            string dt = DecryptString(before);
            DateTime start;
            if (!DateTime.TryParse(dt, out start))
                return 0;
            TimeSpan delta = DateTime.Now.Subtract(start);
            if (Math.Floor(delta.TotalDays) >= license.DemoDays)
                return 0;
            else
                return (int)(license.DemoDays - Math.Floor(delta.TotalDays));
        }

        /// <summary>
        /// Install the license file 
        /// </summary>
        /// <param name="fpath">The path to the license file to install</param>
        public static void InstallLicense(string fpath)
        {
            LicenseDetails lic = null;
            try
            {
                lic = DecodeLicense(fpath);
            }
            catch(Exception ex)
            {
                throw new ApplicationException("license string is invalid");
            } // catch

            // In case we are dealing with an evaluation license - make sure that we can
            // evaluate
            if (lic.Usage == LicenseUsage.Demo && !OkForEvaluation(lic))
                throw new ApplicationException("evaluation license has expired");

            string lpath = Utils.GetLicenseFilePath();

            if (Path.GetFullPath(lpath) != Path.GetFullPath(fpath))
            {
                // Rename the old license file (if exists)
                if (File.Exists(lpath))
                {
                    int index = 1;
                    while (File.Exists(lpath + "_" + index + ".bak"))
                        index++;
                    File.Move(lpath, lpath + "_" + index + ".bak");
                }

                // Copy the new license file to its location
                File.Copy(fpath, lpath);
            } // if

            // Read the license file
            _license = DecodeLicense(lpath);
        }

        public static LicenseDetails DecodeLicense(string fpath)
        {
            string str = File.ReadAllText(fpath);
            string[] parts = str.Split(new string[] { SIGNATURE_DELIM }, StringSplitOptions.None);
            if (parts.Length != 2)
                throw new ApplicationException("Invalid license file");

            string signature = parts[0];
            string data = parts[1];
            
            // Verify the signature part
            bool ok = Verify(data, signature, _publicKey);
            if (!ok)
                throw new ApplicationException("Invalid license file");

            // If the license passed signature verification - decode the data part.
            string decrypted = DecryptString(data);
            return LicenseDetails.Parse(decrypted);
        }

        public static string GetLicenseUsage(LicenseUsage usage)
        {
            switch (usage)
            {
                case LicenseUsage.Demo:
                    return "Evaluation";
                case LicenseUsage.Academic:
                    return "Academic";
                case LicenseUsage.Personal:
                    return "Personal";
                case LicenseUsage.Commercial:
                    return "Commercial";
                default:
                    return string.Empty;
            } // switch
        }

        public static bool Verify(string data, string signature, string publicKey)
        {
            byte[] dataBytes = Encoding.UTF8.GetBytes(data);
            byte[] signatureBytes = Convert.FromBase64String(signature);
            RSACryptoServiceProvider provider = CreateProviderFromKey(publicKey);
            return provider.VerifyData(dataBytes, "SHA1", signatureBytes);
        }

        private static RSACryptoServiceProvider CreateProviderFromKey(string key)
        {
            RSACryptoServiceProvider provider = new RSACryptoServiceProvider();
            provider.FromXmlString(key);
            return provider;
        }

        /// <summary>
        /// Method which does the encryption using Rijndeal algorithm.This is for decrypting the data
        /// which has orginally being encrypted using the above method
        /// </summary>
        /// <param name="InputText">The encrypted data which has to be decrypted</param>
        /// <param name="Password">The string which has been used for encrypting.The same string
        /// should be used for making the decrypt key</param>
        /// <returns>Decrypted Data</returns>
        private static string DecryptString(string str)
        {
            byte[] buffer = Convert.FromBase64String(str);

            // XOR the string with one time pad buffer to create the un-encrypted string
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] ^= hash[i % hash.Length];
            } // for            

            // Converting to string
            string res = Encoding.Unicode.GetString(buffer);
            return res;
        }

        /// <summary>
        /// Hash bytes array
        /// </summary>
        private static byte[] hash = new byte[] {
            218, 103, 181, 235, 187, 187, 43, 216, 72, 195, 166, 25, 180, 15, 76, 
            109, 104, 243, 222, 146, 193, 215, 47, 97, 154, 252, 100, 48, 223, 
            167, 227, 137, 175, 204, 254, 105, 2, 237, 123, 93, 43, 89, 228, 231, 
            122, 183, 8, 75, 180, 64, 142, 155, 162, 79, 81, 151, 80, 28, 119, 
            219, 190, 214, 235, 84
        };

        private static string SIGNATURE_DELIM = "D+E@+L#+I$+M%";

        private static string EVAL_USAGE_KEY_NAME = @"SOFTWARE\Microsoft\Windows\CurrentVersion";
        private static string EVAL_USAGE_KEY_VALUE = @"SQC_Tags";

        /// <summary>
        /// Used to verify the license file
        /// </summary>
        private static string _publicKey = @"<RSAKeyValue><Modulus>rs14hHhgGCJhuLjBtXAxP74bXMunULZ4I/mAq4EBGrZjPIuiCnm2VsQAvCSn+uHnw0byFtLFDNrL+H8G/F8DMi5oR4cGrzuj9PILTcA+U2Q1FEZdehX7hoFxAhSy4R2/O2nksvFJUh6HGNRYNqJ2ku0RVh0XxNtZlhMaLx0hLFKvp8tfsVtTZBGom7LFAvHQZ6zoNT10OZZlQRFB1gG200OmWNsxfvPcasNwbpNBibQ5wwBvQRTvEncIyU1gIHImJYHPjzMdYiznvzR7DKdnRx+LhV85oyDuUhFnL2JA9dk+ldhEPEjxBolQwFyzb7DdNjFxCijbrNqz19jBKePpVQ==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
        
        private static LicenseDetails _license = null;
    }
}
