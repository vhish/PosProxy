﻿using AutoMapper;
using Hubtel.PaymentProxy.Models.Dtos;
using Hubtel.PaymentProxy.Models.ApiRequests;
using Hubtel.PaymentProxyData.EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hubtel.PaymentProxy.Models
{
    public class DtoToEntityMappingProfile : Profile
    {
        public DtoToEntityMappingProfile()
        {
            CreateMap<OrderRequestDto, OrderRequest>().ReverseMap();

            CreateMap<PaymentRequestDto, PaymentRequest>()
                .ForMember(dest => dest.Status, conf => conf.MapFrom(src => "Pending"))
                .ForMember(dest => dest.ChargeCustomer, conf => conf.MapFrom(src => src.ChargeCustomer ?? true))
                .ForMember(dest => dest.PaymentDate, conf => conf.MapFrom(src => src.PaymentDate ?? DateTime.Now))
                .ForMember(dest => dest.ClientReference, conf => conf.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.BranchId, conf => conf.MapFrom(src => src.BranchId))
                .ForMember(dest => dest.BranchName, conf => conf.MapFrom(src => src.BranchName))
                .ForMember(dest => dest.CustomerName, conf => conf.MapFrom(src => src.CustomerName))
                .ForMember(dest => dest.CustomerMobileNumber, conf => conf.MapFrom(src => src.CustomerMobileNumber))
                .ForMember(dest => dest.EmployeeId, conf => conf.MapFrom(src => src.EmployeeId))
                .ForMember(dest => dest.EmployeeName, conf => conf.MapFrom(src => src.EmployeeName))
                .ReverseMap();
        }
    }
}
