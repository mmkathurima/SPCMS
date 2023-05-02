Imports SPCMS.Models

Partial Public Class WishlistView
    Public Overrides Function GetVbXml() As XElement
        Dim original As List(Of SpareParts) = CType(ViewData!original, List(Of SpareParts))

        Return _
        <vbxml>
            @Html.Raw(TempData["Delete"])

            <ul>
                <div class="row">
                    <%= Utils.RenderFilter(original, CInt(ViewData!page.ToString())) %>
                    <div class="col-md-1"></div>

                    <div class="col-md-9" id="loaded">
                        <h2 fff=""> Browse My Wishlist:</h2>
                        <p><%= ViewData!total_rows.ToString() %> listing(s) found:</p>

                        <%= (Iterator Function() As IEnumerable(Of XElement)
                                 For Each spare As SpareParts In Spares
                                     Yield <div class="col-md-9">
                                               <div class="row p-2 border rounded">
                                                   <div class="col-md-3 mt-1">
                                                       <img class="img-fluid img-responsive rounded product-image" src=<%= spare.Uploads.Split(";"c)(0).Replace("\"c, "/"c) %>/>
                                                   </div>
                                                   <div class="col-md-6 mt-1">
                                                       <h5><%= String.Format("{0} for {1} {2} {3}", spare.Name, spare.YearOfManufacture, spare.VehicleMake, spare.VehicleModel) %></h5>
                                                       <div class="mt-1 mb-1 spec-1">
                                                           <span><%= spare.Category %></span>
                                                           <span class="dot"></span>
                                                           <span><%= spare.Brand %></span>
                                                           <span class="dot"></span>
                                                           <span><%= spare.Colour %><br/></span>
                                                       </div>
                                                       <div class="mt-1 mb-1 spec-1">
                                                           <span><%= spare.Material %></span>
                                                           <span class="dot"></span>
                                                           <span><%= spare.Dimensions %></span>
                                                       </div>
                                                       <p class="text-justify text-truncate para mb-0"><%= spare.Description %><br/><br/></p>
                                                   </div>
                                                   <div class="align-items-center align-content-center col-md-3 border-left mt-1">
                                                       <div class="d-flex flex-row align-items-center">
                                                           <h4 class="mr-1">KSh. <%= Format(Val(spare.Price), "#,#.00") %></h4>
                                               WAS <span class="strike-text">KSh. <%= Format((spare.Price * 1.1), "#,#.00") %></span>
                                                       </div>
                                                       <h6 class="text-success">Free shipping</h6>
                                                       <div class="d-flex flex-column mt-4">

                                                           <a class="btn btn-primary stretched-link" asp-controller="Home" asp-action="Details" style="width: 100%; margin-bottom: 10px;" asp-route-id=<%= spare.ID %>>Details</a>
                                                           <!--</form>-->

                                                           <%= (Function() As XElement

                                                                    Return If(Not String.IsNullOrWhiteSpace(Session.User) AndAlso (Session.Role = Session.Roles.CONSUMER Or Session.Role = Session.Roles.ADMIN),
                                                           <form asp-action="DeleteWishlist" method="post" onsubmit="return confirm('Do you really want to delete this item from your wishlist?');" style="z-index: 2">
                                                               <input type="hidden" value=<%= spare.ID %> name="id"/>
                                                               <input type="hidden" value=<%= spare.Username %> name="username"/>

                                                               <input class="btn btn-danger" type="submit" value="Delete" style="width: 100%;"/>
                                                           </form>, Nothing)
                                                                End Function)() %>
                                                       </div>
                                                   </div>
                                               </div>
                                           </div>
                                 Next spare
                             End Function)() %>
                    </div>
                </div>

                <ul class="pagination">
                    <%= (Function() As XElement
                             Console.WriteLine("PREV: {0}{1}FROM: {2}{3}TO: {4}{5}NEXT: {6}{7}TOTAL PAGES: {8}{9}PER: {10}", ViewData!prev, vbLf, ViewData!from, vbLf, ViewData!to, vbLf, ViewData!next, vbLf, ViewData!total_pages, vbLf, ViewData!per)
                             Return If(CInt(ViewData!prev) >= 1,
                             <li class="page-item">
                                 <a class="page-link" href=<%= RequestBuilder("/WishList?", ViewData!getStr, Page.PREV) %>>Previous</a>
                             </li>, Nothing)
                         End Function)() %>

                    <%= (Iterator Function() As IEnumerable(Of XElement)
                             For Each p As Integer In Enumerable.Range(CInt(ViewData!from), CInt(ViewData!to))
                                 Yield <li class="page-item nums">
                                           <%= (Function()
                                                    Return If(p <> 0,
                                                    <a class="page-link" href=<%= RequestBuilder("/WishList?", ViewData!getStr, Page.ELSE, p) %>><%= p %></a>,
                                                    Nothing)
                                                End Function)() %>
                                       </li>
                             Next p
                         End Function)() %>

                    <%= (Function() As XElement
                             Return If(CInt(ViewData!next) <= CInt(ViewData!total_pages),
                             <li class="page-item">
                                 <a class="page-link" href=<%= RequestBuilder("/WishList?", ViewData!getStr, Page.NEXT) %>>Next</a>
                             </li>, Nothing)
                         End Function)() %>
                </ul>
            </ul>
            <script>
                $(document).ready(function () {
                    init_session("<%= Session.User %>", "<%= CInt(Session.Role) %>");
                    $(".nums").removeClass("active");
                    try {
                        document.getElementsByClassName("nums")[parseInt("<%= ViewData!page %>") % parseInt("<%= ViewData!per %>")].classList.add("active");
                    } catch (e) {}
                    initRange([<%= ViewData!lowerPrice %>, <%= ViewData!upperPrice %>], [<%= ViewData!lowerYear %>, <%= ViewData!upperYear %>]);
                    doFilter("/WishList?");
                });
            </script>
        </vbxml>
    End Function

End Class
