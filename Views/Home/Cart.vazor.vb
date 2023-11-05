Imports Microsoft.AspNetCore.Mvc.ViewFeatures
Imports Vazor

Imports SPCMS.Models

Public Class CartView : Inherits VazorView

    Public ReadOnly Property Spares As List(Of SpareParts)

    Public ReadOnly Property ViewData() As ViewDataDictionary

    Public Sub New(spares As List(Of SpareParts), viewData As ViewDataDictionary)
        MyBase.New("Cart", "Views\Home", "Cart")
        Me.Spares = spares
        Me.ViewData = viewData
        viewData!Title = Title
    End Sub

    Public Shared Function CreateNew(spares As List(Of SpareParts), viewData As ViewDataDictionary) As String
        Return VazorViewMapper.Add(New CartView(spares, viewData))
    End Function
End Class
