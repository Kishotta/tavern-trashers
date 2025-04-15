using System.Diagnostics.CodeAnalysis;

namespace TavernTrashers.Api.Common.Domain.Results;

public class Result
{
	protected Result(bool isSuccess, Error error)
	{
		if ((isSuccess && error != Error.None) ||
		    (!isSuccess && error == Error.None))
			throw new ArgumentException("Invalid error", nameof(error));

		IsSuccess = isSuccess;
		Error     = error;
	}

	public bool IsSuccess { get; }
	public bool IsFailure => !IsSuccess;

	public Error Error { get; }

	public static Result Success() => new(true, Error.None);

	public static Result<TValue> Success<TValue>(TValue value) => new(value, true, Error.None);

	public static Result Failure(Error error) => new(false, error);

	public static Result<TValue> Failure<TValue>(Error error) => new(default, false, error);

	public static implicit operator Result(Error error) => Failure(error);
}

public class Result<TValue>(TValue? value, bool isSuccess, Error error) : Result(isSuccess, error)
{
	[NotNull]
	public TValue Value => IsSuccess
		? value!
		: throw new InvalidOperationException("The value of a failed result cannot be accessed");

	public static implicit operator TValue(Result<TValue> result) => result.Value;

	public static implicit operator Result<TValue>(TValue value) =>
		value is not null
			? Success(value)
			: Error.NullValue;

	public static implicit operator Result<TValue>(Error error) =>
		Failure<TValue>(error);

	public static Result<TValue> ValidationFailure(Error error) =>
		new(default, false, error);
}