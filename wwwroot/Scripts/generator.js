// Object lưu cấu hình của trang báo
var jsonGenerator = new Object();
// Danh sách các thuộc tính của thư viện openScaping
var arr = ["_xpath", "_removeTags", "attr", "_transformations"];
var arr1 = ["_xpath", "_removeTags", "attr", "_transformations", "text", "link"];
// Danh sách các thuộc bóc tách chi tiết bài viết
var listTypeDate = ["title_path", "meta_description", "time_path", "content_path", "categories", "tags", "author", "comments_count"];
// Danh sách các thuộc tính được recommend trong thẻ meta của trang báo
var listTypeGetFirstElement = ["title_path", "meta_description", "time_path", "content_path"];
var domain = "";
// Loại dữ liệu càn cấu hình
var type = 0;
var javascript = "";
var id = 0;

//var urlIframe = location.origin + location.pathname.replace("/Crawl", "/html");
// Nhúng Iframe gọi đến /Home/html để lấy ra nội dung của trang báo cấu hình sau khi tiền xử lý
var urlIframe = location.origin + "/Home/html";

//Thay đổi url của iframe
function setUrl(e) {
    urlMain = e;
    if (type == 3)
        recommendDataRegex();
}

// Bẳt sực kiện khi thay chọn một Type khác
$("#Type").change(function () {
    changeAttrType();
    funcionClick();
});

// Xóa thuộc tính của class .typedata. Thêm thuộc tính mới của loại đã chọn
// 0: Loại dữ liệu là trang chi tiết
// 1,2: Loại dữ liệu là link của link bài viết/link chủ đề
// 3: Cấu hình lấy ID của bài viết(nếu có)
function changeAttrType() {
    $(".valueOption").remove();
    $('.selectAttributeCss').remove();
    var type = $("#Type").val();

    switch (type) {
        case '3':
            $(".form-control.typedata").prepend("<option class='valueOption' value='id'>ID</option>");
            $(".form-control.typedata").val("id");
            $("#labelName").html("Nhập Regex lấy ID của bài viết:");
            $('.allPropperty').attr("disabled", "disabled");
            break;
        case '0':
            $(".form-control.typedata").prepend("<option class='valueOption' value='keywords'>Keywords</option>");
            $(".form-control.typedata").prepend("<option class='valueOption' value='comments_count'>Comment count</option>");
            $(".form-control.typedata").prepend("<option class='valueOption' value='author'>Author</option>");
            $(".form-control.typedata").prepend("<option class='valueOption' value='tags'>Tags</option>");
            $(".form-control.typedata").prepend("<option class='valueOption' value='categories'>Categories</option>");
            $(".form-control.typedata").prepend("<option class='valueOption' value='content_path'>Content path</option>");
            $(".form-control.typedata").prepend("<option class='valueOption' value='time_path'>Time path</option>");
            $(".form-control.typedata").prepend("<option class='valueOption' value='meta_description'>Meta description</option>");
            $(".form-control.typedata").prepend("<option class='valueOption' value='title_path'>Title path</option>");
            $(".form-control.typedata").val("title_path");
            $("#labelName").html("Nhập bộ chọn Css:");
            $('.allPropperty').removeAttr("disabled");
            break;
        default:
            $(".form-control.typedata").prepend("<option class='valueOption' value='link'>Link</option>");
            $(".form-control.typedata").val("link");
            $('.form-control.attribute').append("<option class='selectAttributeCss' value='href'>Href</option>");
            $("#labelName").html("Nhập bộ chọn Css:");
            $('.allPropperty').removeAttr("disabled");
            break;

    }
}

// Reload loại iframe khi thay đổi Javascript:
$("#Javascript").change(function () {
    funcionClick();
});

// Hàm bắt sự kiện click vào các trường đã được cấu hình, Lấy ra css của trường đó và highlight phần tử đó lên trong trang web
function clickShowContent(e) {
    var list = new Array();

    // Lấy ra danh sách các trường trong select "Loại dữ liệu"
    $(".form-control.typedata option").each(function () {
        list.push($(this).val());
    });

    // Xóa ô nhập loại dữ liệu khác nếu chọn loại dữ liệu là "Khác"
    $("#typedata-difference").remove();

    // Lấy ra tên của trường 
    var id = $(e).attr('id');
    // Kiểm tra xem trường vừa click trong thuộc trong danh sách loại dữ liệu được định nghĩa hay không
    if (list.indexOf(id) > -1) {
        // Nếu thuộc loại định nghĩ thì cập nhật giá trị của select thành loại vừa chọn
        $(".form-control.typedata").val(id);
        if (id == "difference") {
            $(".div-typedata").append("<input class='form-control ml-auto' placeholder='Nhập loại dữ liệu' id='typedata-difference' type='text'>");
            $("#typedata-difference").val(id);
        }
    } else {
        // Thêm loại dữ liệu là không nằm trong danh sách định nghĩa thì chuyển select thành loại Khác và thêm ô nhập loại dữ liệu
        $(".form-control.typedata").val("difference");
        $(".div-typedata").append("<input class='form-control ml-auto' placeholder='Nhập loại dữ liệu' id='typedata-difference' type='text'>");
        $("#typedata-difference").val(id);
    }
    // Lấy giá trị của cssSelector của dòng đã chọn và cập nhật lại trên input Nhập bộ chọn Css
    $("#myCssSelectorTest").val(jsonGenerator[id]._xpath);

    // Xóa các thuộc tính loại trừ hiện có
    $(".form-control.exclude").remove();
    // Xóa các thuộc tính khác hiện có
    $(".input-group.entityproperty").remove();

    // Bổ sung các thuộc tính loại trừ của cấu hình đang chọn vào các input trong exclude
    if (jsonGenerator[id]._removeTags != null) {
        jsonGenerator[id]._removeTags.forEach(function (e) {
            $(".form-group.exclude").append("<div class='input-group'><input value=\"" + e + "\" type=\"text\" class=\"form-control form-control-sm exclude\" style=\"margin-top:5px;\"><em onclick='deleteInput(this);' class='fas fa-trash-alt trash-delete'></em></div>");
        });
    }
    // Bổ sung thêm vào các thuộc tính khác
    if (jsonGenerator[id]["_transformations"] != null) {
        Object.keys(jsonGenerator[id]["_transformations"][0]).forEach(function (k) {
            $(".form-group.property").append("<div id='" + k + "' class='input-group entityproperty'><input placeholder='Thuộc tính' type=\"text\" value='" + k + "' class=\"col-md-4 form-control form-control-sm\" disabled><input placeholder='Giá trị' type=\"text\" value='" + jsonGenerator[id]["_transformations"][0][k] + "' class=\"col-md-8 form-control form-control-sm\" ></div>");
        });
    }
    // Nếu thuộc tính đang chọn là tags hoặc categories, bổ sung vào ô thuộc tính khác các thuộc tính để bắt link/text của đường dẫn tags/categories
    if (id == "tags" || id == "categories" && jsonGenerator[id].attr == "html") {
        $(".form-group.property").append("<div id='text' class='input-group entityproperty'><input placeholder='Thuộc tính' type=\"text\" value='link' class=\"col-md-4 form-control form-control-sm\" disabled><input placeholder='Giá trị' type=\"text\" value='" + jsonGenerator[id]["link"]["_xpath"] + "' class=\"col-md-8 form-control form-control-sm\" ></div>");
        $(".form-group.property").append("<div id='text' class='input-group entityproperty'><input placeholder='Thuộc tính' type=\"text\" value='text' class=\"col-md-4 form-control form-control-sm\" disabled><input placeholder='Giá trị' type=\"text\" value='" + jsonGenerator[id]["text"]["_xpath"] + "' class=\"col-md-8 form-control form-control-sm\" ></div>");
        Object.keys(jsonGenerator[id]).forEach(function (k) {
            if (arr1.indexOf(k) == -1) {
                $(".form-group.property").append("<div class='input-group entityproperty'><input placeholder='Thuộc tính' type=\"text\" value=\"" + k + "\" class=\"col-md-4 form-control form-control-sm\" ><input placeholder='Giá trị' type=\"text\" value=\"" + jsonGenerator[id][k] + "\" class=\"col-md-8 form-control form-control-sm\" ><em onclick='deleteInput(this);' class='fas fa-trash-alt trash-delete-all'></em></div>");
            }
        });
    } 
    // Nếu thuộc tính đang chọn là link set giá thuộc tính mặc định là href
    else if (id == "link") {
        $('.selectAttributeCss').remove();
        $('.form-control.attribute').append("<option class='selectAttributeCss' value='href'>Href</option>");
    }
    else
        // Lấy ra tất cả các thuộc tính theo id thêm vào trường thuộc tính khác
        Object.keys(jsonGenerator[id]).forEach(function (k) {
            if (arr.indexOf(k) == -1) {
                $(".form-group.property").append("<div class='input-group entityproperty'><input placeholder='Thuộc tính' type=\"text\" value=\"" + k + "\" class=\"col-md-4 form-control form-control-sm\" ><input placeholder='Giá trị' type=\"text\" value=\"" + jsonGenerator[id][k] + "\" class=\"col-md-8 form-control form-control-sm\" ><em onclick='deleteInput(this);' class='fas fa-trash-alt trash-delete-all'></em></div>");
            }
        });
    // Chuyển nút tạo thành cập nhật
    $("#createTableJson").html("Cập nhật");
    //handle nút click cập nhật thành function updateTable
    $("#createTableJson").attr("onclick", "updateTable();");
    if ($("#Type").val() == 0)
        changeAttr(jsonGenerator[id]._xpath);
    $(".form-control.attribute").val(jsonGenerator[id].attr);
    // Lấy nội dung của phần tử đang chọn
    getContent(false);
};

// Cập nhật lại phần tử đã cấu hình trong danh sách
function updateTable() {
    // Lấy ra loại dữ liệu
    var radio = $(".form-control.typedata").val();

    // Nếu loại dữ liệu là difference. Lấy ra loại dữ liệu trong input typedata-difference
    if (radio == "difference") {
        radio = $("#typedata-difference").val();
    }
    if (radio != null && radio.trim() != "") {
        // Xóa loại dữ liệu hiện tại trong mảng
        delete jsonGenerator[radio];
        // Bổ sung lại loại dữ liệu sau khi cập nhật
        setNewType(radio);
        //$(".form-control.typedata").removeAttr('disabled');
        //$("#typedata-difference").removeAttr('disabled');
        //$("#createTableJson").html("Tạo");
        //$("#createTableJson").attr("onclick", "createTable();");
        // Kích hoạt sự kiện change dữ liệu js
        $(".typedata").change();
    } else
        alert("Loại dữ liệu không được bỏ trống");
}
//Hủy thay đổi. Set dữ liệu về như cũ
function cancelTable() {
    $("#ListData #" + $(".typedata").val()).click();
}

// Hàm chuyển đổi lại dữ liệu lấy từ database(strỉng) thành object trên giao diện
function convertData(e) {
    // Parse cấu hình từ string sang object
    jsonGenerator = JSON.parse(e);
    // Lấy ra tất cả các thuộc tính có trong object
    Object.keys(jsonGenerator).forEach(function (k) {
        // Thêm thuộc tính attr cho mỗi loại dữ liệu. Được cắt bới /@ ở cuối css Selector
        jsonGenerator[k].attr = jsonGenerator[k]._xpath.split("/@")[1];
        // Xóa thuộc tính ở cuối css selector trong _xpath
        jsonGenerator[k]._xpath = jsonGenerator[k]._xpath.split("/@")[0];
        // THêm các thuộc tính vào khung đã chọn trên giao diện
        var div = "<tr><td class='col-md-11 myHover' onclick='clickShowContent(this);'  id='" + k + "'>" + k;
        div += "</td><td class='col-md-1'><i class='fa fa-trash' aria-hidden='true' data-toggle='tooltip' title='Xóa!' onclick='funcionRemove($(this));'></i></td></tr>";
        $("#ListData tbody").append(div);
    });
    // Xử lý riêng loại dữ liệu time. Bổ sung thuộc tính _transformations vào regex time
    if (jsonGenerator["time_path"] != null && jsonGenerator["time_path"]["_transformations"] != null)
        delete jsonGenerator["time_path"]["_transformations"][0]._type;
    // XỬ lý riếng trường comments_count. Bổ sung thêm bộ regex và group của regex
    if (jsonGenerator["comments_count"] != null && jsonGenerator["comments_count"]["_transformations"] != null)
        delete jsonGenerator["comments_count"]["_transformations"][0]._type;
}

// CHuyển dữ liệu từ object trên giao diện về chuỗi stỉng json để lưu vào database
function parseData() {
    // clone object cấu hình
    var json = clone(jsonGenerator);
    // Quét tất cả các trường để parse lại
    Object.keys(json).forEach(function (k) {
        // Xóa thuộc tính attr và thêm vào đuôi thuộc tính _xpath. Ngăn cách bằng /@
        if (json[k].attr != null) {
            json[k]._xpath += "/@" + json[k].attr;
            delete json[k].attr;
        }
    });

    // Nếu thuộc tính là time_path. Bổ sung cấu hình regex vào thuộc tính _transformations. Loại tranform datetime là DateTimeTransformation
    if (json["time_path"] != null) {
        if (json["time_path"]["_transformations"] != null)
            json["time_path"]["_transformations"][0]._type = "DateTimeTransformation";
    }
    //Nếu thuộc tính là comments_count.Bổ sung cấu hình regex, group vào thuộc tính _transformations.Loại tranform comments_count là TotalCountTransformation
    if (json["comments_count"] != null) {
        if (json["comments_count"]["_transformations"] != null)
            json["comments_count"]["_transformations"][0]._type = "TotalCountTransformation";
    }
    return json;
}

// Mở modal
function editJson() {
    $('#myModal').modal();
}

// Khởi tạo tooltip bs
$('.tooltip1').tooltip();

// Lấy phần từ iframe trong trang cấu hình (Hiển thị ra nội dung của trang báo)
var iframe = $("#crawlDATA");

// Nhấn enter để load lại iframe
$("#meataData").keyup(function (event) {
    event.preventDefault();
    if (event.keyCode === 13) {
        funcionClick();
    }
});

// Không dùng
$.fn.xpathEvaluate = function (xpathExpression) {
    $this = this.first();
    xpathResult = this[0].evaluate(xpathExpression, this[0], null, XPathResult.ORDERED_NODE_ITERATOR_TYPE, null);
    result = [];
    while (elem = xpathResult.iterateNext()) {
        result.push(elem);
    }
    $result = jQuery([]).pushStack(result);
    return $result;
}

// Lấy nội dung trong trang theo bộ chọc selector
function getContent(isChange) {
    if (isChange && $("#Type").val() == 0) {
        var value = $(".attribute").val();
        changeAttr($("#myCssSelectorTest").val());
        $(".attribute").val(value);
    }
    iframe.contents().find("#divShowInfoTag").hide();
    var value = $("#myCssSelectorTest").val();
    // Xóa class myClass
    iframe.contents().find(".myClass").removeClass("myClass");
    //var xpath = iframe.contents().xpathEvaluate(value);
    try {
        var xpath = $(value, iframe.contents());
        xpath.addClass("myClass");
        var attr = $(".form-control.attribute").find(":selected").val();
        var content = "";
        if (attr != null) {
            if (attr == 'html' || attr == 'text')
                content = iframe.contents().find(".myClass").html();
            else
                content = iframe.contents().find(".myClass").attr(attr);
        }
        $("#myContentValue").val(content);
    } catch (err) {
    }
}


$("#myCssSelectorTest").change(function () {
    // Lấy nội dung theo bộ chọn
    getContent(true);
    // Trim bộ chọn
    $(this).val($(this).val().trim());
});

// Lấy ra danh sách các thuộc tính của element trên DOM
function changeAttr(value) {
    //var xpath = iframe.contents().xpathEvaluate(value);
    // Lấy ra bộ chọn của phần từ
    var xpath = $(value, iframe.contents());
    // Xóa tất cả các thuộc tính trong select
    $('.selectAttributeCss').remove();
    var tailieu = $('.form-control.attribute');
    var value = "";
    // Duyệt tất cả các thuộc tính có trong element(trừ id, class)
    xpath.first().each(function () {
        $.each(this.attributes, function () {
            if (this.specified && this.name != 'id' && this.name != 'class') {
                value = this.value;
                if (value.length > 50)
                    value = value.substring(0, 49);
                // Bổ sung thuộc tính vào selection chọn thuộc tính
                var attr = "<option class='selectAttributeCss' value='" + this.name + "'>" + this.name + " : " + value + "</option>";
                tailieu.append(attr);
            }
        });
    });
}

// Hàm xóa loại dữ liệu đã cấu hình
function funcionRemove(e) {
    var id = e.parent().prev().attr('id');
    delete jsonGenerator[id];
    e.parent().parent().html("");
    $(".typedata").change();
}

// Cập nhật tham số url theo url của báo đang cấu hình
function update_url(url) {
    history.pushState(null, null, '?url=' + encodeURIComponent($("#meataData").val()) + "&type=" + $("#Type").val() + "&javascript=" + $("#Javascript").val());
}

// Hàm để load trang báo cấu hình
function funcionClick() {
    // Lấy url của báo nhập vào trên trang cấu hình
    $("#meataData").val($("#meataData").val().trim());
    // Trim url
    var urltmp = $("#meataData").val().trim();
    if (urltmp != urlMain || $("#Type").val() != type || $("#Javascript").val() != javascript) {
        // Nếu url không thay đổi, không tải lại trang
        if (urltmp != urlMain || $("#Javascript").val() != javascript) {
            // Nếu url nhập vào không có http, bổ sung http vào đầu của url
            if (!urltmp.startsWith("http")) {
                urltmp = "http://" + urltmp;
            }
            // Thay đổi src của iframe để load lại trang
            iframe.attr('src', urlIframe + '?url=' + encodeURIComponent(urltmp) + "&javascript=" + $("#Javascript").val());
            // Ghi đè lại url lên input nhật url
            $("#meataData").val(urltmp);
            javascript = $("#Javascript").val();
        }
        // Khởi tạo object cấu hình mới
        jsonGenerator = new Object();
        domain = urltmp.split('/').length > 2 ? urltmp.split('/')[2].replace("www.", "") : "";
        update_url();
        clearData();
        getData();
    }
};

// Hàm gọi api để kiểm tra xem domain đã được cấu hình hay chưa.
// Nếu chưa thì recommend các loại dữ liệu dựa trên thẻ meta
// Nếu đã có dữ liệu thì parse lại cấu hình từ json string sang object trên giao diện
function getData() {
    // Lấy ra loại của cấu hình
    var typeConfig = $("#Type").val();
    // Xóa các button trong  phần đã chọn
    $(".updateData , .createData, .deletaData").remove();
    // Clear danh đã chọn trên giao diện
    $("table#ListData tbody tr").remove();
    // GỌi api lấy cấu hình dựa theo domain và loại cấu hình
    $.ajax({
        type: "GET",
        url: "./api/get",
        data: {
            "url": domain,
            "type": typeConfig
        }, error: function () {
            alert("Can't get data from database!")
        },
        success: function (data) {
            // XỬ lý nếu có cấu hình trang báo trên db
            if (data != null) {
                // remove các nút trong phần đã chọn
                $(".updateData , .createData, .deletaData").remove();
                // Xóa danh sách đã chọn trên giao diện
                $("table#ListData tbody tr").remove();
                // Xử lý nếu loại cấu hình = 3(Bóc tách id của bài viết).
                if (typeConfig == 3) {
                    // Khở tạo lại trường cấu hình tạm trên js
                    jsonGenerator = new Object();

                    // Parse cấu hình sang object
                    var newType = JSON.parse(data.Content);
                    //newType._xpath = data.Content;
                    //newType.attr = 'text';
                    // Set loại dữ liệu id cho cấu hình
                    jsonGenerator.id = newType;
                    // Bổ sung cấu hình vào phần đã chọn
                    var div = "<tr><td class='col-md-11 myHover' onclick='clickShowContent(this);' id='id'>id</td> <td class='col-md-1'><i class='fa fa-trash' aria-hidden='true' data-toggle='tooltip' title='Xóa!' onclick='funcionRemove($(this));'></i></td></tr >";
                    $("#ListData tbody").append(div);
                }
                // Nếu loại cấu hình != 3. Gọi gàm convertData để parse cấu hình
                else
                    convertData(data.Content);

                if ($("#meataData").val() == urlMain) {
                    $(".typedata").change();
                }

                id = data.Id;
                type = data.Type;
                // Bổ sung nút xóa, cập nhật domain
                $(".createUpdateData").prepend("<button class='btn btn-danger btn-sm deletaData ml-auto' onclick='deleteData();'>Xóa Domain</button>");
                $(".createUpdateData").prepend("<button class='btn btn-primary btn-sm updateData' onclick='updateData();'>Cập nhật Domain</button>");
            } else {
                // Bổ sung nút Lưu Domian nếu không có cấu hình
                $(".createUpdateData").prepend("<button class='btn btn-primary btn-sm createData' onclick='createData();'>Lưu Domain</button>");
                // Gọi hàm recommend các trường dữ liệu theo thẻ meta
                if ($("#meataData").val() == urlMain) {
                    recommendData();
                } else {
                    $('#crawlDATA').one('load', function () {
                        recommendData();
                    });
                }

                type = $("#Type").val();
            }
            urlMain = $("#meataData").val();
        },
        timeout: 5000
    });
}

// Hàm test để lấy cấu hình đã chọn lấy dữ liệu để hiển thị xem có đúng không
function testData() {
    // Kiêm tra xem loại dữ liệu có phải =3 (Cấu hình id của bài viết)
    var isID = false;
    var body;
    if ($("#Type").val() == 3) {
        body = JSON.stringify(jsonGenerator.id);
        isID = true;
    }
    else
        body = JSON.stringify(parseData());
    // Lấy ra url của bài báo
    var url = $("#meataData").val();
    // Tạo object lưu thông tin dữ liệu
    var data = new Object();
    data.url = url;
    data.data = body;
    data.javascript = $("#Javascript").val();
    data.isID = isID;

    // GỌi api để test dữ liệu
    $.ajax({
        url: "./Home/JsonResult",
        type: "POST",
        data: JSON.stringify(data),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (json) {
            // Nếu thành công. Hiển thị modal dữ liệu trả về
            $("#myData").modal();
            $("#url").val($("#meataData").val());
            $("#data").val(JSON.stringify(body, undefined, 2));
            $("#result tbody tr").remove();
            var result = JSON.parse(json);
            Object.keys(result).forEach(function (k) {
                if (k != "content_path") {
                    if (result[k] instanceof Array) {
                        var kq = "";
                        if (listTypeGetFirstElement.indexOf(k) > -1) {
                            if (result[k] instanceof Object)
                                kq = JSON.stringify(result[k][0]);
                            else
                                kq = result[k][0];
                        }
                        else {
                            var arrays = new Array();
                            result[k].forEach(function (e) {
                                if (e instanceof Object)
                                    kq += JSON.stringify(e) + "<br>";
                                else {
                                    if (arrays.indexOf(e) < 0) {
                                        kq += e + "<br>";
                                        arrays.push(e);
                                    }
                                }
                            });
                        }
                        $("#result tbody").append("<tr><td>" + k + "</td><td>" + kq + "</td></tr>");
                    } else if (result[k] instanceof Object)
                        $("#result tbody").append("<tr><td>" + k + "</td><td>" + JSON.stringify(result[k]) + "</td></tr>");
                    else
                        $("#result tbody").append("<tr><td>" + k + "</td><td>" + result[k] + "</td></tr>");
                } else {
                    if (result[k] instanceof Array)
                        var content = result[k][0];
                    else
                        var content = result[k];
                    $("#result tbody").append("<tr><td>" + k + "</td><td><textarea class='form-control ml-auto' rows='10' id='content_path' disabled></textarea><button type='button' onclick='viewHtml();' class='btn btn-sm btn-default'>View html</button></td></tr>");
                    $("#result tbody #content_path").val(content);
                }
            });
        }
    })
}

// Hàm để tạo loại dữ liệu mới
function createTable() {
    // Lấy dữ liệu trong input chọn css
    var text = $("#myCssSelectorTest").val();
    if (text != "") {
        // Lấy thuộc tính của dữ liệu
        var radio = $(".typedata").val();
        if (radio == "difference") {
            // Nếu loại dữ liệu là  difference. Lấy tên loại dữ liệu trong typedata-difference
            radio = $("#typedata-difference").val();
        }
        if (radio != null && radio.trim() != "") {
            //  Kiêm tra xem loại dữ liệu đã có chưa
            var check = $("#ListData #" + radio);
            if (check.length == 0) {
                // Thêm thuộc tính mới vào list đã chọn
                var div = "<tr><td class='col-md-11 myHover' onclick='clickShowContent(this);'  id='" + radio + "'>" + radio;
                div += "</td><td class='col-md-1'><i class='fa fa-trash' aria-hidden='true' data-toggle='tooltip' title='Xóa!' onclick='funcionRemove($(this));'></i></td></tr>";
                $("#ListData tbody").append(div);
                // Set trường mới vào object lưu cấu hình
                setNewType(radio);
                if ($("#Type").val() == 0) {
                    for (var i = 0; i < listTypeDate.length; i++) {
                        if (!(listTypeDate[i] in jsonGenerator)) {
                            $(".typedata").val(listTypeDate[i]);
                            break;
                        }
                    };
                }
                $(".typedata").change();

            } else
                alert("Loại dữ liệu đã tồn tại");
        }
        else
            alert("Loại dữ liệu không được bỏ trống");
    } else
        // Nếu input rỗng. Hiển thị thông báo lỗi
        alert("Xpath không được bỏ trống");
}

// Thêm trường mới vào object jsonGenerator
function setNewType(e) {
    // Xử lý nếu loại cấu hình = 3
    if ($("#Type").val() == 3) {
        var newType = new Object();
        // set _xpath là bộ chọn regex
        newType._xpath = $("#myCssSelectorTest").val();
        // Bổ sung các thuộc tính khác
        var properties = $(".form-group.property .input-group.entityproperty");
        if (properties.length > 0)
            properties.each(function () {
                var ppties = $(this).children();
                if (ppties[0].value != "" && ppties[1].value != "") {
                    if (arr.indexOf(ppties[0].value) == -1)
                        newType[ppties[0].value] = ppties[1].value;
                }
            });
        jsonGenerator[e] = newType;
    } else {
        // XỬ lý các loại cấu hình khác
        var newType = new Object();
        // Set _xpath là bộ chọn cssSelector
        newType._xpath = $("#myCssSelectorTest").val();
        // Chọn thuộc tính cần lấy của elment
        newType.attr = $(".form-control.attribute").find(":selected").val();
        // Lấy ra danh sách cấu hình exclude(loại trừ)
        var excludes = $(".form-control.exclude");
        // Lấy ra danh sách các thuộc tính khác
        var properties = $(".form-group.property .input-group.entityproperty");
        var list = [];
        // Thêm các thuộc tính exclude vào danh sách
        if (excludes.length > 0)
            excludes.each(function () {
                if ($(this).val() != "")
                    list.push($(this).val());
            });
        // Xử lý nếu loại dữ liệu là time_path
        if (e == "time_path") {
            var tranform = new Object();
            // Bổ sung 3 loại thuộc tính khác là date_regex, time_regex và datetime_format vào cấu hình tranform
            tranform[properties[0].children[0].value] = properties[0].children[1].value;
            tranform[properties[1].children[0].value] = properties[1].children[1].value;
            tranform[properties[2].children[0].value] = properties[2].children[1].value;
            newType["_transformations"] = [tranform];
            properties[0].remove();
            properties[1].remove();
            properties[2].remove();

        }
        // Xử lý với trường hợp loại dữ liệu là comments_count
        else if (e == "comments_count") {
            var tranform = new Object();
            // Bổ sung 2 lọai thuộc tính khác là total_comments_regex và group_number vào cấu hình transform
            tranform[properties[0].children[0].value] = properties[0].children[1].value;
            tranform[properties[1].children[0].value] = properties[1].children[1].value;
            newType["_transformations"] = [tranform];
            properties[0].remove();
            properties[1].remove();
        }
            // Xử lý với các loại dữ liệu tags, categories dùng thuộc tính là html (bộ chọn cho link)
        else if (e == "tags" || e == "categories" && $(".attribute").val() == "html") {
            var obj = new Object();
            // Set bộ chọn css vào thuộc tính _xpath
            obj._xpath = properties[0].children[1].value;
            // Thêm thuộc tính xử lý tranform là TrimTransformation
            obj._transformations = ["TrimTransformation"];
            newType[properties[0].children[0].value] = obj;
            obj = new Object();
            obj._xpath = properties[1].children[1].value;
            obj._transformations = ["TrimTransformation"];
            newType[properties[1].children[0].value] = obj;
            properties[0].remove();
            properties[1].remove();
        }
        // Lấy ra danh sách cacc thuộc tính khác và lưu tên thuộc tính vào object thành key
        properties = $(".form-group.property .input-group.entityproperty");
        if (properties.length > 0)
            properties.each(function () {
                var ppties = $(this).children();
                if (ppties[0].value != "" && ppties[1].value != "") {
                    if (arr.indexOf(ppties[0].value) == -1)
                        newType[ppties[0].value] = ppties[1].value;
                }
            });
        // Thêm danh sách exclude mà thuộc tính _removetags để xóa các phần tử loại trừ ra khỏi dom khi bóc tách dữ liệu
        if (list.length > 0)
            newType._removeTags = list;
        jsonGenerator[e] = newType;
    }
}

// Set các thành phần exclude. thuộc tính khác, danh sách đã chọn, bộ chọn css về mặc định
function clearData() {
    $(".form-control.exclude").remove();
    $(".input-group.entityproperty").remove();
    if ($("#Type").val() == 0 || $("#Type").val() == 3)
        $(".form-control.attribute").val("text");
    else
        $(".form-control.attribute").val("href");
    $("#myCssSelectorTest").val("");
    $("#myContentValue").val("");
    $("#typedata-difference").remove();
    $("#createTableJson").html("Tạo");
    $("#createTableJson").attr("onclick", "createTable();");
}

// Không dùng
function getPathTo(element) {
    if (element.id !== '')
        return "//*[@id='" + element.id + "']";
    if (element === document.body)
        return element.tagName;

    var ix = 0;
    var siblings = element.parentNode.childNodes;
    for (var i = 0; i < siblings.length; i++) {
        var sibling = siblings[i];
        if (sibling === element)
            return getPathTo(element.parentNode) + '/' + element.tagName.toLowerCase() + '[' + (ix + 1) + ']';
        if (sibling.nodeType === 1 && sibling.tagName === element.tagName)
            ix++;
    }
}

// Thêm mới input để nhập exclude
function addExclude() {
    $(".form-group.exclude").append("<div class='input-group'><input type=\"text\" class=\"form-control form-control-sm exclude\" style=\"margin-top:5px;\"><em onclick='deleteInput(this);' class='fas fa-trash-alt trash-delete'></em></div>");
}

// THêm mới input để nhập thuộc tính khác
function addPropertiese() {
    $(".form-group.property").append("<div class='input-group entityproperty'><input placeholder='Thuộc tính' type=\"text\" class=\"col-md-4 form-control form-control-sm\" ><input placeholder='Giá trị' type=\"text\" class=\"col-md-8 form-control form-control-sm\" ><em onclick='deleteInput(this);' class='fas fa-trash-alt trash-delete-all'></em></div>");
}

// Hàm clone object
function clone(obj) {
    var copy;
    if (null == obj || "object" != typeof obj) return obj;
    if (obj instanceof Date) {
        copy = new Date();
        copy.setTime(obj.getTime());
        return copy;
    }
    if (obj instanceof Array) {
        copy = [];
        for (var i = 0, len = obj.length; i < len; i++) {
            copy[i] = clone(obj[i]);
        }
        return copy;
    }
    if (obj instanceof Object) {
        copy = {};
        for (var attr in obj) {
            if (obj.hasOwnProperty(attr)) copy[attr] = clone(obj[attr]);
        }
        return copy;
    }

    throw new Error("Unable to copy obj! Its type isn't supported.");
}

//Hàm để xóa element hiện tại click
function deleteInput(e) {
    $(e).parent().remove();
}

// Hàm mởi window brower mới để xem html 
function viewHtml() {
    var w = window.open();
    $(w.document.body).html($("#result tbody #content_path").val());
}

// Hàm gọi api để lưu ljai cấu hình mới
function createData() {
    var body;
    if ($("#Type").val() == 3) {
        body = JSON.stringify(jsonGenerator.id);
    } else
        body = JSON.stringify(parseData());
    // GỌi api lưu domain với cấu hình trong jsonGenerator
    $.ajax({
        type: "PUT",
        url: "./api/domain",
        data: {
            "Domain1": domain,
            "Type": $("#Type").val(),
            "Content": body,
            "Properties": JSON.stringify(new Object().javascript = $("#Javascript").val())
        },
        success: function (data) {
            if (data == "success") {
                alert("Thêm Domain thành công!");
                getData();
            } else
                alert(data);
        }, error: function () {
            alert("Thêm Domain thất bại!");
        }
    });
}

// Hàm Recommend Data khi domain chưa được cấu hình
function recommendData() {

    var type = $("#Type").val();
    // Có 2 loại recomment. 1 cho Loại cấu hình chi tiết (0), 1 cho loại cấu hình id (3)
    if (type == 0)
        recommendDataDetail();
    else if (type == 3)
        recommendDataRegex();
}

// Recommend các trường cho cấu hình chi tiết báo
function recommendDataDetail() {
    jsonGenerator = new Object();
    // Xóa hết các dữ liệu trên trang web về rỗng
    clearData();
    // Tìm trong web nếu meta có thẻ tittle
    var a = iframe.contents().find("meta[name*='title'],meta[property*='title']").first();
    // Nếu có. Bổ sung vào cấu hình đã chọn tittle
    if (a.length > 0) {
        var newType = new Object();
        if (a.attr("name") != undefined)
            newType._xpath = "meta[name='" + a.attr("name") + "']";
        else
            newType._xpath = "meta[property='" + a.attr("property") + "']";
        newType.attr = "content";
        jsonGenerator["title_path"] = newType;
        $("#ListData tbody").append("<tr><td class='col-md-11 myHover' onclick='clickShowContent(this);'  id='title_path'>title_path</td><td class='col-md-1'><i class='fa fa-trash' aria-hidden='true' data-toggle='tooltip' title='Xóa!' onclick='funcionRemove($(this));'></i></td></tr>");
    }

    // Tìm trong thẻ meta nếu có thuộc tính time. THêm vào cấu hình thuộc tính time
    var a = iframe.contents().find("meta[name*='time'],meta[property*='time']").first();
    if (a.length > 0) {
        var newType = new Object();
        var tranform = new Object();
        tranform["date_regex"] = "\\d+-\\d+-\\d+";
        tranform["time_regex"] = "\\d+:\\d+:\\d+";
        tranform["datetime_format"] = "yyyy-MM-dd HH:mm:ss";
        newType["_transformations"] = [tranform];
        if (a.attr("name") != undefined)
            newType._xpath = "meta[name='" + a.attr("name") + "']";
        else
            newType._xpath = "meta[property='" + a.attr("property") + "']";
        newType.attr = "content";
        jsonGenerator["time_path"] = newType;
        $("#ListData tbody").append("<tr><td class='col-md-11 myHover' onclick='clickShowContent(this);'  id='time_path'>time_path</td><td class='col-md-1'><i class='fa fa-trash' aria-hidden='true' data-toggle='tooltip' title='Xóa!' onclick='funcionRemove($(this));'></i></td></tr>");
    }

    // Nếu thẻ meta có thuộc tính description, thêm vào cấu hình
    var a = iframe.contents().find("meta[name*='description'],meta[property*='description']").first();
    if (a.length > 0) {
        var newType = new Object();
        if (a.attr("name") != undefined)
            newType._xpath = "meta[name='" + a.attr("name") + "']";
        else
            newType._xpath = "meta[property='" + a.attr("property") + "']";
        newType.attr = "content";
        jsonGenerator["meta_description"] = newType;
        $("#ListData tbody").append("<tr><td class='col-md-11 myHover' onclick='clickShowContent(this);'  id='meta_description'>meta_description</td><td class='col-md-1'><i class='fa fa-trash' aria-hidden='true' data-toggle='tooltip' title='Xóa!' onclick='funcionRemove($(this));'></i></td></tr>");
    }
    // Nếu thẻ meta có thuộc tính keywords, thêm vào cấu hình
    var a = iframe.contents().find("meta[name*='keywords'],meta[property*='keywords']").first();
    if (a.length > 0) {
        var newType = new Object();
        if (a.attr("name") != undefined)
            newType._xpath = "meta[name='" + a.attr("name") + "']";
        else
            newType._xpath = "meta[property='" + a.attr("property") + "']";
        newType.attr = "content";
        jsonGenerator["keywords"] = newType;
        $("#ListData tbody").append("<tr><td class='col-md-11 myHover' onclick='clickShowContent(this);'  id='keywords'>keywords</td><td class='col-md-1'><i class='fa fa-trash' aria-hidden='true' data-toggle='tooltip' title='Xóa!' onclick='funcionRemove($(this));'></i></td></tr>");
    }
    $(".typedata").change();
}

// Recommend regex để bắt id trên url của web(nếu có)
function recommendDataRegex() {

    if (jsonGenerator.id == null) {
        jsonGenerator = new Object();
        clearData();
        // Thêm thuộc tính group_number vào cấu hình để xác định group chọn trong bộ chọn regex để lấy ra id
        $(".form-group.property").append("<div id='text' class='input-group entityproperty'><input placeholder='Thuộc tính' type=\"text\" value='group_number' class=\"col-md-4 form-control form-control-sm\" disabled><input placeholder='Giá trị' type=\"number\" value='1' class=\"col-md-8 form-control form-control-sm\" ></div>");
        var strinRegex = $("#meataData").val();
        // Đoạn regex được xác định trước phù hợp với nhiều web
        var suggest = ["(\\d+)\\.?(html|htm|epi|chn|vgp|antd|apl|bcb|\\/)?(\\?|$)", "\\/(\\d+)\\/"];
        // Nếu regex bắt được ra id. Thêm vào cấu hình
        for (i = 0; i < suggest.length; i++) {
            if (strinRegex.search(suggest[i]) >= 0) {
                $("#myCssSelectorTest").val(suggest[i]);
                break;
            }
        }
        if ($("#myCssSelectorTest").val().trim() != "") {
            createTable();
            $(".typedata").change();
        }
    }
}

// Xóa cấu hình domain
function deleteData() {
    // Hiển thị confirm xóa cấu hình
    if (confirm("Bạn có chắc chắn xóa domain này!")) {
        // GỌi api để xóa cấu hình trên db
        $.ajax({
            type: "DELETE",
            url: "./api/domain/delete/" + id,
            success: function (data) {
                if (data != null) {
                    alert("Xóa thành công!");
                    type = data.type;
                    update_url();
                    getData();
                    $(".typedata").change();
                } else
                    alert("Xóa thất bại!");
            },
            error: function () {
                alert("Xóa thất bại!")
            }
        })
    }
}

// Cập nhật lại dữ liệu đã lưu trên db theo cấu hình
function updateData() {
    var body;
    if ($("#Type").val() == 3) {
        body = JSON.stringify(jsonGenerator.id);
    } else
        body = JSON.stringify(parseData());
    // Gọi api để ghi đè cấu hình trên db
    $.ajax({
        type: "POST",
        url: "./api/update",
        data: {
            "Id": id,
            "Content": body,
            "Properties": JSON.stringify(new Object().javascript = $("#Javascript").val())
        },
        success: function (data) {
            if (data == true) {
                alert("Đã cập nhật Domain!");
                update_url();
                getData();
            } else
                alert("Cập nhật thất bại!");
        },
        error: function () {
            alert("Cập nhật thất bại!")
        }
    })
}

$(document).ready(function () {

    //$('#crawlDATA').on('load', function () {
    //    $(".typedata").change();
    //});
    // Bắt sự kiện thay đổi loại dữ liệu
    $(".form-control.typedata").change(function () {
        var radio = $(this).val();
        // Lếu loại dữ liệu là difference, thêm một input để nhập loại dữ liệu tự định nghĩa
        if (radio == "difference") {
            radio = $("#typedata-difference").val();
        }
        // nếu có loại dữ liệu đã cấu hình, hiển thị ra loại đầu tiên
        if ($("#ListData #" + radio).length > 0) {
            $("#ListData #" + radio).click();
        } else {
            // Xóa dữ liệu trong phần hiển thị cấu hình loại dữ liệu
            clearData();
            // THêm ô nhập loại dữ liệu khác nếu loại dữ liệu là difference
            if ($(this).val() == "difference")
                $(".div-typedata").append("<input class='form-control ml-auto' placeholder='Nhập loại dữ liệu' id='typedata-difference' type='text'>");
                // Nếu loại dữ liệu là time_path, Thêm các thuộc tính về regex
            else if ($(this).val() == "time_path") {
                $(".form-group.property").prepend("<div id='datetime_format' class='input-group entityproperty'><input placeholder='Thuộc tính' type=\"text\" value='datetime_format' class=\"col-md-4 form-control form-control-sm\" disabled><input placeholder='Giá trị' type=\"text\" class=\"col-md-8 form-control form-control-sm\" value='dd/MM/yyyy HH:mm'></div>");
                $(".form-group.property").prepend("<div id='time_regex' class='input-group entityproperty'><input placeholder='Thuộc tính' type=\"text\" value='time_regex' class=\"col-md-4 form-control form-control-sm\" disabled><input placeholder='Giá trị' type=\"text\" class=\"col-md-8 form-control form-control-sm\" value='\\d+:\\d+' ></div>");
                $(".form-group.property").prepend("<div id='date_regex' class='input-group entityproperty'><input placeholder='Thuộc tính' type=\"text\" value='date_regex' class=\"col-md-4 form-control form-control-sm\" disabled><input placeholder='Giá trị' type=\"text\" class=\"col-md-8 form-control form-control-sm\" value='\\d+/\\d+/\\d+' ></div>");
            }
                // Nếu lọau dữ liệu là comments_count, thêm thuộc tính khác về regex, group regex
            else if ($(this).val() == "comments_count") {
                $(".form-group.property").prepend("<div id='total_comments_regex' class='input-group entityproperty'><input placeholder='Thuộc tính' type=\"text\" value='total_comments_regex' class=\"col-md-4 form-control form-control-sm\" disabled><input placeholder='Giá trị' type=\"text\" class=\"col-md-8 form-control form-control-sm\" value='(\\d+)'></div>");
                $(".form-group.property").append("<div id='text' class='input-group entityproperty'><input placeholder='Thuộc tính' type=\"text\" value='group_number' class=\"col-md-4 form-control form-control-sm\" disabled><input placeholder='Giá trị' type=\"number\" value='1' class=\"col-md-8 form-control form-control-sm\" ></div>");
            }
                // Nếu loại dữ liệu là tags/categories, thêm các thuộc tính khác để bắt ra link/text của link
            else if ($(this).val() == "tags" || $(this).val() == "categories") {
                $(".form-group.property").prepend("<div id='link_" + $(this).val() + "' class='input-group entityproperty'><input placeholder='Thuộc tính' type=\"text\" value='link' class=\"col-md-4 form-control form-control-sm\" disabled><input placeholder='Giá trị' value='a/@href' type=\"text\" class=\"col-md-8 form-control form-control-sm\" ></div>");
                $(".form-group.property").prepend("<div id='text_" + $(this).val() + "' class='input-group entityproperty'><input placeholder='Thuộc tính' type=\"text\" value='text' class=\"col-md-4 form-control form-control-sm\" disabled><input placeholder='Giá trị' value='a/@text' type=\"text\" class=\"col-md-8 form-control form-control-sm\" ></div>");
            }
            // Nếu loại dữ liệu là content_path, tags,categories set thuộc tính mặc định là html
            if ($(this).val() == "content_path" || $(this).val() == "tags" || $(this).val() == "categories")
                $(".form-control.attribute").val("html");
            //else if ($(this).val() == "news_relate") {
            //    $('.form-control.attribute').append("<option class='selectAttributeCss' value='href'>Href</option>");
            //    $(".form-control.attribute").val("href");
            //}
            else $(".form-control.attribute").val("text");
        }
    });

    $("select.attribute").change(function () {
        var type = $(".form-control.typedata").val();
        var attr = $(this).val();
        if (type == "time_path" && $(this).val() == "datetime") {
            $("#datetime_format").children(0).eq(1).val("yyyy-MM-dd HH:mm:ss");
            $("#time_regex").children(0).eq(1).val("\\d+:\\d+:\\d+");
            $("#date_regex").children(0).eq(1).val("\\d+-\\d+-\\d+");
        } else if (type == "tags" || type == "categories") {
            $(".input-group.entityproperty").remove();
            if (attr == 'html') {
                $(".form-group.property").prepend("<div id='link_" + $(this).val() + "' class='input-group entityproperty'><input placeholder='Thuộc tính' type=\"text\" value='link' class=\"col-md-4 form-control form-control-sm\" disabled><input placeholder='Giá trị' value='a/@href' type=\"text\" class=\"col-md-8 form-control form-control-sm\" ></div>");
                $(".form-group.property").prepend("<div id='text_" + $(this).val() + "' class='input-group entityproperty'><input placeholder='Thuộc tính' type=\"text\" value='text' class=\"col-md-4 form-control form-control-sm\" disabled><input placeholder='Giá trị' value='a/@text' type=\"text\" class=\"col-md-8 form-control form-control-sm\" ></div>");
            }
        }
    });
});