Imports System.Text

Imports SPCMS.Models

Public Module Utils
    Public Enum Page
        PREV
        [NEXT]
        [ELSE]
    End Enum

    Public Function RequestBuilder(url As String, getStr As String, page As Page, Optional otherPgs As Integer? = Nothing) As String
        Dim builder As New StringBuilder(url), qSplit As String()
        For Each q As String In getStr.ToString().Split(";"c)
            qSplit = q.Split(":"c)
            For Each q1 As String In qSplit(1).Split(","c)
                If page = Page.PREV And qSplit(0) = "page" Then
                    builder.Append(String.Format("{0}={1}&", qSplit(0), q1 - 1))
                ElseIf page = Page.NEXT And qSplit(0) = "page" Then
                    builder.Append(String.Format("{0}={1}&", qSplit(0), q1 + 1))
                Else
                    builder.Append(If(otherPgs.HasValue And qSplit(0) = "page", String.Format("{0}={1}&", qSplit(0), otherPgs), String.Format("{0}={1}&", qSplit(0), q1)))
                End If
            Next q1
        Next q
        Return builder.ToString().TrimEnd("&"c)
    End Function

    Public Function RenderFilter(spares As List(Of Models.SpareParts), page As Integer) As XElement
        Return _
            <div class="col-md-2">
                <form id="filterForm" method="get">

                    <div>
                        <label for="priceRange">Price range:</label>
                        <input type="text" id="priceRange" readonly="readonly" style="border:0; color:#f6931f; font-weight:bold;"/>
                        <input type="hidden" id="minPrice" name="minPrice"/>
                        <input type="hidden" id="maxPrice" name="maxPrice"/>
                    </div>
                    <div class="mb-3 mt-3" id="price-range"></div>

                    <%= (Iterator Function() As IEnumerable(Of XElement)
                             Dim iter As New Dictionary(Of String, IEnumerable(Of String))() From {
                             {"make", spares.Select(Function(s As SpareParts) s.VehicleMake).Distinct()},
                             {"model", spares.Select(Function(s As SpareParts) s.VehicleModel).Distinct()},
                             {"category", spares.Select(Function(s As SpareParts) s.Category).Distinct()},
                             {"brand", spares.Select(Function(s As SpareParts) s.Brand).Distinct()},
                             {"colour", spares.Select(Function(s As SpareParts) s.Colour).Distinct()},
                             {"material", spares.Select(Function(s As SpareParts) s.Material).Distinct()},
                             {"seller", spares.Select(Function(s As SpareParts) s.Username).Distinct()}
                             }

                             For Each sp As KeyValuePair(Of String, IEnumerable(Of String)) In iter
                                 Yield <div>
                                           <label for=<%= sp.Key %>>Filter <%= sp.Key %>...</label>
                                           <select class="form-control selekt" id=<%= sp.Key %> name=<%= sp.Key %> multiple="multiple" placeholder=<%= "Filter " & sp.Key %>>
                                               <%= (Iterator Function() As IEnumerable(Of XElement)
                                                        For Each c As String In sp.Value
                                                            Yield <option value=<%= c %>><%= c %></option>
                                                        Next c
                                                    End Function)() %>
                                           </select>
                                           <br/>
                                       </div>
                             Next sp
                         End Function)() %>
                    <input type="hidden" name="page" value=<%= page %>/>

                    <div>
                        <label for="yearRange">Year range:</label>
                        <input type="text" id="yearRange" readonly="readonly" style="border:0; color:#f6931f; font-weight:bold;"/>
                        <input type="hidden" id="minYear" name="minYear"/>
                        <input type="hidden" id="maxYear" name="maxYear"/>
                    </div>
                    <div class="mb-3 mt-3" id="year-range"></div>

                    <div>Listings per page:</div>
                    <%= (Iterator Function() As IEnumerable(Of XElement)
                             Dim j As Integer = 0
                             For i As Integer = 5 To 25 Step 5
                                 j += 1
                                 Yield _
                                         <div class="custom-control custom-radio custom-control-inline">
                                             <%= If(i = 5,
                                                 <input type="radio" id=<%= String.Format("customRadioInline{0}", j) %> name="limit" class="custom-control-input" value=<%= i %> checked="checked"/>,
                                                 <input type="radio" id=<%= String.Format("customRadioInline{0}", j) %> name="limit" class="custom-control-input" value=<%= i %>/>) %>
                                             <label class="custom-control-label" for=<%= String.Format("customRadioInline{0}", j) %>><%= i %></label>
                                         </div>
                             Next i
                         End Function)() %>

                    <div class="mb-3 mt-3">
                        <div class="row">

                            <div class="col">
                                <button type="submit" class="btn btn-primary">Set filter</button>
                            </div>
                        </div>
                    </div>

                </form>
                <script>
                    $(".selekt").multiselect({
                        disableIfEmpty: true,
                        numberDisplayed: 1,
                        includeSelectAllOption: true,
                        enableFiltering: true,
                        enableCaseInsensitiveFiltering: true,
                        enableResetButton: true,
                        includeResetAllOption: true,
                        resetText: "Reset all"
                    });
                </script>
            </div>
    End Function

End Module
