using AutoMapper;
using BookStore.OrderService.Contracts.Book.V1_0_0.Delete;
using BookStore.OrderService.Contracts.Book.V1_0_0.Update;

namespace BookStore.OrderService.Settings.ResourceMapperProfiles.Book.V1_0_0;

public class ProfileMap : Profile
{
    public ProfileMap()
    {
        CreateMap<BL.ResourceEntities.Book, UpdateBookCompensateCommand>();
        CreateMap<BL.ResourceEntities.Book, DeleteBookCompensateCommand>();
    }
}