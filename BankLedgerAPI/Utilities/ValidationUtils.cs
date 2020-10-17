using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace BankLedgerAPI.Utilities
{
    public enum ValidationCriterion
    {
        AlphanumericOnly,
        AlphabeticOnly,
        SpecialCharacter,
        AnyUpperCase,
        AnyLowerCase,
        AnyNumber
    }

    public static class ValidationUtils
    {
        static readonly Regex _alphanumeric = new Regex(@"^[a-zA-Z0-9]+\z");
        static readonly Regex _alphabetic = new Regex(@"^[a-zA-Z]+\z");
        static readonly Regex _specialCharacter = new Regex(@"[!@#$%^&*()_+\-=]+");

        /// <summary>
        /// Matches an input string using supplied min and max and list of validation criteria
        /// </summary>
        /// <param name="validationCriteria"></param>
        /// <param name="input"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static bool IsMatch(List<ValidationCriterion> validationCriteria, string input, int min, int max)
        {
            if (validationCriteria.Count < 1 || !(input.Length >= min && input.Length <= max))
            {
                return false;
            }

            if (validationCriteria.Count == 1)
            {
                return IsMatchHelper(validationCriteria.First(), input);
            }
            else
            {
                List<ValidationCriterion> subListValidationCriteria = new List<ValidationCriterion>(validationCriteria.GetRange(1, validationCriteria.Count - 1));
                return (IsMatchHelper(validationCriteria.First(), input) && IsMatch(subListValidationCriteria, input, min, max));
            }
        }

        public static bool IsMatch(ValidationCriterion validationCriterion, string input, int min, int max)
        {
            if (!(input.Length >= min && input.Length <= max))
            {
                return false;
            }

            return IsMatchHelper(validationCriterion, input);
            
        }

        /// <summary>
        /// Does the actual validation for the validation criterion passed in
        /// </summary>
        /// <param name="validationStategy">which validation criterion to use</param>
        /// <param name="input">the string to validate</param>
        /// <returns>true if input matches the given validation criterion, false otherwise</returns>
        private static bool IsMatchHelper(ValidationCriterion validationCriterion, string input)
        {
            switch (validationCriterion)
            {
                case ValidationCriterion.AlphanumericOnly:
                    return _alphanumeric.IsMatch(input);
                case ValidationCriterion.SpecialCharacter:
                    return _specialCharacter.IsMatch(input);
                case ValidationCriterion.AlphabeticOnly:
                    return _alphabetic.IsMatch(input);
            }
            return false;
        }
    }
}
