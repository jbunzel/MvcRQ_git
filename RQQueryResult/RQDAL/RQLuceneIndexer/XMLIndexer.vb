Imports Microsoft.VisualBasic
Imports System.Xml
Imports System.IO
Imports System.Web
Imports Lucene.Net.Index
Imports Lucene.Net.Documents
Imports Lucene.Net.Analysis
Imports Lucene.Net.Analysis.Standard
Imports Lucene.Net.Search
Imports Lucene.Net.QueryParsers

Namespace RQLucene

    Public Class XMLIndexer
        Inherits Indexer


        Private Sub TraverseFolder(ByRef strPath As String, ByVal writer As Lucene.Net.Index.IndexWriter, Optional ByRef iItemCount As Integer = 0)
            Dim objFolder As New DirectoryInfo(HttpContext.Current.Server.MapPath(strPath))
            Dim objFolderItem As DirectoryInfo
            Dim objFileItem As FileInfo
            Dim fields As XmlNodeList

            fields = Me.xNode.SelectNodes("fields/field")
            For Each objFileItem In objFolder.GetFiles()
                Dim strand As StrandDoc = StrandDocLoader.Load(objFileItem)

                If strand.HasContent Then
                    If strand.HasSubsect Then
                        Dim iter As New StrandDocEnum(strand)

                        While (iter.MoveNext)
                            Dim doc As Document = New Document
                            Dim j As Integer = 0
                            Dim values() As String = {CType(iter.Current, StrandSection).DocNo, _
                                                      CType(iter.Current, StrandSection).Title, _
                                                      CType(iter.Current, StrandSection).Content, _
                                                      strand.Uri, _
                                                      strand.Title}

                            For j = 0 To fields.Count - 1
                                Dim name As String = fields(j).Attributes("name").Value

                                doc.Add(New Field(name, values(j), IIf(fields(j).Attributes("isStored").Value = "true", Field.Store.YES, Field.Store.NO), IIf(fields(j).Attributes("isTokenised").Value = "true", Field.Index.TOKENIZED, Field.Index.UN_TOKENIZED)))
                                Console.WriteLine(values(j))
                            Next
                            writer.AddDocument(doc)
                        End While
                    End If
                End If
            Next
            For Each objFolderItem In objFolder.GetDirectories()
                TraverseFolder(strPath + "\" + objFolderItem.Name + "\", writer, iItemCount)
            Next
        End Sub


        Public Sub New(ByVal Node As XmlNode)
            MyBase.New(Node)
        End Sub


        Protected Overrides Sub IndexRecords(ByVal writer As Lucene.Net.Index.IndexWriter)
            TraverseFolder(Me.xNode.SelectSingleNode("xmlSourceDirectory/@path").Value, writer)
        End Sub

    End Class

End Namespace
