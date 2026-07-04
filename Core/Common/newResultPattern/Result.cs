
namespace Core.Common.newResultPattern
{
    public class Result
    {
        public bool IsSuccess { get;  }
        public bool IsFailure => !IsSuccess;
        public IReadOnlyList<Error> Errors { get; } 

        protected  Result(bool isSuccess, IReadOnlyList<Error> errors)
        {
            if (isSuccess && errors?.Count > 0)
                throw new ArgumentException("Successful result cannot have errors.");

            if (!isSuccess && errors?.Count == 0)
                throw new ArgumentException("Failed result must have at least one error.");
            IsSuccess = isSuccess;
            Errors = errors;
        }


        public static Result Success() => new Result(true, []);
        public static Result Failure(IReadOnlyList<Error> error) => new Result(false, error);
        public static Result Failure(Error error) => new Result(false, [error]);


    }
    public class Result<T>:Result
    {

        private readonly T _value;
        public T Value { 
          get=> IsSuccess ?_value: throw new InvalidOperationException("Cannot access the value of a failed result.");
        }
        private Result(bool isSuccess, T data ,IReadOnlyList<Error> error):base(isSuccess, error) 
        {
            _value = data; 
        }
        


        public static Result<T> Success(T value) => new Result<T>(true,value, []);
        public new static Result<T> Failure(IReadOnlyList<Error> error) => new Result<T>(false, default! ,error);
        public new static Result<T> Failure(Error error) => new Result<T>(false, default! ,[error]);
        


    }
}
