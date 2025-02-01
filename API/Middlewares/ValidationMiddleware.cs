using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net.NetworkInformation;

namespace API.Middlewares
{
    //public class ValidationMiddleware
    //{
    //    private readonly RequestDelegate _next;
    //    private readonly IValidator<Book> _validator;

    //    public ValidationMiddleware(RequestDelegate next, IValidator<Book> validator)
    //    {
    //        _next = next;
    //        _validator = validator;
    //    }

    //    public async Task Invoke(HttpContext context)
    //    {
    //        Book store = 
    //         var validationResult = _validator.Validate(store);

    //        if (!validationResult.IsValid)
    //        {
    //            context.Response.StatusCode = StatusCodes.Status400BadRequest;
    //            await context.Response.WriteAsync("Validation failed");
    //            return;
    //        }

    //        await _next(context);
    //    }
    //}
}
//https://medium.com/@lucas.and227/fluent-validation-with-net-core-da0d9da73c8a
//https://medium.com/@madu.sharadika/validation-in-net-8-a250c4d278d2


//@using FluentValidation
//@using FluentValidation.AspNetCore

//... // your other using directives

//<EditForm Model="yourModel" OnValidSubmit="HandleValidSubmit">
//    <DataAnnotationsValidator />
//    <ValidationSummary />

//    ... // your input fields

//    <button type="submit">Submit</button>
//</EditForm>

//@code {
//    private YourModel yourModel = new YourModel();
//private IValidator<YourModel> validator = new YourValidator();

//private void HandleValidSubmit()
//{
//    var context = new ValidationContext<YourModel>(yourModel);
//    var validationResult = validator.Validate(context);

//    if (validationResult.IsValid)
//    {
//        // your logic when the model is valid
//    }
//    else
//    {
//        // your logic when the model is invalid
//    }
//}
//}

//public IActionResult Register([FromBody, Validate] RegisterModel model)
//{
//    if (ModelState.IsValid)
//    {
//        // Do registeration
//    }

//    // return validation errors
//}
//[HttpPost("UpdateNewCustomer", Order = 1)]
//public IActionResult Update(CustomerDTO customer)
//{
//    CustomerValidator validator = new CustomerValidator();
//    var validationResult = validator.Validate(customer);

//    if (!validationResult.IsValid)
//    {
//        return BadRequest(validationResult.Errors);
//    }
//[HttpPost("AddNewCustomer", Order = 0)]
//public IActionResult Add(CustomerDTO customer)
//{
//    if (!ModelState.IsValid)
//    {
//        return BadRequest(ModelState);
//    }

//[HttpPost]
//public IActionResult CreateUser([FromBody] User user)
//{
//    if (!ModelState.IsValid)
//    {
//        var messages = ModelState
//          .SelectMany(modelState => modelState.Value.Errors)
//          .Select(err => err.ErrorMessage)
//          .ToList();

//        return BadRequest(messages);
//    }
