Imports SPCMS.Models

Partial Public Class DetailsView
    Public Overrides Function GetVbXml() As XElement
        Return _
         <vbxml>
             <div id="cart"></div>
             <div id="wishlist"></div>

             <div class="row">
                 <div class="col item-photo">
                     <div class="images p-3">
                         <div class="text-center p-4"><img class="img-fluid" id="main-image" src=<%= Spare.Uploads.Split(";"c)(0).Replace("\"c, "/"c) %> width="570" height="400"/></div>
                         <div class="thumbnail text-center">
                             <%= (Iterator Function() As IEnumerable(Of XElement)
                                      For Each img As String In Spare.Uploads.Split(";"c)
                                          If Not String.IsNullOrWhiteSpace(img) Then
                                              Yield <img class="img-fluid" onclick="change_image(this)" src=<%= img.Replace("\"c, "/"c) %> width="70"/>
                                          End If
                                      Next img
                                  End Function)() %>
                         </div>
                     </div>
                 </div>
                 <div class="col">
                     <div style="padding-left: 1em;">
                         <h3 id="product-name"><%= String.Format("{0} for {1} {2} {3}", Spare.Name, Spare.YearOfManufacture, Spare.VehicleMake, Spare.VehicleModel) %></h3>
                         <h6 class="fw-light"><%= Spare.Category %></h6>
                         <h6>Sold by <%= Spare.Username?.ToString() %></h6>
                         <h4 class="text-primary" style="margin-top:0px;">KSh. <%= Format(Val(Spare.Price), "#,#.00") %></h4>
                         <small>WAS <span class="strike-text">KSh. <%= Format((Spare.Price * 1.1), "#,#.00") %></span></small>

                         <div class="row">
                             <div class="col">
                                 <fieldset class="border border-4 rounded-3 p-3 swatch">
                                     <legend class="float-none w-auto px-1" style="font-size: 12pt;">MATERIAL</legend>
                                     <div class="form-check">
                                         <input type="radio" class="form-check-input" readonly="readonly"/>
                                         <label for="control_01" style="padding: .2em 1em 0 1em;">
                                             <p class="radio-lbl"><%= Spare.Material %></p>
                                         </label>
                                     </div>
                                 </fieldset>
                             </div>
                             <div class="col">
                                 <fieldset class="swatch-picker border border-4 rounded-3 p-3">
                                     <legend class="float-none w-auto px-1" style="font-size: 12pt;">COLOR</legend>
                                     <label>
                                         <input type="radio" readonly="readonly" checked=""/>
                                         <span style=<%= "background-color: " & Spare.Colour %>>Blue</span>
                                     </label>
                                 </fieldset>
                             </div>
                         </div>
                         <hr/>

                         <div class="section" style="padding-bottom:20px;">
                             <%= (Function() As XElement
                                      Console.WriteLine("ORDER QUANTITY: {0}{1}QUANTITY: {2}{3}SESSION USER: {4}{5}SESSION.ROLE: {6}", Spare.OrderQuantity, vbLf, Spare.Quantity, vbLf, Session.User, vbLf, Session.Role)
                                      If Not String.IsNullOrWhiteSpace(Spare.OrderQuantity) And (Session.Role = Session.Roles.ADMIN Or Session.Role = Session.Roles.CONSUMER) Then
                                          Return <div class="num-block skin-2">
                                                     <h6 class="title-attr"><small>Ordered Quantity</small></h6>
                                                     <div class="number">
                                                         <button class="btn minus disabled">-</button>
                                                         <input class="form-control orderQuantity" id="quantity" type="text" value=<%= Convert.ToInt32(Spare.OrderQuantity) %> readonly="readonly"/>
                                                         <button class="btn plus disabled">+</button>
                                                     </div>
                                                 </div>
                                      ElseIf String.IsNullOrWhiteSpace(Session.User) Or String.IsNullOrWhiteSpace(Spare.OrderQuantity) Then
                                          Return <div class="num-block skin-2">
                                                     <h6 class="title-attr"><small>Quantity</small></h6>
                                                     <div class="number">
                                                         <button class="btn minus">-</button>
                                                         <input class="form-control orderQuantity" id="quantity" name="quantity" type="text" value="1"/>
                                                         <button class="btn plus">+</button>
                                                     </div>
                                                 </div>
                                      End If
                                      Return Nothing
                                  End Function)() %>
                         </div>

                         <div class="section">
                             <button class=<%= If(Not String.IsNullOrWhiteSpace(Session.User) AndAlso (Session.Role = Session.Roles.CONSUMER Or Session.Role = Session.Roles.ADMIN) AndAlso Not String.IsNullOrWhiteSpace(Spare.Quantity), "btn btn-primary", "btn btn-primary disabled") %> id="addCart" style="margin-bottom:20px;">
                                 <span class="iconify" data-icon="ant-design:shopping-cart-outlined"></span> Add to Cart
                             </button>
                             <h6>
                                 <a href="#" id="addWishlist" style=<%= If(Not String.IsNullOrWhiteSpace(Session.User) AndAlso (Session.Role = Session.Roles.CONSUMER Or Session.Role = Session.Roles.ADMIN), "", "color: gray; pointer-events: none;") %>><span class="iconify" data-icon="akar-icons:heart"></span> Add to wishlist</a>
                             </h6>
                         </div>
                     </div>
                 </div>
             </div>
             <div class="row">
                 <div class="col">
                     <ul class="nav nav-pills" role="tablist">
                         <li class="nav-item">
                             <a class="nav-link active" data-bs-toggle="pill" href="#home">Description</a>
                         </li>
                         <li class="nav-item">
                             <a class="nav-link" data-bs-toggle="pill" href="#menu1">Location</a>
                         </li>
                     </ul>

                     <!-- Tab panes -->
                     <div class="tab-content">
                         <div id="home" class="container tab-pane active"><br/>
                             <div style="width:100%;border-top:1px solid silver">
                                 <p style="padding:15px;">
                                     <small>
                                         <%= Spare.Description %>
                                     </small>
                                 </p>
                                 <small>
                                     <ul>
                                         <li><b>Category: </b><%= Spare.Category %></li>
                                         <li><b>Brand: </b><%= Spare.Brand %></li>
                                         <li><b>Make: </b><%= Spare.VehicleMake %></li>
                                         <li><b>Model: </b><%= Spare.VehicleModel %></li>
                                         <li><b>Colour: </b><%= Spare.Colour %></li>
                                         <li><b>Material: </b><%= Spare.Material %></li>
                                         <li><b>Dimensions: </b><%= Spare.Dimensions %></li>

                                     </ul>
                                 </small>
                             </div>
                         </div>
                         <div id="menu1" class="container tab-pane fade"><br/>
                             <!--<div style="width: 100%">
                                 <iframe width="100%" height="600" frameborder="0" scrolling="no" marginheight="0" marginwidth="0" src=<%= HttpUtility.HtmlDecode($"https://maps.google.com/maps?width=100%25&amp;height=600&amp;hl=en&amp;q={Spare.Latitude},%20{Spare.Longitude}+(My%20Business%20Name)&amp;t=&amp;z=14&amp;ie=UTF8&amp;iwloc=B&amp;output=embed") %>><a href="https://www.gps.ie/farm-gps/">gps for tractors</a>
                                 </iframe>
                             </div>-->

                             <div id="map-embed" width="100%"></div>
                         </div>
                     </div>
                 </div>
             </div>
             <script>
                $(document).ready(function() {
                    let formData;
                    details(<%= Spare.Quantity %>);
                    loadMap(<%= Spare.Latitude %>, <%= Spare.Longitude %>);
                    document.title = "<%= String.Format("{0} for {1} {2} {3} ", Spare.Name, Spare.YearOfManufacture, Spare.VehicleMake, Spare.VehicleModel) %>" + document.title;
                    
                    init_session("<%= Session.User %>", "<%= CInt(Session.Role) %>");

                    document.getElementById("addCart").addEventListener("click", function (e) {
                        formData = { id: <%= $"""{Spare.ID}""" %>, username: <%= $"""{Session.User}""" %>, quantity: document.getElementById("quantity").value, maxQuantity: <%= $"""{Spare.Quantity}""" %>};
                        e.preventDefault();
                        console.log(formData);
                        $.ajax({
                            type: "GET",
                            url: "AddCart",
                            data: formData,
                            success: function (response, status) {
                                document.getElementById("cart").innerHTML = response;
                            },
                            complete: function (jq, status) {
                                console.log(`[JQUERY AJAX COMPLETE WITH STATUS ${status}\n${jq.responseText}]`);
                            },
                            error: function (jq, exception, detail) {
                                console.log(jq);
                                console.log(exception);
                                console.log(detail);
                            }
                        });
                    });

                    document.getElementById("addWishlist").addEventListener("click", function (e) {
                        e.preventDefault();
                        formData = { id: <%= $"""{Spare.ID}""" %>, username: <%= $"""{Session.User}""" %>, quantity: document.getElementById("quantity").value, maxQuantity: <%= $"""{Spare.Quantity}""" %>};
                        delete formData.quantity;
                        delete formData.maxQuantity;
                        $.ajax({
                            type: "GET",
                            url: "AddWishlist",
                            data: formData,
                            success: function (response, status) {
                                document.getElementById("wishlist").innerHTML = response;
                            },
                            complete: function (jq, status) {
                                console.log(`[JQUERY AJAX COMPLETE FOR ${this.url} WITH STATUS ${status}\n${jq.responseText}]`);
                            },
                            error: function (jq, exception, detail) {
                                console.log(jq);
                                console.log(exception);
                                console.log(detail);
                            }
                        });
                    });
                });
             </script>
         </vbxml>

    End Function

End Class
