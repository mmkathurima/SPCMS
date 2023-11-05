Imports System.Data
Imports Microsoft.EntityFrameworkCore
Imports MySql.Data.MySqlClient

Imports SPCMS.Models
Imports SPCMS.Models.Tables

Public Class SparePartsContext : Inherits DbContext
    Public Shared Property ConnectionString As String

    Private connection As MySqlConnection = Me.GetConnection(), command As MySqlCommand, affected
    Private transaction As MySqlTransaction

    Public Sub New() : End Sub

    Public Sub New(ConnectionString As String)
        SparePartsContext.ConnectionString = ConnectionString
    End Sub

    Public Sub New(options As DbContextOptions(Of SparePartsContext))
        MyBase.New(options)
    End Sub

    Public Property SpareParts As DbSet(Of SpareParts)

    Private ReadOnly Property GetConnection() As MySqlConnection
        Get
            Return New MySqlConnection(ConnectionString)
        End Get
    End Property

    Public Function Create(spare As SpareParts) As Boolean
        If connection.State = ConnectionState.Closed Then
            connection.Open()
        End If
        Select Case Table
            Case TableContext.SPAREPARTS
                command = New MySqlCommand("INSERT INTO spareparts (name, category, brand, id, username, quantity, quantityRequired, price, yearOfManufacture, make, model, uploads, description, colour, material, dimensions) values (@name, @category, @brand, @id, @username, @quantity, @quantityRequired, @price, @yearOfManufacture, @make, @model, @uploads, @description, @colour, @material, @dimensions)", connection:=connection)
                With command.Parameters
                    .AddWithValue("@name", spare.Name)
                    .AddWithValue("@category", spare.Category)
                    .AddWithValue("@brand", spare.Brand)
                    .AddWithValue("@id", spare.ID)
                    .AddWithValue("@username", spare.Username)
                    .AddWithValue("@quantity", spare.Quantity)
                    .AddWithValue("@quantityRequired", spare.QuantityRequired)
                    .AddWithValue("@price", spare.Price)
                    .AddWithValue("@yearOfManufacture", spare.YearOfManufacture)
                    .AddWithValue("@make", spare.VehicleMake)
                    .AddWithValue("@model", spare.VehicleModel)
                    .AddWithValue("@uploads", spare.Uploads)
                    .AddWithValue("@description", spare.Description)
                    .AddWithValue("@colour", spare.Colour)
                    .AddWithValue("@material", spare.Material)
                    .AddWithValue("@dimensions", spare.Dimensions)
                End With
            Case TableContext.CART
                command = New MySqlCommand("INSERT INTO cart (sparepartsid, username, quantity) VALUES (@sparepartsid, @username, @quantity);", connection)
                With command.Parameters
                    .AddWithValue("@sparepartsid", spare.ID)
                    .AddWithValue("@username", spare.Username)
                    .AddWithValue("@quantity", spare.Quantity)
                End With
            Case TableContext.WISHLIST
                command = New MySqlCommand("INSERT INTO wishlist (sparepartsid, username) VALUES (@sparepartsid, @username);", connection)
                With command.Parameters
                    .AddWithValue("@sparepartsid", spare.ID)
                    .AddWithValue("@username", spare.Username)
                End With
            Case TableContext.ORDERS
                command = connection.CreateCommand()
                transaction = connection.BeginTransaction()
                With command
                    .Connection = connection
                    .Transaction = transaction
                    .CommandText = "INSERT INTO orders (sparepartsid, orderid, quantity, username) VALUES (@sparepartsid, @orderid, @quantity, @username);"
                    With .Parameters
                        .AddWithValue("@sparepartsid", spare.ID)
                        .AddWithValue("@orderid", spare.OrderIdentificationNumber)
                        .AddWithValue("@quantity", spare.OrderQuantity)
                        .AddWithValue("@username", spare.Username)
                    End With
                    .Prepare()
                    .ExecuteNonQuery()

                    .CommandText = "UPDATE spareparts SET quantity = quantity - @quantity1 WHERE id = @id;"
                    With .Parameters
                        .AddWithValue("@quantity1", spare.OrderQuantity)
                        .AddWithValue("@id", spare.ID)
                    End With
                    .Prepare()
                    affected = .ExecuteNonQuery() > 0
                    transaction.Commit()
                    command.Dispose()
                    connection.Close()
                    Return affected
                End With
        End Select
        command.Prepare()
        affected = command.ExecuteNonQuery() > 0
        command.Dispose()
        connection.Close()
        Return affected
    End Function

    Public Function Edit(spare As SpareParts) As Integer
        If connection.State.Equals(ConnectionState.Closed) Then
            connection.Open()
        End If
        command = New MySqlCommand("UPDATE spareparts SET category = @category, quantity = @quantity, description = @description, price = @price WHERE id = @id;", connection)
        With command.Parameters
            .AddWithValue("@category", spare.Category)
            .AddWithValue("@quantity", spare.Quantity)
            .AddWithValue("@description", spare.Description)
            .AddWithValue("@price", spare.Price)
            .AddWithValue("@id", spare.ID)
        End With
        command.Prepare()
        affected = command.ExecuteNonQuery()
        command.Dispose()
        connection.Close()
        Return affected
    End Function

    Public Function Delete(spare As SpareParts) As Integer
        If connection.State.Equals(ConnectionState.Closed) Then
            connection.Open()
        End If
        Select Case Table
            Case TableContext.CART
                command = New MySqlCommand("DELETE FROM cart WHERE sparepartsid = @id AND username = @username;", connection)
                With command.Parameters
                    .AddWithValue("@id", spare.ID)
                    .AddWithValue("@username", Session.User)
                End With
            Case TableContext.ORDERS
                command = connection.CreateCommand()
                transaction = connection.BeginTransaction()
                With command
                    .Connection = connection
                    .Transaction = transaction
                    .CommandText = "UPDATE spareparts INNER JOIN orders ON spareparts.id = orders.sparepartsid SET spareparts.quantity = spareparts.quantity + orders.quantity WHERE id = @id1 AND orders.username = @username;"
                    With .Parameters
                        .AddWithValue("@id1", spare.ID)
                        .AddWithValue("@username", Session.User)
                    End With
                    .Prepare()
                    affected = .ExecuteNonQuery()
                    If affected <= 0 Then : Return affected
                    End If

                    .CommandText = "DELETE FROM orders WHERE orders.sparepartsid = @id2 AND username = @username1;"
                    With .Parameters
                        .AddWithValue("@id2", spare.ID)
                        .AddWithValue("@username1", Session.User)
                    End With
                    .Prepare()
                    affected = .ExecuteNonQuery()
                    transaction.Commit()
                    command.Dispose()
                    connection.Close()
                    Return affected
                End With
            Case TableContext.SPAREPARTS
                command = New MySqlCommand("DELETE FROM spareparts WHERE id = @id AND username = @username;", connection)
                With command.Parameters
                    .AddWithValue("@id", spare.ID)
                    .AddWithValue("@username", Session.User)
                End With
            Case TableContext.WISHLIST
                command = New MySqlCommand("DELETE FROM wishlist WHERE sparepartsid = @id AND username = @username;", connection)
                With command.Parameters
                    .AddWithValue("@id", spare.ID)
                    .AddWithValue("@username", Session.User)
                End With
        End Select
        With command
            .Prepare()
            affected = .ExecuteNonQuery()
            .Dispose()
        End With
        connection.Close()
        Return affected
    End Function

End Class