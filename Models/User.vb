Namespace Models
    Public Class User
        Private _password As String
        Public Property UserName As String

        Public Property Password As String
            Get
                'TODO: Decryption
                Return _password
            End Get
            Set(value As String)
                'TODO: Encryption
                _password = value
            End Set
        End Property

        Public Property Role As String
        Public Property Latitude As String
        Public Property Longitude As String
    End Class
End Namespace
