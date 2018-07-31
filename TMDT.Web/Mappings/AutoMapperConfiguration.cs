using AutoMapper;
using TMDT.Model.Models;
using TMDT.Web.Models;
using TMDT.Web.Models.Common;

namespace TMDT.Web.Mappings
{
    public class AutoMapperConfiguration
    {
        public static void Configure()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<Post, PostViewModel>();
                cfg.CreateMap<PostCategory, PostCategoryViewModel>();
                cfg.CreateMap<Tag, TagViewModel>();
                cfg.CreateMap<ProductCategory, ProductCategoryViewModel>(); 
                cfg.CreateMap<Product, ProductViewModel>();
                cfg.CreateMap<ProductTag, ProductTagViewModel>();
                cfg.CreateMap<Footer, FooterViewModel>();
                cfg.CreateMap<Slide, SlideViewModel>();
                cfg.CreateMap<Page, PageViewModel>();
                cfg.CreateMap<ContactDetail, ContactDetailViewModel>();
                cfg.CreateMap<AppRole, ApplicationRoleViewModel>();
                cfg.CreateMap<AppUser, ApplicationUserViewModel>();
                cfg.CreateMap<Function, FunctionViewModel>();
                cfg.CreateMap<Permission, PermissionViewModel>();
                cfg.CreateMap<ProductImage, ProductImageViewModel>();
                cfg.CreateMap<ProductQuantity, ProductQuantityViewModel>();
                cfg.CreateMap<Color, ColorViewModel>();
                cfg.CreateMap<Size, SizeViewModel>();
                cfg.CreateMap<Order, OrderViewModel>();
                cfg.CreateMap<OrderDetail, OrderDetailViewModel>();
                cfg.CreateMap<Announcement, AnnouncementViewModel>();
                cfg.CreateMap<AnnouncementUser, AnnouncementUserViewModel>();
                cfg.CreateMap<Error, ErrorViewModel>();
                cfg.CreateMap<Menu, MenuViewModel>();
                cfg.CreateMap<Comment, CommentViewModel>();
                cfg.CreateMap<CommentVote, CommentVoteViewModel>();
            });
        }
    }
}