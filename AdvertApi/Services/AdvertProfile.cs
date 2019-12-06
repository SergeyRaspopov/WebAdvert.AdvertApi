using AdvertApi.Models;
using AdvertApi.Models.Messages;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvertApi.Services
{
    public class AdvertProfile : Profile
    {
        public AdvertProfile()
        {
            CreateMap<AdvertModel, AdvertDbModel>();
            CreateMap<AdvertDbModel, AdvertConfirmedMessage>();
        }
    }
}
