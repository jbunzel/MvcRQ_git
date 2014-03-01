Imports Microsoft.VisualBasic
Imports System
Imports System.Data
Imports System.Xml
Imports System.Web
Imports Lucene.Net.Index
Imports Lucene.Net.Analysis
Imports Lucene.Net.Analysis.Standard
Imports Lucene.Net.Documents


Namespace RQLucene

    Public MustInherit Class Indexer

        Protected xNode As XmlNode
        Protected Writer As IndexWriter


        Private Function WriteDocument(row As DataRow) As Document
            Dim doc As Document = New Document
            Dim j As Integer
            Dim fields As XmlNodeList = Me.xNode.SelectNodes("fields/field")

            For j = 0 To fields.Count - 1
                Dim name As String = fields(j).Attributes("name").Value

                If Not IsNothing(fields(j).Attributes("type")) Then
                    'Dim fs As Field.Store = Field.Store.YES

                    If fields(j).Attributes("type").Value = "DateTime" And row(name).ToString() <> "" Then
                        doc.Add(New Field(name, DateField.DateToString(Date.Parse(row(name))), IIf(fields(j).Attributes("isStored").Value = "true", Field.Store.YES, Field.Store.NO), IIf(fields(j).Attributes("isTokenised").Value = "true", Field.Index.TOKENIZED, Field.Index.UN_TOKENIZED)))
                    Else
                        doc.Add(New Field(name, row(name).ToString(), IIf(fields(j).Attributes("isStored").Value = "true", Field.Store.YES, Field.Store.NO), IIf(fields(j).Attributes("isTokenised").Value = "true", Field.Index.TOKENIZED, Field.Index.UN_TOKENIZED)))
                    End If
                Else
                    doc.Add(New Field(name, row(name).ToString(), IIf(fields(j).Attributes("isStored").Value = "true", Field.Store.YES, Field.Store.NO), IIf(fields(j).Attributes("isTokenised").Value = "true", Field.Index.TOKENIZED, Field.Index.UN_TOKENIZED)))
                End If
                Console.WriteLine(row(name).ToString())
            Next
            Return doc
        End Function


        Protected Sub New(ByVal Node As XmlNode)
            Me.xNode = Node
        End Sub


        Public Sub Open(Optional ByVal createIndex As Boolean = False)
            Dim indexFolderUrl As String = xNode.Attributes("indexFolderUrl").Value

            If (createIndex) Then
                Dim IndexDirectory As New System.IO.DirectoryInfo(HttpContext.Current.Server.MapPath(indexFolderUrl))

                For Each file In IndexDirectory.GetFiles()
                    file.Delete()
                Next
            End If
            Writer = New IndexWriter(HttpContext.Current.Server.MapPath(indexFolderUrl), New StandardAnalyzer(New String() {}), createIndex)
        End Sub


        Public Sub Close(Optional ByVal optimize As Boolean = False)
            If optimize Then Writer.Optimize()
            Writer.Close()
        End Sub


        Protected Sub Add(ByRef row As DataRow)
            Writer.AddDocument(Me.WriteDocument(row))
        End Sub


        Protected Sub Update(ByRef row As DataRow)
            Writer.UpdateDocument(New Term("ID", row.Item("ID").ToString()), Me.WriteDocument(row))
        End Sub


        Protected MustOverride Sub IndexRecords()

    End Class

End Namespace
