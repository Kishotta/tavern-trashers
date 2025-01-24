using Microsoft.AspNetCore.Http;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Common.Domain.Results.Extensions;

namespace TavernTrashers.Api.Common.Presentation;

public static class ResultExtensions
{
	public static IResult Ok(this Result result) =>
		result.Match(Results.NoContent, ApiResults.Problem);
	
	public static IResult Ok<TIn>(this Result<TIn> result) =>
		result.Match(Results.Ok, ApiResults.Problem);
	
	public static IResult Created<TIn>(this Result<TIn> result, Func<TIn, Uri> uriGenerator) =>
		result.Match(_ => TypedResults.Created(uriGenerator(result), result.Value), ApiResults.Problem);
	
	public static async Task<IResult> OkAsync(this Task<Result> resultTask) =>
		Ok(await resultTask);
	
	public static async Task<IResult> OkAsync<TIn>(this Task<Result<TIn>> resultTask) =>
		Ok(await resultTask);
	
	public static async Task<IResult> CreatedAsync<TIn>(this Task<Result<TIn>> resultTask, Func<TIn, Uri> uriGenerator) =>
		Created(await resultTask, uriGenerator);
}