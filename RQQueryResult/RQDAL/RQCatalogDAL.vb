Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Xml

Imports RQLib.Utilities


Namespace RQDAL

    Public Class RQCatalogDAL

#Region "Public Members"

        Public Enum DatabaseMode
            SearchEngine
            RelationalDB
            Hybrid
        End Enum

#End Region


#Region "Private Members"
        Private _catOleDBI As RQOleDBI
        Private _catRQLuceneDBI As RQLuceneDBI
        Private _catDoc As New XmlDocument()
        Private _catSet As New RQDataSet
        Private _catFieldList As New DataTable

        Private _mode As DatabaseMode = DatabaseMode.SearchEngine
#End Region


#Region "Public Properties"

        Public Property DSName() As String
            Get
                Return _catSet.DataSetName
            End Get
            Set(ByVal value As String)
                _catSet.DataSetName = value
            End Set
        End Property


        Public Property Mode() As DatabaseMode
            Get
                Return Me._mode
            End Get
            Set(ByVal value As DatabaseMode)
                Me._mode = value
            End Set
        End Property


        Public ReadOnly Property CatalogDataFields() As DataTable
            Get
                Return Me._catFieldList
            End Get
        End Property


        Public ReadOnly Property SysTable() As RQDataSet.SystematikDataTable
            Get
                Return CType(Me._catSet.Tables("Systematik"), RQDataSet.SystematikDataTable)
            End Get
        End Property


        Public ReadOnly Property DocTable() As RQDataSet.DokumenteDataTable
            Get
                Return CType(Me._catSet.Tables("Dokumente"), RQDataSet.DokumenteDataTable)
            End Get
        End Property

#End Region


#Region "Public Constructors"

        Public Sub New()
            Me._catRQLuceneDBI = New RQLuceneDBI()
            Me._catOleDBI = New RQOleDBI()
            Me.GetCatalogFieldTable()
        End Sub

        Public Sub New(ByVal databaseMode As DatabaseMode)
            Me.New()

            Me._mode = databaseMode
        End Sub

#End Region


#Region "Private Methods"

        Private Function GetCatalogFieldTable() As DataTable
            If _catFieldList.Columns.Count = 0 Then
                _catFieldList.Columns.Add("fieldname", Type.GetType("System.String"))
                _catFieldList.Columns.Add("fieldadress", Type.GetType("System.String"))
                _catFieldList.Columns.Add("searchfield", Type.GetType("System.Boolean"))
                _catFieldList.TableName = "queryfieldtable"
                Dim pk() As DataColumn = New DataColumn(1) {}
                pk(0) = _catFieldList.Columns("fieldname")
                _catFieldList.PrimaryKey = pk
            End If
            If _catFieldList.Rows.Count = 0 Then
                Me._catOleDBI.GetFieldTable(Me._catFieldList, "Dokumente", "ID,Language,Feld30,Feld31,Feld32,Feld33,Feld34,Feld35,Feld36,Feld37,Feld38")
            End If
            Return _catFieldList
        End Function


        Private Sub CatalogQuery(ByRef SqlStr As String, ByRef DSName As String, ByRef TableName As String)
            Me._catSet.DataSetName = DSName
            Me._catSet.Namespace = ""
            Me._catSet.Prefix = ""
            Me._catOleDBI.Fill(SqlStr, Me._catSet, TableName)
        End Sub


        Private Sub CatalogQuery(ByRef Query As RQQueryForm.RQquery, ByRef DSName As String, ByRef TableName As String)
            Me._catSet.DataSetName = DSName
            Me._catSet.Namespace = ""
            Me._catSet.Prefix = ""
            Me._catRQLuceneDBI.Fill(Query, Me._catSet, TableName)
        End Sub


        Private Function GetRecordTable(ByRef Query As RQQueryForm.RQquery) As RQDataSet.DokumenteDataTable
            Me._catSet.Tables("Dokumente").Clear()
            If (Me._mode = DatabaseMode.SearchEngine) Then
                Me.CatalogQuery(Query, "Dokumentliste", "Dokumente")
            ElseIf (Me._mode = DatabaseMode.Hybrid) Or (Me._mode = DatabaseMode.RelationalDB) Then
                Me.CatalogQuery(Query.GetQueryCommand(), "Dokumentliste", "Dokumente")
            End If
            If Query.QuerySort <> RQQueryForm.RQquery.SortType.undefined Then
                Me.SetSortField(Query)
            End If
            'Me._catSet.AcceptChanges()
            Return Me._catSet.Tables("Dokumente")
        End Function


        Private Function GetClassificationTable(ByRef SQLString As String) As RQDataSet.SystematikDataTable
            Me._catSet.Tables("Systematik").Clear()
            Me.CatalogQuery(SQLString, "Systematiken", "Systematik")
            'To generate correct sort orders by classification codes, the hyphenized classification
            'code ranges in table "Systematik" have to be expanded.
            Dim i As Integer = 0

            For i = 0 To Me._catSet.Tables("Systematik").Rows.Count - 1
                Dim LK As LexicalClass = New LexicalClass(CType(Me._catSet.Tables("Systematik").Rows(i).Item("RegensburgSign"), String))

                Me._catSet.Tables("Systematik").Rows(i).Item("RegensburgSign") = LK.Expand
            Next
            Return Me._catSet.Tables("Systematik")
        End Function


        Private Function GetClassificationSQL(ByRef QueryType As String, ByRef QueryString As String, Optional ByVal ParentClassID As Int32 = 0) As String
            'Check if ParentClassID > 0 ever occurs
            If QueryType = "class" Then
                If ParentClassID > 0 Then
                    Return "SELECT DDCNumber, Description, DocRefCount, ID, ParentID, " & _
                     "RegensburgDesc, RegensburgSign, SubClassCount FROM Systematik " & _
                     "WHERE ID=" & ParentClassID
                Else
                    'WORK IN PROGRESS: 
                    'Diese Variante müßte eigentlich auch dann funktionieren wenn iParentClassID > 0
                    Return "SELECT DDCNumber, Description, DocRefCount, ID, ParentID, " & _
                     "RegensburgDesc, RegensburgSign, SubClassCount FROM Systematik " & _
                     "WHERE DDCNumber='" & QueryString & "'"
                End If
            Else
                Return "SELECT DDCNumber, Description, DocRefCount, ID, ParentID, " & _
                 "RegensburgDesc, RegensburgSign, SubClassCount FROM Systematik " & _
                 "WHERE ParentId=" & ParentClassID
            End If
        End Function


        Private Sub WriteSortFieldbyClass(index As Integer)
            Dim strClasses() As String
            Dim strRVKClasses As String = ""
            Dim j As Integer

            strClasses = CStr(Me._catSet.Tables("Dokumente").Rows(index).Item("Classification")).Split(";"c)
            For j = 0 To strClasses.Length - 1
                If strClasses(j).IndexOf(Globals.ClassCodePrefix) > 0 Then
                    strRVKClasses += strClasses(j).Substring(strClasses(j).IndexOf(":") + 1).Trim() + ";"
                End If
            Next
            If strRVKClasses <> "" Then
                Me._catSet.Tables("Dokumente").Rows(index).Item(RQQueryResult.RQResultSet.SortField) = strRVKClasses
            Else
                Me._catSet.Tables("Dokumente").Rows(index).Item(RQQueryResult.RQResultSet.SortField) = "ZZ99999"
            End If
        End Sub


        Private Sub SetSortField(ByRef Query As RQQueryForm.RQquery)
            Dim i As Integer = 0
            Dim SortCriterion As RQQueryForm.RQquery.SortType = Query.QuerySort

            For i = Me._catSet.Tables("Dokumente").Rows.Count - 1 To 0 Step -1
                Select Case SortCriterion
                    Case RQQueryForm.RQquery.SortType.ByTitle
                        Me._catSet.Tables("Dokumente").Rows(i).Item(RQQueryResult.RQResultSet.SortField) = Me._catSet.Tables("Dokumente").Rows(i).Item(SortCriterion)
                    Case RQQueryForm.RQquery.SortType.ByCreationDate
                        Me._catSet.Tables("Dokumente").Rows(i).Item(RQQueryResult.RQResultSet.SortField) = Me._catSet.Tables("Dokumente").Rows(i).Item("CreateTime")
                    Case RQQueryForm.RQquery.SortType.ByShelf
                        If Me._catSet.Tables("Dokumente").Rows(i).Item("Classification").GetType.FullName = "System.String" Then
                            Dim classStr = Me._catSet.Tables("Dokumente").Rows(i).Item("Classification").ToString()

                            If (Not classStr.Substring(0, classStr.IndexOf(";")) = Query.QueryString) Then
                                Me._catSet.Tables("Dokumente").Rows(i).Delete()
                            Else
                                WriteSortFieldbyClass(i)
                            End If
                        Else
                            Me._catSet.Tables("Dokumente").Rows(i).Item(RQQueryResult.RQResultSet.SortField) = "ZZ99999"
                        End If
                    Case RQQueryForm.RQquery.SortType.BySubject
                        If Me._catSet.Tables("Dokumente").Rows(i).Item("Classification").GetType.FullName = "System.String" Then
                            WriteSortFieldbyClass(i)
                        Else
                            Me._catSet.Tables("Dokumente").Rows(i).Item(RQQueryResult.RQResultSet.SortField) = "ZZ99999"
                        End If
                End Select
            Next
        End Sub

#End Region


#Region "Public Methods"

        Public Function GetRecordSetByClassCode(ByRef classCode As String, ByRef OutXml As System.Xml.XmlWriter, Optional ByVal SortCriterion As String = "") As RQDataSet.DokumenteDataTable
            Return Nothing
        End Function


        Public Function GetRecordSetByClassCode(ByRef classCode As String, Optional ByVal SortCriterion As String = "") As RQDataSet.DokumenteDataTable
            Return Nothing
        End Function


        Public Function GetRecordSetByClassString(ByRef classString As String, ByRef OutXml As System.Xml.XmlWriter) As RQDataSet.DokumenteDataTable
            Return Nothing
        End Function


        Public Function GetRecordSetByClassString(ByRef classString As String, ByVal ClearDataSet As Boolean) As RQDataSet.DokumenteDataTable
            Dim strSQL As String
            Dim lcLexClass As LexicalClass = New LexicalClass(classString)

            If ClearDataSet Then Me._catSet.Clear()
            strSQL = "SELECT * FROM Dokumente WHERE " + lcLexClass.Expand(" OR ", "Dokumente.Classification LIKE '%; " + Globals.ClassCodePrefix, "%'")
            Me.CatalogQuery(strSQL, DSName, "Dokumente")
            Return Me._catSet.Tables("Dokumente")
        End Function


        Public Function GetRecordSetByClassString(ByRef classString As String, Optional ByVal SortCriterion As String = "", Optional ByVal ClearDataSet As Boolean = True) As RQDataSet.DokumenteDataTable
            Dim strSQL As String
            Dim lcLexClass As LexicalClass = New LexicalClass(classString)

            If ClearDataSet = True Then Me._catSet.Clear()
            strSQL = "SELECT * FROM Dokumente WHERE " + lcLexClass.Expand(" OR ", "Dokumente.Classification LIKE '%; " + Globals.ClassCodePrefix, "%'")
            Me.CatalogQuery(strSQL, "RQDataSet", "Dokumente")
            Return Me._catSet.Tables("Dokumente")
        End Function


        Public Function GetDocumentSet() As RQDataSet.DokumenteDataTable
            Return Me._catSet.Tables("Dokumente")
        End Function


        Public Function GetDocumentSet(ByRef Query As RQQueryForm.RQquery) As RQDataSet.DokumenteDataTable
            Return Me.GetRecordTable(Query)
        End Function


        Public Function GetRecordSetBySWDName(ByRef MySWDName As String, ByRef DSName As String, Optional ByVal ClearDataSet As Boolean = True) As DataTable
            Dim strSQL As String

            If ClearDataSet = True Then Me._catSet.Clear()
            If MySWDName <> "" Then
                strSQL = "SELECT * FROM Dokumente WHERE Dokumente.Description LIKE '%; " + MySWDName + "%'"
            Else
                strSQL = "SELECT ID, DocNo, IndexTerms, Subjects FROM Dokumente"
            End If
            Me.CatalogQuery(strSQL, DSName, "Dokumente")
            Return Me._catSet.Tables("Dokumente")
        End Function


        Public Function GetRecordByID(ByRef RecordID As String, ByRef DSName As String, ByRef TableName As String, Optional ByVal ClearDataSet As Boolean = False) As DataRow
            Dim strSQL As String

            If ClearDataSet Or Me._catSet.Tables(TableName).Rows.Count = 0 Then
                If ClearDataSet = True Then Me._catSet.Clear()
                strSQL = "SELECT * FROM " + TableName + " WHERE " + TableName + ".ID = " + RecordID
                Me.CatalogQuery(strSQL, DSName, TableName)
            End If
            'STRANGE ERROR: If Rows(0).DirRefCount is DBNull or Nothing (depending of RQDataSet-Properties) if taken from the stored Me._catSet. It is "0" if taken from Me.CatalogQuery
            Return Me._catSet.Tables(TableName).Rows(0)
        End Function


        Public Function GetRecordByParentID(ByRef ParentID As String, ByRef DSName As String, ByRef TableName As String, Optional ByVal ClearDataSet As Boolean = False) As DataTable
            Dim strSQL As String

            If ClearDataSet = True Then Me._catSet.Clear()
            strSQL = "SELECT * FROM " + TableName + " WHERE " + TableName + ".ParentID = " + ParentID
            Me.CatalogQuery(strSQL, DSName, TableName)
            Return Me._catSet.Tables(TableName)
        End Function


        Public Function GetClassification(ByRef Query As RQQueryForm.RQquery, Optional ByVal ParentClassID As Int32 = 0) As RQDataSet.SystematikDataTable
            Return Me.GetClassificationTable(Me.GetClassificationSQL(Query.QueryTypeString, Query.QueryString, ParentClassID))
        End Function


        Public Function GetClassID(ByRef ClassCode As String) As Integer
            Try
                Return Me.GetClassificationTable(Me.GetClassificationSQL("class", ClassCode, 0)).Rows(0).Item(0)
            Catch
                Return 0
            End Try
        End Function


        Public Function GetMyClassNameFromRVKCode(ByRef RVKCode As String, ByRef DSName As String, Optional ByVal ClearDataSet As Boolean = False) As String
            Dim strSQL As String
            Dim strSearchArg As String = RVKCode

            While RVKCode.Length > 0
                If ClearDataSet = True Then Me._catSet.Clear()
                strSQL = "SELECT * FROM Systematik WHERE (Systematik.RegensburgSign LIKE '" + strSearchArg + "%') OR (Systematik.RegensburgSign LIKE '%; " + strSearchArg + "%') OR (Systematik.RegensburgSign LIKE '%-" + strSearchArg + "%') ORDER BY Systematik.DDCNumber ASC"
                Me.CatalogQuery(strSQL, DSName, "Systematik")
                If Me._catSet.Tables("Systematik").Rows.Count = 0 Then
                    strSearchArg = strSearchArg.Substring(0, strSearchArg.Length - 1)
                Else
                    Dim i As Integer
                    Dim strClassName As String = ""

                    For i = 0 To Me._catSet.Tables("Systematik").Rows.Count - 1
                        Dim TestLexClass As New LexicalClass(CStr(Me._catSet.Tables("Systematik").Rows(i).Item(4)))

                        If TestLexClass.IsInRange(RVKCode) Then
                            If strClassName <> "" Then
                                If CType(Me._catSet.Tables("Systematik").Rows(i).Item(2), String).StartsWith(strClassName) Then
                                    strClassName = CStr(Me._catSet.Tables("Systematik").Rows(i).Item(2))
                                Else
                                    'FEHLER IN DER SYSTEMATIKTABELLE
                                    Return "ERROR: Inconsistency in Table 'Systematik'"
                                End If
                            Else
                                strClassName = CStr(Me._catSet.Tables("Systematik").Rows(i).Item(2))
                            End If
                        End If
                    Next
                    If strClassName <> "" Then
                        Return strClassName
                    Else
                        If strSearchArg.Length > 1 Then
                            strSearchArg = strSearchArg.Substring(0, strSearchArg.Length - 1)
                        Else
                            Return strClassName
                        End If
                    End If
                End If
            End While
            Return ""
        End Function


        Public Function NewRow(ByRef DSName As String, ByRef TableName As String) As DataRow
            If TableName = "Dokumente" Then
                Return Me._catSet.Dokumente.NewDokumenteRow
            End If
            If TableName = "Systematik" Then
                Return Me._catSet.Systematik.NewSystematikRow
            End If
            Return Nothing
        End Function


        Public Function AddDokumente(ByRef NewRow As DataRow) As Integer
            Dim iMaxRecNr As Integer
            Dim iMaxRecID As Integer
            Dim strNewDocNo As String = ""
            Dim dtIdDocNoTable As DataTable
            Dim drLastRow As System.Data.DataRow
            Dim dcIDColumns As System.Data.DataColumn() = {Me._catSet.Tables("Dokumente").Columns("ID")}
            Dim dsChangeSet As System.Data.DataSet
            Dim dsTotalSet As New RQLib.RQDataSet

            Me._catOleDBI.Fill("SELECT ID, DocNo FROM Dokumente", dsTotalSet, "ALL")
            dsTotalSet.Tables("ALL").DefaultView.ApplyDefaultSort = True
            dtIdDocNoTable = dsTotalSet.Tables("ALL").DefaultView.ToTable()
            iMaxRecNr = dtIdDocNoTable.Rows.Count
            drLastRow = dtIdDocNoTable.Rows(iMaxRecNr - 1)
            iMaxRecID = System.Convert.ToInt32(drLastRow.Item("ID"))
            If CInt(drLastRow.Item("DocNo")) + 1 < 10000 Then
                strNewDocNo = "0"
            End If
            strNewDocNo += CStr(CInt(drLastRow.Item("DocNo")) + 1)
            'Me._catSet.Tables("Dokumente").PrimaryKey = dcIDColumns
            'Me._catSet.Tables("Dokumente").Columns("ID").AutoIncrement = True
            'Me._catSet.Tables("Dokumente").Columns("ID").ReadOnly = True
            'Me._catSet.Tables("Dokumente").Columns("ID").AutoIncrementSeed = iMaxRecID + 1
            'Me._catSet.Tables("Dokumente").Columns("ID").Unique = True
            NewRow.Item("ID") = iMaxRecID + 1
            NewRow.Item("DocNo") = strNewDocNo
            NewRow.Item("Feld30") = "NOSORT"
            NewRow.Item("Feld31") = System.DateTime.Now
            Me._catSet.Dokumente.AddDokumenteRow(CType(NewRow, RQDataSet.DokumenteRow))
            dsChangeSet = Me._catSet.GetChanges()
            If Not IsNothing(dsChangeSet) Then
                Try
                    Me._catOleDBI.Update(dsChangeSet, "Dokumente")
                    Me._catRQLuceneDBI.Add(dsChangeSet, "Dokumente")
                    Me._catSet.Tables("Dokumente").AcceptChanges()
                    Return 0
                Catch ex As Exception
                    Return 1
                End Try
            Else
                Return 0
            End If
        End Function


        'Public Function AddDokumente(ByRef NewRow As DataRow) As Integer
        ' OUTCOMMENTED: 130716 because function modifies the underlying catSet 
        '    Dim iMaxRecNr As Integer
        '    Dim iMaxRecID As Integer
        '    Dim strNewDocNo As String = ""
        '    Dim dtIdDocNoTable As DataTable
        '    Dim drLastRow As System.Data.DataRow
        '    Dim dcIDColumns As System.Data.DataColumn() = {Me._catSet.Tables("Dokumente").Columns("ID")}
        '    Dim dsChangeSet As System.Data.DataSet

        '    'Me._catSet.EnforceConstraints = False
        '    Me.CatalogQuery("SELECT ID, DocNo FROM Dokumente", "RQDataSet", "Dokumente")
        '    Me._catSet.Tables("Dokumente").DefaultView.ApplyDefaultSort = True
        '    dtIdDocNoTable = Me._catSet.Tables("Dokumente").DefaultView.ToTable()
        '    iMaxRecNr = dtIdDocNoTable.Rows.Count
        '    drLastRow = dtIdDocNoTable.Rows(iMaxRecNr - 1)
        '    iMaxRecID = System.Convert.ToInt32(drLastRow.Item("ID"))
        '    If CInt(drLastRow.Item("DocNo")) + 1 < 10000 Then
        '        strNewDocNo = "0"
        '    End If
        '    strNewDocNo += CStr(CInt(drLastRow.Item("DocNo")) + 1)
        '    Me._catSet.Tables("Dokumente").PrimaryKey = dcIDColumns
        '    Me._catSet.Tables("Dokumente").Columns("ID").AutoIncrement = True
        '    Me._catSet.Tables("Dokumente").Columns("ID").ReadOnly = True
        '    Me._catSet.Tables("Dokumente").Columns("ID").AutoIncrementSeed = iMaxRecID + 1
        '    Me._catSet.Tables("Dokumente").Columns("ID").Unique = True
        '    NewRow.Item("ID") = iMaxRecID + 1
        '    NewRow.Item("DocNo") = strNewDocNo
        '    NewRow.Item("Feld31") = System.DateTime.Now
        '    Me._catSet.Dokumente.AddDokumenteRow(CType(NewRow, RQDataSet.DokumenteRow))
        '    dsChangeSet = Me._catSet.GetChanges()
        '    If Not IsNothing(dsChangeSet) Then
        '        Try
        '            Me._catOleDBI.Update(dsChangeSet, "Dokumente")
        '            Me._catRQLuceneDBI.Update(dsChangeSet, "Dokumente")
        '            Me._catSet.AcceptChanges()
        '            Return 0
        '        Catch ex As Exception
        '            Return 1
        '        End Try
        '    Else
        '        Return 0
        '    End If
        'End Function


        Public Function AddSystematik(ByRef NewRow As DataRow) As Integer
            Dim iMaxRecNr As Integer
            Dim iMaxRecID As Integer
            Dim drLastRow As System.Data.DataRow
            Dim dcIDColumns As System.Data.DataColumn() = {Me._catSet.Tables("Systematik").Columns("ID")}
            Dim dsChangeSet As System.Data.DataSet

            Me.CatalogQuery("SELECT * FROM Systematik ORDER BY ID", "RQDataSet", "Systematik")
            iMaxRecNr = Me._catSet.Tables("Systematik").Rows.Count
            drLastRow = Me._catSet.Tables("Systematik").Rows(iMaxRecNr - 1)
            iMaxRecID = System.Convert.ToInt32(drLastRow.Item("ID"))
            Me._catSet.Tables("Systematik").PrimaryKey = dcIDColumns
            Me._catSet.Tables("Systematik").Columns("ID").AutoIncrement = True
            Me._catSet.Tables("Systematik").Columns("ID").ReadOnly = True
            Me._catSet.Tables("Systematik").Columns("ID").AutoIncrementSeed = iMaxRecID + 1
            Me._catSet.Tables("Systematik").Columns("ID").Unique = True
            NewRow.Item("ID") = iMaxRecID + 1
            Me._catSet.Systematik.AddSystematikRow(CType(NewRow, RQDataSet.SystematikRow))
            dsChangeSet = Me._catSet.GetChanges()
            If Not IsNothing(dsChangeSet) Then
                Try
                    Me._catOleDBI.Update(dsChangeSet, "Systematik")
                    Me._catSet.Tables("Systematik").AcceptChanges()
                    Return 0
                Catch ex As Exception
                    Return 1
                End Try
            Else
                Return 0
            End If
        End Function


        Public Function Update() As Integer
            Dim dsChangeSet As DataSet

            dsChangeSet = Me._catSet.GetChanges()
            If Not IsNothing(dsChangeSet) Then
                Try
                    If (Me._mode = DatabaseMode.Hybrid) Or (Me._mode = DatabaseMode.RelationalDB) Then
                        Me._catOleDBI.Update(dsChangeSet, "Dokumente")
                    End If
                    If (Me._mode = DatabaseMode.Hybrid) Or (Me._mode = DatabaseMode.SearchEngine) Then
                        Me._catRQLuceneDBI.Update(dsChangeSet, "Dokumente")
                    End If
                    Me._catSet.Tables("Dokumente").AcceptChanges()
                    Return 0
                Catch ex As Exception
                    Throw New ApplicationException(ex.Message, ex.InnerException)
                    'Return 1
                End Try
            Else
                Return 0
            End If
        End Function


        Public Function UpdateSystematik() As Integer
            Dim dsChangeSet As DataSet

            dsChangeSet = Me._catSet.GetChanges()
            If Not IsNothing(dsChangeSet) Then
                Try
                    Me._catOleDBI.Update(dsChangeSet, "Systematik")
                    Me._catSet.Tables("Systematik").AcceptChanges()
                    Return 0
                Catch ex As Exception
                    Return 1
                End Try
            Else
                Return 0
            End If
        End Function


        Public Function UpdateClassRelation(ByRef OldClass As String, ByRef NewClass As String, ByRef Message As String) As Integer
            Dim strSQL As String = ""
            Dim strSrc As String = NewClass
            Dim strComp As String = OldClass
            Dim i As Integer
            Dim dsChangeSet As DataSet
            Dim iDec As Integer = 1
            Dim iRuns As Integer = 2
            Dim strSubHead As String = "Added classification codes"

            Do While (iRuns > 0)
                Dim strTarget As String() = strSrc.Split(";"c)

                iRuns = iRuns - 1
                For i = 0 To strTarget.Length() - 1
                    If strTarget(i) <> "" And strComp.IndexOf(strTarget(i).Trim() + ";") = -1 Then
                        If strSQL <> "" Then
                            strSQL += " OR (DDCNumber = "
                        Else
                            strSQL = "SELECT * FROM Systematik WHERE (DDCNumber = "
                        End If
                        strSQL += "'" + strTarget(i).Trim + "')"
                    End If
                Next
                If strSQL <> "" Then
                    Message += "<p class='instructions'>" + strSubHead + "</p>"
                    Message += "<table class='instructions' border='1' cellpadding='0' cellspacing='0'>"
                    Message += "<tr><td class='instructions'><b>class</b></td><td class='instructions'><b>name</b></td><td class='instructions'><b>#docs</b></td></tr>"
                    Me._catSet.Systematik.Clear()
                    Me.CatalogQuery(strSQL, "RQDataSet", "Systematik")
                    If Me._catSet.Systematik.Rows.Count > 0 Then
                        For i = 0 To Me._catSet.Systematik.Rows.Count - 1
                            Dim iCount As Integer = CInt(_catSet.Systematik.Rows(i).Item("DocRefCount"))

                            iCount += iDec
                            If (iCount < 0) Then iCount = 0
                            _catSet.Systematik.Rows(i).Item("DocRefCount") = iCount
                            Message += "<tr><td class='instructions'>" & CStr(_catSet.Systematik.Rows(i).Item("DDCNumber")) & "</td><td class='instructions'>" & CStr(_catSet.Systematik.Rows(i).Item("Description")) & "</td><td class='instructions'>" & CStr(iCount) & "</td></tr>"
                        Next
                        For i = 0 To strTarget.Length - 1
                            Dim j As Integer

                            If strTarget(i) <> "" And strComp.IndexOf(strTarget(i).Trim() + ";") = -1 Then
                                Dim out As Boolean = True

                                For j = 0 To Me._catSet.Systematik.Rows.Count - 1
                                    If strTarget(i).Trim = CStr(_catSet.Systematik.Rows(j).Item("DDCNumber")) Then
                                        out = False
                                    End If
                                Next
                                If out Then
                                    Message += "<tr><td class='instructions'>" & strTarget(i).Trim & "</td><td class='instructions'>na</td><td class='instructions'>na</td></tr>"
                                End If
                            End If
                        Next
                    Else
                        For i = 0 To strTarget.Length - 1
                            If strTarget(i) <> "" And strComp.IndexOf(strTarget(i).Trim() + ";") = -1 Then
                                If Not strTarget(i).StartsWith(Globals.ClassCodePrefix) Then
                                    Message += "<tr><td class='instructions'>" & strTarget(i).Trim & "</td><td class='instructions'>na</td><td class='instructions'>na</td></tr>"
                                End If
                            End If
                        Next
                    End If
                    dsChangeSet = Me._catSet.GetChanges()
                    If Not IsNothing(dsChangeSet) Then
                        Try
                            Me._catOleDBI.Test(dsChangeSet, "Systematik")
                            Me._catSet.Tables("Systematik").AcceptChanges()
                            Message += "<tr><td class='instructions' colspan='3'>Storage OK</td></tr>"
                        Catch ex As Exception
                            Message += "<tr><td class='instructions' colspan='3'>Storage ERROR</td></tr>"
                        End Try
                    End If
                    Message += "</table>"
                End If
                strSubHead = "Deleted classification codes"
                strSrc = OldClass
                strComp = NewClass
                strSQL = ""
                iDec = -1
            Loop
            Return 0
        End Function

#End Region


    End Class

End Namespace