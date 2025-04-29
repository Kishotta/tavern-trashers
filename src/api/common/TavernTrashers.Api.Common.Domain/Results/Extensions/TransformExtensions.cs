namespace TavernTrashers.Api.Common.Domain.Results.Extensions;

public static class TransformExtensions
{
	public static Result<TOut> Transform<TIn, TOut>(
		this Result<TIn> result,
		Func<TIn, TOut> mapper) =>
		result.IsSuccess
			? mapper(result)
			: result.Error;

	public static async Task<Result<TOut>> TransformAsync<TIn, TOut>(
		this Task<Result<TIn>> resultTask,
		Func<TIn, TOut> mapper)
	{
		var result = await resultTask.ConfigureAwait(false);
		return result.IsSuccess
			? mapper(result)
			: result.Error;
	}

	public static async Task<TOut> TransformAsync<TIn, TOut>(
		this Task<TIn> taskValue,
		Func<TIn, TOut> mapper)
	{
		var value = await taskValue.ConfigureAwait(false);

		return mapper(value);
	}
}