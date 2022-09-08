using AutoMapper;
using BookStore.Base.Abstractions.BaseResources.Inner;
using BookStore.Base.Abstractions.OptionsScopedChanges;
using BookStore.Base.Abstractions.UserClaimsProfile.Contracts.Profile;
using BookStore.Base.DefaultConfigs;
using BookStore.Base.Implementations.Result;
using BookStore.Base.Implementations.Result.Builders.BadRequest.Extensions;
using BookStore.Base.Implementations.Result.Extensions;
using BookStore.UserService.BL.ResourceEntities;
using BookStore.UserService.BL.Resources.MyBookReview.V1_0_0.ReadShort.Abstractions;
using BookStore.UserService.BL.Resources.MyBookReview.V1_0_0.Update.Abstractions;
using BookStore.UserService.Contracts.MyBookReview.V1_0_0.Update;

namespace BookStore.UserService.BL.Resources.MyBookReview.V1_0_0.Update.Implementations.Default;

public class UpdateMyBookReviewResource : IUpdateMyBookReviewResource
{
    private readonly AppConfig _appConfig;
    private readonly IBaseResource<BookReview> _bookReviewResource;
    private readonly IMapper _mapper;
    private readonly IReadShortMyBookReviewResource _readShortMyBookReviewResource;

    private readonly IUserClaimsProfile _userClaimsProfile;

    public UpdateMyBookReviewResource(
        IBaseResource<BookReview> bookReviewResource,
        IUserClaimsProfile userClaimsProfile,
        IMapper mapper,
        IOptionsSnapshotMixOptionsMonitor<AppConfig> appConfigAccessor,
        IReadShortMyBookReviewResource readShortMyBookReviewResource
    )
    {
        _appConfig = appConfigAccessor.Value;
        _bookReviewResource = bookReviewResource;
        _userClaimsProfile = userClaimsProfile;
        _mapper = mapper;
        _readShortMyBookReviewResource = readShortMyBookReviewResource;
    }

    public async Task<ResultModel> Update(UpdateRequest request)
    {
        var userProfileId = Guid.Parse(_userClaimsProfile.UserId);

        var targetBookReview = await _readShortMyBookReviewResource.ReadShort(request.BookId, userProfileId);

        if (targetBookReview == default)
            return ResultModelBuilder
                .BadRequest()
                .ApplyDefaultSettings("")
                .Environment(_appConfig.Environment)
                .Build()
                .ToResultModel();

        _bookReviewResource.Update(targetBookReview, bookReview =>
        {
            request.FillEntity(bookReview);
            bookReview.DateTimeSet = DateTime.Now;
        });

        var targetResourceToResult = _mapper.Map<UpdateResponse>(targetBookReview);

        return ResultModelBuilder
            .Updated(targetResourceToResult)
            .Build()
            .ToResultModel();
    }
}

public static class UpdateByBookRequestExtensions
{
    public static BookReview FillEntity(this UpdateRequest request, BookReview entity)
    {
        entity.Review = request.Review;

        return entity;
    }
}