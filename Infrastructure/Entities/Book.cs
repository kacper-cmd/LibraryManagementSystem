using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Entities
{
    [Table(nameof(Book))]
    public class Book : BaseEntity
    {
        #region Properties
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public DateTime PublishedDate { get; set; }
        public string ISBN { get; set; } = string.Empty;
        public bool Available { get; set; }

        private static List<Book> _allBooks = new List<Book>();

        #endregion

        #region Constructors

        public Book()
        {
            
        }

        #endregion

        #region Methods

        public static int TotalBooks => _allBooks.Count;
        public static List<Book> GetAllBooks() => _allBooks;
        public void Borrow()
        {
            if (Available)
            {
                Available = false;
            }
            else
            {
                throw new InvalidOperationException("The book is already borrowed.");
            }
        }

        public void Return()
        {
            if (!Available)
            {
                Available = true;
            }
            else
            {
                throw new InvalidOperationException("The book was not borrowed.");
            }
        }

        #endregion


    }
}