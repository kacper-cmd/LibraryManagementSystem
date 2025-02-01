using Application.DTOs;
using Infrastructure.Entities;

namespace Application.Mapper
{
    public static class BookMapper
    {
		/// <summary>
		/// Map Book entity to dto book . See <see cref="Book"/> class for details.
		/// </summary>
		/// <param name="book">extends book type to provide projection between  db entities and dto </param>
		/// <returns>Converted Dto </returns>
		public static BookDTO MapToDTO(this Book book)
        {
            return new BookDTO
            {
                Title = book.Title,
                ISBN = book.ISBN,
                Available = book.Available,
                Author = book.Author,
                PublishedDate = book.PublishedDate,
                ID= book.ID,
            };
        }
        public static IEnumerable<BookDTO> MapToDTO(this IEnumerable<Book> query)
        {
            return query.Select(x => new BookDTO
            {
                Title = x.Title,
                ISBN = x.ISBN,
                Available = x.Available,
                Author = x.Author,
                ID = x.ID,
                PublishedDate = x.PublishedDate
            });
        }
        public static Book MapToDomainModel(this BookDTO bookDTO)
        {
            return new Book
            {
                Author = bookDTO.Author,
                Available = bookDTO.Available,
                ISBN = bookDTO.ISBN,
                PublishedDate = bookDTO.PublishedDate,
                Title = bookDTO.Title,
                ID = bookDTO.ID,
            };
        }
        public static IEnumerable<Book> MapToDomainModel(this IEnumerable<BookDTO> query)
        {
            return query.Select(x => new Book
            {
                Author = x.Author,
                Available = x.Available,
                ISBN = x.ISBN,
                PublishedDate = x.PublishedDate,
                Title = x.Title,
                 ID = x.ID,
            });
        }
    }
}
