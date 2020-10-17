using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankLedgerAPI.Utilities
{
    public static class Settings
    {
        public static int PasswordMinLength = 8;
        public static int PasswordMaxLength = 32;

        public static int ZeroCharVal = 48;
        public static int NineCharVal = 57;

        public static int UsernameMinLength = 3;
        public static int UsernameMaxLength = 32;
        public static ValidationCriterion UsernameCriterion = ValidationCriterion.AlphanumericOnly;
        public static string UsernameValidationMessage = "alphanumeric characters only";

        public static ValidationCriterion NameCriterion = ValidationCriterion.AlphabeticOnly;
        public static int NameMinLength = 2;
        public static int NameMaxLength = 32;

        public static ValidationCriterion AccountNameCriterion = ValidationCriterion.AlphanumericOnly;
        public static int AccountNameMinLength = 1;
        public static int AccountNameMaxLength = 32;

        
    }
}
