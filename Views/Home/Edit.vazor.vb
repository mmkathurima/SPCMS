Imports Microsoft.AspNetCore.Mvc.ViewFeatures
Imports Vazor

Imports SPCMS.Models

Public Class EditView : Inherits VazorView

    Public ReadOnly Property Spares As List(Of SpareParts)
    Public Spare As SpareParts

    Public ReadOnly Property ViewData() As ViewDataDictionary

    Public Sub New(spares As List(Of SpareParts), viewData As ViewDataDictionary)
        MyBase.New("Edit", "Views\Home", "Edit")
        Me.Spares = spares
        Me.ViewData = viewData
        viewData!Title = Title
    End Sub

    Public Sub New(spare As SpareParts, viewData As ViewDataDictionary)
        MyBase.New("Edit", "Views\Home", "Edit")
        Me.Spare = spare
        Me.ViewData = viewData
        viewData!Title = Title
    End Sub

    Public Shared Function CreateNew(spares As List(Of SpareParts), viewData As ViewDataDictionary) As String
        Return VazorViewMapper.Add(New EditView(spares, viewData))
    End Function

    Public Shared Function CreateNew(spare As SpareParts, viewData As ViewDataDictionary) As String
        Return VazorViewMapper.Add(New EditView(spare, viewData))
    End Function
End Class
