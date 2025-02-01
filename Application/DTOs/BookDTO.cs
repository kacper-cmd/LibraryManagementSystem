using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class BookDTO : BaseDTO
    {
        public BookDTO() { }
        public BookDTO(Guid iD, bool available, string author, string title, string iSBN, DateTime publishedDate)
        {
            ID = iD;
            Available = available;
            Author = author;
            Title = title;
            ISBN = iSBN;
            PublishedDate = publishedDate;
        }

        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public DateTime PublishedDate { get; set; } = DateTime.Now;
        public string ISBN { get; set; }
        public bool Available { get; set; }
        public BookDTO DeepCopy()
        {
            var clone = this.MemberwiseClone() as BookDTO;
            clone.Available = Available;
            clone.Author = new string(Author);
            clone.Title = new string(Title);
            clone.ISBN = new string(ISBN);
            clone.PublishedDate = PublishedDate;
            return clone;
        }
    }
}
