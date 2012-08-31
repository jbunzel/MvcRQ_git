Imports Lucene.Net.Documents.DateField

Namespace RQLucene

    Public Class Utilities

        Public Shared Function StringToDate(ByVal dateTimeCode As String) As String
            Try
                Return Lucene.Net.Documents.DateField.StringToDate(dateTimeCode)
            Catch ex As FormatException
                'dateTimeCode is already in date format
                Return dateTimeCode
            Catch ex As Exception
                Throw
            End Try
        End Function

    End Class

End Namespace