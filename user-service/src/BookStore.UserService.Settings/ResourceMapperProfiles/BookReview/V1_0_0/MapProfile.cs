using BookStore.UserService.Contracts.BookReview.V1_0_0.Delete;
using BookStore.UserService.Contracts.BookReview.V1_0_0.Read;
using BookStore.UserService.Contracts.BookReview.V1_0_0.Update;
using BookStore.UserService.Contracts.MyBookReview.V1_0_0.Create;

namespace BookStore.UserService.Settings.ResourceMapperProfiles.BookReview.V1_0_0
{
    public class BookReview : AutoMapper.Profile
    {
        public BookReview()
        {
            CreateMap<BL.ResourceEntities.BookReview, UpdateRequest>();

            CreateMap<BL.ResourceEntities.BookReview, UpdateResponse>();
            CreateMap<BL.ResourceEntities.BookReview, DeleteResponse>();

            CreateMap<BL.ResourceEntities.BookReview, CreateResponse>()
                .ForMember(
                    destination => destination.BookId,
                    conf => conf.MapFrom(source => source.Book.BookId));
            CreateMap<BL.ResourceEntities.BookReview, ReadResponse>()
                .ForMember(
                    destination => destination.BookId,
                    conf => conf.MapFrom(source => source.Book.BookId));
            CreateMap<BL.ResourceEntities.BookReview, Contracts.MyBookReview.V1_0_0.Read.ReadResponse>()
                .ForMember(
                    destination => destination.BookId,
                    conf => conf.MapFrom(source => source.Book.BookId));

            CreateMap<BL.ResourceEntities.BookReview, UpdateResponse>()
                .ForMember(
                    destination => destination.BookId,
                    conf => conf.MapFrom(source => source.Book.BookId));
            CreateMap<BL.ResourceEntities.BookReview, Contracts.MyBookReview.V1_0_0.Update.UpdateResponse>()
                .ForMember(
                    destination => destination.BookId,
                    conf => conf.MapFrom(source => source.Book.BookId));
        }
    }
}