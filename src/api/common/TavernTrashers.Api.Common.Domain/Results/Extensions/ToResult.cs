namespace TavernTrashers.Api.Common.Domain.Results.Extensions;

public static partial class ResultExtensions
{
	public static Result<TValue> ToResult<TValue>(this TValue? value, Error error) =>
		value is not null
			? Result.Success(value)
			: Result.Failure<TValue>(error);
	
	public static Task<Result<TResult>> ToResultAsync<TResult>(this Task<TResult?> task, Error error) =>
		task.ContinueWith(t => t.Result.ToResult(error));
}