Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Web
Imports Lucene.Net.Analysis.Standard
Imports Lucene.Net.Documents
Imports Lucene.Net.QueryParsers
Imports Lucene.Net.Search
Imports Lucene.Net.Index

Imports RQLib.RQQueryForm
Imports RQLib.RQDAL.RQLuceneIndexer


Namespace RQDAL

    Public Class RQLuceneDBI
        '<summary>
        'Datenbankschnittstelle zur Implemetierung der RQLucene-Suchmaschine
        '</summary>


#Region "Private Members"

        Private IndexConfigNodes As System.Xml.XmlNodeList

#End Region


#Region "Private Methods"

        'Private Sub IndexFiles()
        '    Dim i As Integer = 0
        '    Dim indexType As String = IndexConfigNodes(i).SelectSingleNode("@indexType").Value
        '    Dim indexer As RQLucene.Indexer

        '    If (indexType = "XMLIndexer") Then
        '        indexer = New RQLucene.XMLIndexer(IndexConfigNodes(i))
        '    Else
        '        indexer = New RQLucene.OleDBIndexer(IndexConfigNodes(i))
        '        indexer.Generate()
        '    End If
        'End Sub


        Private Sub IndexFiles(Optional ByVal Optimize As Boolean = False, Optional ByVal CreateIndex As Boolean = False)
            Try
                Dim indexType As String = IndexConfigNodes(0).SelectSingleNode("@indexType").Value
                Dim indexer As New OleDBIndexer(IndexConfigNodes(0))

                If (indexType = "OLEDBIndexer") Then
                    indexer.Open(CreateIndex)
                    indexer.Reindex()
                    indexer.Close(Optimize)
                End If
            Catch ex As System.Data.OleDb.OleDbException
                Throw New Exception("Invalid path to OLE database!", ex)
            Catch ex As Lucene.Net.Store.LockObtainFailedException
                Throw New Exception("Lucene database locked!", ex)
            Catch ex As Exception
                Throw New Exception(ex.Message, ex)
            End Try
        End Sub


        Private Sub OptimizeSegments()
            Try
                Dim indexType As String = IndexConfigNodes(0).SelectSingleNode("@indexType").Value
                Dim indexer As New OleDBIndexer(IndexConfigNodes(0))

                If (indexType = "OLEDBIndexer") Then
                    indexer.Open()
                    indexer.Close(True)
                End If
            Catch ex As System.Data.OleDb.OleDbException
                Throw New Exception("Invalid path to OLE database!", ex)
            Catch ex As Lucene.Net.Store.LockObtainFailedException
                Throw New Exception("Lucene database locked!", ex)
            Catch ex As Exception
                Throw New Exception(ex.Message, ex)
            End Try
        End Sub



        Private Sub UpdateFiles(ByRef dataSet As DataSet, ByVal srcTable As String, ByVal changeType As UpdateIndexer.ChangeType)
            Dim item As DataRow
            Dim updater As New UpdateIndexer(IndexConfigNodes(0))
            'Dim searcher As New RQLucene.Searcher(IndexConfigNodes(0), dataSet.Tables(srcTable))

            updater.Open()
            Select Case srcTable
                Case "Dokumente"
                    For Each item In dataSet.Tables(srcTable).Rows
                        updater.UpdateIndex(CType(item, RQDataSet.DokumenteRow), UpdateIndexer.ChangeType.Change, UpdateIndexer.DocType.Dokumente)
                    Next
                Case Else
            End Select
            updater.Close(True)
        End Sub

#End Region


#Region "Public Constructors"

        Public Sub New()
            Dim doc As System.Xml.XmlDocument = New System.Xml.XmlDocument()
            Dim xpath As String = "[@name='ProjectA']"

            doc.Load(IO.Path.Combine(HttpRuntime.AppDomainAppPath, "xml/indexConfig.xml"))
            IndexConfigNodes = doc.SelectNodes("/indexConfiguration/*" + xpath)
        End Sub

#End Region


#Region "Public Methods"

        Public Sub Fill(ByVal RQQuery As RQLib.RQQueryForm.RQquery, ByRef DataSet As RQDataSet, ByVal name As String)
            Dim searcher As New RQLucene.Searcher(IndexConfigNodes(0), DataSet.Tables(name))

            searcher.search(RQQuery, 0)
        End Sub


        Public Sub Update(ByRef dataSet As DataSet, ByVal srcTable As String)
            Me.UpdateFiles(dataSet, srcTable, UpdateIndexer.ChangeType.Change)
        End Sub


        Public Sub Add(ByRef dataSet As DataSet, ByVal srcTable As String)
            Me.UpdateFiles(dataSet, srcTable, UpdateIndexer.ChangeType.Add)
        End Sub


        Public Sub Reindex()
            IndexFiles(True, True)
        End Sub


        Public Sub Optimize()
            OptimizeSegments()
        End Sub

#End Region

    End Class

End Namespace
