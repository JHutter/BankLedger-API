using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using BankLedgerAPI.Utilities;

namespace BLAUnitTests
{
    public class ValidationUtilsUnitTests
    {
        //private readonly ValidationUtils _validationUtils;
        private List<ValidationCriterion> validationCriteriaEmpty;
        //private List<ValidationCriterion> validationCriteriaAlphanumeric;
        //private List<ValidationCriterion> validationCriteriaSpecialChars;
        private List<ValidationCriterion> validationCriteriaContradictorySpecialCharButAlphaNumOnly;
        public ValidationUtilsUnitTests()
        {
            validationCriteriaEmpty = new List<ValidationCriterion>();
            //validationCriteriaAlphanumeric = new List<ValidationCriterion>()
            //{
            //    ValidationCriterion.AlphanumericOnly
            //};

            //validationCriteriaSpecialChars = new List<ValidationCriterion>()
            //{
            //    ValidationCriterion.SpecialCharacter
            //};

            validationCriteriaContradictorySpecialCharButAlphaNumOnly = new List<ValidationCriterion>()
            {
                ValidationCriterion.AlphanumericOnly,
                ValidationCriterion.SpecialCharacter
            };

        }

        [Theory]
        [InlineData("test", 20, int.MaxValue)]
        [InlineData("", 5, int.MaxValue)]
        [InlineData("", 1, int.MaxValue)]
        [InlineData("thisiskindofalongstringinputbutitsnotlongenough", 127, 255)]
        [InlineData("thisiskindofalongstringinputbutit'snotlongenough", 127, 255)]
        [InlineData("thisiskind \r\nof a longstringinputbutitsnot longenough", 127, 255)]
        public void Validate_ShortString_ReturnFalse(string input, int min, int max)
        {
            // regardless of validation criteria passed, too short must fail
            Assert.False(ValidationUtils.IsMatch(ValidationCriterion.AlphanumericOnly, input, min, max));
            Assert.False(ValidationUtils.IsMatch(ValidationCriterion.AlphabeticOnly, input, min, max));
            Assert.False(ValidationUtils.IsMatch(ValidationCriterion.SpecialCharacter, input, min, max));
            Assert.False(ValidationUtils.IsMatch(validationCriteriaContradictorySpecialCharButAlphaNumOnly, input, min, max));
        }

        [Theory]
        [InlineData("test", 2, 2)]
        [InlineData("thisiskindofalongstringinputandindeeditstoolong", 8, 32)]
        [InlineData("thisiskindofalongstringinputandindeedit'stoolong!!", 8, 32)]
        [InlineData("this is kind of a long string input\r\n\tand indeed its too long", 8, 32)]
        public void Validate_LongString_ReturnFalse(string input, int min, int max)
        {
            // regardless of validation criteria passed, too long must fail
            Assert.False(ValidationUtils.IsMatch(ValidationCriterion.AlphanumericOnly, input, min, max));
            Assert.False(ValidationUtils.IsMatch(ValidationCriterion.AlphabeticOnly, input, min, max));
            Assert.False(ValidationUtils.IsMatch(ValidationCriterion.SpecialCharacter, input, min, max));
            Assert.False(ValidationUtils.IsMatch(validationCriteriaContradictorySpecialCharButAlphaNumOnly, input, min, max));
        }

        [Theory]
        [InlineData("test123", 3, 32)]
        [InlineData("a", 1, 32)]
        [InlineData("A", 1, 32)]
        [InlineData("123", 1, 32)]
        [InlineData("thIsCapItaLIzaTioNLoOkSsaRCAstiC", 8, 32)]
        public void Validate_AlphanumericValid_ReturnTrue(string input, int min, int max)
        {
            Assert.True(ValidationUtils.IsMatch(ValidationCriterion.AlphanumericOnly, input, min, max));
        }

        [Theory]
        [InlineData("test", 3, 32)]
        [InlineData("a", 1, 32)]
        [InlineData("A", 1, 32)]
        [InlineData("thIsCapItaLIzaTioNLoOkSsaRCAstiC", 8, 32)]
        public void Validate_AlphabeticValid_ReturnTrue(string input, int min, int max)
        {
            Assert.True(ValidationUtils.IsMatch(ValidationCriterion.AlphanumericOnly, input, min, max));
        }

        [Theory]
        [InlineData("test123 ", 3, 8)]
        [InlineData("thIsCapItaLIzaTioNLoOkSsaRCAstiC!", 8, 32)]
        [InlineData("superSecretString\r\n", 8, 32)]
        [InlineData("s uper Secre tString ", 8, 32)]
        [InlineData("", 0, 32)]
        [InlineData(".", 1, 32)]
        [InlineData("\t", 1, 32)]
        [InlineData(" ", 1, 32)]
        [InlineData("_____", 3, 8)]
        public void Validate_AlphanumericInvalid_ReturnFalse(string input, int min, int max)
        {
            Assert.False(ValidationUtils.IsMatch(ValidationCriterion.AlphanumericOnly, input, min, max));
        }

        [Theory]
        [InlineData("test123#", 1, 32)]
        [InlineData("$test123#", 1, 32)]
        [InlineData("@", 1, 8)]
        [InlineData("@2", 1, 8)]
        [InlineData("@2a", 1, 8)]
        [InlineData("_____", 1, 8)]
        public void Validate_SpecialCharsValid_ReturnTrue(string input, int min, int max)
        {
            Assert.True(ValidationUtils.IsMatch(ValidationCriterion.SpecialCharacter, input, min, max));
        }

        [Theory]
        [InlineData("test123#", 1, 32)]
        [InlineData("$test123#", 1, 32)]
        [InlineData("@", 1, 8)]
        [InlineData("@2", 1, 8)]
        [InlineData("@2a", 1, 8)]
        [InlineData("_____", 1, 8)]
        [InlineData("test123 ", 3, 8)]
        [InlineData("thIsCapItaLIzaTioNLoOkSsaRCAstiC!", 8, 32)]
        [InlineData("superSecretString\r\n", 8, 32)]
        [InlineData("s uper Secre tString ", 8, 32)]
        [InlineData("test123", 3, 32)]
        [InlineData("a", 1, 32)]
        [InlineData("A", 1, 32)]
        [InlineData("123", 1, 32)]
        [InlineData("thIsCapItaLIzaTioNLoOkSsaRCAstiC", 8, 32)]
        public void Validate_ContradictoryValidationCriteria_ReturnFalse(string input, int min, int max)
        {
            Assert.False(ValidationUtils.IsMatch(validationCriteriaContradictorySpecialCharButAlphaNumOnly, input, min, max));
        }
    }
}
