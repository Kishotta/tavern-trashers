namespace TavernTrashers.Api.Common.Domain.Results.Extensions;

public static class CombineExtensions
{
	public  static Result<(T1, T2)> Combine<T1, T2>(this Result<T1> first, Result<T2> second) =>
		first.Then(v1 => second.Transform(v2 => (v1, v2)));
}