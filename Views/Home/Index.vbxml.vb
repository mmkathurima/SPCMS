Imports SPCMS.Models

Partial Public Class IndexView
    Private Iterator Function GenerateCards(skip As Integer) As IEnumerable(Of XElement)
        For Each spare As SpareParts In Spares.Skip(skip).Take(3)
            Yield _
                <div class="col">
                    <div class="card" style="width: 400px; height: 100%;">
                        <img class="card-img-top" src=<%= spare.Uploads.Split(";"c)(0).Replace("\"c, "/"c) %> alt="" style="width: 100%"/>
                        <div class="card-body">
                            <h4 class="card-title"><%= spare.Name %></h4>
                            <p class="card-text"><%= spare.Brand %></p>
                            <a class="btn btn-primary stretched-link" asp-controller="Home" asp-action="Details" asp-route-id=<%= spare.ID %>>Details</a>
                        </div>
                    </div>
                </div>
        Next spare
    End Function

    Public Overrides Function GetVbXml() As XElement
        Dim i As Integer = 0%
        REM Console.WriteLine("KEY: [{0}]", Newtonsoft.Json.Linq.JObject.Parse(IO.File.ReadAllText(IO.Path.Combine(IO.Directory.GetParent(Startup.GetEnv.WebRootPath).ToString(), "appsettings.json")))!key.ToString())
        Return _
            <vbxml>
                @Html.Raw(TempData["Response"])
                <div class="container-fluid mt-3">
                    <!-- Carousel -->
                    <div id="demo" class="carousel slide" data-bs-ride="carousel">

                        <!-- Indicators/dots -->
                        <div class="carousel-indicators">
                            <%= (Iterator Function() As IEnumerable(Of XElement)
                                     For i = 0 To 4
                                         If i = 0 Then
                                             Yield _
                                             <button type="button" data-bs-target="#demo" data-bs-slide-to=<%= i %> class="active"></button>
                                         Else Yield _
                                             <button type="button" data-bs-target="#demo" data-bs-slide-to=<%= i %>></button>
                                         End If
                                     Next i
                                 End Function)() %>
                        </div>

                        <!-- The slideshow/carousel -->
                        <div class="carousel-inner">
                            <%= (Iterator Function() As IEnumerable(Of XElement)
                                     i = 0
                                     For Each spare As SpareParts In Spares.OrderByDescending(Function(sp As SpareParts) sp.ID).Skip(1).Take(5)
                                         Yield _
                                         <div class=<%= If(i = 0, "carousel-item active", "carousel-item") %>>
                                             <img src=<%= spare.Uploads.Split(";"c)(0).Replace("\"c, "/"c) %> alt="" class="img d-block img-responsive" style="width: 50%; margin: auto;"/>
                                             <div class="carousel-caption">
                                                 <h3><%= spare.Name %></h3>
                                                 <p><%= spare.Brand %></p>
                                             </div>
                                         </div>
                                         i += 1
                                     Next spare
                                 End Function)() %>

                        </div>

                        <!-- Left and right controls/icons -->
                        <button class="carousel-control-prev" type="button" data-bs-target="#demo" data-bs-slide="prev">
                            <span class="carousel-control-prev-icon"></span>
                        </button>
                        <button class="carousel-control-next" type="button" data-bs-target="#demo" data-bs-slide="next">
                            <span class="carousel-control-next-icon"></span>
                        </button>
                    </div>
                    <div class="row">
                        <div class="col">
                            <div class="card">
                                <div class="card-body">
                                    <span class="iconify" style="font-size: 40pt;" data-icon="subway:tick"></span>
                                    <span>Quality Spare Parts</span>
                                </div>
                            </div>
                        </div>
                        <div class="col">
                            <div class="card">
                                <div class="card-body">
                                    <span class="iconify" style="font-size: 40pt;" data-icon="fa-solid:shipping-fast"></span>
                                    <span>Free shipping</span>
                                </div>
                            </div>
                        </div>
                        <div class="col">
                            <div class="card">
                                <div class="card-body">
                                    <span class="iconify" style="font-size: 40pt;" data-icon="fontisto:arrow-return-left"></span>
                                    <span>14-day return</span>
                                </div>
                            </div>
                        </div>
                        <div class="col">
                            <div class="card">
                                <div class="card-body">
                                    <span class="iconify" style="font-size: 40pt;" data-icon="ic:outline-support-agent"></span>
                                    <span>24/7 customer support</span>
                                </div>
                            </div>
                        </div>
                    </div>
                    <br/>

                    <div class="row">
                        <h2 class="section-title px-5"><span class="px-2">Just Arrived</span></h2>
                        <%= GenerateCards(skip:=5) %>
                    </div>

                    <div class="row">
                        <%= GenerateCards(skip:=8) %>
                    </div>
                    <br/>
                </div>

                <%= (Function()
                         If Not String.IsNullOrWhiteSpace(Session.User) And (Session.Role = Session.Roles.SPARE_PARTS_DEALER Or Session.Role = Session.Roles.ADMIN) Then
                             If CInt(ViewData!CriticalCount) > 0 Then
                                 Return _
                                 <div>
                                     <div class="position-fixed bottom-0 end-0 p-3" style="z-index: 11">
                                         <div id="liveToast" class="toast bg-danger hide" role="alert" aria-live="assertive" aria-atomic="true">
                                             <div class="toast-header">
                                                 <img class="rounded me-2" src="/favicon.ico"/>
                                                 <strong class="me-auto">SPCMS</strong>
                                                 <small>Now</small>
                                                 <button type="button" class="btn-close" data-bs-dismiss="toast" aria-label="Close"></button>
                                             </div>
                                             <div class="toast-body"><%= ViewData!CriticalCount.ToString() %> item(s) need to be restocked. View item(s) with ID(s) <%= String.Join(", ", CType(ViewData!lowStock, List(Of String))) %> in the 'View listings' section.</div>
                                         </div>
                                     </div>
                                 </div>
                             End If
                         End If
                         Return Nothing
                     End Function)() %>
                <br/>
                <div class="row">
                    <h2 class="section-title px-5">
                        <span class="px-2">Top Brands</span>
                    </h2>
                    <div class="col">
                        <img class="card-img-top invertible" src="https://cdn.freebiesupply.com/logos/large/2x/maxxis-logo-black-and-white.png" alt=""/>
                    </div>
                    <div class="col">
                        <img class="card-img-top invertible" src="https://cdn.freebiesupply.com/logos/large/2x/monroe-logo-png-transparent.png" alt=""/>
                    </div>
                    <div class="col" style="display: flex">
                        <img class="card-img-top invertible" src="https://cdn.freebiesupply.com/logos/large/2x/brembo-2-logo-png-transparent.png" alt=""/>
                    </div>
                </div>

                <script>
                    $(document).ready(function () {
                        init_session("<%= Session.User %>", "<%= CInt(Session.Role) %>");
                    });
                </script>
            </vbxml>
    End Function
End Class
