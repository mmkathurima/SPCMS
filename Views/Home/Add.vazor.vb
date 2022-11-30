Imports Microsoft.AspNetCore.Mvc.ViewFeatures
Imports Vazor

Public Class AddView : Inherits VazorView

    Public ReadOnly Property ViewData() As ViewDataDictionary

    Public Sub New(viewData As ViewDataDictionary)
        MyBase.New("AddListing", "Views\Home", "Add Spare Part Listing")
        Me.ViewData = viewData
        viewData!Title = Title
    End Sub

    Public Shared Function CreateNew(viewData As ViewDataDictionary) As String
        Return VazorViewMapper.Add(New AddView(viewData))
    End Function
End Class
