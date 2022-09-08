using AutoMapper;
using BookStore.BookService.Contracts.BookCategory.V1_0_0.Create;
using BookStore.BookService.Contracts.BookCategory.V1_0_0.Delete;
using BookStore.BookService.Contracts.BookCategory.V1_0_0.Read;
using BookStore.BookService.Contracts.BookCategory.V1_0_0.Update;

namespace BookStore.BookService.Settings.ResourceMapperProfiles.BookCategory.V1_0_0;

public class ProfileMap : Profile
{
    public ProfileMap()
    {
        CreateMap<BL.ResourceEntities.BookCategory, CreateResponse>();
        CreateMap<BL.ResourceEntities.BookCategory, ReadResponse>();
        CreateMap<BL.ResourceEntities.BookCategory, UpdateResponse>();
        CreateMap<BL.ResourceEntities.BookCategory, DeleteResponse>();

        CreateMap<CreateBookReferenceRequest, CreateBookReferenceResponse>();
    }
}