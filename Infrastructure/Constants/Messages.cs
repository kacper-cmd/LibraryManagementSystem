using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Constants
{
    public static class Messages
    {
        #region Error Messages
        public const string RequiredName = "Required Name";
        public const string RequiredRole = "Required Role";
        public const string RequiredEmail = "Required Email";
        public const string RequiredPublish = "Published date invalid";
        public const string RequiredAuthor = "Author is required.";
        public const string ValidAuthor = "Author cannot be greather than 100 characters.";
        public const string RequiredTitle = "Title is required.";
        public const string ValidTitle = "Title cannot be greather than 200 characters.";
        public const string ValidName = "Name length should between 3 to 100";
        public const string ValidISBN = "ISBN cannot be empty";
        public const string ValidISBNLong = "ISBN must be exactly 13 characters long";
        public const string ValidISBNStart = "Invalid ISBN code for PL";
        public const string ValueCannotBeZero = "Add positive value";
        public const string InvalidLength = "Invalid Length of characters";
        public const string InvalidEmail = "Invalid Email Address";
        public const string InvalidRole = "Role must be  'Librarian' or 'Customer' or 'Admin'.";
        public const string MinUserName = "Minimum user name must be 3 letters length";


        #endregion
    }
}
