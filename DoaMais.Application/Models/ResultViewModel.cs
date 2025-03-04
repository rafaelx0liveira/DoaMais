
namespace DoaMais.Application.Models
{
    public class ResultViewModel
    {
        public ResultViewModel(bool isSucess = true, string message = "")
        {
            IsSuccess = isSucess;
            Message = message;
        }

        public bool IsSuccess {  get; private set; }
        public string Message { get; private set; }

        public static ResultViewModel Success() => new();
        public static ResultViewModel Error(string message) => new() { IsSuccess = false, Message = message };
    }

    public class ResultViewModel<T> : ResultViewModel
    {
        public ResultViewModel(T? data, bool isSuccess = true, string message = "") : base(isSuccess, message)
        {
            Data = data;
        }

        public T? Data { get; private set; }
        public static ResultViewModel<T> Success(T data) => new(data);
        public static ResultViewModel<T> Success(T data, string message) => new(data, true, message);
        public static ResultViewModel<T> Error(string message) => new(default, false, message);
    }
}
