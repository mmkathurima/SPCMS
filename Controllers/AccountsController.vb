Option Explicit Off
Option Infer On

Imports Microsoft.AspNetCore.Http
Imports Microsoft.AspNetCore.Mvc
Imports Microsoft.Extensions.Configuration

Imports SPCMS.Models

Namespace Controllers
    Public Class AccountsController : Inherits Controller
        Private ReadOnly repository As New AccountsRepository()
        Private ReadOnly SpareRepository As New SparePartsRepository()
        Private users As List(Of (String, String, Double, Double))

        <HttpGet(), Route("Accounts/Login")>
        Public Function Login() As IActionResult
            Return MyBase.View()
        End Function

        <HttpPost(), Route("Accounts/Login")>
        Public Function Login(username As String, password As String) As IActionResult
            Dim handler As AccountsHandler = AccountsHandler.handler.Authenticate(New User() With {.UserName = username, .Password = password})
            If handler.Valid Then
                HttpContext.Session.SetString("user", username)
                Session.User = username
                Session.Role = handler.role
                Return RedirectToAction("Index", "Home")
            Else
                TempData!Response = <div Class="alert alert-danger">Invalid username or password.</div>.ToString()
                Return RedirectToAction()
            End If
        End Function

        <HttpGet(), Route("Accounts/Register")>
        Public Function Register() As IActionResult
            Return MyBase.View()
        End Function

        <HttpPost(), Route("Accounts/Register")>
        Public Function Register(username As String, password As String, confirmPassword As String, role As String, latitude As Double, longitude As Double) As IActionResult
            Try
                If password = confirmPassword AndAlso AccountsHandler.handler.Create(New User() With {.UserName = username, .Password = password, .Role = role, .Latitude = latitude, .Longitude = longitude}).Valid Then
                    TempData!Response = <div class="alert alert-success"><b>Success</b> Successful registration</div>.ToString()
                    Return MyBase.RedirectToAction("Login", "Accounts")
                Else TempData!Response = <div class="alert alert-danger">Something went wrong during account creation. Perhaps passwords do not match. Try again later.</div>.ToString()
                End If
            Catch e As MySql.Data.MySqlClient.MySqlException
                If e.Number = 1062 Then
                    TempData!Response = <div class="alert alert-danger">This account already exists.</div>.ToString()
                Else Throw
                End If
            End Try
            Return MyBase.View()
        End Function

        <HttpGet("Accounts/{user}/Settings"), Route("Accounts/AccountSettings")>
        Public Function AccountSettings(user As String) As IActionResult
            users = repository.GetUsers()
            Dim first = users.First(Function(w As ValueTuple(Of String, String, Double, Double)) w.Item1 = Session.User)
            If users IsNot Nothing Then
                ViewData!User = first
                ViewData!username = first.Item1
                Return MyBase.View(AccountSettingsView.CreateNew(ViewData), SpareRepository.SpareList)
            End If
            REM Return RedirectToAction("Login", "Accounts")
            Return MyBase.View(AccountSettingsView.CreateNew(ViewData), SpareRepository.SpareList)
        End Function

        <HttpPost(), Route("Accounts/SavePassword")>
        Public Function SavePassword(oldPassword As String, newPassword As String, newPassword2 As String, latitude As Double, longitude As Double, username As String, actual As String) As IActionResult
            users = repository.GetUsers()
            If Session.Role = Session.Roles.ADMIN Then
                actual = users.Where(Function(w As (String, String, Double, Double)) w.Item1 = username).Select(Function(w As (String, String, Double, Double)) w.Item2).First()
            Else actual = SecurityHelper.Secure.Decrypt(actual)
            End If

            If oldPassword = actual Then
                If newPassword = newPassword2 Then
                    If newPassword <> oldPassword Then
                        If Session.Role = Session.Roles.ADMIN And Session.User <> username Then
                            latitude = users.Where(Function(w As (String, String, Double, Double)) w.Item1 = username).Select(Function(w As (String, String, Double, Double)) w.Item3).First()
                            longitude = users.Where(Function(w As (String, String, Double, Double)) w.Item1 = username).Select(Function(w As (String, String, Double, Double)) w.Item4).First()
                        End If
                        If AccountsHandler.handler.Edit(New User() With {.UserName = username, .Password = newPassword, .Latitude = latitude, .Longitude = longitude}).Valid Then
                            TempData!Response = <div class="alert alert-success">New password successfully saved.</div>.ToString()
                            Console.WriteLine(TempData!Response)
                            Return MyBase.RedirectToAction("Index", "Home")
                        End If
                    Else
                        TempData!Response = <div class="alert alert-danger">New password should not match old password.</div>.ToString()
                    End If
                Else
                    TempData!Response = <div class="alert alert-danger">Passwords do not match.</div>.ToString()
                End If
            Else
                TempData!Response = <div class="alert alert-danger">Incorrect credentials provided.</div>.ToString()
            End If
            Console.WriteLine("RESPONSE: " & TempData!Response)
            Return MyBase.Redirect($"{Session.User}/Settings")
        End Function

        <HttpPost(), Route("Accounts/Delete")>
        Public Function Delete(username As String) As IActionResult
            Console.WriteLine("DELETING USER {0}", username)
            If AccountsHandler.handler.Delete(New User() With {.UserName = username}).Valid Then
                If Session.Role = Session.Roles.ADMIN Then
                    TempData!Response = <div class="alert alert-success">Deletion of account successful</div>.ToString()
                Else
                    HttpContext.Session.Remove("user")
                    Session.User = Nothing
                    TempData!Response = <div class="alert alert-success">Deletion of account successful</div>.ToString()
                    Return MyBase.RedirectToAction("Index", "Home")
                End If
            Else
                TempData!Response = <div class="alert alert-danger">Something went wrong during account deletion. Please try again later.</div>.ToString()
            End If
            Return MyBase.Redirect($"{Session.User}/Settings")
        End Function

        <HttpGet(), Route("Accounts/Logout")>
        Public Function Logout() As IActionResult
            HttpContext.Session.Remove("user")
            Session.User = Nothing
            Return RedirectToAction("Index", "Home")
        End Function


        <HttpGet(), Route("Accounts/{user}/Statistics")>
        Public Function Statistics(user As String) As IActionResult
            Dim dataset

            Select Case Session.Role
                Case Session.Roles.ADMIN
                    dataset = AccountsHandler.handler.Statistics()
                Case Session.Roles.SPARE_PARTS_DEALER
                    dataset = AccountsHandler.handler.Statistics(New User() With {.UserName = user})
                Case Else
                    Return MyBase.Redirect(Request.Headers!Referer.ToString())
            End Select
            If dataset IsNot Nothing Then
                ViewData!dataset = dataset
            End If
            Return MyBase.View(StatisticsView.CreateNew(SpareRepository.SpareList, ViewData))
        End Function
    End Class
End Namespace
