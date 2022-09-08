using BookStore.Base.Abstractions.BaseResources.Inner;
using BookStore.Base.Implementations.BookStoreDefaultExceptionHandling.OtherHelpers;
using BookStore.Base.InterserviceContracts.UserService.V1_0_0.Profile.V1_0_0.Delete;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace BookStore.UserService.WebEntryPoint.Profile.V1_0_0.Delete.MassTransitCourierActivities;

public class DeleteProfileCommandActivity : IActivity<DeleteProfileCommand, BL.ResourceEntities.Profile>
{
    private readonly IBaseResource<BL.ResourceEntities.Profile> _profileBaseResource;
    private readonly IResourcesCommitter _resourcesCommitter;

    public DeleteProfileCommandActivity(
        IResourcesCommitter resourcesCommitter,
        IBaseResource<BL.ResourceEntities.Profile> profileBaseResource
    )
    {
        _resourcesCommitter = resourcesCommitter;
        _profileBaseResource = profileBaseResource;
    }

    public async Task<ExecutionResult> Execute(ExecuteContext<DeleteProfileCommand> context)
    {
        var oldEntityState = await _profileBaseResource
            .ReadSettings(settings => settings
                .Where(profile => profile.UserId == context.Arguments.UserId)
                .Include(profile => profile.BookRatings)
                .Include(profile => profile.BookReviews)
                .AsNoTracking())
            .ReadAsync();

        if (oldEntityState == default)
            throw new InvalidOperationException("Requested resource not found");

        ClearCircularReferences(oldEntityState);

        _profileBaseResource.Delete(profile => profile.UserId = context.Arguments.UserId);

        try
        {
            await _resourcesCommitter.CommitAsync();
        }
        catch (Exception e)
            when (BookStoreExceptionHandlingDefaults.WhenExpressions.RepeatCommit(e))
        {
            await _resourcesCommitter.CommitAsync();
        }

        return context.Completed(oldEntityState);
    }

    public async Task<CompensationResult> Compensate(CompensateContext<BL.ResourceEntities.Profile> context)
    {
        var oldEntityState = context.Log;

        CompensateCompositeEntityAfterDelete(oldEntityState);

        try
        {
            await _resourcesCommitter.CommitAsync();
        }
        catch (Exception e)
            when (BookStoreExceptionHandlingDefaults.WhenExpressions.RepeatCommit(e))
        {
            await _resourcesCommitter.CommitAsync();
        }

        return context.Compensated();
    }

    private void CompensateCompositeEntityAfterDelete(BL.ResourceEntities.Profile entity)
    {
        var bookRatings = entity.BookRatings;
        var bookReviews = entity.BookReviews;

        entity.BookRatings = new List<BL.ResourceEntities.BookRating>();
        entity.BookReviews = new List<BL.ResourceEntities.BookReview>();

        _profileBaseResource
            .Create(entity)
            .CreateReferences(author => author.BookRatings, bookRatings)
            .CreateReferences(author => author.BookReviews, bookReviews);
    }

    private void ClearCircularReferences(BL.ResourceEntities.Profile profile)
    {
        profile.BookRatings = profile.BookRatings
            .Select(bookRating =>
            {
                bookRating.Profile = default!;
                return bookRating;
            })
            .ToList();

        profile.BookReviews = profile.BookReviews
            .Select(bookReview =>
            {
                bookReview.Profile = default!;
                return bookReview;
            })
            .ToList();
    }
}