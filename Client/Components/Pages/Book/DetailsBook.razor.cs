using Application.DTOs;
using Client.Services;
using Microsoft.AspNetCore.Components;

namespace Client.Components.Pages.Book
{
    public partial class DetailsBook : ComponentBase
    {
        #region Properties
        [Parameter]
        public Guid BookId { get; set; }
        [Inject]
        private NavigationManager NavigationManager { get; set; }

        [Inject]
        private IBookService bookService { get; set; }

        private BookDTO book = new BookDTO(); 
        #endregion

        protected override async Task OnInitializedAsync()
        {
            try
            {
                book = await bookService.GetBookAsync(BookId);
            }
            catch (Exception e)
            {
                toastService.ShowError(e.Message);

            }
        }

        private void BackToList()
        {
            NavigationManager.NavigateTo("/books");
        }
    }
}