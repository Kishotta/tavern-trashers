namespace TavernTrashers.Api.Common.Domain.Results.Extensions;

public static class MatchExtensions
{
	public static TOut Match<TOut>(
		this Result result,
		Func<TOut> onSuccess,
		Func<Result, TOut> onFailure)
	{
		return result.IsSuccess 
			? onSuccess() 
			: onFailure(result);
	}
	
	public static TOut Match<TIn, TOut>(
		this Result<TIn> result,
		Func<TIn, TOut> onSuccess,
		Func<Result<TIn>, TOut> onFailure)
	{
		return result.IsSuccess 
			? onSuccess(result) 
			: onFailure(result);
	}
	
	public static Task<TOut> MatchAsync<TOut>(
		this Task<Result> resultTask,
		Func<TOut> onSuccess,
		Func<Result, TOut> onFailure)
	{
		return resultTask.ContinueWith(task =>
		{
			var result = task.Result;
			return result.IsSuccess
				? onSuccess()
				: onFailure(result);
		});
	}
	
	public static Task<TOut> MatchAsync<TIn, TOut>(
		this Task<Result<TIn>> resultTask,
		Func<TIn, TOut> onSuccess,
		Func<Result<TIn>, TOut> onFailure)
	{
		return resultTask.ContinueWith(task =>
		{
			var result = task.Result;
			return result.IsSuccess
				? onSuccess(result)
				: onFailure(result);
		});
	}
}