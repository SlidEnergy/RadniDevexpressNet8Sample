using System;
using System.Net;
using System.Text.Json.Serialization;
using Common.Infrastructure.Exceptions;
//using CommonBlazor.Exceptions;

namespace CommonBlazor.Models
{
    public abstract class ApiResponseBase
    {
        public HttpStatusCode StatusCode { get; set; }
        public string ErrorMessage { get; set; }

        public virtual bool IsSuccessfull => ErrorMessage is null;

        protected ApiResponseBase(ApiResponseBase from)
        {
            StatusCode = from.StatusCode;
            ErrorMessage = from.ErrorMessage;
        }

        protected ApiResponseBase(HttpStatusCode statusCode)
        {
            StatusCode = statusCode;
        }

        protected ApiResponseBase(HttpStatusCode statusCode, string errorMessage)
        {
            StatusCode = statusCode;
            ErrorMessage = errorMessage;
        }

        public static bool operator ==(ApiResponseBase response, HttpStatusCode statusCode) => response.StatusCode == statusCode;
        public static bool operator !=(ApiResponseBase response, HttpStatusCode statusCode) => response.StatusCode != statusCode;

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;

            return ReferenceEquals(this, obj);
        }

        public override int GetHashCode()
        {
            return ErrorMessage is null ? StatusCode.GetHashCode() : ErrorMessage.GetHashCode();
        }

        public void ThrowException<TException>(HttpStatusCode expectedApiStatusCode, Func<string, TException> exceptionFactory)
            where TException : ApplicationException, new()
        {
            if (this != expectedApiStatusCode)
                throw new ApplicationCustomException(ErrorMessage);

            if (IsSuccessfull == false)
                throw exceptionFactory.Invoke(ErrorMessage);
        }
    }

    public class ApiResponse : ApiResponseBase
    {
        public ApiResponse(ApiResponseBase from)
            : base(from)
        {
        }

        public ApiResponse(HttpStatusCode statusCode)
            : base(statusCode)
        {
        }

        [JsonConstructor]
        public ApiResponse(HttpStatusCode statusCode, string errorMessage)
            : base(statusCode)
        {
            ErrorMessage = errorMessage;
        }

        public static ApiResponse<TResult> From<TResult>(HttpStatusCode httpStatusCode, TResult result)
        {
            return new ApiResponse<TResult>(httpStatusCode, result);
        }

        public static bool operator ==(ApiResponse response, HttpStatusCode statusCode) => response.StatusCode == statusCode;
        public static bool operator !=(ApiResponse response, HttpStatusCode statusCode) => response.StatusCode != statusCode;

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class ApiResponse<TResult> : ApiResponseBase
    {
        public TResult Result { get; set; }

        public ApiResponse(ApiResponseBase from)
            : base(from)
        {
            if (from is ApiResponse<TResult> resultResponse)
            {
                Result = resultResponse.Result;
            }
        }

        public ApiResponse(HttpStatusCode statusCode, string errorMessage)
            : base(statusCode, errorMessage)
        {
        }

        [JsonConstructor]
        public ApiResponse(HttpStatusCode statusCode, TResult result)
            : base(statusCode)
        {
            Result = result;
        }

        public static implicit operator ApiResponse(ApiResponse<TResult> from) => new(from);
        public static implicit operator ApiResponse<TResult>(ApiResponse from) => new(from);

        public static bool operator ==(ApiResponse<TResult> response, HttpStatusCode statusCode) => response.StatusCode == statusCode;
        public static bool operator !=(ApiResponse<TResult> response, HttpStatusCode statusCode) => response.StatusCode != statusCode;

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return Result is not null
                ? HashCode.Combine(base.GetHashCode(), Result.GetHashCode())
                : base.GetHashCode();
        }
    }
}
