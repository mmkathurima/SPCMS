Imports Microsoft.AspNetCore.Http
Imports Microsoft.AspNetCore.Mvc

Public Class LayoutView : Inherits Vazor.VazorSharedView

    Public Sub New()
        MyBase.New("_Layout", "Views\Shared", "Spare Parts Catalogue Management System")
    End Sub

    Public Overrides Function GetVbXml() As XElement
        Return _
        <vbxml>
            <html>
                <head>
                    <meta charset="utf-8"/>
                    <meta name="viewport" content="width=device-width, initial-scale=1.0"/>
                    <!-- The page supports both dark and light color schemes, and the page author prefers / default is DARK. -->
                    <meta name="color-scheme" content="light dark"/>

                    <title>@ViewData["Title"] - <%= Title %></title>

                    <environment include="Development">
                        <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap-dark-5@1.1.3/dist/css/bootstrap-nightshade.css"/>
                        <link rel="stylesheet" href="https://code.jquery.com/ui/1.13.2/themes/base/jquery-ui.css"/>
                    </environment>

                    <environment exclude="Development">
                        <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap-dark-5@1.1.3/dist/css/bootstrap-nightshade.min.css"
                            asp-fallback-href="~/lib/bootstrap/dist/css/bootstrap.min.css"
                            asp-fallback-test-class="sr-only" asp-fallback-test-property="position" asp-fallback-test-value="absolute"
                            crossorigin="anonymous"
                            integrity="sha256-o4/KvOMbqkanQIhEKRG5j11aFGYHpgfB8Zy8mInEzSU="/>
                        <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/jqueryui/1.13.2/themes/base/jquery-ui.min.css" integrity="sha512-ELV+xyi8IhEApPS/pSj66+Jiw+sOT1Mqkzlh8ExXihe4zfqbWkxPRi8wptXIO9g73FSlhmquFlUOuMSoXz5IRw==" crossorigin="anonymous" referrerpolicy="no-referrer"/>
                    </environment>

                    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/bootstrap-multiselect/0.9.13/css/bootstrap-multiselect.css"/>
                    <script src="https://cdn.jsdelivr.net/npm/popper.js@1.16.1/dist/umd/popper.min.js"></script>

                    <environment include="Development">
                        <script src="https://cdnjs.cloudflare.com/ajax/libs/jquery/3.6.0/jquery.js" asp-fallback-src="~/lib/jquery/dist/jquery.js"></script>
                        <script src="https://cdn.jsdelivr.net/npm/jquery-ui@1.13.2/dist/jquery-ui.js" integrity="sha256-xLD7nhI62fcsEZK2/v8LsBcb4lG7dgULkuXoXB/j91c=" crossorigin="anonymous"></script>
                        <script src="https://cdnjs.cloudflare.com/ajax/libs/bootstrap/4.5.3/js/bootstrap.js" asp-fallback-href="~/lib/bootstrap/dist/js/bootstrap.js"></script>
                        <script src="https://cdn.jsdelivr.net/npm/bootstrap-dark-5@1.1.3/dist/js/darkmode.js"></script>
                        <script src="https://cdnjs.cloudflare.com/ajax/libs/bootstrap/5.2.0/js/bootstrap.bundle.js" integrity="sha512-mC4dqqYOpYgZRW1MR8YVU4N569ZoiWDOXtW5oP4XhTfPRgioTQfBNrH2GkoG3IwlXorj/Ut9ZIw7JT2md4dfdw==" crossorigin="anonymous" referrerpolicy="no-referrer"></script>
                        <script src="https://cdnjs.cloudflare.com/ajax/libs/iconify/3.0.0-beta.3/iconify.js" integrity="sha512-V5u1OrxHC3TjHnFPU+JulLVGyIgH7ynasgm1IkG/p81sgHi6KAgFICESYe8qxihqfQXsXTHBPaNvquAt5EFzvQ==" crossorigin="anonymous" referrerpolicy="no-referrer"></script>
                        <script src="https://cdnjs.cloudflare.com/ajax/libs/bootstrap-multiselect/0.9.13/js/bootstrap-multiselect.js"></script>
                    </environment>

                    <environment exclude="Development">
                        <script src="https://cdnjs.cloudflare.com/ajax/libs/jquery/3.6.0/jquery.min.js" integrity="sha512-894YE6QWD5I59HgZOGReFYm4dnWc1Qt5NtvYSaNcOP+u1T9qYdvdihz0PPSiiqn/+/3e7Jo4EaG7TubfWGUrMQ==" crossorigin="anonymous" referrerpolicy="no-referrer"></script>
                        <script src="https://cdn.jsdelivr.net/npm/jquery-ui@1.13.2/dist/jquery-ui.min.js" integrity="sha256-lSjKY0/srUM9BE3dPm+c4fBo1dky2v27Gdjm2uoZaL0=" crossorigin="anonymous"></script>
                        <script src="https://cdnjs.cloudflare.com/ajax/libs/bootstrap/4.5.3/js/bootstrap.min.js"></script>
                        <script src="https://cdn.jsdelivr.net/npm/bootstrap-dark-5@1.1.3/dist/js/darkmode.min.js"></script>
                        <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.1.3/dist/js/bootstrap.bundle.min.js"></script>
                        <script src="https://code.iconify.design/2/2.2.1/iconify.min.js"></script>
                        <script src="https://cdnjs.cloudflare.com/ajax/libs/bootstrap-multiselect/0.9.13/js/bootstrap-multiselect.min.js" integrity="sha512-p/rWSzeAUmYFvtMy6D5VfINfnHEWSz1h87a7fyGMaQQGmniI54CGQyecfvy5MTtGzZ80CyL/JG39SfVJjqiUJw==" crossorigin="anonymous" referrerpolicy="no-referrer"></script>
                    </environment>

                    <link rel="stylesheet" href="~/css/site.css"/>
                    <link rel="stylesheet" href="https://unpkg.com/leaflet-search@2.3.7/dist/leaflet-search.src.css"/>
                    <link href="https://unpkg.com/leaflet@1.6.0/dist/leaflet.css" rel="stylesheet"/>

                    <script src="https://unpkg.com/leaflet@1.2.0/dist/leaflet-src.js"></script>
                    <script src="https://unpkg.com/leaflet-search@2.3.7/dist/leaflet-search.src.js"></script>
                    <script src="~/js/site.js" asp-append-version="true"></script>
                </head>
                <body onload="onPageLoad()">
                    <header>
                        <nav class="navbar navbar-expand-sm navbar-toggleable-sm border-bottom box-shadow mb-3">
                            <div class="container">
                                <a class="navbar-brand" asp-controller="Home" asp-action="Index">
                                    <span>Spare Parts Catalogue Management System</span>
                                </a>
                                <button class="navbar-toggler navbar-light" type="button" data-toggle="collapse" data-target=".navbar-collapse"
                                    data-bs-target="#navbarSupportedContent" aria-controls="navbarSupportedContent"
                                    aria-expanded="false" aria-label="Toggle navigation">
                                    <span class="navbar-toggler-icon"></span>
                                </button>
                                <div class="collapse navbar-collapse" id="navbarSupportedContent">
                                    <ul class="navbar-nav me-auto mb-2">
                                        <li class="nav-item">
                                            <a class="nav-link" asp-controller="Home" asp-action="Index">Home</a>
                                        </li>
                                        <!--for dealer [0]-->
                                        <div id="addListingLink"></div>

                                        <li class="nav-item">
                                            <a class="nav-link" asp-controller="Home" asp-action="Listings" asp-route-page="1">View Listings</a>
                                        </li>
                                    </ul>

                                    <ul class="navbar-nav ml-auto">
                                        <!--for consumer [1]-->

                                        <div class="form-check form-switch d-flex">
                                            <input class="form-check-input" type="checkbox" id="theme-toggle" onclick="toggleTheme()"/>
                                            <label class="form-check-label" for="mySwitch" style="margin: .2em 0 0 .4em;">
                                                <span class="iconify" id="theme-icon" data-icon="clarity:sun-solid"></span>
                                            </label>
                                        </div>
                                        <li class="nav-item" style="margin: -.3em .5em; font-size: 1.5em;">
                                            <a class="nav-link" id="display-search-btn" title="Search" onclick="show_search()"><span class="iconify" data-icon="ant-design:search-outlined"></span></a>
                                        </li>

                                        <li class="nav-item dropdown" style="margin-top: -.6em;">
                                            <a href="#" class="nav-link dropdown-toggle" role="button" data-bs-toggle="dropdown">
                                                <span class="iconify" data-icon="ant-design:user-outlined"></span>
                                            </a>
                                            <ul class="dropdown-menu" id="dropdown">
                                                <li id="option1">
                                                    <a class="dropdown-item" id="dropdown-option1">
                                                        <span class="iconify" id="option1-icon"></span>
                                                        <span id="option1-value"></span>
                                                    </a>
                                                </li>
                                                <li id="option2">
                                                    <a class="dropdown-item" id="dropdown-option2">
                                                        <span class="iconify" id="option2-icon"></span>
                                                        <span id="option2-value"></span>
                                                    </a>
                                                </li>
                                            </ul>
                                        </li>
                                    </ul>

                                </div>
                            </div>
                        </nav>
                        <div class="d-flex flex-row-reverse">
                            <form method="get" id="search-form" style="display: none !important;" class="p-2" asp-controller="Home" asp-action="Search">
                                <input class="form-control me-2" type="search" placeholder="Search" name="query" aria-label="Search" required="required"/>
                                <input type="hidden" name="page" value="1"/>
                                <button class="btn btn-outline-success me-2" type="submit">
                                    <span class="iconify" data-icon="ant-design:search-outlined"></span>
                                </button>
                            </form>
                        </div>

                    </header>
                    <div class="container">
                        <partial name="_CookieConsentPartial"/>
                        <main role="main" class="pb-3">
                            @RenderBody()
                        </main>
                    </div>

                    <footer class="border-top footer text-muted">
                        <div class="container">
                        © 2019 - 2022 - Mark Mwirigi - <a asp-controller="Home" asp-action="Privacy">Privacy</a>
                        </div>
                    </footer>

                    @RenderSection("Scripts", required: false)
                 </body>
            </html>
        </vbxml>
    End Function

End Class