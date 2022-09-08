using BookStore.Base.Contracts.Abstractions.Result;
using BookStore.Base.Implementations.Result.Location;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Base.Implementations.Result.Extensions
{
    public static class ResultModelExtensions
    {
        private static readonly ControllerBaseResultGenerator _resultGenerator = new();

        private static readonly Dictionary<ResultStatus, ActionResult> _actionResultMap = new()
        {
            [ResultStatus.Created] = objs => _resultGenerator.Created(
                objs?.ElementAtOrDefault(0) as string ?? string.Empty,
                objs?.ElementAtOrDefault(1) ?? objs?.ElementAtOrDefault(0)),
            [ResultStatus.Read] = objs => _resultGenerator.Ok(objs?.ElementAtOrDefault(0)),
            [ResultStatus.Updated] = objs => _resultGenerator.Ok(objs?.ElementAtOrDefault(0)),
            [ResultStatus.Deleted] = objs => _resultGenerator.Ok(objs?.ElementAtOrDefault(0)),

            [ResultStatus.Success] = objs => _resultGenerator.Ok(
                objs?.ElementAtOrDefault(0)),
            [ResultStatus.Conflict] = objs => _resultGenerator.Conflict(
                objs?.ElementAtOrDefault(0)),
            [ResultStatus.NotFound] = objs => _resultGenerator.NotFound(
                objs?.ElementAtOrDefault(0)),
            [ResultStatus.BadRequest] = objs => _resultGenerator.BadRequest(
                objs?.ElementAtOrDefault(0)),

            [ResultStatus.Exception] = objs => _resultGenerator.StatusCode(
                StatusCodes.Status500InternalServerError, objs?.ElementAtOrDefault(0)),

            [ResultStatus.Init] = objs => new ObjectResult(objs?.ElementAtOrDefault(0))
        };

        public static ResultModel ToResultModel<TResult>(this ResultModelGeneric<TResult>
            resultModel) => new(resultModel.Result, resultModel.ResultStatus);

        public static IActionResult ToActionResult(this ResultModel resultModel) =>
            _actionResultMap[resultModel.ResultStatus](resultModel.Result);

        public static IActionResult ToActionResult<TResult>(
            this ResultModelGeneric<TResult> resultModel)
            => _actionResultMap[resultModel.ResultStatus](resultModel.Result);

        public static IActionResult ToActionResult(this ResultModel
                resultModelMayBeWithDecoratedLocationResult,
            ControllerBase targetController,
            string getTargetResourceActionName)
        {
            var locationResult = (resultModelMayBeWithDecoratedLocationResult.Result as DecoratedResult)
                ?.Value as LocationResult;
            return _actionResultMap[resultModelMayBeWithDecoratedLocationResult.ResultStatus](
                targetController.Url.Action(getTargetResourceActionName,
                    null, locationResult?.RequestForLocation, targetController.Request.Scheme),
                locationResult?.Result ?? resultModelMayBeWithDecoratedLocationResult.Result);
        }

        private delegate IActionResult ActionResult(params object?[]? objs);
    }
}