Namespace Models
    Public Structure Tables
        Public Enum TableContext
            SPAREPARTS
            CART
            ORDERS
            WISHLIST
            SECURITY
            SEARCH
        End Enum
        Public Shared Property Table As TableContext
    End Structure
End Namespace