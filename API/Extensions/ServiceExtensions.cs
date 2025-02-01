using Application.DTOs;
using Application.Factory.Contracts;
using Application.Factory.Implementations;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Services;
using Application.Strategy;
using Application.ValidatorsDto;
using Database.Repository;
using FluentValidation;

namespace API.Extensions;

public static class ServiceExtensions
{
    public static void ConfigureApplication(this IServiceCollection services)
    {
        services.AddTransient<IValidator<BookDTO>, BookDtoValidator>();
        services.AddScoped<IBookRepository, BookRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
		services.AddTransient<IValidator<UserDTO>, UserDtoValidator>();

		services.AddScoped<IBookService, BookService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAccount, Account>();

        services.AddScoped<IUserFactory,UserFactory>();

        services.AddScoped<ISearchStrategyFactory, SearchStrategyFactory>();

        services.AddScoped<ByTitle>()
                .AddScoped<ISearchStrategy, ByTitle>(s => s.GetService<ByTitle>());
        services.AddScoped<ByAuthor>()
                .AddScoped<ISearchStrategy, ByAuthor>(s => s.GetService<ByAuthor>());
        services.AddScoped<ByISBN>()
                .AddScoped<ISearchStrategy, ByISBN>(s => s.GetService<ByISBN>());
    }
}
