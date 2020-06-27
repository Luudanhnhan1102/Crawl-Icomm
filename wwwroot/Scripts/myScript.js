// file script được nhúng vào trong trang báo sau khi xử lý
var isLink = false;
var isHide = true;
var isTurnOn = false;
var css = "";
// Lấy ra thuộc tính cssSelector trong input chọn thuộc tỉnh của trang nhúng iframe
var myCssSelector = $('#myCssSelectorTest', window.parent.document);
// Lấy ra button Lấy nội dung trong trang scrapingcrawl.icomm.vn (Trang nhúng iframe)
var myCheck = $('#myCheck', window.parent.document);
var listType = ["link", "news_relate"];

// Đặt lại giá trị css trong input Css Selector
function getFullCss() {
    $("#cssSelector").val(css);
}

// Hàm thêm cssSelector từ khung chọn css Selector trong trang báo ra input bộ chọn css selector ngoài trang cấu hình và cuối input
function AddCss() {
    var csstmp = $("#cssSelector").val().trim();
    if (myCssSelector.val() != "")
        myCssSelector.val(myCssSelector.val() + (csstmp != "" ? ", " + csstmp : ""));
    else
        myCssSelector.val(csstmp);
    myCheck.click();
}

// Thay thế css Selector trong khung chọn css Selector trong trang báo ra input nhập bộ chọn css của trang cấu hình
function ReplaceCss() {
    myCssSelector.val($("#cssSelector").val().trim());
    myCheck.click();
}

// Thêm class myClass vào bộ chọn tương ứng để làm highlight element được chọn lên
function TestCss() {
    $(".myClass").removeClass("myClass");
    $($("#cssSelector").val().trim()).addClass("myClass");
}

// Thêm cssSelectỏ vào danh sách Exclude trong trang cấu hình
function ExcludeCss() {
    $('.form-group.exclude', window.parent.document).append("<div class='input-group'><input type=\"text\" class=\"form-control form-control-sm exclude\" style=\"margin-top:5px;\" value=\"" + $("#cssSelector").val().trim() + "\"><em onclick='deleteInput(this);' class='fas fa-trash-alt trash-delete'></em></div>");
}

// Cập nhật tham số url của trang cấu hình theo url truyền vào
function update_url(url) {
    parent.history.pushState(null, null, url);
}

// Hàm để nhảy đến trang báo nằm trong link href
function LinkInfo(x) {
    if (isLink) {
        location.href = location.origin + location.pathname + '?url=' + encodeURIComponent(x) + "&javascript=" + window.parent.$("#Javascript").val();
    }
}

$(document).ready(function () {
    // Thay đổi input nhập url của trang cấu hình
    parent.document.getElementById(window.parent.$("#meataData").val(url));
    // Cập nhật lại url cho trang cấu hình
    update_url('?url=' + encodeURIComponent(url) + "&type=" + window.parent.$("#Type").val() + "&javascript=" + window.parent.$("#Javascript").val());
    parent.setUrl(url);

    // Hàm xử lý để lấy ra bộ chọn css của element và tối ưu lại bộ chọn css Selector để ngắn nhất
    jQuery.fn.getPath = function () {
        // Lấy ra tất cả parents của element
        var parents = $(this).parents();
        // Khởi tạo lại bộ chọn css Seletor
        var selector = "";

        // Duyệt tất cả các phần tử cha của element. Lấy ra tag name, class và id của mỗi phần tử được duyệt và thêm vào biến selector
        for (var i = parents.length - 1; i >= 0; i--) {
            if (i < parents.length - 2) {
                selector += $(parents[i]).prop("tagName").toLowerCase();
                var classNames = $(parents[i]).attr("class");
                if (classNames) {
                    var array = classNames.split(" ");
                    array.forEach(function (item) {
                        if (item)
                            selector += "." + item;
                    });
                }
                var id = $(parents[i]).attr("id");
                if (id) {
                    var array = id.split(" ");
                    array.forEach(function (item) {
                        if (item)
                            selector += "#" + item;
                    });
                }
                //if (id || classNames)
                selector += " ";
            }
        };

        // THêm phần tử hiện tại (tagname, class, id) vào cuối chuỗi selector
        selector += $(this).prop("tagName").toLowerCase();
        var classNames = $(this).attr("class");
        if (classNames) {
            var array = classNames.split(" ");
            array.forEach(function (item) {
                if (item)
                    selector += "." + item;
            });
        }
        var id = $(this).attr("id");
        if (id) {
            var array = id.split(" ");
            array.forEach(function (item) {
                if (item)
                    selector += "#" + item;
            });
        }
        return selector;
    };


    // Thêm div để hiển thị bộ chọn cssSelector trong trang báo
    $("body").append("<div id='divShowInfoTag' style=' display: none;  top:-100px; left:-100px; position: fixed'>"
        + "<b>Css Selector: </b></br><textarea id='cssSelector' ></textarea>"
        + "<br/><div class='totaltooltip'>"
        + "<button style = 'padding:3px 5px; background:green;  border:solid 1px #CCC; cursor: pointer;color: #FFF; margin: 3px;' onclick = 'myFunction()' onmouseout = 'outFunc()' > <span class='tooltiptext' id='myTooltip'>Copy to clipboard</span> Copy </button></div> "
        + "<button style = 'padding:3px 5px; background:green;  border:solid 1px #CCC; cursor: pointer;color: #FFF; margin: 3px;' onclick = 'getFullCss();' > Full Css</button>"
        + "<button style = 'padding:3px 5px; background:green;  border:solid 1px #CCC; cursor: pointer;color: #FFF; margin: 3px;' onclick = 'AddCss();' > Append</button>"
        + "<button style = 'padding:3px 5px; background:green;  border:solid 1px #CCC; cursor: pointer;color: #FFF; margin: 3px;' onclick = 'TestCss();' > Test</button>"
        + "<button style = 'padding:3px 5px; background:green;  border:solid 1px #CCC; cursor: pointer;color: #FFF; margin: 3px;' onclick = 'ReplaceCss();' > Replace</button>"
        + "<button style = 'padding:3px 5px; background:green;  border:solid 1px #CCC; cursor: pointer;color: #FFF; margin: 3px;' onclick = 'ExcludeCss();' > Exclude</button>"
        + "<div style='float: right'><input id='myClose' type='button' value='Đóng' style='padding:3px 5px; background:#9d234c;  border:solid 1px #CCC; cursor: pointer;color: #FFF; margin: 3px' /></div></div> ");

    // Bắt sự kiện bấm nút đống để ẩn đi khung chọn css selector
    $("#myClose").click(function () {
        $("#divShowInfoTag").hide();
        isHide = true;
        $(".myClass").removeClass("myClass");
    });

    //Hàm xử lý sự kiện để di chuyển khung chọn cssSelector theo element đang di chuột vào
    $("*").on("mousemove", function (e) {
        if (isTurnOn) {
            //if (e === undefined) e = window.e;
            //var target = 'target' in e ? e.target : e.srcElement;
            $(".myClass").removeClass("myClass");
            //var value = getPathTo(target);
            //$("#cssSelector").val(value);
            $("#cssSelector").val($(e.target).getPath());
            //console.log($(e.target).css());
            //parent.document.getElementById("myCssSelectorTest").value = value;
            $(e.target).addClass("myClass");

            clientX = e.clientX + 30;
            clientY = e.clientY + 30;
            if (clientX > 0.5 * window.innerWidth) {
                clientX = clientX - $("#divShowInfoTag").width() - 80;
            }
            if (clientY > 0.5 * window.innerHeight)
                clientY = clientY - $("#divShowInfoTag").height() - 80;
            $("#divShowInfoTag").css({ left: clientX + "px", top: clientY + "px" });
        }
    });


    // Giữ alt để có thể click vào được link
    $(document).keydown(function (e) {
        if (e.which == 18) {
            if (!isTurnOn)
                isTurnOn = true;
            if (isHide)
                $("#divShowInfoTag").show();
        } else
            if (e.which == 17)
                isLink = true;
    });


    $(document).keyup(function (e) {
        if (e.which == 18) {
            isTurnOn = false;
            var cssSelector = $("#cssSelector").val();
            if (listType.indexOf($('.typedata', window.parent.document).val()) >= 0)
                while (true) {
                    if (cssSelector.indexOf(" ") > 0) {
                        var csstmp = cssSelector.substring(cssSelector.lastIndexOf(" ") + 1, cssSelector.length);
                        if (!csstmp.startsWith("a"))
                            cssSelector = cssSelector.substring(0, cssSelector.lastIndexOf(" "));
                        else
                            break;
                    } else {
                        if (!csstmp.startsWith("a"))
                            cssSelector = "";
                        break;
                    }
                }
            css = cssSelector;
            var count = $(cssSelector).length;
            //console.log(count);
            var firstElement = $(cssSelector).first();
            while (cssSelector.indexOf(" ") > 0) {
                var csstmp = cssSelector.substring(cssSelector.indexOf(" ") + 1, cssSelector.length);
                var elementstmp = $(csstmp);
                if (elementstmp.length == count && firstElement[0] === elementstmp[0])
                    cssSelector = csstmp;
                else
                    break;
            }

            $("#cssSelector").val(cssSelector);
            $(".myClass").removeClass("myClass");
            $(cssSelector).addClass("myClass");
        } else
            if (e.which == 17)
                isLink = false;
    });

});

// Coppy input css Selector vào clip board
function myFunction() {
    var textArea = document.createElement("textarea");
    textArea.value = $("#cssSelector").val();
    document.body.appendChild(textArea);
    textArea.select();
    document.execCommand('copy');
    textArea.remove();

    var tooltip = document.getElementById("myTooltip");
    tooltip.innerHTML = "Copied! "
}

// Coppy input css Selector vào clip board
function outFunc() {
    var tooltip = document.getElementById("myTooltip");
    tooltip.innerHTML = "Copy to clipboard";
}