using AutoMapper;
using Checkout.PaymentGateway.Dto;
using Checkout.PaymentGateway.Models;

namespace Checkout.PaymentGateway.Utils;

public class MappingProfile : Profile
{
    public MappingProfile(IEncryptionServices _encryption)
    {
        CreateMap<TransactionModel, TransactionDto>();
        CreateMap<CardModel, CardBaseDto>()
            .ForMember(dest => dest.CardName,
                opt =>
                    opt.MapFrom(src => _encryption.DecryptString(src.CardName)))
            .ForMember(dest => dest.CardNumber,
                opt =>
                    opt.MapFrom(src => Utility.MaskCardNumber(_encryption.DecryptString(src.CardNumber))));
        ;
        CreateMap<MerchantModel, MerchantDto>();
        CreateMap<CurrencyModel, CurrencyDto>();
    }
}