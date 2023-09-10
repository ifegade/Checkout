using System.Text.Json.Serialization;
using Checkout.PaymentGateway.Utils;

namespace Checkout.PaymentGateway.Dto;

public class ResponseDto<T>
{
    public ResponseDto()
    {
        response_code = ResponseCode.Successful;
        response_message = MessageStrings.Successful;
    }

    public ResponseDto(string message) : this()
    {
        response_message = message;
    }

    public ResponseDto(ResponseCode code, string message) : this()
    {
        response_code = code;
        response_message = message;
    }

    public ResponseDto(T data, string message) : this()
    {
        response_code = ResponseCode.Successful;
        content = data;
        response_message = message;
    }

    public ResponseDto(T data) : this()
    {
        response_code = ResponseCode.Successful;
        content = data;
        response_message = "Success";
    }

    [JsonPropertyName("isSuccessful")] public bool isSuccessful => response_code == ResponseCode.Successful;

    public string response_message { get; set; }
    public ResponseCode response_code { get; set; }
    public T content { get; set; }
}

public enum ResponseCode
{
    Successful = 200,
    Unsuccessful = 300,
    UnAuthorized
}