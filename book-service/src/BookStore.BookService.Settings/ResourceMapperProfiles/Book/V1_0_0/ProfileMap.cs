using AutoMapper;
using BookStore.Base.InterserviceContracts.BookService.V1_0_0.Book.V1_0_0.DeleteOut;
using BookStore.Base.InterserviceContracts.BookService.V1_0_0.Book.V1_0_0.UpdateOut;
using BookStore.Base.InterserviceContracts.OrderService.V1_0_0.Book.V1_0_0.Delete;
using BookStore.BookService.Contracts.Book.V1_0_0;
using BookStore.BookService.Contracts.Book.V1_0_0.Create;
using BookStore.BookService.Contracts.Book.V1_0_0.Delete;
using BookStore.BookService.Contracts.Book.V1_0_0.Read;
using BookStore.BookService.Contracts.Book.V1_0_0.Update;
using UpdateBookCommand = BookStore.Base.InterserviceContracts.OrderService.V1_0_0.Book.V1_0_0.Update.UpdateBookCommand;

namespace BookStore.BookService.Settings.ResourceMapperProfiles.Book.V1_0_0;

public class ProfileMap : Profile
{
    public ProfileMap()
    {
        CreateMap<BL.ResourceEntities.Book, CreateResponse>();
        CreateMap<BL.ResourceEntities.Book, ReadResponse>();
        CreateMap<BL.ResourceEntities.Book, UpdateResponse>();
        CreateMap<BL.ResourceEntities.Book, DeleteResponse>();
        CreateMap<BL.ResourceEntities.Book, UpdateBookCompensateCommand>();

        CreateMap<UpdateRequest, UpdateResponse>();
        CreateMap<DeleteRequest, DeleteResponse>();

        CreateMap<DeleteBookRequest, DeleteBookCommand>().AsProxy();
        CreateMap<DeleteBookRequest,
            Base.InterserviceContracts.UserService.V1_0_0.Book.V1_0_0.Delete.DeleteBookCommand>().AsProxy();

        CreateMap<UpdateBookRequest, UpdateBookCommand>().AsProxy();

        CreateMap<BL.ResourceEntities.Book, ReadBookResponse>();
    }
}