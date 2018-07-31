var cart = {
    init: function () {
        cart.loadData();
        cart.registerEvent();
    },
    registerEvent: function () {
        //$('#frmPayment').validate({
        //    rules: {
        //        name: "required",
        //        address: "required",
        //        email: {
        //            required: true,
        //            email: true
        //        },
        //        phone: {
        //            required: true,
        //            number: true
        //        }
        //    },
        //    messages: {
        //        name: "Yêu cầu nhập tên",
        //        address: "Yêu cầu nhập địa chỉ",
        //        email: {
        //            required: "Bạn cần nhập email",
        //            email: "Định dạng email chưa đúng"
        //        },
        //        phone: {
        //            required: "Số điện thoại được yêu cầu",
        //            number: "Số điện thoại phải là số."
        //        }
        //    }
        //});
        $('.qtyplus').off('click').on('click', function (e) {
            e.preventDefault();
            fieldName = $(this).attr('field');
            // Get its current value
            var currentVal = parseInt($('input[name=' + fieldName + ']').val());
            // If is not undefined
            if (!isNaN(currentVal)) {
                // Increment
                var number = $('input[name=' + fieldName + ']').val(currentVal + 1);
                var category = $('input[name=' + fieldName + ']').data('category');
                var productid = parseInt($('input[name=' + fieldName + ']').data('id'));
                var quantity = parseInt($('input[name=' + fieldName + ']').val());
                var price = parseFloat($('input[name=' + fieldName + ']').data('price'));
                var amount = quantity * price;
                $('#amount_' + productid).text(numeral(amount).format('0,0'));
                $('#lblTotalOrder').text(numeral(cart.getTotalOrder()).format('0,0'));
                cart.updateItem(productid, category, quantity);
            } else {
                // Otherwise put a 0 there
                $('input[name=' + fieldName + ']').val(0);
            }
        });
        $('.qtyminus').off('click').on('click', function (e) {
            e.preventDefault();
            fieldName = $(this).attr('field');
            // Get its current value
            var currentVal = parseInt($('input[name=' + fieldName + ']').val());
            // If is not undefined
            if (!isNaN(currentVal)) {
                // Increment
                var number = $('input[name=' + fieldName + ']').val(currentVal - 1);
                var category = $('input[name=' + fieldName + ']').data('category');
                var productid = parseInt($('input[name=' + fieldName + ']').data('id'));
                var quantity = parseInt($('input[name=' + fieldName + ']').val());
                var price = parseFloat($('input[name=' + fieldName + ']').data('price'));
                var amount = quantity * price;
                $('#amount_' + productid).text(numeral(amount).format('0,0'));
                $('#lblTotalOrder').text(numeral(cart.getTotalOrder()).format('0,0'));
                cart.updateItem(productid, category, quantity);
            } else {
                // Otherwise put a 0 there
                $('input[name=' + fieldName + ']').val(0);
            }
        });
        $('#btnAddToCart').off('click').on('click', function (e) {
            e.preventDefault();
            var productId = parseInt($(this).data('id'));
            var name = $("#log2").val();
            var colorId = parseInt($("#colorId").val());
            var sizeId = parseInt($("#sizeId").val());
            var soluong = parseInt($("#txtsoluong").val());
            cart.addItem(productId, name, colorId, sizeId, soluong);
        });
        $('.btnDeleteItem').off('click').on('click', function (e) {
            e.preventDefault();
            var productId = parseInt($(this).data('id'));
            cart.deleteItem(productId);
        });
        $('#btnDeleteAll').off('click').on('click', function (e) {
            e.preventDefault();
            cart.deleteAll();
        });
        $('.txtQuantity').off('keyup').on('keyup', function () {
            var quantity = parseInt($(this).val());
            var productid = parseInt($(this).data('id'));
            var price = parseFloat($(this).data('price'));
            if (isNaN(quantity) == false) {

                var amount = quantity * price;

                $('#amount_' + productid).text(numeral(amount).format('0,0'));
            }
            else {
                $('#amount_' + productid).text(0);
            }

            $('#lblTotalOrder').text(numeral(cart.getTotalOrder()).format('0,0'));

        })
        $('#btnContinue').off('click').on('click', function (e) {
            e.preventDefault();
            window.location.href = "/";
        });
        $('#chkUserLoginInfo').off('click').on('click', function (e) {
            if ($(this).prop('checked'))
                cart.getLoginUser();
            else {
                $('#txtName').val('');
                $('#txtAddress').val('');
                $('#txtEmail').val('');
                $('#txtPhone').val('');
            }
        });
        $('#btnCreateOrder').off('click').on('click', function (e) {
            e.preventDefault();
            //var isValid = $('#frmPayment').valid();
            //if (isValid) {
                cart.createOrder();
            //}

        });
    },
    getTotalOrder: function () {
        var listTextBox = $('.txtQuantity');
        var total = 0;
        $.each(listTextBox, function (i, item) {
            total += parseInt($(item).val()) * parseFloat($(item).data('price'));
        });
        return total;
    },
    addItem: function (productId,name,colorId,sizeId,soluong) {
        $.ajax({
            url: '/ShoppingCart/Add',
            data: {
                productId: productId,
                name: name,
                colorId: colorId,
                sizeId:sizeId,
                soluong:soluong
            },
            type: 'POST',
            dataType: 'json',
            success: function (response) {
                if (response.status) {
                    var data = response.data;
                    alert('Thêm sản phẩm thành công.');
                    $('#lblCount').text(data);
                    $('#lblCartItem').text(data +" sản phẩm");
                }
            }
        });
    },
    updateItem:function(productId,name,quantity){
        $.ajax({
            url: '/ShoppingCart/UpdateItem',
            data: {
                productId: productId,
                name: name,
                quantity:quantity
            },
            type: 'POST',
            dataType: 'json',
            success: function (response) {
                if (response.status) {
                    cart.loadData();
                }
            }
        });
    },
    deleteItem: function (productId) {
        $.ajax({
            url: '/ShoppingCart/DeleteItem',
            data: {
                productId: productId
            },
            type: 'POST',
            dataType: 'json',
            success: function (response) {
                if (response.status) {
                    var data = response.data;
                    cart.loadData();
                    $('#lblCount').text(data);
                    $('#lblCartItem').text(data +" sản phẩm");
                }
            }
        });
    },
    deleteAll: function () {
        $.ajax({
            url: '/ShoppingCart/DeteleAll',
            type: 'POST',
            dataType: 'json',
            success: function (response) {
                if (response.status) {
                    cart.loadData();
                }
            }
        });
    },
    loadData: function () {
        $.ajax({
            url: '/ShoppingCart/GetAll',
            type: 'GET',
            dataType: 'json',
            success: function (res) {
                if (res.status) {
                    var template = $('#tplCart').html();
                    var html = '';
                    var data = res.data;
                    $.each(data, function (i, item) {
                        html += Mustache.render(template, {
                            ProductId: item.ProductId,
                            ProductName: item.Product.Name,
                            Category:item.Category,
                            Size:item.SizeId,
                            Color:item.ColorId,
                            Image: item.Product.ThumbnailImage,
                            Price: item.Product.OriginalPrice,
                            PriceF: numeral(item.Product.OriginalPrice).format('0,0'),
                            Quantity: item.Quantity,
                            Amount: numeral(item.Quantity * item.Product.OriginalPrice).format('0,0')
                        });
                    });

                    $('#cartBody').html(html);
                    $('#lblTotalOrder').text(numeral(cart.getTotalOrder()).format('0,0'));
                    cart.registerEvent();
                }
            }
        })
    },
    getLoginUser: function () {
        $.ajax({
            url: '/ShoppingCart/GetUser',
            type: 'POST',
            dataType: 'json',
            success: function (response) {
                if (response.status) {
                    var user = response.data;
                    $('#txtName').val(user.FullName);
                    $('#txtAddress').val(user.Address);
                    $('#txtEmail').val(user.Email);
                    $('#txtPhone').val(user.PhoneNumber);
                }
            }
        });
    },
    createOrder: function () {
        var order = {
            CustomerName: $('#txtName').val(),
            CustomerEmail: $('#txtEmail').val(),
            CustomerAddress: $('#txtAddress').val(),
            CustomerMobile: $('#txtPhone').val(),
            CustomerMessage: $('#txtMessage').val(),
            PaymentMethod: $('#txtPaymentMethod').val(),
            Status: false,
            StatusUser: false
        };
        $.ajax({
            url: '/ShoppingCart/CreateOrder',
            type: 'POST',
            dataType: 'json',
            data: {
                orderVM: JSON.stringify(order)
            },
            success: function (response) {
                if (response.status) {
                    cart.deleteAll();
                    setTimeout(function () {
                        $('#cartContent').html('Cảm ơn bạn đã đặt hàng thành công. Chúng tôi sẽ liên hệ sớm nhất.');
                    }, 2000);
                }
            }
        });
    }
}
cart.init();