using System.Windows.Controls;
using System.IO;

namespace ATPWork.MyApp.ViewModel.PlombEditorVm
{

    public class ValidNumber : ValidationRule
    {
        public override ValidationResult Validate
     (object value, System.Globalization.CultureInfo cultureInfo)
        {
            var rt = value.ToString().Length;
            if ((value.ToString().Length < 4)) return new ValidationResult(false, "Введите номер  лицевого счета");
            return ValidationResult.ValidResult;
        }
    }

    public class ValidPlombNumber : ValidationRule
    {
        public override ValidationResult Validate
     (object value, System.Globalization.CultureInfo cultureInfo)
        {
            if (value.ToString().IndexOfAny(Path.GetInvalidFileNameChars()) > 0) return new ValidationResult(false, "Недопустимые символы в номере");
            return ValidationResult.ValidResult;
        }
    }
}
