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


        Protected Sub New(ByVal Node As XmlNode)
            Me.xNode = Node
        End Sub


        Public Sub Generate(Optional ByVal createIndex As Boolean = True)
            'Create the Index
            Dim indexFolderUrl As String = xNode.Attributes("indexFolderUrl").Value
            Dim writer As IndexWriter = New IndexWriter(HttpContext.Current.Server.MapPath(indexFolderUrl), New StandardAnalyzer(New String() {}), createIndex)

            IndexRecords(writer)
            writer.Optimize()
            writer.Close()
        End Sub


        Protected Sub Add(ByRef row As DataRow, ByRef writer As IndexWriter)
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
            writer.AddDocument(doc)
        End Sub


        Protected Sub Update(ByRef row As DataRow, ByVal writer As IndexWriter)
            Dim doc As Document = New Document
            Dim j As Integer
            Dim fields As XmlNodeList = Me.xNode.SelectNodes("fields/field")

            For j = 0 To fields.Count - 1
                Dim name As String = fields(j).Attributes("name").Value

                If Not IsNothing(fields(j).Attributes("type")) Then
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
            writer.UpdateDocument(New Term("ID", row.Item("ID").ToString()), doc)
        End Sub


        Protected MustOverride Sub IndexRecords(ByVal writer As IndexWriter)

    End Class

End Namespace
