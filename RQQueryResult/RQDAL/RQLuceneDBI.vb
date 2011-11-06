Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Web
Imports Lucene.Net.Analysis.Standard
Imports Lucene.Net.Documents
Imports Lucene.Net.QueryParsers
Imports Lucene.Net.Search
Imports Lucene.Net.Index

Imports RQLib.RQQueryForm
Imports RQLib.RQLucene


Namespace RQDAL

    Public Class RQLuceneDBI
        '<summary>
        'Datenbankschnittstelle zur Implemetierung der RQLucene-Suchmaschine
        '</summary>


#Region "Private Members"

        Private IndexConfigNodes As System.Xml.XmlNodeList

#End Region


#Region "Private Methods"

        Private Sub IndexFiles()
            Dim i As Integer = 0
            Dim indexType As String = IndexConfigNodes(i).SelectSingleNode("@indexType").Value
            Dim indexer As RQLucene.Indexer

            If (indexType = "XMLIndexer") Then
                indexer = New RQLucene.XMLIndexer(IndexConfigNodes(i))
            Else
                indexer = New RQLucene.OleDBIndexer(IndexConfigNodes(i))
                indexer.Generate()
            End If
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
            Dim item As DataRow

            Select Case srcTable
                Case "Dokumente"
                    For Each item In dataSet.Tables(srcTable).Rows
                        Dim row As RQDataSet.DokumenteRow = CType(item, RQDataSet.DokumenteRow)
                        Dim searcher As New RQLucene.Searcher(IndexConfigNodes(0), dataSet.Tables(srcTable))
                        Dim t As Term = New Term("ID", row.ID)
                        Dim q As Lucene.Net.Search.Query = New TermQuery(t)
                        Dim d As Hits = searcher.search(q)

                        Select Case d.Length
                            Case 1
                                'Dokument ändern
                                If d.Doc(0).GetField("ID").StringValue() = row.ID Then
                                    Dim updater As New UpdateIndexer(IndexConfigNodes(0))

                                    updater.UpdateIndex(row, UpdateIndexer.ChangeType.Change, UpdateIndexer.DocType.Dokumente)
                                End If
                            Case 0
                                'Dokument hinzufügen
                                Dim updater As New RQLucene.UpdateIndexer(IndexConfigNodes(0))

                                updater.UpdateIndex(row, UpdateIndexer.ChangeType.Add, UpdateIndexer.DocType.Dokumente)
                            Case Else
                                'KONSISTENZFEHLER DATENBANK
                        End Select
                    Next
                Case Else
            End Select
        End Sub

#End Region

    End Class

End Namespace
