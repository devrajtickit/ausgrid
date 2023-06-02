using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ausgrid.Models;

namespace Ausgrid.Models
{
    public class PasswordPolicy
    {

        public int MinLength { get; set; }
        public int MaxLength { get; set; }
        public int LetterReqLength { get; set; }
        public int DigitReqLength { get; set; }
        public int ChangeRequiredDay { get; set; }
        public int ChangeWarningDay { get; set; }
        public int MustNotSame { get; set; }
        public int DisableWrongPasswordAttempt { get; set; }
        public int PreventLogonUnusedDays { get; set; }
        public int MinCapitalLetters { get; set; }
        public int MinSmallLetters { get; set; }
        public int MinSpecialCharacters { get; set; }

    }
}