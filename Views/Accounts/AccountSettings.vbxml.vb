Imports SPCMS.Models

Partial Public Class AccountSettingsView

    Public Overrides Function GetVbXml() As XElement
        Dim user = ViewData!User
        Dim latitude As Double = user.Item3
        Dim longitude As Double = user.Item4

        Return _
        <vbxml>
            @Html.Raw(TempData["Response"])
            <h2>Account Settings</h2>
            <div>
                <h5 class="fw-light">Change Password</h5>
                <form asp-controller="Accounts" asp-action="SavePassword" method="post" onsubmit=<%= String.Format("return savePassword('{0}')", Session.User) %> id="savePassword">
                    <div class="mb-3 mt-3">
                        <label for="category" class="form-label">Enter old password:</label>
                        <input type="password" Class="form-control" id="oldPassword" placeholder="Enter old password" name="oldPassword" required="true"/>
                        <div class="valid-feedback">Valid.</div>
                        <div class="invalid-feedback">Please fill out this field.</div>
                    </div>

                    <div class="mb-3 mt-3">
                        <label for="quantity" class="form-label">Enter new password:</label>
                        <input type="password" class="form-control" id="newPassword" placeholder="Enter new password" name="newPassword" required="true"/>
                        <div class="valid-feedback">Valid.</div>
                        <div class="invalid-feedback">Please fill out this field.</div>
                    </div>

                    <div class="mb-3 mt-3">
                        <label for="quantity" class="form-label">Re-enter new password:</label>
                        <input type="password" class="form-control" id="newPassword2" placeholder="Re-enter new password" name="newPassword2" required="true"/>
                        <div class="valid-feedback">Valid.</div>
                        <div class="invalid-feedback">Please fill out this field.</div>
                    </div>
                    <%= (Function() As XElement
                             If Session.Role = Session.Roles.SPARE_PARTS_DEALER Or Session.Role = Session.Roles.ADMIN Then
                                 Return <div>
                                            <p>Spare part dealer location: <small>(LEAVE UNALTERED IF NO CHANGE)</small></p>
                                            <div id="location">
                                                <div class="row" style="display: none">
                                                    <div class="col">
                                                        <label for="latitude">Latitude</label>
                                                        <input class="form-control" type="text" name="latitude" id="latitude" required="required"/>
                                                    </div>
                                                    <div class="col">
                                                        <label for="longitude">Longitude:</label>
                                                        <input class="form-control" type="text" name="longitude" id="longitude" required="required"/>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                             Else Return Nothing
                             End If
                         End Function)() %>
                    <input type="hidden" name="actual" value=<%= SecurityHelper.Secure.Encrypt(user.Item2) %>/>
                    <input type="hidden" id="username0" name="username" value=<%= Session.User %>/>

                    <button type="submit" class="btn btn-primary" style="margin-top: .5em;">Submit</button>
                </form>

                <fieldset class="border border-4 rounded-3 p-3">
                    <legend class="float-none w-auto px-1" style="font-size: 12pt;">Danger Zone</legend>
                    <h5 class="fw-light">Delete Account</h5><br/>
                    <form asp-controller="Accounts" asp-action="Delete" method="post" onsubmit=<%= String.Format("return deleteAccount('{0}')", Session.User) %> id="deleteAcc">
                        <input type="hidden" id="username1" name="username" value=<%= Session.User %>/>
                        <input type="submit" class="btn btn-danger" value="Delete account"/>
                    </form>
                </fieldset>
            </div>
            <script>
                $(document).ready(function () {
                    mapping();
                    $("input#latitude").val("<%= latitude %>");
                    $("input#longitude").val("<%= longitude %>");
                    init_session("<%= Session.User %>", "<%= CInt(Session.Role) %>");
                });
            </script>
        </vbxml>

    End Function

End Class
