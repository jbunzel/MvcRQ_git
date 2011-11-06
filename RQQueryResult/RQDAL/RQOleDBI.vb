Imports Microsoft.VisualBasic
Imports System.Data
'Imports Modules.RQError

Namespace RQDAL

    Public Class RQOleDBI

#Region "Private Members"

        Protected WithEvents _catOleDataAdapter As System.Data.OleDb.OleDbDataAdapter
        Protected WithEvents _catOleConnection As System.Data.OleDb.OleDbConnection
        Protected WithEvents _catOleSelectCommand As System.Data.OleDb.OleDbCommand
        Protected _catOleCommandBuilder As System.Data.OleDb.OleDbCommandBuilder

#End Region


#Region "Public Constructors"

        Public Sub New()
            Me._catOleDataAdapter = New System.Data.OleDb.OleDbDataAdapter
            Me._catOleConnection = New System.Data.OleDb.OleDbConnection
            Me._catOleSelectCommand = New System.Data.OleDb.OleDbCommand
            Me._catOleCommandBuilder = New System.Data.OleDb.OleDbCommandBuilder(Me._catOleDataAdapter)
            '
            '_catOleDataAdapter
            '
            Me._catOleDataAdapter.SelectCommand = Me._catOleSelectCommand
            Me._catOleDataAdapter.MissingSchemaAction = MissingSchemaAction.Add
            Me._catOleDataAdapter.MissingMappingAction = MissingMappingAction.Passthrough
            '
            '_catOleConnection
            '
            Me._catOleConnection.ConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0; Data Source=" + glbDataSrc + ";User Id=admin;Password="
            '
            '_catSelectCommand
            '
            Me._catOleSelectCommand.CommandText = "SELECT * FROM Dokumente"
            Me._catOleSelectCommand.Connection = Me._catOleConnection
            '
            '_catCommandBuilder
            '
            Me._catOleCommandBuilder.QuotePrefix = "["
            Me._catOleCommandBuilder.QuoteSuffix = "]"
            Me._catOleDataAdapter.InsertCommand = Me._catOleCommandBuilder.GetInsertCommand
            Me._catOleDataAdapter.DeleteCommand = Me._catOleCommandBuilder.GetDeleteCommand
            Me._catOleDataAdapter.UpdateCommand = Me._catOleCommandBuilder.GetUpdateCommand
        End Sub

#End Region


#Region "Public Methods"

        Public Sub Fill(ByVal RQQueryStr As String, ByRef DataSet As RQDataSet, ByVal Name As String)
            Me._catOleSelectCommand.CommandText = RQQueryStr
            Try
                Me._catOleDataAdapter.Fill(DataSet, Name)
                'Throw New ApplicationException("Something gone wrong")
            Catch ex As OleDb.OleDbException
                Dim errStr As String = ex.Message
                Dim errNr As Integer = ex.ErrorCode
                errStr = ex.Message
            Catch ex As ApplicationException
                'Dim rqex As New Modules.RQError.RQAppException(ex.Message, ex, RQErrorLevel.medium)
            End Try
        End Sub


        Public Sub GetFieldTable(ByRef Table As DataTable, ByVal Name As String, Optional ByVal ExcludeList As String = "")
            Try
                Me._catOleConnection.Open()
                If Me._catOleConnection.State = ConnectionState.Open Then
                    Dim dataReader As System.Data.OleDb.OleDbDataReader

                    Me._catOleSelectCommand.CommandText = "SELECT * FROM " + Name
                    dataReader = Me._catOleSelectCommand.ExecuteReader(CommandBehavior.SchemaOnly)

                    Dim i As Integer

                    For i = 0 To dataReader.FieldCount - 1 Step 1
                        Dim strFieldName As String = CType(dataReader.GetName(i), String)
                        Dim strTest0 As String = ExcludeList '"ID,Language,Feld30,Feld31,Feld32,Feld33,Feld34,Feld35,Feld36,Feld37,Feld38"

                        If InStr(strTest0, strFieldName) = 0 Then
                            Dim drNewRow As DataRow = Table.NewRow()

                            drNewRow.Item("fieldname") = strFieldName
                            drNewRow.Item("fieldadress") = Name + ". " + strFieldName '"Dokumente." + strFieldName
                            drNewRow.Item("searchfield") = CType(True, Boolean)
                            Table.Rows.Add(drNewRow)
                        End If
                    Next
                    Me._catOleConnection.Close()
                End If
            Catch
            End Try
        End Sub


        Public Sub Update(ByRef dataSet As DataSet, ByVal srcTable As String)
            Me._catOleSelectCommand.CommandText = "SELECT * FROM " + srcTable
            Me._catOleCommandBuilder.RefreshSchema()
            Me._catOleDataAdapter.UpdateCommand = Me._catOleCommandBuilder.GetUpdateCommand
            Me._catOleDataAdapter.Update(dataSet, srcTable)
        End Sub


        Public Sub Test(ByRef dataSet As DataSet, ByVal srcTable As String)
            Dim cb As OleDb.OleDbCommandBuilder = New OleDb.OleDbCommandBuilder(Me._catOleDataAdapter)

            Me._catOleDataAdapter.UpdateCommand = cb.GetUpdateCommand
            Me._catOleDataAdapter.Update(dataSet, srcTable)
        End Sub

#End Region

    End Class

End Namespace
