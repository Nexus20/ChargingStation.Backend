﻿using AutoMapper;
using ChargingStation.ChargePoints.Models.Requests;
using ChargingStation.ChargePoints.Models.Responses;
using ChargingStation.Common.Models;
using ChargingStation.Domain.Entities;

namespace ChargingStation.ChargePoints.Mappings
{
    public class ChargePointProfile : Profile
    {
        public ChargePointProfile()
        {
            CreateMap<ChargePoint, ChargePointResponse>();
            CreateMap<ChargePoint, CreateChargePoint>();
            CreateMap<ChargePoint, UpdateChargePoint>();
            CreateMap(typeof(IPagedCollection<>), typeof(PagedCollection<>));
            CreateMap(typeof(IPagedCollection<>), typeof(IPagedCollection<>))
                .As(typeof(PagedCollection<>));
        }
    }
}
