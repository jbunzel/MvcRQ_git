Imports Microsoft.VisualBasic
Imports System
Imports System.Data
Imports System.IO
Imports System.Text.RegularExpressions
Imports System.Xml
Imports System.Web
Imports Lucene.Net.Analysis.Standard
Imports Lucene.Net.Documents
Imports Lucene.Net.QueryParsers
Imports Lucene.Net.Search
'Imports Lucene.Net.Search.Highlight

Imports RQLib.RQQueryForm


Namespace RQLucene

    Public Class Searcher

#Region "Private Members"

        Protected xNode As XmlNode
        Protected indexDirectory As String
        Protected Results As DataTable
        Private startAt As Integer
        Private fromItem As Integer
        Private toItem As Integer
        Private total As Integer
        Private duration As TimeSpan = System.TimeSpan.Zero
        Private ReadOnly maxResults As Integer = 50000
        Private bExternalResultTable As Boolean = False
        Private bResultFieldsFromIndex As Boolean = False

#End Region


#Region "Public Properties"

        Public ReadOnly Property ResultTable() As DataTable
            Get
                Return Results
            End Get
        End Property


        Public ReadOnly Property SearchTime() As TimeSpan
            Get
                Return duration
            End Get
        End Property

#End Region


#Region "Private Methods"

        Private Function smallerOf(ByVal first As Integer, ByVal second As Integer) As Integer
            If first < second Then
                Return first
            Else
                Return second
            End If
        End Function


        Private Function pageCount() As Integer
            '<summary>
            'How many pages are there in the results.
            '</summary>

            Return (total - 1) / maxResults 'floor
        End Function


        Private Function lastPageStartsAt() As Integer
            '<summary>
            'First item of the last page
            '</summary>

            Return pageCount() * maxResults
        End Function


        Private Function initStartAt(ByVal requestStartValue As Integer) As Integer
            '<summary>
            'Initializes startAt value. Checks for bad values.
            '</summary>
            '<returns></returns>

            Try
                Dim sa As Integer = Convert.ToInt32(requestStartValue)

                'too small starting item, return first page
                If sa < 0 Then
                    Return 0
                End If

                'too big starting item, return last page
                If sa >= total - 1 Then
                    Return lastPageStartsAt()
                End If
                Return sa
            Catch
                Return 0
            End Try
        End Function

#End Region


#Region "Public Constructors"

        Public Sub New(ByVal Node As XmlNode, Optional ByVal ExternalResultTable As Boolean = False)
            Me.xNode = Node
            Me.indexDirectory = xNode.Attributes("indexFolderUrl").Value
            Me.bResultFieldsFromIndex = False 'Generating result fields from index with internal result table not yet implemented
            Me.bExternalResultTable = ExternalResultTable
            If Not Me.bExternalResultTable Then
                Me.Results = New DataTable
            End If
        End Sub


        Public Sub New(ByVal Node As XmlNode, ByRef Table As DataTable, Optional ByVal ResultFieldsFromIndex As Boolean = True)
            Me.New(Node, True)
            Me.bResultFieldsFromIndex = ResultFieldsFromIndex
            Me.Results = Table
        End Sub

#End Region


#Region "Public Methods"

        Public Function search(ByVal query As TermQuery) As Hits
            Dim s As IndexSearcher = New IndexSearcher(HttpContext.Current.Server.MapPath(indexDirectory))

            Return s.Search(query)
        End Function


        Public Sub search(ByVal queryStr As String, ByRef queryFields() As String, ByVal from As Integer)
            'Try
            Dim start As DateTime = DateTime.Now
            Dim searcher As IndexSearcher = New IndexSearcher(HttpContext.Current.Server.MapPath(indexDirectory))
            Dim hits As Hits
            Dim query As Lucene.Net.Search.Query

            query = New MultiFieldQueryParser(queryFields, New StandardAnalyzer(New String() {})).Parse(queryStr)
            hits = searcher.Search(query)
            Me.total = hits.Length()
            Me.startAt = initStartAt(from)

            'create the result DataTable
            Dim fields As XmlNodeList = Me.xNode.SelectNodes("results/field")

            If Not bExternalResultTable Then

                If Me.bResultFieldsFromIndex Then
                    'Generating result fields from index with internal result table not yet implemented
                Else
                    Dim j As Integer = 0

                    For j = 0 To fields.Count - 1
                        Me.Results.Columns.Add(fields(j).Attributes("name").Value, Type.GetType("System.String"))
                    Next
                End If
            End If

            'create highlighter
            'Dim highlighter As QueryHighlightExtractor = New QueryHighlightExtractor(query, New StandardAnalyzer, "<B>", "</B>")

            'how many items we should show - less than defined at the end of the results
            Dim resultsCount As Integer = smallerOf(total, Me.maxResults + Me.startAt)

            Dim i As Integer
            For i = startAt To resultsCount - 1
                'get the document from index
                Dim doc As Document = hits.Doc(i)
                'this is the place where the documents are stored on the server

                'get the document filename
                'we can't get the text from the index because we didn't store it there
                'Dim path As String = doc.Get("path")
                'Dim location As String = Server.MapPath("1.4\" + path)
                'instead, load it from the original location
                'Dim plainText As String
                'Dim sr As StreamReader = New StreamReader(location, System.Text.Encoding.Default)
                'plainText = parseHtml(sr.ReadToEnd())

                'create a new row with the result data
                Dim row As DataRow = Me.Results.NewRow()

                If Not Me.bResultFieldsFromIndex Then
                    Dim j As Integer = 0

                    For j = 0 To fields.Count - 1
                        row(fields(j).Attributes("name").Value) = fields(j).Attributes("prefix").Value + doc.Get(fields(j).Attributes("valueField").Value)
                    Next
                Else
                    Dim k As Integer = 0

                    For k = 0 To doc.GetFields.Count - 1
                        row(doc.GetFields().Item(k).Name) = doc.GetFields().Item(k).StringValue
                    Next
                End If
                Me.Results.Rows.Add(row)
            Next
            searcher.Close()

            'result information
            Me.duration = DateTime.Now - start
            Me.fromItem = startAt + 1
            Me.toItem = smallerOf(startAt + maxResults, total)
            'Catch ex As System.IO.FileNotFoundException
            'Throw New Modules.RQError.RQSessionTimeoutException()
            'End Try
        End Sub


        Public Sub search(ByRef RQQuery As RQLib.RQQueryForm.RQquery, ByVal from As Integer)
            Dim qryFields() As String = New String(0) {}
            Dim j As Integer = 0
            Dim i As Integer = 0

            For j = 0 To RQQuery.QueryFieldList.Rows.Count - 1
                If RQQuery.QueryFieldList.Rows.Item(j).Item("searchfield") = True Then
                    qryFields(i) = RQQuery.QueryFieldList.Rows(j).Item("fieldname")
                    i = i + 1
                    Array.Resize(qryFields, i + 1)
                End If
            Next
            If i > 0 Then
                Array.Resize(qryFields, i)
                Me.search(RQQuery.GetQueryCommand(RQLib.RQQueryForm.RQquery.QuerySyntax.Lucene), qryFields, from)
            Else
                Me.search(RQQuery.QueryString, from)
            End If
        End Sub


        Public Sub search(ByVal queryString As String, ByVal from As Integer)
            Dim queries As XmlNodeList = Me.xNode.SelectNodes("queries/query")
            Dim qryFields() As String = New String(queries.Count - 1) {}
            Dim j As Integer = 0

            For j = 0 To queries.Count - 1
                qryFields(j) = CType(queries(j).Attributes("field").Value, System.String)
            Next
            Me.search(queryString, qryFields, from)
        End Sub

#End Region

    End Class

End Namespace
