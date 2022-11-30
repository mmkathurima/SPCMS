Imports System
Imports System.IO
Imports Newtonsoft.Json

Imports SPCMS.Models

Partial Public Class AddView
    Public Overrides Function GetVbXml() As XElement
        Return _
        <vbxml>
            @Html.Raw(ViewData["Response"])
            <h3 class="fw-light">Add Spare Part Listing</h3>
            <form id="addSpares" asp-controller="Home" asp-action="AddListing" method="post" enctype="multipart/form-data">
                <div class="mb-3 mt-3">
                    <label for="name" class="form-label">Spare part name:</label>
                    <input type="text" class="form-control" id="name" placeholder="Name of the spare part" name="name" required="true"/>
                    <div class="valid-feedback">Valid.</div>
                    <div class="invalid-feedback">Please fill out this field.</div>
                </div>

                <div class="mb-3 mt-3">
                    <label for="category" class="form-label">Spare part category:</label>
                    <input type="text" class="form-control" id="category" placeholder="Spare part categorization" name="category" required="true"/>
                    <div class="valid-feedback">Valid.</div>
                    <div class="invalid-feedback">Please fill out this field.</div>
                </div>

                <div class="mb-3 mt-3">
                    <label for="brand" class="form-label">Spare part brand:</label>
                    <input type="text" class="form-control" id="brand" placeholder="Brand of the spare part" name="brand" required="true"/>
                    <div class="valid-feedback">Valid.</div>
                    <div class="invalid-feedback">Please fill out this field.</div>
                </div>

                <div class="mb-3 mt-3">
                    <label for="id" class="form-label">Spare part ID number:</label>
                    <input type="text" class="form-control" id="id" placeholder="Spare part identification number" name="id" required="true"/>
                    <div class="valid-feedback">Valid.</div>
                    <div class="invalid-feedback">Please fill out this field.</div>
                </div>

                <input type="hidden" id="make" name="make"/>

                <div class="mb-3 mt-3">
                    <label for="model" class="form-label">Vehicle make:</label>
                    <select class="form-select selekt" id="model" name="model" required="true">
                    @using System
                    @using System.IO
                    @using Newtonsoft.Json

                    @{
                        StreamReader reader = new StreamReader(System.IO.Path.Combine(Startup.GetEnv.WebRootPath, "cars.json"));
                        dynamic j = JsonConvert.DeserializeObject(reader.ReadToEnd());
                        reader.Close();
                    }
                        @foreach (var l in j["list"])
                        {
                            <optgroup label="@l.Name">
                                @foreach (var k in l)
                                {
                                    @foreach (var m in k)
                                    {
                                        <option value="@m">@m</option>
                                    }
                                }
                            </optgroup>
                        }
                    </select>
                    <div class="valid-feedback">Valid.</div>
                    <div class="invalid-feedback">Please fill out this field.</div>
                </div>

                <div class="mb-3 mt-3">
                    <label for="quantity" class="form-label">Spare part quantity:</label>
                    <input type="number" class="form-control" id="quantity" placeholder="Spare part quantity" name="quantity" required="true"/>
                    <div class="valid-feedback">Valid.</div>
                    <div class="invalid-feedback">Please fill out this field.</div>
                </div>

                <div class="mb-3 mt-3">
                    <label for="quantityRequired" class="form-label">Spare part quantity required:</label>
                    <input type="number" class="form-control" id="quantityRequired" placeholder="Spare part required quantity" name="quantityRequired" required="true"/>
                    <div class="valid-feedback">Valid.</div>
                    <div class="invalid-feedback">Please fill out this field.</div>
                </div>

                <div class="mb-3 mt-3">
                    <label for="price" class="form-label">Spare part pricing:</label>
                    <input type="number" class="form-control" id="price" placeholder="Spare part pricing" name="price" required="true"/>
                    <div class="valid-feedback">Valid.</div>
                    <div class="invalid-feedback">Please fill out this field.</div>
                </div>

                <div class="mb-3 mt-3">
                    <label for="year" class="form-label">Year of manufacture:</label>
                    <input type="number" class="form-control" id="year" placeholder="Year of manufacture" name="year" required="true"/>
                    <div class="valid-feedback">Valid.</div>
                    <div class="invalid-feedback">Please fill out this field.</div>
                </div>

                <div class="upload__box">
                    <div class="upload__btn-box">
                        <label class="upload__btn">
                            <p style="margin: 1em 10px;">Upload spare part images:</p>
                            <input type="file" multiple="true" name="uploads" data-max_length="20" class="upload__inputfile" required="true"/>
                        </label>
                    </div>
                    <div class="upload__img-wrap"></div>
                </div>

                <div class="mb-3 mt-3">
                    <label for="description" class="form-label">Spare part description and note:</label>
                    <textarea class="form-control" id="description" placeholder="Description and note of the spare part." name="description" required="true">
                    </textarea>
                    <div class="valid-feedback">Valid.</div>
                    <div class="invalid-feedback">Please fill out this field.</div>
                </div>

                <div class="mb-3 mt-3">
                    <label for="colour" class="form-label">Spare part colour:</label>
                    <input class="form-control" type="color" name="colour" id="colour" value="#f0f8ff" list="colourList" required="true"/>
                    <datalist id="colourList">
                        <%= (Iterator Function() As IEnumerable(Of XElement)
                                 Dim reader As New StreamReader(IO.Path.Combine(Startup.GetEnv.WebRootPath, "colors.json"))
                                 Dim colors As IOrderedEnumerable(Of KeyValuePair(Of String, String)) = JsonConvert.DeserializeObject(Of Dictionary(Of String, String))(reader.ReadToEnd()).DistinctBy(Function(d As KeyValuePair(Of String, String)) d.Value).OrderBy(Function(o As KeyValuePair(Of String, String)) o.Value)
                                 reader.Close()
                                 For Each k As KeyValuePair(Of String, String) In colors
                                     Yield <option value=<%= k.Value %>/>
                                 Next k
                             End Function)()
                        %>
                    </datalist>
                    <div class="valid-feedback">Valid.</div>
                    <div class="invalid-feedback">Please fill out this field.</div>
                </div>

                <div class="mb-3 mt-3">
                    <label for="material" class="form-label">Spare part material:</label>
                    <input type="text" class="form-control" id="material" placeholder="Material of the spare part" name="material" required="true"/>
                    <div class="valid-feedback">Valid.</div>
                    <div class="invalid-feedback">Please fill out this field.</div>
                </div>

                <div class="mb-3 mt-3">
                    <label for="dimensions" class="form-label">Spare part dimensions:</label>
                    <input type="text" class="form-control" id="dimensions" placeholder="Dimensions of the spare part" name="dimensions" required="true"/>
                    <div class="valid-feedback">Valid.</div>
                    <div class="invalid-feedback">Please fill out this field.</div>
                </div>
                <input type="hidden" name="username" value=<%= Session.User %>/>
                <button type="submit" class="btn btn-primary">Submit</button>
            </form>
            <script>
	            $(document).ready(function(){
                    imgUpload();
                    let li;
                    function select_onchange() {
                        //alert(selected.closest('optgroup').attr("label"));
                        li = $(".multiselect-container").find("li.active:not(.multiselect-group)")[0];
                        $("#make").val($(li).prevAll(".multiselect-group:first").text());
                        console.log($("#make").val());
                    }
                    $('select').change(select_onchange);
                    init_session("<%= Session.User %>", "<%= CInt(Session.Role) %>");
                    $(".selekt").multiselect({
                        disableIfEmpty: true,
                        numberDisplayed: 1,
                        includeSelectAllOption: true,
                        enableFiltering: true,
                        enableCaseInsensitiveFiltering: true,
                        maxHeight: 300
                    });
	                select_onchange();
                });
            </script>
        </vbxml>

    End Function

End Class
