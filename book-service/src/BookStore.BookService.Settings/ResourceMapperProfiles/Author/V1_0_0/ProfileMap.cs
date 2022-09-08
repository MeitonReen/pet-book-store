using AutoMapper;
using BookStore.BookService.Contracts.Author.V1_0_0.Create;
using BookStore.BookService.Contracts.Author.V1_0_0.Delete;
using BookStore.BookService.Contracts.Author.V1_0_0.Read;
using BookStore.BookService.Contracts.Author.V1_0_0.Update;

namespace BookStore.BookService.Settings.ResourceMapperProfiles.Author.V1_0_0;

public class ProfileMap : Profile
{
    public ProfileMap()
    {
        CreateMap<BL.ResourceEntities.Author, CreateResponse>();
        CreateMap<BL.ResourceEntities.Author, ReadResponse>();
        CreateMap<BL.ResourceEntities.Author, UpdateResponse>();
        CreateMap<BL.ResourceEntities.Author, DeleteResponse>();
    }
}