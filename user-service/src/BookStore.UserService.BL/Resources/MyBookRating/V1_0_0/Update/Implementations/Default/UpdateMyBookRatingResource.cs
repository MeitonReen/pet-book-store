using AutoMapper;
using BookStore.Base.Abstractions.BaseResources.Inner;
using BookStore.Base.Abstractions.OptionsScopedChanges;
using BookStore.Base.Abstractions.UserClaimsProfile.Contracts.Profile;
using BookStore.Base.DefaultConfigs;
using BookStore.Base.Implementations.Result;
using BookStore.Base.Implementations.Result.Builders.BadRequest.Extensions;
using BookStore.Base.Implementations.Result.Extensions;
using BookStore.UserService.BL.ResourceEntities;
using BookStore.UserService.BL.Resources.MyBookRating.V1_0_0.ReadShort.Abstractions;
using BookStore.UserService.BL.Resources.MyBookRating.V1_0_0.Update.Abstractions;
using BookStore.UserService.Contracts.MyBookRating.V1_0_0.Update;

namespace BookStore.UserService.BL.Resources.MyBookRating.V1_0_0.Update.Implementations.Default;

public class UpdateMyBookRatingResource : IUpdateMyBookRatingResource
{
    private readonly AppConfig _appConfig;
    private readonly IBaseResource<BookRating> _bookRatingResource;
    private readonly IMapper _mapper;
    private readonly IReadShortMyBookRatingResource _readShortMyBookRatingResource;

    private readonly IUserClaimsProfile _userClaimsProfile;

    public UpdateMyBookRatingResource(
        IBaseResource<BookRating> bookRatingResource,
        IUserClaimsProfile userClaimsProfile,
        IMapper mapper,
        IOptionsSnapshotMixOptionsMonitor<AppConfig> appConfigAccessor,
        IReadShortMyBookRatingResource readShortMyBookRatingResource
    )
    {
        _appConfig = appConfigAccessor.Value;
        _bookRatingResource = bookRatingResource;
        _userClaimsProfile = userClaimsProfile;
        _mapper = mapper;
        _readShortMyBookRatingResource = readShortMyBookRatingResource;
    }

    public async Task<ResultModel> Update(UpdateRequest request)
    {
        var userProfileId = Guid.Parse(_userClaimsProfile.UserId);

        var targetBookRating = await _readShortMyBookRatingResource.ReadShort(request.BookId, userProfileId);

        if (targetBookRating == default)
            return ResultModelBuilder
                .BadRequest()
                .ApplyDefaultSettings("")
                .Environment(_appConfig.Environment)
                .Build()
                .ToResultModel();

        _bookRatingResource.Update(targetBookRating, bookRating =>
        {
            request.FillEntity(bookRating);
            bookRating.DateTimeSet = DateTime.Now;
        });

        var targetResourceToResult = _mapper.Map<UpdateResponse>(targetBookRating);

        return ResultModelBuilder
            .Updated(targetResourceToResult)
            .Build()
            .ToResultModel();
    }
}

public static class UpdateByBookRequestExtensions
{
    public static BookRating FillEntity(this UpdateRequest request, BookRating entity)
    {
        entity.Rating = request.Rating;

        return entity;
    }
}