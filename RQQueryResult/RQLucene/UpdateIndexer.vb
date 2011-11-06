Imports Microsoft.VisualBasic
'Imports System
Imports System.Data
'Imports System.Data.OleDb
Imports System.Xml
'Imports Lucene.Net.Index
'Imports Lucene.Net.Documents
'Imports Lucene.Net.Analysis
'Imports Lucene.Net.Analysis.Standard
'Imports Lucene.Net.Search
'Imports Lucene.Net.QueryParsers


Namespace RQLucene

    Public Class UpdateIndexer
        Inherits Indexer


#Region "Public Members"

        Public Enum DocType
            Dokumente
            Systematik
            Bookmarks
            undefined
        End Enum


        Public Enum ChangeType
            Change
            Delete
            Add
            undefined
        End Enum

#End Region


#Region "Private Members"

        Private _dt As DocType = DocType.undefined
        Private _ct As ChangeType = ChangeType.undefined
        Private _dr As DataRow = Nothing

#End Region


#Region "Public Properties"

        Public Property DocTypeOfUpdate() As DocType
            Get
                Return _dt
            End Get
            Set(ByVal value As DocType)
                _dt = value
            End Set
        End Property


        Public Property ChangeTypeOfUpdate() As DocType
            Get
                Return _dt
            End Get
            Set(ByVal value As DocType)
                _ct = value
            End Set
        End Property

#End Region


#Region "Public Constructors"

        Sub New(ByVal Node As XmlNode)
            MyBase.New(Node)
        End Sub

#End Region


#Region "Protected Methods"

        Protected Overrides Sub IndexRecords(ByVal writer As Lucene.Net.Index.IndexWriter)
            Select Case Me._dt
                Case DocType.Dokumente
                    Select Case Me._ct
                        Case ChangeType.Add
                            MyBase.Add(CType(Me._dr, RQDataSet.DokumenteRow), writer)
                        Case ChangeType.Change
                            MyBase.Update(CType(Me._dr, RQDataSet.DokumenteRow), writer)
                        Case ChangeType.Delete
                        Case ChangeType.undefined
                    End Select
                Case DocType.Systematik
                Case DocType.Bookmarks
                Case DocType.undefined
            End Select
        End Sub

#End Region


#Region "Public Methods"

        Public Function UpdateIndex(ByVal item As RQDataSet.DokumenteRow, Optional ByVal UpdateChangeType As ChangeType = ChangeType.Change, Optional ByVal UpdateDocType As DocType = DocType.Dokumente) As Boolean
            Dim retVal As Boolean = False

            ChangeTypeOfUpdate = UpdateChangeType
            DocTypeOfUpdate = UpdateDocType
            Me._dr = item
            MyBase.Generate(False)

            Return retVal
        End Function

#End Region


    End Class

End Namespace
