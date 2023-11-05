Imports Microsoft.AspNetCore.Mvc.ViewFeatures
Imports Vazor

Imports SPCMS.Models

Public Class AccountSettingsView : Inherits VazorView

    Public ReadOnly Property Spares As List(Of SpareParts)
    Public ReadOnly Property ViewData() As ViewDataDictionary

    Public Sub New(spares As List(Of SpareParts), viewData As ViewDataDictionary)
        MyBase.New("AccountSettings", "Views\Accounts", "Account Settings")
        Me.Spares = spares
        Me.ViewData = viewData
        viewData!Title = Title
    End Sub

    Public Sub New(viewData As ViewDataDictionary)
        MyBase.New("AccountSettings", "Views\Accounts", "Account Settings")
        Me.ViewData = viewData
        viewData("Title") = Title
    End Sub

    Public Shared Function CreateNew(spares As List(Of SpareParts), viewData As ViewDataDictionary) As String
        Return VazorViewMapper.Add(New AccountSettingsView(spares, viewData))
    End Function

    Public Shared Function CreateNew(viewData As ViewDataDictionary) As String
        Return VazorViewMapper.Add(New AccountSettingsView(viewData))
    End Function
End Class
