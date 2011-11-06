Imports Microsoft.VisualBasic
Imports System
Imports System.Data
Imports System.Data.OleDb
Imports System.Xml
Imports Lucene.Net.Index
Imports Lucene.Net.Documents
Imports Lucene.Net.Analysis
Imports Lucene.Net.Analysis.Standard
Imports Lucene.Net.Search
Imports Lucene.Net.QueryParsers


Namespace RQLucene

    Public Class OleDBIndexer
        Inherits Indexer

        Public Sub New(ByVal Node As XmlNode)
            MyBase.New(Node)
        End Sub


        Private Function GetData() As DataTable
            'Get Data using SQL
            Dim selectCommandText As String = xNode.SelectSingleNode("selectCommandText").InnerText
            Dim connectionString As String = xNode.SelectSingleNode("selectCommandText/@connectionString").Value
            Dim da As OleDbDataAdapter = New OleDbDataAdapter(selectCommandText, New OleDbConnection(connectionString))
            Dim dt As DataTable = New DataTable

            da.Fill(dt)
            Return dt
        End Function


        Protected Overrides Sub IndexRecords(ByVal writer As Lucene.Net.Index.IndexWriter)
            Dim dt As DataTable = GetData()
            Dim fields As XmlNodeList
            Dim i As Integer

            'Index all records
            fields = Me.xNode.SelectNodes("fields/field")
            For i = 0 To dt.Rows.Count - 1
                Dim doc As Document = New Document

                MyBase.Add(dt.Rows(i), writer)
            Next
        End Sub

    End Class

End Namespace