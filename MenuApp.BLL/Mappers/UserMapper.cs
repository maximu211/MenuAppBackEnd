using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using MenuApp.BLL.DTO.ReceiptsDTOs;
using MenuApp.BLL.DTO.RecipesDTOs;
using MenuApp.BLL.DTO.UserDTOs;
using MenuApp.DAL.Models.AggregetionModels;
using MenuApp.DAL.Models.EntityModels;
using MongoDB.Bson;

namespace MenuApp.BLL.Mappers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Users, UserDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username))
                .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.Image));

            CreateMap<RecipesDTO, Recipes>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => ObjectId.Parse(src.Id)))
                .ForMember(
                    dest => dest.CreatorId,
                    opt => opt.MapFrom(src => ObjectId.Parse(src.CreatorId))
                )
                .ForMember(
                    dest => dest.CookingDifficulty,
                    opt => opt.MapFrom(src => src.CookingDifficulty)
                )
                .ForMember(dest => dest.CookTime, opt => opt.MapFrom(src => src.CookTime))
                .ForMember(dest => dest.RecipeType, opt => opt.MapFrom(src => src.RecipeType))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(
                    dest => dest.RecipeDescriptionElements,
                    opt => opt.MapFrom(src => src.RecipeDescriptionElements)
                )
                .ForMember(
                    dest => dest.RecipeIngradients,
                    opt => opt.MapFrom(src => src.RecipeIngradients)
                );

            CreateMap<Recipes, RecipeDetailDTO>()
                .ForMember(
                    dest => dest.RecipeDescriptionElements,
                    opt => opt.MapFrom(src => src.RecipeDescriptionElements)
                )
                .ForMember(
                    dest => dest.RecipeIngradients,
                    opt => opt.MapFrom(src => src.RecipeIngradients)
                );
        }
    }
}
