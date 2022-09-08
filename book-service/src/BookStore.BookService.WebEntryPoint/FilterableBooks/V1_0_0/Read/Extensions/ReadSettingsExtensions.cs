using BookStore.Base.Implementations;
using BookStore.BookService.Contracts.FilterableBooks.V1_0_0.Read;
using Microsoft.EntityFrameworkCore;

namespace BookStore.BookService.WebEntryPoint.FilterableBooks.V1_0_0.Read.Extensions;

public static class ReadSettingsExtensions
{
    public static IQueryable<BL.ResourceEntities.Book> ApplyTargetFilters(
        this IQueryable<BL.ResourceEntities.Book> targetBooksQuery,
        ReadPartRequest request)
    {
        var booksFilteredByAuthors = request.AuthorLastName != default
            ? targetBooksQuery
                .Where(book => book.Authors
                    .Any(author => EF.Functions
                        .Like(author.LastName, $"%{request.AuthorLastName}%")))
            : targetBooksQuery;

        var booksFilteredByAuthorsAndCategories = request.CategoryName != default
            ? booksFilteredByAuthors
                .Where(book => book.Categories
                    .Any(author => EF.Functions
                        .Like(author.Name, $"%{request.CategoryName}%")))
            : booksFilteredByAuthors;

        var booksIncludingAllFilters =
            request.OrderPrice == SortOrder.Ascending
                ? booksFilteredByAuthorsAndCategories.OrderBy(book => book.Price)
                : booksFilteredByAuthorsAndCategories.OrderByDescending(book => book.Price);

        return booksIncludingAllFilters;
    }
}