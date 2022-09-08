using System.Security.Cryptography;
using AutoMapper;
using BookStore.Base.Abstractions.BaseResources.Inner;
using BookStore.Base.Abstractions.OptionsScopedChanges;
using BookStore.Base.Abstractions.UserClaimsProfile.Contracts.Profile;
using BookStore.Base.DefaultConfigs;
using BookStore.Base.Implementations.Result;
using BookStore.Base.Implementations.Result.Builders.Conflict.Extensions;
using BookStore.Base.Implementations.Result.Extensions;
using BookStore.UserService.BL.ResourceEntities;
using BookStore.UserService.BL.Resources.Book.V1_0_0.CreateIfNotCreated.Abstractions;
using BookStore.UserService.BL.Resources.MyBookRating.V1_0_0.Create.Abstractions;
using BookStore.UserService.BL.Resources.MyBookRating.V1_0_0.ReadShort.Abstractions;
using BookStore.UserService.Contracts.MyBookRating.V1_0_0.Create;
using Profile = BookStore.UserService.BL.ResourceEntities.Profile;

namespace BookStore.UserService.BL.Resources.MyBookRating.V1_0_0.Create.Implementations.Default;

public class CreateMyBookRatingResource : ICreateMyBookRatingResource
{
    private readonly AppConfig _appConfig;
    private readonly IBaseResource<BookRating> _bookRatingResource;
    private readonly ICreateIfNotCreatedBookResource _createIfNotCreatedBookResource;
    private readonly IMapper _mapper;
    private readonly IReadShortMyBookRatingResource _readShortMyBookRatingResource;
    private readonly IUserClaimsProfile _userClaimsProfile;

    public CreateMyBookRatingResource(
        IBaseResource<BookRating> bookRatingResource,
        IUserClaimsProfile userClaimsProfile,
        IMapper mapper,
        ICreateIfNotCreatedBookResource createIfNotCreatedBookResource,
        IOptionsSnapshotMixOptionsMonitor<AppConfig> appConfigAccessor,
        IReadShortMyBookRatingResource readShortMyBookRatingResource
    )
    {
        _appConfig = appConfigAccessor.Value;
        _bookRatingResource = bookRatingResource;
        _userClaimsProfile = userClaimsProfile;
        _mapper = mapper;
        _createIfNotCreatedBookResource = createIfNotCreatedBookResource;
        _readShortMyBookRatingResource = readShortMyBookRatingResource;
    }

    public async Task<ResultModel> Create(CreateRequest request)
    {
        var myProfile = new Profile {UserId = Guid.Parse(_userClaimsProfile.UserId)};

        var targetBook = new ResourceEntities.Book {BookId = request.BookId};

        var targetBookPresenceCheckResult = await _createIfNotCreatedBookResource.CreateIfNotCreated(
            targetBook.BookId, createdBook => targetBook = createdBook);

        if (!targetBookPresenceCheckResult.IsCreated
            && !targetBookPresenceCheckResult.IsSuccess) return targetBookPresenceCheckResult;

        var targetBookRating = await _readShortMyBookRatingResource.ReadShort(request.BookId, myProfile.UserId);

        if (targetBookRating != default)
            return ResultModelBuilder
                .Conflict()
                .ApplyDefaultSettings("My book rating already created")
                .Environment(_appConfig.Environment)
                .Build()
                .ToResultModel();

        targetBookRating = CreateBookRating(targetBook, myProfile, request);

        var targetResourceToResult = _mapper.Map<CreateResponse>(targetBookRating);

        return ResultModelBuilder
            .Created(targetResourceToResult)
            .Build()
            .ToResultModel();
    }

    private BookRating CreateBookRating(ResourceEntities.Book targetBook, Profile myProfile,
        CreateRequest request)
    {
        var targetBookRating = _bookRatingResource
            .Create(bookRating =>
            {
                bookRating.RatingId = new Guid(ConcurrencyCreateProtect(
                    myProfile.UserId, targetBook.BookId));
                bookRating.DateTimeSet = DateTime.Now;
                bookRating.Rating = request.Rating;
            })
            .CreateReference(path => path.Book, targetBook)
            .CreateReference(path => path.Profile, myProfile)
            .ResourceEntity;

        return targetBookRating;
    }

    private byte[] ConcurrencyCreateProtect(Guid myProfileId, Guid targetBookId)
    {
        using var sha256Hash = SHA256.Create();
        return sha256Hash
            .ComputeHash(myProfileId.ToByteArray()
                .Union(targetBookId.ToByteArray())
                .ToArray())
            .TakeWhile((_, i) => i < 16)
            .ToArray();
    }
}

public static class CreateForBookRequestExtensions
{
    public static BookRating FillEntity(this CreateRequest request, BookRating entity)
    {
        entity.Rating = request.Rating;

        return entity;
    }
}