Imports Microsoft.VisualBasic
Imports System.Data

Namespace RQQueryForm

    Public Class RQquery

#Region "Private Members"

        Private _id As Guid
        Private _queryString As String = ""
        Private _queryType As QueryTypeEnum = QueryTypeEnum.form
        Private _DocId As String = ""
        Private _QryExt As String = ""
        Private _QryBm As Boolean = False
        Private _QryRpt As Boolean = False
        Private _QryIs As Integer = -1
        Private _QryPsi As Integer = -1
        Private _QryPs As Integer = -1
        Private _QrySort As SortType = SortType.undefined
        Private _queryFieldList As New DataTable
        Private _queryTermList As New ArrayList
        Private _queryLogic As String = String.Empty

#End Region


#Region "Private Methods"

        Private Function GetQueryTypeString(ByVal qryType As QueryTypeEnum) As String
            Select Case qryType
                Case QueryTypeEnum.bookmarks
                    Return "bookmarks"
                Case QueryTypeEnum.access
                    Return "access"
                Case QueryTypeEnum.browse
                    Return "browse"
                Case QueryTypeEnum.cdlist
                    Return "cdlist"
                Case QueryTypeEnum.database
                    Return "database"
                Case QueryTypeEnum.digcollection
                    Return "digcollection"
                Case QueryTypeEnum.digital
                    Return "digital"
                Case QueryTypeEnum.film
                    Return "film"
                Case QueryTypeEnum.form
                    Return "form"
                Case QueryTypeEnum.music
                    Return "music"
                Case QueryTypeEnum.recent
                    Return "recent"
                Case QueryTypeEnum.tag
                    Return "tag"
                Case QueryTypeEnum.tape
                    Return "tape"
                Case QueryTypeEnum.classification
                    Return "class"
                Case QueryTypeEnum.index
                    Return "index"
            End Select
            Return "undefined"
        End Function


        Private Function GetQueryTypeEnum(ByVal qryType As String) As QueryTypeEnum
            Select Case qryType
                Case "bookmarks"
                    Return QueryTypeEnum.bookmarks
                Case "access"
                    Return QueryTypeEnum.access
                Case "browse"
                    Return QueryTypeEnum.browse
                Case "cdlist"
                    Return QueryTypeEnum.cdlist
                Case "database"
                    Return QueryTypeEnum.database
                Case "digcollection"
                    Return QueryTypeEnum.digcollection
                Case "digital"
                    Return QueryTypeEnum.digital
                Case "film"
                    Return QueryTypeEnum.film
                Case "form"
                    Return QueryTypeEnum.form
                Case "music"
                    Return QueryTypeEnum.music
                Case "recent"
                    Return QueryTypeEnum.recent
                Case "tag"
                    Return QueryTypeEnum.tag
                Case "tape"
                    Return QueryTypeEnum.tape
                Case "undefined"
                    Return QueryTypeEnum.undefined
                Case "class"
                    Return QueryTypeEnum.classification
                Case "index"
                    Return QueryTypeEnum.index
            End Select
            Return QueryTypeEnum.undefined
        End Function


        Private Function ParseQueryString(Optional ByVal Syntax As QuerySyntax = QuerySyntax.SQL) As String
            Dim sSQL As New System.Text.StringBuilder
            Dim iFinalClause As Integer = 0
            Dim iEnd As Integer
            Dim sTerm, sValue As String

            If Syntax = QuerySyntax.SQL Then sSQL.Append("SELECT * FROM Dokumente WHERE ")
            Me._queryTermList.Clear()
            Me._queryLogic = " OR "
            sValue = Me.QueryString
            If (Len(sValue) > 0) Then
                Do While (Len(sValue) > 0)
                    iEnd = InStr(1, sValue, " ")
                    If (iEnd > 0) Then
                        sTerm = Mid(sValue, 1, iEnd - 1)
                        sValue = Right(sValue, Len(sValue) - iEnd)
                    Else
                        sTerm = sValue
                        sValue = ""
                    End If
                    If (Mid(sTerm, 1, 1) = "+") Then
                        Me._queryLogic = " AND "
                        sTerm = Right(sTerm, Len(sTerm) - 1)
                    End If
                    Me._queryTermList.Add(Replace(sTerm.ToLower, "*", ""))
                    If Syntax = QuerySyntax.SQL Then
                        If (Len(sValue) > 0) Or (iFinalClause > 0) Then
                            sSQL.Append("(")
                        End If
                        If Me.QueryFieldList.Rows.Count > 0 Then
                            Dim i As Integer
                            Dim j As Integer = 0
                            Dim bAppOr As Boolean = False

                            For i = 0 To Me.QueryFieldList.Rows.Count - 1
                                If (j < 14) And (CType(Me.QueryFieldList.Rows.Item(i).Item("searchfield"), Boolean)) = True Then
                                    If bAppOr Then
                                        sSQL.Append(" OR ")
                                    End If
                                    j = j + 1
                                    bAppOr = True
                                    sSQL.Append("((")
                                    sSQL.Append(Me.QueryFieldList.Rows(i).Item("fieldadress"))
                                    sSQL.Append(" Like '% ")
                                    sSQL.Append(sTerm)
                                    sSQL.Append("%') OR (")
                                    sSQL.Append(Me.QueryFieldList.Rows(i).Item("fieldadress"))
                                    sSQL.Append(" Like '")
                                    sSQL.Append(sTerm)
                                    sSQL.Append("%'))")
                                End If
                            Next
                        End If
                        If (Len(sValue) > 0) Then
                            sSQL.Append(")")
                            sSQL.Append(Me.QueryLogic)
                            iFinalClause = 1
                        End If
                    End If
                Loop
                If Syntax = QuerySyntax.SQL And (iFinalClause > 0) Then
                    sSQL.Append(")")
                End If
            End If
            If Syntax = QuerySyntax.SQL Then sSQL.Append(" ORDER BY PublTime")
            Return sSQL.ToString
        End Function

#End Region


#Region "Public Properties"

        Public Enum QuerySyntax
            undefined
            bookmark
            Lucene
            SQL
            CQL
        End Enum


        Public Enum QueryTypeEnum
            undefined
            bookmarks
            classification
            tag
            film
            music
            cdlist
            tape
            digital
            database
            digcollection
            browse
            access
            recent
            form
            index
        End Enum


        Public Enum SortType As Short
            ByTitle
            BySubject
            ByPublicationDate
            ByCreationDate
            ByShelf
            ByPrimarySubject
            ByShelfClass
            undefined
        End Enum


        Public ReadOnly Property Id() As Guid
            Get
                Return Me._id
            End Get
        End Property


        Public Property QueryFieldList() As DataTable
            Get
                Return _queryFieldList
            End Get
            Set(ByVal value As DataTable)
                _queryFieldList = value
            End Set
        End Property


        Public Property QueryTermList() As ArrayList
            Get
                If (Me._queryType = QueryTypeEnum.form) And (Me._queryTermList.Count = 0) Then Me.ParseQueryString(QuerySyntax.undefined)
                Return _queryTermList
            End Get
            Set(ByVal value As ArrayList)
                _queryTermList = value
            End Set
        End Property


        Public Property QueryLogic() As String
            Get
                Return _queryLogic
            End Get
            Set(ByVal value As String)
                _queryLogic = value
            End Set
        End Property


        Public Property QueryString() As String
            Get
                If (_queryString.Length > 0) Then
                    If _queryString.StartsWith("$") And _queryString.LastIndexOf("$") > 1 Then
                        Return _queryString.Substring(_queryString.LastIndexOf("$") + 1)
                    Else
                        Return _queryString
                    End If
                Else
                    Return _queryString
                End If
            End Get
            Set(ByVal value As String)
                _queryString = value
            End Set
        End Property


        Public Property QueryTypeString() As String
            Get
                If (_queryString.Length > 0) Then
                    If _queryString.StartsWith("$") And _queryString.LastIndexOf("$") > 1 Then
                        Return _queryString.Substring(1, _queryString.LastIndexOf("$") - 1)
                    Else
                        Return Me.GetQueryTypeString(_queryType)
                    End If
                Else
                    Return Me.GetQueryTypeString(_queryType)
                End If
            End Get
            Set(ByVal value As String)
                '_queryType = Me.GetQueryTypeEnum(value)
                If (_queryString.Length > 0) Then
                    If _queryString.StartsWith("$") And _queryString.LastIndexOf("$") > 1 Then
                        Me._queryType = Me.GetQueryTypeEnum(_queryString.Substring(1, _queryString.LastIndexOf("$") - 1))
                    Else
                        _queryType = Me.GetQueryTypeEnum(value)
                    End If
                Else
                    _queryType = Me.GetQueryTypeEnum(value)
                End If
            End Set
        End Property


        Public Property QueryType() As QueryTypeEnum
            Get
                If (_queryString.Length > 0) Then
                    If _queryString.StartsWith("$") And _queryString.LastIndexOf("$") > 1 Then
                        Return Me.GetQueryTypeEnum(_queryString.Substring(1, _queryString.LastIndexOf("$") - 1))
                    Else
                        Return _queryType
                    End If
                Else
                    Return _queryType
                End If
            End Get
            Set(ByVal value As QueryTypeEnum)
                'Me._queryType = value
                If (_queryString.Length > 0) Then
                    If _queryString.StartsWith("$") And _queryString.LastIndexOf("$") > 1 Then
                        Me._queryType = Me.GetQueryTypeEnum(_queryString.Substring(1, _queryString.LastIndexOf("$") - 1))
                    Else
                        Me._queryType = value
                    End If
                Else
                    Me._queryType = value
                End If
            End Set
        End Property


        Public Property DocId() As String
            Get
                Return _DocId
            End Get
            Set(ByVal value As String)
                _DocId = value
            End Set
        End Property


        Public Property QueryExternal() As String
            Get
                Return _QryExt
            End Get
            Set(ByVal value As String)
                _QryExt = value
            End Set
        End Property


        Public Property QueryBookmarks() As Boolean
            Get
                Return _QryBm
            End Get
            Set(ByVal value As Boolean)
                _QryBm = value
            End Set
        End Property


        Public Property QueryRepeat() As Boolean
            Get
                Return _QryRpt
            End Get
            Set(ByVal value As Boolean)
                _QryRpt = value
            End Set
        End Property


        Public Property QueryItemSelect() As Integer
            Get
                Return _QryIs
            End Get
            Set(ByVal value As Integer)
                _QryIs = value
            End Set
        End Property


        Public Property QueryPageStartIndex() As Integer
            Get
                Return _QryPsi
            End Get
            Set(ByVal value As Integer)
                _QryPsi = value
            End Set
        End Property


        Public Property QueryPageSize() As Integer
            Get
                Return _QryPs
            End Get
            Set(ByVal value As Integer)
                _QryPs = value
            End Set
        End Property


        Public Property QuerySort() As SortType
            Get
                Return _QrySort
            End Get
            Set(ByVal value As SortType)
                _QrySort = value
            End Set
        End Property

#End Region


#Region "Public Constructors"

        Public Sub New(ByVal Query As String, _
                       Optional ByVal Type As String = "form", _
                       Optional ByVal QueryFields As DataTable = Nothing, _
                       Optional ByVal Doc As String = "", _
                       Optional ByVal External As String = "", _
                       Optional ByVal Bookmarks As Boolean = False, _
                       Optional ByVal SortOrder As SortType = SortType.BySubject)
            Me._id = Guid.NewGuid()
            Me.QueryString = Query
            Me.QueryTypeString = Type
            Me.QueryFieldList = QueryFields
            Me.DocId = Doc
            Me.QueryExternal = External
            Me.QueryBookmarks = Bookmarks
            Me.QuerySort = SortOrder
        End Sub


        Public Sub New()
            Me.New("")
        End Sub

#End Region


#Region "Public Methods"

        Public Sub SetDefaultQueryFields()
            Dim strTest As String = "Title,Authors,Institutions,Series,Publisher,DocTypeName,IndexTerms,Subjects,AboutLocation,AboutPersons,Abstract,Classification,WorkType,Signature"
            Dim i As Integer

            For i = 0 To _queryFieldList.Rows.Count - 1
                _queryFieldList.Rows.Item(i).Item("searchfield") = strTest.Contains(_queryFieldList.Rows.Item(i).Item("fieldname"))
            Next
        End Sub


        Public Function GetQueryCommand(Optional ByVal Syntax As QuerySyntax = QuerySyntax.SQL) As String

            Select Case Syntax
                Case QuerySyntax.SQL
                    If (Me.QueryTypeString = "tag") Then
                        If String.IsNullOrEmpty(Me.QueryString) Then
                            Throw New ArgumentOutOfRangeException("Query string missing. A query string is required for query type 'tag'.")
                        Else
                            GetQueryCommand = "SELECT * FROM Dokumente WHERE (Dokumente.IndexTerms LIKE '" + Me.QueryString + ";%') OR (Dokumente.IndexTerms LIKE '%; " + Me.QueryString + ";%') OR (Dokumente.Subjects LIKE '" + Me.QueryString + ";%') OR (Dokumente.Subjects LIKE '%; " + Me.QueryString + ";%')"
                        End If
                    ElseIf (Me.QueryTypeString = "film") Then
                        GetQueryCommand = "SELECT * FROM Dokumente WHERE (Dokumente.Signature LIKE 'JB: VHS=%') or (Dokumente.Signature LIKE 'JB: DVD=%') OR (Dokumente.Signature LIKE 'JB: MyVideo=%') ORDER BY Dokumente.Signature ASC"
                    ElseIf (Me.QueryTypeString = "music") Then
                        GetQueryCommand = "SELECT * FROM Dokumente WHERE (Dokumente.Signature LIKE 'JB: CDC=%') OR (Dokumente.Signature LIKE 'JB: TB=%') OR (Dokumente.Signature LIKE 'JB: MyMusic=%') OR (Dokumente.Signature LIKE 'JB: TBC%')ORDER BY Dokumente.Signature ASC"
                    ElseIf (Me.QueryTypeString = "cdlist") Then
                        GetQueryCommand = "SELECT * FROM Dokumente WHERE Dokumente.Signature LIKE 'JB: CDC%' ORDER BY Dokumente.Signature ASC"
                    ElseIf (Me.QueryTypeString = "tape") Then
                        GetQueryCommand = "SELECT * FROM Dokumente WHERE Dokumente.Signature LIKE 'JB: TBC%' ORDER BY Dokumente.Signature ASC"
                    ElseIf (Me.QueryTypeString = "digital") Then
                        GetQueryCommand = "SELECT * FROM Dokumente WHERE Dokumente.Signature LIKE '%My%' ORDER BY Dokumente.Signature ASC"
                    ElseIf (Me.QueryTypeString = "database") Then
                        GetQueryCommand = "SELECT * FROM Dokumente WHERE Dokumente.DocTypeName LIKE '%Datenbank%' ORDER BY Dokumente.Signature ASC"
                    ElseIf (Me.QueryTypeString = "digcollection") Then
                        GetQueryCommand = "SELECT * FROM Dokumente WHERE Dokumente.DocTypeName LIKE '%Digitale Dokumentsammlung%' ORDER BY Dokumente.Signature ASC"
                    ElseIf (Me.QueryTypeString = "browse") Or (Me.QueryTypeString = "class") Then
                        If String.IsNullOrEmpty(Me.QueryString) Then
                            Throw New ArgumentOutOfRangeException("Query string missing. A query string is required for query type '" + Me.QueryTypeString + "'.")
                        Else
                            GetQueryCommand = "SELECT * FROM Dokumente WHERE (Dokumente.Classification LIKE '" + Me.QueryString + ";%') OR (Dokumente.Classification LIKE '%; " + Me.QueryString + ";%')"
                        End If
                    ElseIf (Me.QueryTypeString = "access") Then
                        If Not String.IsNullOrEmpty(Me.QueryString) Then
                            GetQueryCommand = "SELECT * FROM Dokumente WHERE Dokumente.DocNo LIKE '" + Me.QueryString + "%'"
                        Else
                            Throw New Exception("Query string missing. For this request a query string is mandatory.")
                        End If
                    ElseIf (Me.QueryTypeString = "recent") Then
                        GetQueryCommand = "SELECT * FROM Dokumente WHERE Dokumente.Feld31 BETWEEN " + System.DateTime.Now.Date.AddMonths(-1).ToString("#MM-dd-yyyy#") + " AND " + System.DateTime.Now.Date.AddDays(+1).ToString("#MM-dd-yyyy#")
                    Else
                        GetQueryCommand = Me.ParseQueryString()
                    End If
                Case QuerySyntax.Lucene
                    If (Me.QueryTypeString = "tag") Then
                        If String.IsNullOrEmpty(Me.QueryString) Then
                            Throw New ArgumentOutOfRangeException("Query string missing. A query string is required for query type 'tag'.")
                        Else
                            GetQueryCommand = "IndexTerms:" + Me.QueryString + " Subjects:" + Me.QueryString
                        End If
                    ElseIf (Me.QueryTypeString = "film") Then
                        GetQueryCommand = "Signature:VHS* OR Signature:DVD* OR Signature:MyVideo*"
                    ElseIf (Me.QueryTypeString = "music") Then
                        GetQueryCommand = "Signature:CDC* OR Signature:TB* OR Signature:MyMusic* OR Signature:TBC*"
                    ElseIf (Me.QueryTypeString = "cdlist") Then
                        GetQueryCommand = "Signature:CDC*"
                    ElseIf (Me.QueryTypeString = "tape") Then
                        GetQueryCommand = "Signature:TBC*"
                    ElseIf (Me.QueryTypeString = "digital") Then
                        GetQueryCommand = "Signature:My*"
                    ElseIf (Me.QueryTypeString = "database") Then
                        GetQueryCommand = "DocTypeName:Datenbank"
                    ElseIf (Me.QueryTypeString = "digcollection") Then
                        GetQueryCommand = "DocTypeName:'Digitale Dokumentsammlung'"
                    ElseIf (Me.QueryTypeString = "browse") Or (Me.QueryTypeString = "class") Then
                        If String.IsNullOrEmpty(Me.QueryString) Then
                            Throw New ArgumentOutOfRangeException("Query string missing. A query string is required for query type '" + Me.QueryTypeString + "'.")
                        Else
                            GetQueryCommand = "Classification:" + Me.QueryString
                        End If
                    ElseIf (Me.QueryTypeString = "access") Then
                        If String.IsNullOrEmpty(Me.QueryString) Then
                            Throw New ArgumentOutOfRangeException("Query string missing. A query string is required for query type 'access'.")
                        Else
                            GetQueryCommand = "DocNo:" + Me.QueryString
                        End If
                    ElseIf (Me.QueryTypeString = "recent") Then
                        If String.IsNullOrEmpty(Me.QueryString) Then
                            Throw New ArgumentOutOfRangeException("Query string missing. A query string is required for query type 'recent'.")
                        Else
                            GetQueryCommand = "Feld31:[" + Lucene.Net.Documents.DateField.DateToString(System.DateTime.Now.Date.AddMonths(-1)) + " TO " + Lucene.Net.Documents.DateField.DateToString(System.DateTime.Now.Date.AddDays(+1)) + "]"
                        End If
                    Else
                        If String.IsNullOrEmpty(Me.QueryString) Then
                            Throw New ArgumentOutOfRangeException("Query string missing. A query string is required for this query type.")
                        Else
                            GetQueryCommand = Me.QueryString
                        End If
                    End If
                Case Else
                        If String.IsNullOrEmpty(Me.QueryString) Then
                            Throw New ArgumentOutOfRangeException("Query string missing. A query string is required for this query type.")
                        Else
                            GetQueryCommand = Me.QueryString
                        End If
            End Select
        End Function

#End Region

    End Class

End Namespace
