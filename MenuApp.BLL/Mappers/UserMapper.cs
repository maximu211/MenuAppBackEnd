using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using MenuApp.BLL.DTO.ReceiptsDTOs;
using MenuApp.BLL.DTO.UserDTO;
using MenuApp.DAL.Models.EntityModels;
using MongoDB.Bson;

namespace MenuApp.BLL.Mappers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Users, UsersDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username))
                .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.Image));

            CreateMap<ReceiptsDTO, Receipts>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => ObjectId.Parse(src.Id)))
                .ForMember(
                    dest => dest.UserId,
                    opt => opt.MapFrom(src => ObjectId.Parse(src.UserId))
                )
                .ForMember(
                    dest => dest.CookingDifficulty,
                    opt => opt.MapFrom(src => src.CookingDifficulty)
                )
                .ForMember(dest => dest.CookTime, opt => opt.MapFrom(src => src.CookTime))
                .ForMember(dest => dest.ReceiptType, opt => opt.MapFrom(src => src.ReceiptType))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(
                    dest => dest.ReceiptDescriptionElements,
                    opt => opt.MapFrom(src => src.ReceiptDescriptionElements)
                )
                .ForMember(
                    dest => dest.ReceiptIngradients,
                    opt => opt.MapFrom(src => src.ReceiptIngradients)
                );
        }
    }
}
