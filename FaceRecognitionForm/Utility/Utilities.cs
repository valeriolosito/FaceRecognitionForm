using System.Drawing;
using System.Text.RegularExpressions;

namespace FaceRecognitionForm.Utility
{
    public class Utilities
    {
        public static bool isValidTaxCode(string taxCode){
            bool isValid = false;
            taxCode = taxCode.ToUpper();
            if(taxCode.Length == 16)
            {
                isValid = Regex.IsMatch(taxCode, @"^[A-Z]{6}\d{2}[A-Z]\d{2}[A-Z]\d{3}[A-Z]");
            }
            return isValid;
        }

        public static Image GetCopyImage(string path)
        {
            using (Image im = Image.FromFile(path))
            {
                Bitmap bm = new Bitmap(im);
                return bm;
            }
        }

        public static bool isValidTelephone(string telephoneNumber)
        {
            telephoneNumber = telephoneNumber.Trim().Replace(" ", "");
            return Regex.IsMatch(telephoneNumber, @"^\d{10}$");
        }

        public static bool containsOnlyLetters(string text)
        {
            text = text.Trim().Replace(" ", "").ToUpper();          
            return Regex.IsMatch(text, @"^[A-Z]{2}[A-Z]+$");
        }

        /// <summary>
        /// AddressNumber deve avere almeno un numero,
        /// non deve iniziare con 0 e deve avere al max una lettera alla fine
        /// </summary>
        /// <param name="addressNumber"></param>
        /// <returns></returns>
        public static bool isValidAddressNumber(string addressNumber)
        {
            addressNumber = addressNumber.Trim().Replace(" ", "");
            return Regex.IsMatch(addressNumber, @"^[1-9]{1}[0-9]*[a-zA-Z]?$");
        }

        public static bool isValidEmail(string email)
        {
            return Regex.IsMatch(email, @"^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])(\.[a-zA-Z0-9]([a-zA-Z0-9-]{0,61}[a-zA-Z0-9]))+$");
        }
    }
}
