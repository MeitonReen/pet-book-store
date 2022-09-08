using BookStore.Base.Implementations.Result;

namespace BookStore.UserService.BL.Resources.Book.V1_0_0.CreateIfNotCreated.Abstractions;

public interface ICreateIfNotCreatedBookResource
{
    Task<ResultModel> CreateIfNotCreated(Guid targetBookId, Action<ResourceEntities.Book> fillIfCreate);
}