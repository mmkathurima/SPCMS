Imports System.Data
Imports Microsoft.EntityFrameworkCore
Imports MySql.Data.MySqlClient

Imports SPCMS.Models
Imports SPCMS.Models.Session

Public Class AccountsHandler : Inherits DbContext
    Private ReadOnly connection As MySqlConnection = GetConnection
    Private command As MySqlCommand
    Public Shared handler As New AccountsHandler()
    Public Valid As Boolean = False
    Public role As Session.Roles
    Private transaction As MySqlTransaction

    'TODO: Encryption and decryption
    'Public security As SecurityHelper = SecurityHelper.Secure

    Private Sub New() : End Sub

    Private ReadOnly Property GetConnection As MySqlConnection
        Get
            Return New MySqlConnection(SparePartsContext.ConnectionString)
        End Get
    End Property

    Public Function Authenticate(user As User) As AccountsHandler
        If connection.State = ConnectionState.Closed Then
            connection.Open()
        End If
        command = New MySqlCommand("SELECT username, password, role FROM security WHERE username = @username;", connection)
        With command
            .Parameters.AddWithValue("@username", user.UserName)
            .Prepare()
            Using reader As MySqlDataReader = .ExecuteReader
                'TODO: Add decryption here
                If reader.Read() Then
                    handler = New AccountsHandler() With {
                        .Valid = reader!username.ToString() = user.UserName And reader!password.ToString() = user.Password,
                        .role = If(reader!role.ToString() = "Spare Part Dealer", Roles.SPARE_PARTS_DEALER, If(reader!role.ToString() = "Consumer", Roles.CONSUMER, Roles.ADMIN))
                    }
                Else handler = New AccountsHandler() With {.Valid = False, .role = Nothing}
                End If
                connection.Close()
                Return handler
            End Using
        End With
    End Function

    Public Function Create(user As User) As AccountsHandler
        If connection.State = ConnectionState.Closed Then
            connection.Open()
        End If
        command = New MySqlCommand("INSERT INTO security (username, password, role, latitude, longitude) VALUES (@username, @password, @role, @latitude, @longitude);", connection)
        With command.Parameters
            .AddWithValue("username", user.UserName)
            .AddWithValue("password", user.Password)
            .AddWithValue("role", user.Role)
            .AddWithValue("latitude", user.Latitude)
            .AddWithValue("longitude", user.Longitude)
        End With
        command.Prepare()
        handler = New AccountsHandler() With {.Valid = command.ExecuteNonQuery() > 0}
        connection.Close()
        Return handler
    End Function

    Public Function Edit(user As User) As AccountsHandler
        If connection.State = ConnectionState.Closed Then
            connection.Open()
        End If
        command = New MySqlCommand("UPDATE security SET password = @password, latitude = @latitude, longitude = @longitude WHERE username = @username;", connection)
        With command
            With .Parameters
                .AddWithValue("@password", user.Password)
                .AddWithValue("@latitude", user.Latitude)
                .AddWithValue("@longitude", user.Longitude)
                .AddWithValue("@username", user.UserName)
            End With
            .Prepare()
        End With
        handler = New AccountsHandler() With {.Valid = command.ExecuteNonQuery() > 0}
        command.Dispose()
        connection.Close()
        Return handler
    End Function

    Public Function Delete(user As User) As AccountsHandler
        If connection.State = ConnectionState.Closed Then
            connection.Open()
        End If
        command = connection.CreateCommand()
        transaction = connection.BeginTransaction()
        With command
            .Connection = connection
            .Transaction = transaction
            .CommandText = "DELETE FROM security WHERE username = @username;"
            .Parameters.AddWithValue("@username", user.UserName)
            .Prepare()
            Dim affectedFirst As Integer = .ExecuteNonQuery()

            .CommandText = "DELETE FROM spareparts WHERE username = @username0;"
            .Parameters.AddWithValue("@username0", user.UserName)
            .Prepare()
            Dim affectedSecond As Integer = .ExecuteNonQuery()

            transaction.Commit()
            handler = New AccountsHandler() With {.Valid = affectedFirst > 0 Or affectedSecond > 0}
            Console.WriteLine("DELETION RETURNS {0} AND {1}", affectedFirst, affectedSecond)
            .Dispose()
            connection.Close()
            Return handler
        End With
    End Function

    Public Function Statistics(Optional user As User = Nothing) As (Users As List(Of User), Spares As List(Of SpareParts), Orders As List(Of Order))
        Dim users As New List(Of User), spares As New List(Of SpareParts), orders As New List(Of Order)
        If connection.State = ConnectionState.Closed Then
            connection.Open()
        End If

        command = connection.CreateCommand()
        transaction = connection.BeginTransaction()
        With command
            .Connection = connection
            .Transaction = transaction
            .CommandText = "SELECT * FROM security;"
            .Prepare()
            Using read As MySqlDataReader = command.ExecuteReader()
                While read.Read()
                    users.Add(New User() With {.UserName = read!username})
                End While
            End Using

            If user IsNot Nothing Then
                .CommandText = "SELECT * FROM spareparts WHERE username = @username0;"
                .Parameters.AddWithValue("@username0", user.UserName)
            Else .CommandText = "SELECT * FROM spareparts;"
            End If
            .Prepare()
            Using read As MySqlDataReader = command.ExecuteReader()
                While read.Read()
                    spares.Add(New SpareParts() With {
                                  .Name = read!name.ToString(),
                                  .Category = read!category.ToString(),
                                  .Brand = read!brand.ToString(),
                                  .ID = read!id.ToString(),
                                  .Username = read!username.ToString(),
                                  .Quantity = read!quantity.ToString(),
                                  .QuantityRequired = read!quantityRequired.ToString(),
                                  .Price = read!price.ToString(),
                                  .YearOfManufacture = read!yearOfManufacture.ToString(),
                                  .VehicleMake = read!make.ToString(),
                                  .VehicleModel = read!model.ToString(),
                                  .Uploads = read!uploads.ToString(),
                                  .Description = read!description.ToString(),
                                  .Colour = read!colour.ToString(),
                                  .Material = read!material.ToString(),
                                  .Dimensions = read!dimensions.ToString()
                              })
                End While
            End Using

            If user IsNot Nothing Then
                .CommandText = "SELECT spareparts.name, orders.* FROM orders INNER JOIN spareparts ON orders.sparepartsid = spareparts.id WHERE spareparts.username = @username1;"
                .Parameters.AddWithValue("@username1", user.UserName)
            Else .CommandText = "SELECT spareparts.name, orders.* FROM orders INNER JOIN spareparts ON orders.sparepartsid = spareparts.id;"
            End If
            .Prepare()
            Using read As MySqlDataReader = command.ExecuteReader()
                While read.Read()
                    orders.Add(New Order() With {
                                  .Name = read!name.ToString(),
                                  .ID = read!sparepartsid.ToString(),
                                  .OrderIdentificationNumber = read!orderid.ToString(),
                                  .Username = read!username.ToString(),
                                  .Quantity = read!quantity.ToString()
                              })
                End While
            End Using
            transaction.Commit()
            .Dispose()
            connection.Close()

            Return (users, spares, orders)
        End With
    End Function
End Class