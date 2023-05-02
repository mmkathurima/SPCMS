Imports System.IO
Imports System.Text

Imports SPCMS.Models

Imports iText.Kernel.Pdf
Imports iText.Html2pdf

Partial Public Class CheckoutView
    Private Function RenderPdf() As String
        Dim builder As New StringBuilder("<h2>" & Spares.Count & " listings in checkout</h2>"),
            base64 As String, subTotal As Integer = 0, path As String
        For Each spare As SpareParts In Spares
            subTotal% += spare.Price
            path$ = IO.Path.Combine(Startup.GetEnv.WebRootPath, spare.Uploads.Split(";"c)(0).Replace("\"c, "/"c).Substring(2))
            base64$ = String.Format("data:image/{0};base64,{1}", IO.Path.GetExtension(path$).Replace("."c, ""), Convert.ToBase64String(File.ReadAllBytes(path$)))
            builder.Append(<div>
                               <h3><%= String.Format("{0} for {1} {2} {3}", spare.Name, spare.YearOfManufacture, spare.VehicleMake, spare.VehicleModel) %></h3>

                               <table border="1" width="100%">
                                   <tr>
                                       <td rowspan="10">
                                           <img class="img-fluid img-responsive rounded product-image" width="150" src=<%= base64 %>/>
                                       </td>
                                   </tr>
                                   <tr>
                                       <td style="font-weight: bold; width: 50%;">Ordered Quantity:</td>
                                       <td><%= spare.OrderQuantity %></td>
                                   </tr>
                                   <tr>
                                       <td style="font-weight: bold;">Order Identification Number:</td>
                                       <td><%= spare.OrderIdentificationNumber %></td>
                                   </tr>
                                   <tr>
                                       <td style="font-weight: bold;">Seller:</td>
                                       <td><%= spare.Username %></td>
                                   </tr>
                                   <tr>
                                       <td style="font-weight: bold;">Price:<small> (EXCLUSIVE OF VAT)</small></td>
                                       <td>KSh. <%= Format(Val(spare.Price), "#,#.00") %></td>
                                   </tr>
                                   <tr>
                                       <td style="font-weight: bold;">Spare Part Category:</td>
                                       <td><%= spare.Category %></td>
                                   </tr>
                                   <tr>
                                       <td style="font-weight: bold;">Spare Part Brand:</td>
                                       <td><%= spare.Brand %></td>
                                   </tr>
                                   <tr>
                                       <td style="font-weight: bold;">Spare part colour:</td>
                                       <td><%= spare.Colour %></td>
                                   </tr>
                                   <tr>
                                       <td style="font-weight: bold;">Spare part material:</td>
                                       <td><%= spare.Material %></td>
                                   </tr>
                                   <tr>
                                       <td style="font-weight: bold;">Spare part dimensions:</td>
                                       <td><%= spare.Dimensions %></td>
                                   </tr>
                               </table><br/>
                           </div>)
        Next spare
        builder.Append(<table border="1" width="100%">
                           <tr>
                               <td style="font-weight: bold; width: 50%;">SUBTOTAL</td>
                               <td>KSh. <%= Format(subTotal%, "#,#.00") %></td>
                           </tr>
                           <tr>
                               <td style="font-weight: bold;">SHIPPING</td>
                               <td>0.00</td>
                           </tr>
                           <tr style="font-size: 16pt;">
                               <td style="font-weight: bold;">TOTAL:</td>
                               <td style="font-weight: bold;">KSh. <%= Format(subTotal%, "#,#.00") %></td>
                           </tr>
                       </table>)

        Using stream As New MemoryStream()
            Using writer As New PdfWriter(stream)
                HtmlConverter.ConvertToPdf(builder.ToString(), writer)
                Return Convert.ToBase64String(stream.ToArray())
            End Using
        End Using
    End Function

    Public Overrides Function GetVbXml() As XElement
        Return _
        <vbxml>
            <h2 fff=""> Browse Checkout</h2>
            <p><%= Spares.Count %> listing(s) found:</p>
            <%= (Iterator Function() As IEnumerable(Of XElement)
                     For Each spare As SpareParts In Spares
                         Yield <div class="row p-2 border rounded">
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
                                               WAS <span class="strike-text">KSh. <%= Format(Val(spare.Price * 1.1), "#,#.00") %></span>
                                       </div>
                                       <h6 class="text-success">Free shipping</h6>
                                   </div>

                               </div>
                     Next spare
                 End Function)()
            %>
            <div>
                <button class="btn btn-success" id="confirm" style="margin: 20px 0;" onclick="confirmCheckout(this)">
                    <span class="iconify" data-icon="line-md:confirm-circle"></span>Confirm</button>
            </div>
            <object width="100%" height="100%" data=<%= "data:application/pdf;base64," & RenderPdf$().Trim() %> type="application/pdf"/>

            <script>
                $(document).ready(function () {
                    init_session("<%= Session.User %>", "<%= CInt(Session.Role) %>");
                });
            </script>
        </vbxml>

    End Function

End Class
