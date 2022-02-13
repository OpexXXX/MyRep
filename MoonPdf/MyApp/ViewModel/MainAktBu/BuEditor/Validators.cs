using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ATPWork.MyApp.ViewModel.MainAktBu.BuEditor
{
    public class ValidNumber : ValidationRule
    {
        public override ValidationResult Validate
     (object value, System.Globalization.CultureInfo cultureInfo)
        {

            if (value == null || (value.ToString() == "0")) return new ValidationResult(false, "Укажите номер проверки");


            return ValidationResult.ValidResult;
        }
    }
    public class ValidFileName : ValidationRule
    {
        public override ValidationResult Validate
     (object value, System.Globalization.CultureInfo cultureInfo)
        {
            if (value.ToString().IndexOfAny(Path.GetInvalidFileNameChars()) > 0) return new ValidationResult(false, "Недопустимые символы в имени файла");
            return ValidationResult.ValidResult;
        }
    }
    public class ValidNumberLS : ValidationRule
    {
        public override ValidationResult Validate
     (object value, System.Globalization.CultureInfo cultureInfo)
        {
            float res;
            bool isInt = float.TryParse(value.ToString(), out res);
            if (!isInt)
            {
                return new ValidationResult(false, "Лицевой счет должен состоять из чисел");
            }

            var rt = value.ToString().Length;
            if ((value.ToString().Length < 10)) return new ValidationResult(false, "Введите не менее 10 символов для поиска");

            return ValidationResult.ValidResult;
        }
    }
    public class ValidOldNumberPU : ValidationRule
    {
        public override ValidationResult Validate
     (object value, System.Globalization.CultureInfo cultureInfo)
        {
            var rt = value.ToString().Length;
            if ((value.ToString().Length < 5)) return new ValidationResult(false, "Введите не менее 4х символов для поиска");
            return ValidationResult.ValidResult;
        }
    }
    public class ValidPokazanie : ValidationRule
    {
        public override ValidationResult Validate
     (object value, System.Globalization.CultureInfo cultureInfo)
        {
            int res;
            bool isInt = Int32.TryParse(value.ToString(), out res);
            if (!isInt)
            {
                return new ValidationResult(false, "Показание должено состоять из чисел");
            }

            return ValidationResult.ValidResult;
        }
    }
    public class ValidKvartal : ValidationRule
    {
        public override ValidationResult Validate
     (object value, System.Globalization.CultureInfo cultureInfo)
        {
            int res;
            bool isInt = Int32.TryParse(value.ToString(), out res);
            if (!isInt)
            {
                return new ValidationResult(false, "Квартал должен состоять из чисел");
            }


            if (res > 4 || res < 1) return new ValidationResult(false, "Введите число от 1 до 4х");

            return ValidationResult.ValidResult;
        }
    }
    public class ValidYearPu : ValidationRule
    {
        public override ValidationResult Validate
     (object value, System.Globalization.CultureInfo cultureInfo)
        {
            int res;
            bool isInt = Int32.TryParse(value.ToString(), out res);
            if (!isInt)
            {
                return new ValidationResult(false, "Год должен состоять из чисел");
            }
            if (res > 2025 || res < 1999) return new ValidationResult(false, "Введите год");

            return ValidationResult.ValidResult;
        }
    }
}
