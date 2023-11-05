Imports System.Data
Imports Microsoft.EntityFrameworkCore
Imports MySql.Data.MySqlClient

Public Class AccountsRepository : Inherits DbContext
    Private ReadOnly connection As MySqlConnection = GetConnection

    Public Shared ReadOnly Property GetConnection As MySqlConnection
        Get
            Return New MySqlConnection(SparePartsContext.ConnectionString)
        End Get
    End Property

    Public Function GetUsers() As List(Of ValueTuple(Of String, String, Double, Double))
        If connection.State = ConnectionState.Closed Then
            connection.Open()
        End If
        Dim command As New MySqlCommand()
        Dim users As New List(Of ValueTuple(Of String, String, Double, Double))()
        With command
            .Connection = connection
            .CommandText = "SELECT * FROM security"
            .Prepare()
            Using reader As MySqlDataReader = .ExecuteReader()
                While reader.Read()
                    users.Add((reader!username.ToString(), reader!password.ToString(), Val(reader!Latitude), Val(reader!Longitude)))
                End While
            End Using
            command.Dispose()
            connection.Close()
            Return users
        End With
    End Function
End Class
