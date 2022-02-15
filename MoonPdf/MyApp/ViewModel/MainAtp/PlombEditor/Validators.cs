using System.Windows.Controls;

namespace ATPWork.MyApp.ViewModel.PlombEditor
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
}
