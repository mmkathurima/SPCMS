Imports Microsoft.AspNetCore.Mvc.ViewFeatures
Imports Vazor

Imports SPCMS.Models

Public Class IndexView : Inherits VazorView

    Public ReadOnly Property ViewData() As ViewDataDictionary
    Public ReadOnly Property Spares As List(Of SpareParts)

    Public Sub New(viewData As ViewDataDictionary, spares As List(Of SpareParts))
        MyBase.New("Index", "Views\Home", "Home")
        Me.Spares = spares
        Me.ViewData = viewData
        viewData!Title = Title
    End Sub

    Public Shared Function CreateNew(viewData As ViewDataDictionary, spares As List(Of SpareParts)) As String
        Return VazorViewMapper.Add(New IndexView(viewData, spares))
    End Function
End Class
