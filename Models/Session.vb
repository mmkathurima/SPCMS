Namespace Models
    Public Structure Session
        Public Shared Property User As String
        Public Enum Roles
            SPARE_PARTS_DEALER
            CONSUMER
            ADMIN
        End Enum
        Public Shared Property Role As Roles
    End Structure
End Namespace