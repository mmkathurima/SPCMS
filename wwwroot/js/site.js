// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function toggleTheme() {
    darkmode.toggleDarkMode();
    if (darkmode.inDarkMode) {
        document.getElementById("theme-icon").setAttribute("data-icon", "bytesize:moon");
    } else {
        document.getElementById("theme-icon").setAttribute("data-icon", "clarity:sun-solid");
    }
    try {
        if (darkmode.inDarkMode) {
            document.querySelector(".carousel-indicators").style.backgroundColor = "";
        } else {
            document.querySelector(".carousel-indicators").style.backgroundColor = "rgba(0, 0, 0, .3)";
        }
    } catch (e) { }
    invertible();
}

const invertible = () => {
    for (let i of Array.from(document.getElementsByClassName("invertible")))
        i.style.filter = darkmode.inDarkMode ? "invert(1)" : "invert(0)";
}

function onPageLoad() {
    if (darkmode.inDarkMode) {
        document.getElementById("theme-toggle").setAttribute("checked", true);
        document.getElementById("theme-icon").setAttribute("data-icon", "bytesize:moon");
    } else {
        document.getElementById("theme-toggle").removeAttribute("checked");
        document.getElementById("theme-icon").setAttribute("data-icon", "clarity:sun-solid");
    }
    try {
        if (darkmode.inDarkMode) {
            document.querySelector(".carousel-indicators").style.backgroundColor = "";
        } else {
            document.querySelector(".carousel-indicators").style.backgroundColor = "rgba(0, 0, 0, .3)";
        }
    } catch (e) { }
    try {
        new bootstrap.Toast(document.getElementById("liveToast"), {
            animation: true
        }).show();
    } catch (e) { }
    document.addEventListener("keypress", function (e) {
        const node = e.target.nodeName.toLowerCase().trim();
        if (e.code == "Slash" && node == "body") {
            e.preventDefault();
            show_search();
        }
    });
    invertible();
}

function mapping() {
    try {
        let map;
        let pin;
        const tilesURL = 'https://{s}.tile.openstreetmap.fr/hot/{z}/{x}/{y}.png';
        const mapAttrib = '&copy; <a href="http://www.openstreetmap.org/copyright">OpenStreetMap</a>, Tiles courtesy of <a href="http://hot.openstreetmap.org/" target="_blank">Humanitarian OpenStreetMap Team</a>';

        // add map container
        $('#location').prepend('<div id="map" style="height:70vh;width:100%;"></div>');
        MapCreate();

        function MapCreate() {
            // create map instance
            if (!(typeof map == "object")) {
                map = L.map('map', {
                    center: [40, 0],
                    zoom: 14
                });
            }
            else {
                map.setZoom(3).panTo([40, 0]);
            }
            // create the tile layer with correct attribution
            L.tileLayer(tilesURL, {
                attribution: mapAttrib,
                maxZoom: 19
            }).addTo(map);
        }

        map.on('click', function (ev) {
            $('#latitude').val(ev.latlng.lat);
            $('#longitude').val(ev.latlng.lng);
            if (typeof pin == "object") {
                pin.setLatLng(ev.latlng);
            }
            else {
                pin = L.marker(ev.latlng, { riseOnHover: true, draggable: true });
                pin.addTo(map);
                pin.on('drag', function (ev) {
                    $('#latitude').val(ev.latlng.lat);
                    $('#longitude').val(ev.latlng.lng);
                });
            }
        });

        map.addControl(new L.Control.Search({
            url: 'https://nominatim.openstreetmap.org/search?format=json&q={s}',
            jsonpParam: 'json_callback',
            propertyName: 'display_name',
            propertyLoc: ['lat', 'lon'],
            marker: pin,
            autoCollapse: true,
            autoType: false,
            minLength: 2
        }));
    } catch (e) {
        console.error(e);
    }
}

function loadMap(latitude, longitude) {
    const element = document.getElementById("map-embed");
    element.style.height = "600px";
    const map = L.map(element);
    L.tileLayer("https://{s}.tile.osm.org/{z}/{x}/{y}.png", {
        attribution: "&copy; <a href='https://osm.org/copyright'>OpenStreetMap</a> contributors"
    }).addTo(map);
    const target = L.latLng(latitude.toString(), longitude.toString());
    map.setView(target, 14);
    $("a[href='#menu1']").on("shown.bs.tab", function () {
        window.setTimeout(function () {
            console.log("INVALIDATING...");
            map.invalidateSize()
        }, 1);
    });
    L.marker(target).addTo(map);
}

function imgUpload() {
    let imgWrap = "";
    const imgArray = [];

    $('.upload__inputfile').each(function () {
        $(this).on('change', function (e) {
            imgWrap = $(this).closest('.upload__box').find('.upload__img-wrap');
            const maxLength = $(this).attr('data-max_length');

            const files = e.target.files;
            const filesArr = Array.prototype.slice.call(files);
            let iterator = 0;
            filesArr.forEach(function (f, index) {

                if (!f.type.match('image.*')) {
                    return;
                }

                if (imgArray.length > maxLength) {
                    return false;
                } else {
                    let len = 0;
                    for (let i = 0; i < imgArray.length; i++) {
                        if (imgArray[i] !== undefined) {
                            len++;
                        }
                    }
                    if (len > maxLength) {
                        return false;
                    } else {
                        imgArray.push(f);

                        const reader = new FileReader();
                        reader.onload = function (e) {
                            const html = "<div class='upload__img-box'><div style='background-image: url(" +
                                e.target.result + ")' data-number='" + $(".upload__img-close").length +
                                "' data-file='" + f.name + "' class='img-bg'><div class='upload__img-close'></div></div></div>";
                            imgWrap.append(html);
                            iterator++;
                        }
                        reader.readAsDataURL(f);
                    }
                }
            });
        });
    });

    $('body').on('click', ".upload__img-close", function (e) {
        const file = $(this).parent().data("file");
        for (let i = 0; i < imgArray.length; i++) {
            if (imgArray[i].name === file) {
                imgArray.splice(i, 1);
                break;
            }
        }
        $(this).parent().parent().remove();
    });
}

function details(maxQuantity) {
    let $input;
    $('.minus').click(function (e) {
        e.preventDefault();
        $input = $(this).parent().find('input');
        if ($input.val().toString().trim() == "") $input.val("0");
        let count = parseInt($input.val()) - 1;
        count = count < 1 ? 1 : count;
        $input.val(count);
        $input.change();
        return false;
    });
    $('.plus').click(function (e) {
        e.preventDefault();
        $input = $(this).parent().find('input');
        if ($input.val().toString().trim() == "") $input.val("0");
        $input.val(parseInt($input.val()) + 1);
        $input.change();
        return false;
    });
    $("#quantity").attr("max", maxQuantity).on("keypress", function (e) {
        return e.metaKey || // cmd/ctrl
            e.which <= 0 || // arrow keys
            e.which == 8 || // delete key
            /\d/.test(String.fromCharCode(e.which)); // numbers
    }).on("input", function (e) {
        if (e.target.value == "0") e.target.value = '';
    }).on("change", function (e) {
        if (parseInt($(this).val()) >= parseInt(maxQuantity))
            $(".plus").addClass("disabled");
        else
            $(".plus").removeClass("disabled");
    });
}

function manageListings(username) {
    $(".deleteListing").on("click", function (e) {
        e.preventDefault();
        if (username == "admin")
            username = window.prompt("Enter proxy authorization(Leave blank if own): ", username);

        if (confirm("Do you really want to delete this listing?")) {
            console.log(`VALUE: ${$(e.target).closest(".mt-4").find(".id").val()}`)
            $.ajax({
                type: "GET",
                url: "DeleteListing",
                data: { id: $(e.target).closest(".mt-4").find(".id").val(), username: username },
                success: (response, status) => {
                    window.location.reload();
                    document.getElementById("delete").innerHTML = response;
                },
                complete: function (jq, status) {
                    console.log(`[JQUERY AJAX COMPLETE FOR DeleteListing WITH STATUS ${status}\n${jq.responseText}]`);
                },
                error: function (jq, exception, detail) {
                    console.log(jq);
                    console.log(exception);
                    console.log(detail);
                }
            });
        }
    });
}

function savePassword(username) {
    let uElement = document.getElementById("username0");
    if (username == "admin") {
        let user = window.prompt("Enter proxy authorization (Leave blank if own):", username);
        uElement.value = user;
        return user != null;
    } else return true;
}

function deleteAccount(username) {
    if (username == "admin") {
        let user = window.prompt("Enter proxy authorization (Note that you cannot delete your account):");
        document.getElementById("username1").value = user;
        return user != null;
    } else
        return confirm('Do you really want to delete your account?');
}

function change_image(image) {
    const container = document.getElementById("main-image");
    container.src = image.src;
}

function show_search() {
    let search_form = document.getElementById("search-form");
    search_form.setAttribute("style", search_form.getAttribute("style") == "display: none !important;" ? "display: inline-flex !important;" : "display: none !important;");
    let search = search_form.querySelector("input[type=search]");
    search.focus();
}

function init_session(user, role) {
    if (user.trim() == "") {
        //is NOT logged in
        $("a#dropdown-option1 #option1-value").html("Login");
        $("a#dropdown-option1").attr("href", "/Accounts/Login");
        $("a#dropdown-option2 #option2-value").html("Register");
        $("a#dropdown-option2").attr("href", "/Accounts/Register");

        $("a#dropdown-option1 span#option1-icon").attr("data-icon", "entypo:login");
        $("a#dropdown-option2 span#option2-icon").attr("data-icon", "eos-icons:system-re-registered");
    } else {
        //is logged in
        $("a#dropdown-option1 #option1-value").html(user);
        $("a#dropdown-option1").attr("href", `/Accounts/${user}/Settings`);
        $("a#dropdown-option2 #option2-value").html("Logout");
        $("a#dropdown-option2").attr("href", "/Accounts/Logout");

        $("a#dropdown-option1 span#option1-icon").attr("data-icon", "akar-icons:settings-horizontal");
        $("a#dropdown-option2 span#option2-icon").attr("data-icon", "entypo:log-out");
        let li, a, span;
        if (role.trim() != "") {
            /*
             * SPARE_PARTS_DEALER = 0
             * CONSUMER = 1
             * ADMIN = 2
             */
            if (parseInt(role) == 0) {
                /*<li>
                    <a class="dropdown-item" id="dropdown-option2">
                        <span class="iconify" id="option2-icon"></span>
                        <span id="option2-value"></span>
                    </a>
                </li>*/
                const dropdown = document.getElementById("dropdown");
                li = document.createElement("li");
                a = document.createElement("a");
                a.className = "dropdown-item";
                a.id = "dropdown-option3";
                a.href = `/Accounts/${user}/Statistics`;
                span = document.createElement("span");
                span.className = "iconify";
                span.id = "option3-icon";
                span.setAttribute("data-icon", "wpf:statistics");
                a.appendChild(span);
                span = document.createElement("span");
                span.id = "option3-value";
                span.innerHTML = "&nbsp;Business Statistics";
                a.appendChild(span);
                li.appendChild(a);
                dropdown.insertBefore(li, document.getElementById("option2"));
            }
            if (parseInt(role) == 0) {
                li = document.createElement("li");
                li.className = "nav-item";

                a = document.createElement("a");
                a.className = "nav-link";
                a.href = "/AddListing";
                a.innerHTML = "Add Listing";

                li.appendChild(a);
                document.getElementById("addListingLink").appendChild(li);


                li = document.createElement("li");
                li.className = "nav-item";
                li.style.margin = "-.2em 1em 0 0";
                li.style.fontSize = "1.2em";

                a = document.createElement("a");
                a.className = "nav-link";
                a.href = "/Orders?page=1";
                a.title = "My Orders";

                span = document.createElement("span");
                span.className = "iconify";
                span.setAttribute("data-icon", "bx:package");

                a.appendChild(span);
                li.appendChild(a);

                document.querySelector("ul.navbar-nav.ml-auto").prepend(li);
            } else if (parseInt(role) == 1) {
                const section = document.querySelector("ul.navbar-nav.ml-auto");

                li = document.createElement("li");
                li.className = "nav-item";
                li.style.margin = "-.2em 1em 0 0";
                li.style.fontSize = "1.2em";

                a = document.createElement("a");
                a.className = "nav-link";
                a.href = "/Cart?page=1";
                a.title = "My Shopping Cart";

                span = document.createElement("span");
                span.className = "iconify";
                span.setAttribute("data-icon", "ant-design:shopping-cart-outlined");

                a.appendChild(span);
                li.appendChild(a);

                section.prepend(li);

                li = li.cloneNode(true);
                a = $(li).find("a");
                a.attr("href", "/Orders?page=1");
                a.attr("title", "My Orders");
                span = $(a).find("span");
                span.attr("data-icon", "bx:package");

                section.prepend(li);

                li = li.cloneNode(true);
                a = $(li).find("a");
                a.attr("href", "/WishList?page=1");
                a.attr("title", "My WishList");
                span = $(a).find("span");
                span.attr("data-icon", "akar-icons:heart");

                section.prepend(li);
            }
        }
    }
}

function doFilter(url) {
    document.getElementById("filterForm").addEventListener("submit", function (e) {
        e.preventDefault();
        url = url.substring(0, url.indexOf("?") + 1);
        url += new URLSearchParams(new FormData(document.getElementById("filterForm"))).toString();
        if (url != "" && url.includes("Search")) url += "&query=" + document.querySelector("input.form-control.me-2").value;
        console.log(`URL: ${url}`);
        $("#loaded").load(`${url} #loaded > *`);
        $(".pagination").load(`${url} .pagination`);
        window.history.pushState({}, '', url);
    });
}

function doFilterList(target) {
    const lis = $(".dropdown-item").closest("ul.dropdown-menu.pre-scrollable").find(".dropdown-item");
    for (let li of lis) {
        li.style.display = (li.innerText.toLowerCase().indexOf($(target).val().toLowerCase()) > -1) ? "" : "none";
    }
}

function initRange(priceBounds, yearBounds) {
    if (priceBounds[0] == undefined) priceBounds[0] = 300;
    if (priceBounds[1] == undefined) priceBounds[1] = 10000;

    if (yearBounds[0] == undefined) yearBounds[0] = 1980;
    if (yearBounds[1] == undefined) yearBounds[1] = 2020;

    $(function () {
        $("#price-range").slider({
            range: true,
            min: priceBounds[0],
            max: priceBounds[1],
            values: priceBounds,
            slide: function (event, ui) {
                $("#priceRange").val("KShs." + ui.values[0] + " - KShs." + ui.values[1]);
                $("#minPrice").val(ui.values[0]);
                $("#maxPrice").val(ui.values[1]);
            }
        });

        $("#year-range").slider({
            range: true,
            min: yearBounds[0],
            max: yearBounds[1],
            values: yearBounds,
            slide: function (event, ui) {
                $("#yearRange").val(ui.values[0] + " - " + ui.values[1]);
                $("#minYear").val(ui.values[0]);
                $("#maxYear").val(ui.values[1]);
            }
        });

        $("#yearRange").val($("#year-range").slider("values", 0) + " - " + $("#year-range").slider("values", 1));
        $("#priceRange").val("KShs. " + $("#price-range").slider("values", 0) + " - KShs. " + $("#price-range").slider("values", 1));
    });
}

function confirmCheckout(me) {
    document.querySelectorAll("vbxml")[1].insertAdjacentHTML("afterbegin", `<div class="alert alert-success">Order successfully confirmed</div>`);
    me.classList.add("disabled");
}

function validatePassword(offset = 1) {
    let first,
        len = document.getElementById("len"),
        upr = document.getElementById("upr"),
        lwr = document.getElementById("lwr"),
        spc = document.getElementById("spc"),
        num = document.getElementById("num"),
        match_ = document.getElementById("match"),
        submit = document.querySelectorAll("button[type=submit]")[1],
        isValid = true;
    submit.classList.add("disabled");

    const secondValidate = (second) => {
        if (first.value == second.value) {
            match_.setAttribute("icon", "mdi:tick-circle");
        } else {
            match_.setAttribute("icon", "gridicons:cross-circle");
            if (!submit.classList.contains("disabled")) submit.classList.add("disabled");
            return false;
        }
        submit.classList.remove("disabled");
    };

    for (let [index, i] of document.querySelectorAll("#password, #confirmPassword, #newPassword, #newPassword2").entries()) {
        i.addEventListener("focus", function () {
            document.getElementsByClassName("validate")[index].style.display = "block";
        });
        i.addEventListener("blur", function () {
            document.getElementsByClassName("validate")[index].style.display = "none";
        });
        switch (index) {
            case 0:
                first = i;
                i.addEventListener("input", function () {
                    if (this.value.length >= 8) {
                        len.setAttribute("icon", "mdi:tick-circle");
                    } else {
                        len.setAttribute("icon", "gridicons:cross-circle");
                        if (!submit.classList.contains("disabled")) submit.classList.add("disabled");
                        isValid = false;
                    }
                    if (this.value.match(/[a-z]/g)) {
                        lwr.setAttribute("icon", "mdi:tick-circle");
                    } else {
                        lwr.setAttribute("icon", "gridicons:cross-circle");
                        if (!submit.classList.contains("disabled")) submit.classList.add("disabled");
                        isValid = false;
                    }
                    if (this.value.match(/[A-Z]/g)) {
                        upr.setAttribute("icon", "mdi:tick-circle");
                    } else {
                        upr.setAttribute("icon", "gridicons:cross-circle");
                        if (!submit.classList.contains("disabled")) submit.classList.add("disabled");
                        isValid = false;
                    }
                    if (this.value.match(/[!@#$%^&*.]/g)) {
                        spc.setAttribute("icon", "mdi:tick-circle");
                    } else {
                        spc.setAttribute("icon", "gridicons:cross-circle");
                        if (!submit.classList.contains("disabled")) submit.classList.add("disabled");
                        isValid = false;
                    }
                    if (this.value.match(/[0-9]/g)) {
                        num.setAttribute("icon", "mdi:tick-circle");
                    } else {
                        num.setAttribute("icon", "gridicons:cross-circle");
                        if (!submit.classList.contains("disabled")) submit.classList.add("disabled");
                        isValid = false;
                    }
                    if (!secondValidate(document.querySelectorAll("input[type=password]")[offset]))
                        isValid = false;

                    return isValid;
                });
                break;
            case 1:
                i.addEventListener("input", function () {
                    if (!secondValidate(this))
                        isValid = false;
                    return isValid;
                });
                break;
        }
    }
}