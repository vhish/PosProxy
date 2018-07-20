using AutoMapper;
using Hubtel.PosProxyData.EntityModels;
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
            //CreateMap<CreatePaymentRequestDto, CardPaymentRequest>().ReverseMap();
            CreateMap<CreatePaymentRequestDto, PaymentRequest>()
                .ForMember(dest => dest.Status, conf => conf.MapFrom(src => "Pending"))
                .ForMember(dest => dest.PaymentDate, conf => conf.MapFrom(src => src.PaymentDate ?? DateTime.Now))
                .ForMember(dest => dest.ClientReference, conf => conf.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.BranchId, conf => conf.MapFrom(src => src.Branch.BranchId))
                .ForMember(dest => dest.BranchName, conf => conf.MapFrom(src => src.Branch.Name))
                .ForMember(dest => dest.CustomerName, conf => conf.MapFrom(src => src.Customer.Name))
                .ForMember(dest => dest.CustomerEmail, conf => conf.MapFrom(src => src.Customer.Email))
                .ForMember(dest => dest.EmployeeId, conf => conf.MapFrom(src => src.Employee.EmployeeId))
                .ForMember(dest => dest.EmployeeName, conf => conf.MapFrom(src => src.Employee.Name))
                .ForMember(dest => dest.EmployeeEmail, conf => conf.MapFrom(src => src.Employee.EmailAddress))
                .ForMember(dest => dest.EmployeePhone, conf => conf.MapFrom(src => src.Employee.PhoneNumber))
                .ReverseMap();
        }
    }
}
