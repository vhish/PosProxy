using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hubtel.PosProxy.Models.Dtos
{
    public class DtoToEntityMappingProfile : Profile
    {
        public DtoToEntityMappingProfile()
        {
            CreateMap<CreatePaymentRequestDto, CardPaymentRequest>().ReverseMap();
            CreateMap<CreatePaymentRequestDto, PaymentRequest>().ReverseMap();
        }
    }
}
