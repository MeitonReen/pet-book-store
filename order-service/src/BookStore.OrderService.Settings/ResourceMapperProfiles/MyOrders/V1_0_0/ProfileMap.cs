using BookStore.OrderService.BL.ResourceEntities;
using BookStore.OrderService.Contracts.MyOrder.V1_0_0;
using Profile = AutoMapper.Profile;

namespace BookStore.OrderService.Settings.ResourceMapperProfiles.MyOrders.V1_0_0;

public class ProfileMap : Profile
{
    public ProfileMap()
    {
        CreateMap<Order, ReadResponse>();
        CreateMap<BookInOrder, ReadBookResponse>()
            .ForMember(dest => dest.BookId, sets => sets.MapFrom(source =>
                source.Book.BookId))
            .ForMember(dest => dest.Name, sets => sets.MapFrom(source =>
                source.Book.Name))
            .ForMember(dest => dest.PublicationDate, sets => sets.MapFrom(source =>
                source.Book.PublicationDate))
            .ForMember(dest => dest.Price, sets => sets.MapFrom(source =>
                source.Book.Price));
    }
}