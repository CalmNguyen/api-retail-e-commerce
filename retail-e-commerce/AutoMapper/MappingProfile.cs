using AutoMapper;
using Newtonsoft.Json;
using retail_e_commerce.DTOs.Product;
using retail_e_commerce.Entities;

namespace retail_e_commerce.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Product -> ListItem
            CreateMap<Product, ProductListItemDto>()
                .ForMember(d => d.CategoryName, opt => opt.MapFrom(s => s.Category.Name))
                .ForMember(d => d.BrandName, opt => opt.MapFrom(s => s.Brand != null ? s.Brand.Name : null));

            // ProductVariant <-> ProductVariantDto
            CreateMap<ProductVariant, ProductVariantDto>().ReverseMap();

            // Product -> ProductDetailDto
            CreateMap<Product, ProductDetailDto>()
                .ForMember(d => d.ExtraAttributes, opt => opt.MapFrom(s =>
                    string.IsNullOrEmpty(s.ExtraAttributesJson)
                        ? null
                        : JsonConvert.DeserializeObject<Dictionary<string, string>>(s.ExtraAttributesJson)))
                .ForMember(d => d.ImageUrls, opt => opt.MapFrom(s => s.Images.Select(i => i.Url)))
                .ForMember(d => d.RowVersion, opt => opt.MapFrom(s => Convert.ToBase64String(s.RowVersion)));

            // ProductCreateUpdateDto -> Product
            CreateMap<ProductCreateUpdateDto, Product>()
                .ForMember(d => d.ExtraAttributesJson, opt => opt.MapFrom(s =>
                    s.ExtraAttributes == null
                        ? null : JsonConvert.SerializeObject(s.ExtraAttributes)))
                .ForMember(d => d.Images, opt => opt.Ignore())
                .ForMember(d => d.Variants, opt => opt.Ignore());
        }
    }
}
