using TMDT.Model.Models;
using System;
using TMDT.Web.Models;
using System.Globalization;
using TMDT.Web.Models.Common;

namespace TMDT.Web.Infrastructure.Extensions
{
    public static class EntityExtensions
    {
        public static void UpdatePostCategory(this PostCategory postCategory, PostCategoryViewModel postCategoryVm)
        {
            postCategory.ID = postCategoryVm.ID;
            postCategory.Name = postCategoryVm.Name;
            postCategory.Description = postCategoryVm.Description;
            postCategory.Alias = postCategoryVm.Alias;
            postCategory.ParentID = postCategoryVm.ParentID;
            postCategory.DisplayOrder = postCategoryVm.DisplayOrder;
            postCategory.Image = postCategoryVm.Image;
            postCategory.HomeFlag = postCategoryVm.HomeFlag;

            postCategory.CreatedDate = postCategoryVm.CreatedDate;
            postCategory.CreatedBy = postCategoryVm.CreatedBy;
            postCategory.UpdatedDate = postCategoryVm.UpdatedDate;
            postCategory.UpdatedBy = postCategoryVm.UpdatedBy;
            postCategory.MetaKeyword = postCategoryVm.MetaKeyword;
            postCategory.MetaDescription = postCategoryVm.MetaDescription;
            postCategory.Status = postCategoryVm.Status;
        }
        public static void UpdateRole(this AppRole appRole, ApplicationRoleViewModel appRoleVm)
        {
            appRole.Description = appRoleVm.Description;
            appRole.Id = appRoleVm.Id;
            appRole.Name = appRoleVm.Name;
        }
        public static void UpdateProductCategory(this ProductCategory productCategory, ProductCategoryViewModel productCategoryVm)
        {
            productCategory.ID = productCategoryVm.ID;
            productCategory.Name = productCategoryVm.Name;
            productCategory.Description = productCategoryVm.Description;
            productCategory.Alias = productCategoryVm.Alias;
            productCategory.ParentID = productCategoryVm.ParentID;
            productCategory.DisplayOrder = productCategoryVm.DisplayOrder;
            productCategory.Image = productCategoryVm.Image;
            productCategory.HomeFlag = productCategoryVm.HomeFlag;
            productCategory.HomeOrder = productCategoryVm.HomeOrder;
            productCategory.CreatedDate = productCategoryVm.CreatedDate;
            productCategory.CreatedBy = productCategoryVm.CreatedBy;
            productCategory.UpdatedDate = productCategoryVm.UpdatedDate;
            productCategory.UpdatedBy = productCategoryVm.UpdatedBy;
            productCategory.MetaKeyword = productCategoryVm.MetaKeyword;
            productCategory.MetaDescription = productCategoryVm.MetaDescription;
            productCategory.Status = productCategoryVm.Status;
        }

        public static void UpdatePost(this Post post, PostViewModel postVm)
        {
            post.ID = postVm.ID;
            post.Name = postVm.Name;
            post.Description = postVm.Description;
            post.Alias = postVm.Alias;
            post.CategoryID = postVm.CategoryID;
            post.Content = postVm.Content;
            post.Image = postVm.Image;
            post.HomeFlag = postVm.HomeFlag;
            post.ViewCount = postVm.ViewCount;

            post.CreatedDate = postVm.CreatedDate;
            post.CreatedBy = postVm.CreatedBy;
            post.UpdatedDate = postVm.UpdatedDate;
            post.UpdatedBy = postVm.UpdatedBy;
            post.MetaKeyword = postVm.MetaKeyword;
            post.MetaDescription = postVm.MetaDescription;
            post.Status = postVm.Status;
        }

        public static void UpdateProduct(this Product product, ProductViewModel productVm)
        {
            product.ID = productVm.ID;
            product.Name = productVm.Name;
            product.Description = productVm.Description;
            product.Alias = productVm.Alias;
            product.CategoryID = productVm.CategoryID;
            product.Content = productVm.Content;
            product.ThumbnailImage = productVm.ThumbnailImage;
            product.Price = productVm.Price;
            product.PromotionPrice = productVm.PromotionPrice;
            product.Warranty = productVm.Warranty;
            product.HomeFlag = productVm.HomeFlag;
            product.HotFlag = productVm.HotFlag;
            product.ViewCount = productVm.ViewCount;

            product.CreatedDate = productVm.CreatedDate;
            product.CreatedBy = productVm.CreatedBy;
            product.UpdatedDate = productVm.UpdatedDate;
            product.UpdatedBy = productVm.UpdatedBy;
            product.MetaKeyword = productVm.MetaKeyword;
            product.MetaDescription = productVm.MetaDescription;
            product.Status = productVm.Status;
            product.Tags = productVm.Tags;
            product.OriginalPrice = productVm.OriginalPrice;
        }

        public static void UpdateFeedback(this Feedback feedback, FeedbackViewModel feedbackVm)
        {
            feedback.Name = feedbackVm.Name;
            feedback.Email = feedbackVm.Email;
            feedback.Message = feedbackVm.Message;
            feedback.Status = feedbackVm.Status;
            feedback.CreateDate = DateTime.Now;
        }

        public static void UpdateProductQuantity(this ProductQuantity quantity, ProductQuantityViewModel quantityVm)
        {
            quantity.ColorId = quantityVm.ColorId;
            quantity.ProductId = quantityVm.ProductId;
            quantity.SizeId = quantityVm.SizeId;
            quantity.Quantity = quantityVm.Quantity;
        }
        public static void UpdateOrder(this Order order, OrderViewModel orderVm)
        {
            order.CustomerName = orderVm.CustomerName;
            order.CustomerAddress = orderVm.CustomerAddress;
            order.CustomerEmail = orderVm.CustomerEmail;
            order.CustomerMobile = orderVm.CustomerMobile;
            order.CustomerMessage = orderVm.CustomerMessage;
            order.PaymentMethod = orderVm.PaymentMethod;
            order.CreatedDate = DateTime.Now;
            order.CreatedBy = orderVm.CreatedBy;
            order.PaymentStatus = orderVm.PaymentStatus;
            order.Status = orderVm.Status;
            order.CustomerId = orderVm.CustomerId;
            order.CreatedBy = orderVm.CreatedBy;
        }

        public static void UpdateProductImage(this ProductImage image, ProductImageViewModel imageVm)
        {
            image.ProductId = imageVm.ProductId;
            image.Path = imageVm.Path;
            image.Caption = imageVm.Caption;
        }
        public static void UpdateFunction(this Function function, FunctionViewModel functionVm)
        {
            function.Name = functionVm.Name;
            function.DisplayOrder = functionVm.DisplayOrder;
            function.IconCss = functionVm.IconCss;
            function.Status = functionVm.Status;
            function.ParentId = functionVm.ParentId;
            function.Status = functionVm.Status;
            function.URL = functionVm.URL;
            function.ID = functionVm.ID;
        }
        public static void UpdatePermission(this Permission permission, PermissionViewModel permissionVm)
        {
            permission.RoleId = permissionVm.RoleId;
            permission.FunctionId = permissionVm.FunctionId;
            permission.CanCreate = permissionVm.CanCreate;
            permission.CanDelete = permissionVm.CanDelete;
            permission.CanRead = permissionVm.CanRead;
            permission.CanUpdate = permissionVm.CanUpdate;
        }

        public static void UpdateApplicationRole(this AppRole appRole, ApplicationRoleViewModel appRoleViewModel, string action = "add")
        {
            if (action == "update")
                appRole.Id = appRoleViewModel.Id;
            else
                appRole.Id = Guid.NewGuid().ToString();
            appRole.Name = appRoleViewModel.Name;
            appRole.Description = appRoleViewModel.Description;
        }

        public static void UpdateUser(this AppUser appUser, ApplicationUserViewModel appUserViewModel, string action = "add")
        {
            appUser.Id = appUserViewModel.Id;
            appUser.FullName = appUserViewModel.FullName;
            if (!string.IsNullOrEmpty(appUserViewModel.BirthDay))
            {
                DateTime dateTime = DateTime.ParseExact(appUserViewModel.BirthDay, "dd/MM/yyyy", new CultureInfo("vi-VN"));
                appUser.BirthDay = dateTime;
            }
            appUser.BirthDay =DateTime.Parse(appUserViewModel.BirthDay);
            appUser.Email = appUserViewModel.Email;
            appUser.Address = appUserViewModel.Address;
            appUser.UserName = appUserViewModel.UserName;
            appUser.PhoneNumber = appUserViewModel.PhoneNumber;
            appUser.Gender = appUserViewModel.Gender == "True" ? true : false;
            appUser.Status = appUserViewModel.Status;
            appUser.Address = appUserViewModel.Address;
            appUser.Avatar = appUserViewModel.Avatar;
        }
        public static void UpdateColor(this Color color,ColorViewModel colorVm)
        {
            color.ID = colorVm.ID;
            color.Name = colorVm.Name;
            color.Code = colorVm.Code;
        }
        public static void UpdateSize(this Size size, SizeViewModel sizeVm)
        {
            size.ID = sizeVm.ID;
            size.Name = sizeVm.Name;
        }
        public static void UpdateError(this Error error, ErrorViewModel errorVm)
        {
            error.ID = errorVm.ID;
            error.Messeage = errorVm.Messeage;
            error.StackTrace = errorVm.StackTrace;
            error.CreateDate = errorVm.CreateDate;
        }
        public static void UpdateMenu(this Menu menu, MenuViewModel menuVm)
        {
            menu.Name = menuVm.Name;
            menu.DisplayOrder = menuVm.DisplayOrder;
            menu.IconCss = menuVm.IconCss;
            menu.Class = menuVm.Class;
            menu.Status = menuVm.Status;
            menu.ParentId = menuVm.ParentId;
            menu.Status = menuVm.Status;
            menu.URL = menuVm.URL;
            menu.ID = menuVm.ID;
        }
        public static void UpdateSlide(this Slide slide, SlideViewModel slideVm)
        {
            slide.Content = slideVm.Content;
            slide.Description = slideVm.Description;
            slide.DisplayOrder = slideVm.DisplayOrder;
            slide.ID = slideVm.ID;
            slide.Image = slideVm.Image;
            slide.Name = slideVm.Name;
            slide.Status = slideVm.Status;
            slide.Url = slideVm.Url;
        }
        public static void UpdateComment(this Comment comment, CommentViewModel commentVm)
        {
            comment.AttachImage = commentVm.AttachImage;
            comment.Content = commentVm.Content;
            comment.CreateBy = commentVm.CreateBy;
            comment.CreateDate = commentVm.CreateDate;
            comment.ID = commentVm.ID;
            comment.ModifiedDate = commentVm.ModifiedDate;
            comment.ParentId = commentVm.ParentId;
            comment.Pings = commentVm.Pings;
            comment.PostId = commentVm.PostId;
            comment.ProductId = commentVm.ProductId;
            comment.ReplyCount = commentVm.ReplyCount;
            comment.Status = commentVm.Status;
            comment.UserId = commentVm.UserId;

        }

    }
}