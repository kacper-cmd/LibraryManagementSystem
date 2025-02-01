using Application.DTOs;
using Application.RequestModel;
using Blazored.FluentValidation;
using Client.Services;
using FluentValidation;
using FluentValidation.Results;
using Infrastructure.Constants;
using Infrastructure.Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using static System.Net.WebRequestMethods;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.JSInterop;

namespace Client.Components.Pages.Book;
public partial class EditBook : ComponentBase
{
    #region Properties
    [Parameter]
    public Guid BookId { get; set; }
    [Inject]
    private NavigationManager NavigationManager { get; set; }
    [Inject]
    private IBookService bookService { get; set; }

    [Inject]
    private AuthenticationStateProvider AuthenticationStateProvider { get; set; }
    private BookDTO book = new BookDTO();
    private BookDTO originalBook = new BookDTO();

    #endregion
    protected override async Task OnInitializedAsync()
	{
        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;
        try
        {
            book = await bookService.GetBookAsync(BookId);
            originalBook = book.DeepCopy();
        }
        catch (Exception e)
        {
            toastService.ShowError(e.Message);
        }

    }
	private async Task HandleValidSubmit()
	{
            try
            {

                var patchDocument = CreatePatchDocument();
                if (patchDocument.Operations.Count > 0)
                {
                    var response = await bookService.PatchBookAsync(BookId, patchDocument);
                }
                NavigationManager.NavigateTo("/books");
            }
            catch(BadHttpRequestException e)
            {
                toastService.ShowError(e.Message);
            }
            catch (Exception e)
            {
                toastService.ShowError(e.Message);

            }
	}
    private JsonPatchDocument<BookDTO> CreatePatchDocument()
    {
        var patchDocument = new JsonPatchDocument<BookDTO>();

        if (book.Title != originalBook.Title) patchDocument.Replace(b => b.Title, book.Title);
        if (book.Author != originalBook.Author) patchDocument.Replace(b => b.Author, book.Author);
        if (book.PublishedDate != originalBook.PublishedDate) patchDocument.Replace(b => b.PublishedDate, book.PublishedDate);
        if (book.ISBN != originalBook.ISBN) patchDocument.Replace(b => b.ISBN, book.ISBN);
        if (book.Available != originalBook.Available) patchDocument.Replace(b => b.Available, book.Available);

        return patchDocument;
    }
	private void Cancel()
	{
		NavigationManager.NavigateTo("/books");
	}
}
