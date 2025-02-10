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

public static partial class ResultExtensions
{
	public static Result<TOut> Then<TIn, TOut>(
		this Result<TIn> result, Func<TIn, TOut> binder) =>
		result.IsSuccess
			? binder(result)
			: result.Error;

	public static async Task<Result<TOut>> ThenAsync<TIn, TOut>(
		this Task<Result<TIn>> taskResult,
		Func<TIn, Result<TOut>> binder)
	{
		var result = await taskResult.ConfigureAwait(false);
		return result.IsSuccess
			? binder(result)
			: result.Error;
	}
}

public static partial class ResultExtensions
{
	public static Result<TOut> Transform<TIn, TOut>(this Result<TIn> result, Func<TIn, TOut> mapper) =>
		result.IsSuccess
			? mapper(result)
			: result.Error;
	
	public static async Task<Result<TOut>> TransformAsync<TIn, TOut>(this Task<Result<TIn>> resultTask,
		Func<TIn, TOut> mapper)
	{
		var result = await resultTask.ConfigureAwait(false);
		return result.IsSuccess
			? mapper(result)
			: result.Error;
	}
	
	public static async Task<TOut> TransformAsync<TIn, TOut>(this Task<TIn> taskValue, Func<TIn, TOut> mapper)
	{
		var value = await taskValue.ConfigureAwait(false);

		return mapper(value);
	}
}

public static partial class ResultExtensions
{
	public static Result<TValue> Do<TValue>(this Result<TValue> result, Action<TValue> action)
	{
		if (result.IsSuccess)
			action(result.Value);

		return result;
	}
	
	public static async Task<Result<TValue>> DoAsync<TValue>(this Result<TValue> result, Func<TValue, Task> action)
	{
		if (result.IsSuccess)
			await action(result);

		return result;
	}
	
	public static async Task<Result<TValue>> DoAsync<TValue>(this Task<Result<TValue>> resultTask, Action<TValue> action)
	{
		var result = await resultTask.ConfigureAwait(false);

		if (result.IsSuccess)
			action(result);

		return result;
	}
	
	public static async Task<Result<TValue>> DoAsync<TValue>(this Task<Result<TValue>> resultTask, Func<TValue, Task> action)
	{
		var result = await resultTask.ConfigureAwait(false);

		if (result.IsSuccess)
			await action(result);

		return result;
	}
}

public static partial class ResultExtensions
{
	public  static Result<(T1, T2)> Combine<T1, T2>(this Result<T1> first, Result<T2> second) =>
		first.Then(v1 => second.Transform(v2 => (v1, v2)));
}