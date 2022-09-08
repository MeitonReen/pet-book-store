using BookStore.UserService.Contracts.BookRating.V1_0_0.Delete;
using BookStore.UserService.Contracts.BookRating.V1_0_0.Read;
using BookStore.UserService.Contracts.BookRating.V1_0_0.Update;
using BookStore.UserService.Contracts.MyBookRating.V1_0_0.Create;

namespace BookStore.UserService.Settings.ResourceMapperProfiles.BookRating.V1_0_0
{
    public class MapProfile : AutoMapper.Profile
    {
        public MapProfile()
        {
            CreateMap<BL.ResourceEntities.BookRating, UpdateRequest>();

            CreateMap<BL.ResourceEntities.BookRating, UpdateResponse>();
            CreateMap<BL.ResourceEntities.BookRating, DeleteResponse>();

            CreateMap<BL.ResourceEntities.BookRating, CreateResponse>()
                .ForMember(
                    destination => destination.BookId,
                    conf => conf.MapFrom(source => source.Book.BookId));
            CreateMap<BL.ResourceEntities.BookRating, ReadResponse>()
                .ForMember(
                    destination => destination.BookId,
                    conf => conf.MapFrom(source => source.Book.BookId));
            CreateMap<BL.ResourceEntities.BookRating, Contracts.MyBookRating.V1_0_0.Read.ReadResponse>()
                .ForMember(
                    destination => destination.BookId,
                    conf => conf.MapFrom(source => source.Book.BookId));


            CreateMap<BL.ResourceEntities.BookRating, Contracts.MyBookRating.V1_0_0.Update.UpdateResponse>()
                .ForMember(
                    destination => destination.BookId,
                    conf => conf.MapFrom(source => source.Book.BookId));
        }
    }
}