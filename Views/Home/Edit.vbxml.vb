Imports SPCMS.Models

Partial Public Class EditView
    Public Overrides Function GetVbXml() As XElement
        Return _
             <vbxml>
                 <div>
                     @Html.Raw(ViewData["Update"])
                     <h3>Edit Records</h3>
                     <form asp-controller="Home" asp-action="SaveEdits" method="post">
                         <div class="mb-3 mt-3">
                             <label for="category" class="form-label">Spare part category:</label>
                             <input type="text" class="form-control" id="category" placeholder="Spare part categorization" name="category" required="true" value=<%= Spare.Category %>/>
                             <div class="valid-feedback">Valid.</div>
                             <div class="invalid-feedback">Please fill out this field.</div>
                         </div>

                         <div class="mb-3 mt-3">
                             <label for="quantity" class="form-label">Spare part quantity:</label>
                             <input type="number" class="form-control" id="quantity" placeholder="Spare part quantity" name="quantity" required="true" value=<%= Spare.Quantity %>/>
                             <div class="valid-feedback">Valid.</div>
                             <div class="invalid-feedback">Please fill out this field.</div>
                         </div>

                         <div class="mb-3 mt-3">
                             <label for="description" class="form-label">Spare part description and note:</label>
                             <textarea class="form-control" id="description" placeholder="Description and note of the spare part" name="description" required="true"><%= Spare.Description %></textarea>
                             <div class="valid-feedback">Valid.</div>
                             <div class="invalid-feedback">Please fill out this field.</div>
                         </div>

                         <div class="mb-3 mt-3">
                             <label for="price" class="form-label">Spare part pricing:</label>
                             <input type="number" class="form-control" id="price" placeholder="Spare part pricing" name="price" required="true" value=<%= Spare.Price %>/>
                             <div class="valid-feedback">Valid.</div>
                             <div class="invalid-feedback">Please fill out this field.</div>
                         </div>

                         <input type="hidden" name="ID" value=<%= Spare.ID %>/>
                         <button type="submit" class="btn btn-primary">Submit</button>
                     </form>
                 </div>
                 <script>
                    $(document).ready(function () {
                        init_session("<%= Session.User %>", "<%= CInt(Session.Role) %>");
                    });
                </script>
             </vbxml>
    End Function

End Class
