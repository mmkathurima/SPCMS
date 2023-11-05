Imports System.Data
Imports System.Runtime.CompilerServices
Imports MySql.Data.MySqlClient

Imports SPCMS.Models
Imports SPCMS.Models.Tables
Imports Microsoft.EntityFrameworkCore

Friend Module Reader
    <Extension()>
    Public Function ContainsField(reader As IDataReader, column As String) As Boolean
        ContainsField = Enumerable.Range(0, reader.FieldCount).Any(Function(a As Integer) reader.GetName(a).Equals(column, StringComparison.InvariantCultureIgnoreCase))
    End Function
End Module

Public Class SparePartsRepository : Inherits DbContext
    Public SpareList As New List(Of SpareParts)

    Private connection As MySqlConnection = GetConnection, command As MySqlCommand

    Private Shared ReadOnly Property GetConnection As MySqlConnection
        Get
            Return New MySqlConnection(SparePartsContext.ConnectionString)
        End Get
    End Property

    Public Sub GetAllListings(Optional query As String = Nothing)
        If connection.State = ConnectionState.Closed Then
            connection.Open()
        End If
        Select Case Table
            Case TableContext.SPAREPARTS
                command = New MySqlCommand("SELECT spareparts.*, security.latitude, security.longitude FROM spareparts INNER JOIN security ON spareparts.username = security.username;", connection:=connection)
            Case TableContext.CART
                command = New MySqlCommand("SELECT spareparts.name, spareparts.category, spareparts.brand, spareparts.id, spareparts.username, spareparts.quantity, spareparts.quantityRequired, spareparts.price, spareparts.yearOfManufacture, spareparts.make, spareparts.model, spareparts.description, spareparts.colour, spareparts.material, spareparts.dimensions, spareparts.uploads, cart.sparepartsid, cart.quantity AS orderQuantity, security.latitude, security.longitude FROM spareparts INNER JOIN cart ON spareparts.id = cart.sparepartsid INNER JOIN security ON spareparts.username = security.username WHERE cart.username = @username;", connection:=connection)
                command.Parameters.AddWithValue("@username", Session.User)
            Case TableContext.ORDERS
                If Not String.IsNullOrWhiteSpace(Session.User) Then
                    Select Case Session.Role
                        Case Session.Roles.SPARE_PARTS_DEALER
                            command = New MySqlCommand("SELECT spareparts.name, spareparts.category, spareparts.brand, spareparts.id, spareparts.username, spareparts.quantity, spareparts.quantityRequired, spareparts.price, spareparts.yearOfManufacture, spareparts.make, spareparts.model, spareparts.description, spareparts.colour, spareparts.material, spareparts.dimensions, spareparts.uploads, orders.orderid, security.latitude, security.longitude FROM spareparts INNER JOIN orders ON spareparts.id = orders.sparepartsid INNER JOIN security ON spareparts.username = security.username WHERE spareparts.username = @username;", connection:=connection)
                        Case Session.Roles.CONSUMER
                            command = New MySqlCommand("SELECT spareparts.name, spareparts.category, spareparts.brand, spareparts.id, spareparts.username, spareparts.quantity, spareparts.quantityRequired, spareparts.price, spareparts.yearOfManufacture, spareparts.make, spareparts.model, spareparts.description, spareparts.colour, spareparts.material, spareparts.dimensions, spareparts.uploads, orders.orderid, orders.quantity AS orderQuantity, security.latitude, security.longitude FROM spareparts INNER JOIN orders ON spareparts.id = orders.sparepartsid INNER JOIN security ON spareparts.username = security.username WHERE orders.username = @username;", connection:=connection)
                    End Select
                    command.Parameters.AddWithValue("@username", Session.User)
                End If
            Case TableContext.WISHLIST
                command = New MySqlCommand("SELECT spareparts.name, spareparts.category, spareparts.brand, spareparts.id, spareparts.username, spareparts.quantity, spareparts.quantityRequired, spareparts.price, spareparts.yearOfManufacture, spareparts.make, spareparts.model, spareparts.description, spareparts.colour, spareparts.material, spareparts.dimensions, spareparts.uploads, wishlist.sparepartsid, security.latitude, security.longitude FROM spareparts INNER JOIN wishlist ON spareparts.id = wishlist.sparepartsid INNER JOIN security ON wishlist.username = security.username WHERE wishlist.username = @username;", connection:=connection)
                command.Parameters.AddWithValue("@username", Session.User)
            Case TableContext.SEARCH
                If query IsNot Nothing Then
                    command = New MySqlCommand("SELECT * FROM spareparts WHERE CONCAT(spareparts.name, spareparts.category, spareparts.brand, spareparts.id, spareparts.username, spareparts.quantity, spareparts.quantityRequired, spareparts.price, spareparts.yearOfManufacture, spareparts.make, spareparts.model, spareparts.description, spareparts.colour, spareparts.material, spareparts.dimensions, spareparts.uploads) LIKE @query", connection:=connection)
                    command.Parameters.AddWithValue("@query", String.Format("%{0}%", query))
                Else
                    Throw New ArgumentNullException(paramName:=NameOf(query), message:="Search query cannot be empty")
                    Exit Sub
                End If
        End Select
        Try
            With command
                .Prepare()
                Using reader As MySqlDataReader = .ExecuteReader()
                    While reader.Read()
                        SpareList.Add(New SpareParts() With {
                        .Name = CStr(reader!name),
                        .Category = CStr(reader!category),
                        .Brand = CStr(reader!brand),
                        .ID = CStr(reader!id),
                        .OrderIdentificationNumber = If(reader.ContainsField("orderid"), CStr(reader!orderid), Nothing),
                        .Username = CStr(reader!username),
                        .Quantity = If(reader.ContainsField("quantity"), CStr(reader!quantity), Nothing),
                        .OrderQuantity = If(reader.ContainsField("orderQuantity"), CStr(reader!orderQuantity), Nothing),
                        .QuantityRequired = CStr(reader!quantityRequired),
                        .Price = CStr(reader!price),
                        .YearOfManufacture = CStr(reader!yearOfManufacture),
                        .VehicleMake = CStr(reader!make),
                        .VehicleModel = CStr(reader!model),
                        .Uploads = CStr(reader!uploads),
                        .Latitude = If(reader.ContainsField("latitude"), CDbl(reader!latitude), Nothing),
                        .Longitude = If(reader.ContainsField("latitude"), CDbl(reader!longitude), Nothing),
                        .Description = CStr(reader!description),
                        .Colour = CStr(reader!colour),
                        .Material = CStr(reader!material),
                        .Dimensions = CStr(reader!dimensions)
                    })
                    End While
                End Using
            End With
        Catch ex As Exception
            Console.Error.WriteLine(ex.ToString())
        End Try
        command.Dispose()
        connection.Close()
    End Sub

End Class
