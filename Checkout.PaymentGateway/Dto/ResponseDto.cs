using System.Text.Json.Serialization;
using Checkout.PaymentGateway.Utils;

namespace Checkout.PaymentGateway.Dto;

public class ResponseDto<T>
{
    public ResponseDto()
    {
        this.response_code = ResponseCode.Successful;
        this.response_message = MessageStrings.Successful;
    }

    [JsonPropertyName("isSuccessful")] public bool isSuccessful => response_code == ResponseCode.Successful;

    public ResponseDto(string message) : this()
    {
        this.response_message = message;
    }

    public ResponseDto(ResponseCode code, string message) : this()
    {
        this.response_code = code;
        this.response_message = message;
    }

    public ResponseDto(T data, string message) : this()
    {
        this.response_code = ResponseCode.Successful;
        this.content = data;
        this.response_message = message;
    }

    public ResponseDto(T data) : this()
    {
        this.response_code = ResponseCode.Successful;
        this.content = data;
        this.response_message = "Success";
    }

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