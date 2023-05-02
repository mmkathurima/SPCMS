﻿Imports Microsoft.AspNetCore.Mvc.ViewFeatures
Imports Vazor

Imports SPCMS.Models

Public Class OrdersView : Inherits VazorView

    Public ReadOnly Property Spares As List(Of SpareParts)

    Public ReadOnly Property ViewData() As ViewDataDictionary

    Public Sub New(spares As List(Of SpareParts), viewData As ViewDataDictionary)
        MyBase.New("Orders", "Views\Home", "Orders")
        Me.Spares = spares
        Me.ViewData = viewData
        viewData!Title = Title
    End Sub

    Public Shared Function CreateNew(spares As List(Of SpareParts), viewData As ViewDataDictionary) As String
        Return VazorViewMapper.Add(New OrdersView(spares, viewData))
    End Function
End Class
