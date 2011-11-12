Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Xml
'Imports RQDigitalObject
'Imports Modules.RQImport
Imports RQLib.RQDAL
Imports RQLib.RQQueryForm
Imports RQLib.RQConverter


Namespace RQQueryResult

    Public Class RQResultSet
        Implements Collections.IEnumerable

#Region "Private Variables"

        Private _docDAL As New RQCatalogDAL
        Private _bmDAL As New RQBookmarkDAL
        Private _docTable As RQDataSet.DokumenteDataTable
        Private _bmTable As RQBookmarkSet.BookmarksDataTable
        Private _sysTable As RQDataSet.SystematikDataTable
        Private _items() As RQResultItem

#End Region


#Region "Public Variables"

        Public Shared SortField As String = "Feld30"

#End Region


#Region "Public Properties"

        Public ReadOnly Property count() As Integer
            Get
                If Not IsNothing(_items) Then
                    Return _items.Length
                Else
                    Return 0
                End If
            End Get
        End Property


        Public ReadOnly Property items() As RQResultItem()
            Get
                Return _items
            End Get
        End Property

#End Region


#Region "Public Constructors"

        Public Sub New()

        End Sub


        Public Sub New(ByVal fromResultDocList As System.Xml.XmlDocument, Optional ByRef index As Integer = 1)
            If Not IsNothing(fromResultDocList.SelectNodes("//Dokument").Item(index - 1).Item("ID")) Then
                Dim strId As String = fromResultDocList.SelectNodes("//Dokument").Item(index - 1).Item("ID").InnerText

                If Not String.IsNullOrEmpty(strId) Then
                    Me.Find(CInt(strId), True)
                Else
                    Me.CreateItem(fromResultDocList, index)
                End If
            End If
        End Sub


        'Public Sub New(ByVal fromDigitalObject As DigitalObject)
        '    Me.CreateItem(fromDigitalObject, 0)
        'End Sub


        'Public Sub New(ByRef fromFileUpload As System.Web.HttpPostedFile, ByVal format As RQConverter.BibliographicFormats)
        '    Dim xmlResultDoc As New XmlDataDocument
        '    Dim xmlImportDoc As New XmlDataDocument
        '    Dim xmlResultStream As System.IO.MemoryStream = New System.IO.MemoryStream()
        '    Dim xslResultTransform As New System.Xml.Xsl.XslCompiledTransform

        '    Try
        '        'Dim i As Integer = 0

        '        xmlResultDoc.Load(fromFileUpload.InputStream)
        '        xslResultTransform.Load(HttpContext.Current.Request.MapPath("DeskTopModules/RQQueryResult/xsl/pbmtransform.xsl"))
        '        xslResultTransform.Transform(xmlResultDoc, Nothing, xmlResultStream)
        '        xmlResultStream.Seek(0, IO.SeekOrigin.Begin)
        '        xmlImportDoc.Load(xmlResultStream)
        '        'Do While (Not IsNothing(xmlImportDoc.SelectNodes("//Dokument").Item(i)))
        '        '    Me.CreateItem(xmlImportDoc, i + 1)
        '        '    i = i + 1
        '        'Loop
        '        ReadFrom(xmlImportDoc)
        '    Catch ex As Exception

        '    End Try
        'End Sub


        'Public Sub New(ByVal URL As String, ByVal format As RQConverter.BibliographicFormats)
        '    Dim webImport As New RQImport.WebPageImport(URL)
        '    Dim xmlResultDoc As New XmlDocument

        '    If webImport.ContainsDocRefs() = True Then
        '        xmlResultDoc = webImport.ConvertTo(RQConverter.BibliographicFormats.RQintern)
        '        If Not IsNothing(xmlResultDoc) Then
        '            Me.ReadFrom(xmlResultDoc)
        '        End If
        '    End If
        'End Sub


        Public Sub New(ByRef ClassString As String)
            Find(ClassString)
        End Sub

#End Region


#Region "Private Methods"

        Private Sub InitItemList()
            Dim c As Integer = 0

            If Not IsNothing(_docTable) Then
                c += _docTable.Rows.Count
            End If
            If Not IsNothing(_bmTable) Then
                c += _bmTable.Rows.Count
            End If
            If c > 0 Then
                _items = New RQResultItem(c - 1) {}
            Else
                '_items = New RQResultItem(0) {}
            End If
        End Sub


        Private Function AppendItem(ByRef type As RQResultItem.RQItemType) As RQResultItem
            Dim ni As New RQResultItem(type)

            If Not IsNothing(_items) Then
                Array.Resize(_items, _items.Length + 1)
            Else
                _items = New RQResultItem(0) {}
            End If
            Me._items(Me._items.Length - 1) = ni
            Return Me._items(Me._items.Length - 1)
        End Function


        Private Function AppendItem(ByRef item As RQResultItem) As RQResultItem
            If Not IsNothing(_items) Then
                Array.Resize(_items, _items.Length + 1)
            Else
                _items = New RQResultItem(0) {}
            End If
            Me._items(Me._items.Length - 1) = item
            Return Me._items(Me._items.Length - 1)
        End Function


        Private Sub ReadFrom(ByRef xmlList As XmlDocument)
            Dim i As Integer = 0

            Do While (Not IsNothing(xmlList.SelectNodes("//Dokument").Item(i)))
                Me.CreateItem(xmlList, i + 1)
                i = i + 1
            Loop
        End Sub

#End Region


#Region "Public Methods"

        'Create empty new item in result set
        Public Function CreateItem(Optional ByVal type As RQResultItem.RQItemType = RQResultItem.RQItemType.docdesc) As RQResultItem
            Dim ni As RQResultItem = Me.AppendItem(type)

            ni.SetChanged()
            ni.RQResultItemOwner = Me
            Return ni
        End Function


        Public Function CreateItem(ByVal fromResultDocList As System.Xml.XmlDocument, ByVal index As Integer, Optional ByRef type As RQResultItem.RQItemType = RQResultItem.RQItemType.docdesc) As RQResultItem
            Dim ni As RQResultItem = Me.CreateItem(type)

            ni.Read(fromResultDocList, index)
            Return ni
        End Function


        'Public Function CreateItem(ByVal fromDigitalObject As DigitalObject, ByRef index As Integer, Optional ByRef type As RQResultItem.RQItemType = RQResultItem.RQItemType.docdesc) As RQResultItem
        '    Dim ni As RQResultItem = Me.CreateItem(type)

        '    If (index = 0) Then
        '        'Create item for the root element of the DigitalObject structure
        '        ni.Read(fromDigitalObject, index)
        '    Else
        '        'Take the ItemDescription of the root element for all other elements of the DigitalObject structure
        '        ni.Read(fromDigitalObject, index, Me.GetItem(0))
        '    End If
        '    Return ni
        'End Function


        Public Function CreateItem(ByVal fromResultItem As RQResultItem) As RQResultItem
            Return Me.AppendItem(fromResultItem)
        End Function


        'Return items pertinent to query as an XML-document
        Public Function GetResultDoc(ByVal Query As RQquery) As System.Xml.XmlDocument
            'NOTE: as implemented GetItem will return nothing

            Dim xmlInStream As New System.IO.MemoryStream
            Dim xmlWriter As New System.Xml.XmlTextWriter(xmlInStream, System.Text.Encoding.UTF8)
            Dim xmlResult As New System.Xml.XmlDocument

            'retrieve classification list from table systematik of riquest database
            Me._sysTable = Me._docDAL.GetClassification(Query)
            ' retrieve document list from table dokumente of riquest database
            If Query.QueryString.Length > 0 Then
                Me._docTable = Me._docDAL.GetDocumentSet(Query)
            End If
            xmlWriter.WriteStartDocument()
            xmlWriter.WriteStartElement("", "QueryResults", "")
            Me._docDAL.DSName = "Systematiken"
            Me._sysTable.WriteXml(xmlWriter, XmlWriteMode.IgnoreSchema)
            xmlWriter.Flush()
            If Not IsNothing(Me._docTable) Then
                Me._docDAL.DSName = "Dokumentliste"
                Me._docTable.TableName = "Dokument"
                Me._docTable.WriteXml(xmlWriter, XmlWriteMode.IgnoreSchema)
            End If
            ' retrieve bookmark list from virtual library directory
            If Query.QueryBookmarks Then
                If Query.QueryType = RQquery.QueryTypeEnum.form _
                Or Query.QueryType = RQquery.QueryTypeEnum.recent _
                Or Query.QueryType = RQquery.QueryTypeEnum.browse _
                Or Query.QueryType = RQquery.QueryTypeEnum.classification Then
                    Me._bmTable = Me._bmDAL.GetBookmarkSet(Query, xmlWriter)
                End If
            End If
            'Initialize items list
            InitItemList()
            ' retrieve from bibliographic web services with dokumentliste-type data output
            If Query.QueryExternal <> "" Then
                If Query.QueryType = RQquery.QueryTypeEnum.form Then
                    'Dim _gl As New RQWebServiceDAL

                    '_gl.GetResults(Query, CType(xmlWriter, System.Xml.XmlWriter))
                End If
            End If
            xmlWriter.Flush()
            xmlWriter.WriteEndElement()
            xmlWriter.WriteEndDocument()
            xmlWriter.Flush()
            xmlInStream.Seek(0, IO.SeekOrigin.Begin)
            xmlResult.Load(xmlInStream)
            'xmlResult.Save(IO.Path.Combine(HttpRuntime.AppDomainAppPath, "upload/raw.xml"))
            xmlInStream.Close()
            xmlWriter.Close()
            Return xmlResult
        End Function


        'Finds items pertinent to query
        Public Sub Find(ByVal Query As RQquery, Optional ByVal iParentClassID As Int32 = 0)
            'Generic Find for RQResultSet; eventually substitute for RQResultSet.GetResultDoc
            'Should first retrieve tables for all data sources (Katalog, Bookmark, External)
            'Then xml-output file should be generated by RQResultSet.WriteXML
            'NOTE: as implemented GetItem will return nothing

            'retrieve classification list from table systematik of riquest database
            Me._sysTable = Me._docDAL.GetClassification(Query, iParentClassID)
            ' retrieve document list from table dokumente of riquest database
            If Query.QueryString.Length > 0 Then
                Me._docTable = Me._docDAL.GetDocumentSet(Query)
            End If
            ' retrieve bookmark list from virtual library directory
            If Query.QueryBookmarks Then
                If Query.QueryType = RQquery.QueryTypeEnum.form _
                Or Query.QueryType = RQquery.QueryTypeEnum.recent _
                Or Query.QueryType = RQquery.QueryTypeEnum.browse _
                Or Query.QueryType = RQquery.QueryTypeEnum.classification Then
                    Me._bmTable = Me._bmDAL.GetBookmarkSet(Query)
                End If
            End If
            'Initialize items list
            InitItemList()
        End Sub


        'Finds items with classification codes in the lexical set defined by ClassString
        Public Sub Find(ByVal ClassString As String)
            _docTable = _docDAL.GetRecordSetByClassString(ClassString)
            _bmTable = _bmDAL.GetBookmarkSetByClassString(ClassString)
            _items = New RQResultItem(_docTable.Rows.Count + _bmTable.Rows.Count - 1) {}
        End Sub


        'Finds single item with ID = RecordID
        Public Sub Find(ByVal RecordID As Integer, Optional ByVal clearResultSet As Boolean = False)
            Try
                Dim res As DataRow = _docDAL.GetRecordByID(CStr(RecordID), "RQDataSet", "Dokumente", clearResultSet)

                If Not IsNothing(res) Then
                    Me._docTable = Me._docDAL.GetDocumentSet()
                    _items = New RQResultItem(_docTable.Rows.Count - 1) {}
                End If
            Catch ex As Exception

            End Try
        End Sub


        Public Function GetItem(ByVal i As Integer) As RQResultItem
            If i < count Then
                If Not IsNothing(_items(i)) Then
                    Return _items(i)
                Else
                    Dim item As New RQResultItem()

                    If i < _docTable.Rows.Count Then
                        item.Read(_docTable.Rows(i))
                    Else
                        item.Read(_bmTable.Rows(i - _docTable.Rows.Count))
                    End If
                    item.RQResultItemOwner = Me
                    items(i) = item
                    Return items(i)
                End If
            End If
            Return Nothing
        End Function


        Public Function Update() As Integer
            Dim i As Integer
            Dim item As RQResultItem
            Dim changed As Boolean = False
            Dim retVal As Integer = 0

            If IsNothing(Me._docTable) Then Me._docTable = Me._docDAL.GetDocumentSet()
            For i = 0 To Me._docTable.Rows.Count - 1
                If Not IsNothing(_items(i)) Then
                    item = _items(i)
                    If item.RQResultItemChanged Then
                        If item.RQResultItemType = RQResultItem.RQItemType.docdesc Then
                            If i > Me._docTable.Rows.Count - 1 Then Me._docTable.AddDokumenteRow(CType(Me._docDAL.NewRow("RQDataSet", "Dokumente"), RQDataSet.DokumenteRow))
                            item.Write(_docTable.Rows(i))
                        End If
                        If item.RQResultItemType = RQResultItem.RQItemType.bookmark Then
                            item.Write(_bmTable.Rows(i - _docTable.Rows.Count))
                        End If
                        changed = True
                    End If
                End If
            Next
            If changed Then retVal = _docDAL.Update() + _bmDAL.Update
            For i = Me._docTable.Rows.Count To Me.count - 1
                If Not IsNothing(_items(i)) Then
                    item = _items(i)
                    If item.RQResultItemChanged Then
                        If item.RQResultItemType = RQResultItem.RQItemType.docdesc Then
                            Dim newRow As RQDataSet.DokumenteRow = CType(Me._docDAL.NewRow("RQDataSet", "Dokumente"), RQDataSet.DokumenteRow)

                            item.Write(newRow)
                            retVal += Me._docDAL.AddDokumente(newRow)
                            item.ItemDescription.ID = newRow.ID
                            item.ItemDescription.DocNo = newRow.DocNo
                        End If
                        If item.RQResultItemType = RQResultItem.RQItemType.bookmark Then
                            'not yet implemented
                        End If
                    End If
                End If
            Next
            Return retVal
        End Function


        Public Function UpdateClassRelation(ByRef OldClass As String, ByRef NewClass As String, ByRef Message As String) As Integer
            Return Me._docDAL.UpdateClassRelation(OldClass, NewClass, Message)
        End Function


        Public Function GetEnumerator() As IEnumerator _
           Implements IEnumerable.GetEnumerator

            Return New RQResultSetEnum(Me)
        End Function


        Public Sub WriteXML(ByRef XmlWriter As System.Xml.XmlWriter)
            XmlWriter.WriteStartDocument()
            XmlWriter.WriteStartElement("", "QueryResults", "")
            Me._docDAL.DSName = "Systematiken"
            Me._sysTable.WriteXml(XmlWriter, XmlWriteMode.IgnoreSchema)
            XmlWriter.Flush()
            If Not IsNothing(Me._docTable) Then
                Me._docDAL.DSName = "Dokumentliste"
                Me._docTable.TableName = "Dokument"
                Me._docTable.WriteXml(XmlWriter, XmlWriteMode.IgnoreSchema)
            End If
            If Not IsNothing(Me._bmTable) Then
                Me._bmTable.TableName = "Bookmark"
                Me._bmTable.WriteXml(XmlWriter, XmlWriteMode.IgnoreSchema)
            End If
            XmlWriter.Flush()
            XmlWriter.WriteEndElement()
            XmlWriter.WriteEndDocument()
            XmlWriter.Flush()
        End Sub


        Public Function GetDataFieldTable() As DataTable
            Return Me._docDAL.CatalogDataFields
        End Function


        Public Function Serialize(Optional ByVal fromRecord As Integer = 1,
                                  Optional ByVal maxRecords As Integer = 0,
                                  Optional ByVal IncludeEmptyFields As Boolean = True,
                                  Optional ByVal ListOnly As Boolean = False,
                                  Optional ByVal IncludeNamespace As Boolean = False,
                                  Optional ByVal IncludeSortField As Boolean = True) As XmlTextReader
            Dim writer As New XmlTextWriter(New System.IO.MemoryStream, Text.Encoding.UTF8)
            Dim i As Integer

            If IncludeNamespace Then
                writer.WriteStartElement("rq:ArrayOfRQItem")
                writer.WriteAttributeString("xmlns:rq", "http://riquest.de/formats/rq")
            Else
                writer.WriteStartElement("RQResultList")
                Me._docDAL.DSName = "Systematiken"
                Me._sysTable.WriteXml(writer, XmlWriteMode.IgnoreSchema)
                writer.WriteStartElement("RQResultSet")
            End If
            For i = fromRecord - 1 To maxRecords - 1
                Dim item As RQResultItem = Me.GetItem(i)

                writer.WriteNode(item.Serialize(IncludeEmptyFields, ListOnly, IncludeNamespace, IncludeSortField), True)
            Next
            writer.WriteEndElement()
            If Not (IncludeNamespace) Then writer.WriteEndElement()
            writer.Flush()
            writer.BaseStream.Seek(0, IO.SeekOrigin.Begin)
            Serialize = New XmlTextReader(writer.BaseStream)
        End Function


        Public Function ConvertTo(ByVal format As String, Optional ByVal fromRecord As Integer = 1,
                                                          Optional ByVal maxRecords As Integer = 0,
                                                          Optional ByVal IncludeEmptyFields As Boolean = True,
                                                          Optional ByVal ListOnly As Boolean = False,
                                                          Optional ByVal IncludeNamespace As Boolean = False,
                                                          Optional ByVal IncludeSortField As Boolean = True) As XmlTextReader
            Dim writer As New XmlTextWriter(New System.IO.MemoryStream, Text.Encoding.UTF8)
            'Dim i As Integer

            If fromRecord < 1 Then fromRecord = 1
            If fromRecord < Me.count Then
                If (maxRecords = 0) Or (fromRecord + maxRecords > Me.count + 1) Then maxRecords = Me.count - fromRecord + 1
                Select Case format
                    Case "info_ofi"
                    Case "mods"
                    Case "oai_dc"
                        Return New RQ2DC(Me, ServiceType.UNAPI, fromRecord, maxRecords).GetReader()
                    Case "pubmed"
                    Case "srw_dc"
                    Case Else
                        Return Me.Serialize(fromRecord, maxRecords, IncludeEmptyFields, ListOnly, IncludeNamespace, IncludeSortField)
                End Select
            End If
            writer.Flush()
            writer.BaseStream.Seek(0, IO.SeekOrigin.Begin)

            'TESTDATEI(EZEUGEN)
            'Dim Doc As New XmlDocument()

            'Doc.Load(writer.BaseStream)
            'Doc.Save("C:/MVCTest.xml")
            'writer.BaseStream.Seek(0, IO.SeekOrigin.Begin)

            ConvertTo = New XmlTextReader(writer.BaseStream)
        End Function

#End Region

    End Class


    Public Class RQResultSetEnum
        Implements IEnumerator


#Region "Private Members"

        Private _rqResultSet As RQResultSet

        ' Enumerators are positioned before the first element
        ' until the first MoveNext() call.
        Dim position As Integer = -1

#End Region


#Region "Constructors"

        Public Sub New(ByVal ResultSet As RQResultSet)
            _rqResultSet = ResultSet
        End Sub

#End Region


#Region "Public Methods"

        Public Function MoveNext() As Boolean Implements IEnumerator.MoveNext
            position = position + 1
            Return (position < _rqResultSet.count)
        End Function


        Public Sub Reset() Implements IEnumerator.Reset
            position = -1
        End Sub


        Public ReadOnly Property Current() As Object Implements IEnumerator.Current
            Get
                Try
                    Return _rqResultSet.GetItem(position)
                Catch ex As IndexOutOfRangeException
                    Throw New InvalidOperationException()
                End Try
            End Get
        End Property

#End Region

    End Class

End Namespace