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
using BookStore.UserService.BL.Resources.MyBookReview.V1_0_0.Create.Abstractions;
using BookStore.UserService.BL.Resources.MyBookReview.V1_0_0.ReadShort.Abstractions;
using BookStore.UserService.Contracts.MyBookReview.V1_0_0.Create;
using Profile = BookStore.UserService.BL.ResourceEntities.Profile;

namespace BookStore.UserService.BL.Resources.MyBookReview.V1_0_0.Create.Implementations.Default;

public class CreateMyBookReviewResource : ICreateMyBookReviewResource
{
    private readonly AppConfig _appConfig;
    private readonly IBaseResource<BookReview> _bookReviewResource;
    private readonly ICreateIfNotCreatedBookResource _createIfNotCreatedBookResource;
    private readonly IMapper _mapper;
    private readonly IReadShortMyBookReviewResource _readShortMyBookReviewResource;
    private readonly IUserClaimsProfile _userClaimsProfile;

    public CreateMyBookReviewResource(
        IBaseResource<BookReview> bookReviewResource,
        IUserClaimsProfile userClaimsProfile,
        IMapper mapper,
        ICreateIfNotCreatedBookResource createIfNotCreatedBookResource,
        IOptionsSnapshotMixOptionsMonitor<AppConfig> appConfigAccessor,
        IReadShortMyBookReviewResource readShortMyBookReviewResource
    )
    {
        _appConfig = appConfigAccessor.Value;
        _bookReviewResource = bookReviewResource;
        _userClaimsProfile = userClaimsProfile;
        _mapper = mapper;
        _createIfNotCreatedBookResource = createIfNotCreatedBookResource;
        _readShortMyBookReviewResource = readShortMyBookReviewResource;
    }

    public async Task<ResultModel> Create(CreateRequest request)
    {
        var myProfile = new Profile {UserId = Guid.Parse(_userClaimsProfile.UserId)};

        var targetBook = new ResourceEntities.Book {BookId = request.BookId};

        var targetBookPresenceCheckResult = await _createIfNotCreatedBookResource.CreateIfNotCreated(
            targetBook.BookId, createdBook => targetBook = createdBook);

        if (!targetBookPresenceCheckResult.IsCreated
            && !targetBookPresenceCheckResult.IsSuccess) return targetBookPresenceCheckResult;

        var targetBookReview = await _readShortMyBookReviewResource.ReadShort(request.BookId, myProfile.UserId);

        if (targetBookReview != default)
            return ResultModelBuilder
                .Conflict()
                .ApplyDefaultSettings("My book review already created")
                .Environment(_appConfig.Environment)
                .Build()
                .ToResultModel();

        targetBookReview = CreateBookReview(targetBook, myProfile, request);

        var targetResourceToResult = _mapper.Map<CreateResponse>(targetBookReview);

        return ResultModelBuilder
            .Created(targetResourceToResult)
            .Build()
            .ToResultModel();
    }

    private BookReview CreateBookReview(ResourceEntities.Book targetBook, Profile myProfile,
        CreateRequest request)
    {
        var targetBookReview = _bookReviewResource
            .Create(bookReview =>
            {
                bookReview.ReviewId = new Guid(ConcurrencyCreateProtect(
                    myProfile.UserId, targetBook.BookId));
                bookReview.DateTimeSet = DateTime.Now;
                bookReview.Review = request.Review;
            })
            .CreateReference(path => path.Book, targetBook)
            .CreateReference(path => path.Profile, myProfile)
            .ResourceEntity;

        return targetBookReview;
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
    public static BookReview FillEntity(this CreateRequest request, BookReview entity)
    {
        entity.Review = request.Review;

        return entity;
    }
}