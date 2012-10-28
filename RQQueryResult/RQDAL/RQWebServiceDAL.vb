Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Net
Imports System.IO

Namespace RQDAL

    Public Class RQWebServiceDAL

#Region "Private Members"

        Private Const gMaxResults As Integer = 10
        Private gStart As Integer = 1
        Private CQLContextSet As System.Collections.Specialized.StringDictionary

        Private _xtTable As New RQDataSet.DokumenteDataTable()

#End Region


#Region "Private Methods"

        Private Function CQLParse(ByRef parseString As String) As Boolean
            'Test if parseString is a valid CQL-Query. Not yet implemented

            Return False
        End Function


        Private Function GetQueryCQL(ByRef QueryString As String, ByRef dtQueryFields As DataTable) As String
            Dim sCQL As New System.Text.StringBuilder
            Dim sQueryLogic As String = " or "
            Dim sQryString As String = ""
            Dim sQryTerms As String()

            If Not CQLParse(QueryString) Then
                Dim sFieldTest As String = ""
                Dim iFieldCount As Integer = 0
                Dim i As Integer = 0

                QueryString = QueryString.Replace("(", "")
                QueryString = QueryString.Replace(")", "")
                QueryString = QueryString.Replace("=", "")
                QueryString = QueryString.Replace("<", "")
                QueryString = QueryString.Replace(">", "")
                QueryString = QueryString.Replace("/", "")
                QueryString = QueryString.Replace(Chr(34), "")
                sQryTerms = QueryString.Split
                For i = 0 To sQryTerms.Length - 1
                    If sQryTerms.Length > 1 And i = 0 Then sQryString += "("
                    If sQryTerms(i).StartsWith("+") Then
                        sQryTerms(i) = sQryTerms(i).Replace("+", "")
                        sQueryLogic = " and "
                    End If
                    sQryString += sQryTerms(i)
                    If sQryTerms.Length > 1 Then
                        If i < sQryTerms.Length - 1 Then sQryString += sQueryLogic
                        If i = sQryTerms.Length - 1 Then sQryString += ")"
                    End If
                Next
                If dtQueryFields.Rows.Count > 0 Then
                    For i = 0 To dtQueryFields.Rows.Count - 1
                        If (CType(dtQueryFields.Rows.Item(i).Item("searchfield"), Boolean)) = True Then
                            sFieldTest += dtQueryFields.Rows.Item(i).Item("fieldname") + " "
                            If iFieldCount > 3 Then
                                GetQueryCQL = sQryString
                                Exit Function
                            Else
                                iFieldCount += 1
                            End If
                        End If
                    Next
                End If

                Dim it As IDictionaryEnumerator = CQLContextSet.GetEnumerator
                Dim bAppOr As Boolean = False

                While it.MoveNext()
                    Dim sFields() As String = CQLContextSet.Item(it.Key).Split
                    Dim bGo As Boolean = False

                    For i = 0 To sFields.Length - 1
                        bGo = bGo Or sFieldTest.Contains(sFields(i))
                    Next
                    If bGo Then
                        If bAppOr Then
                            sCQL.Append(" or ")
                        End If
                        bAppOr = True
                        sCQL.Append(it.Key)
                        sCQL.Append("%3D")
                        sCQL.Append(sQryString)
                    End If
                End While
                GetQueryCQL = sCQL.ToString
            Else
                GetQueryCQL = QueryString
            End If
        End Function


        Private Function GetXMLContent(ByVal ContentURL As String) As System.Xml.XmlReader
            Try
                'create an HTTP request.
                Dim wr As HttpWebRequest = CType(WebRequest.Create(ContentURL), HttpWebRequest)
                Dim proxy As System.Net.IWebProxy = WebRequest.GetSystemWebProxy()

                wr.Timeout = 10000 ' 10 seconds
                wr.KeepAlive = False
                wr.Proxy = proxy

                'get the response object.
                Dim resp As WebResponse = wr.GetResponse()
                Dim stream As Stream = resp.GetResponseStream()

                ' load XML document
                Dim reader As System.Xml.XmlTextReader = New System.Xml.XmlTextReader(stream)

                reader.XmlResolver = Nothing
                Return reader
            Catch ex As Exception
                'return some error code.
                Dim stream As System.IO.MemoryStream = New System.IO.MemoryStream()
                Dim writer As System.Xml.XmlTextWriter = New System.Xml.XmlTextWriter(stream, Nothing)

                writer.WriteComment(ex.Message)
                Return New System.Xml.XmlTextReader(writer.BaseStream)
            End Try
        End Function


        Private Function WriteTableRow(ByRef DirNode As System.Xml.XmlNode, ByVal SortCriterion As RQLib.RQQueryForm.RQquery.SortType) As RQDataSet.DokumenteRow
            Dim row As RQDataSet.DokumenteRow = Me._xtTable.NewRow()
            Dim col As System.Data.DataColumn

            For Each col In Me._xtTable.Columns
                If col.DataType().ToString().Contains("String") Then
                    Dim field As System.Xml.XmlNode = DirNode.SelectSingleNode(col.ColumnName)

                    If (Not IsNothing(field)) Then row.Item(col.ColumnName) = field.InnerText()
                End If
            Next
            Return row
        End Function


        Private Function ConvertNodesToTable(ByRef DirNodes As System.Xml.XPath.XPathNodeIterator, ByVal SortCriterion As RQLib.RQQueryForm.RQquery.SortType) As RQDataSet.DokumenteDataTable
            Dim doc As New System.Xml.XmlDocument()
            Dim ind As Integer = 0

            While DirNodes.MoveNext
                doc.LoadXml(DirNodes.Current().OuterXml)
                Me._xtTable.AddDokumenteRow(WriteTableRow(doc.DocumentElement(), SortCriterion))
                CType(Me._xtTable.Rows(Me._xtTable.Count - 1), RQDataSet.DokumenteRow).DocNo = "09" + CStr(Me._xtTable.Count)
            End While
            Return Me._xtTable
        End Function

#End Region


#Region "Public Methods"

        Public Function GetDocumentSet(ByRef Query As RQLib.RQQueryForm.RQquery) As System.Data.DataTable
            Dim xmlRQI As New MemoryStream()

            GetResults(Query, xmlRQI)
            xmlRQI.Seek(0, System.IO.SeekOrigin.Begin)
            Return ConvertNodesToTable(New System.Xml.XPath.XPathDocument(xmlRQI).CreateNavigator().Select("/Systematik/Dokument"), Query.QuerySort)
        End Function


        Public Sub GetResults(ByRef Query As RQLib.RQQueryForm.RQquery, ByRef OutXml As System.IO.MemoryStream)
            Dim xslResultTransform As New System.Xml.Xsl.XslCompiledTransform()
            Dim xslTransformArgs As New System.Xml.Xsl.XsltArgumentList()
            Dim xslSettings As New System.Xml.Xsl.XsltSettings()
            Dim xmlIn As System.Xml.XmlReader = Nothing
            Dim ms1 As New MemoryStream()

            If Query.QueryExternal = "001" Then xmlIn = GetXMLContent("http://gso.gbv.de/sru/DB=1.50/?version=1.1&operation=searchRetrieve&query=" & GetQueryCQL(Query.QueryString, Query.QueryFieldList) & "&maximumRecords=100&recordSchema=marc21")
            If Query.QueryExternal = "002" Then xmlIn = GetXMLContent("http://gso.gbv.de/sru/DB=1.55/?version=1.1&operation=searchRetrieve&query=" & GetQueryCQL(Query.QueryString, Query.QueryFieldList) & "&maximumRecords=100&recordSchema=marc21")
            If Query.QueryExternal = "003" Then xmlIn = GetXMLContent("http://gso.gbv.de/sru/DB=2.1/?version=1.1&operation=searchRetrieve&query=" & GetQueryCQL(Query.QueryString, Query.QueryFieldList) & "&maximumRecords=100&recordSchema=marc21")
            If Query.QueryExternal = "004" Then xmlIn = GetXMLContent("http://worldcat.org/webservices/catalog/search/opensearch?q=" & GetQueryCQL(Query.QueryString, Query.QueryFieldList) & "&format=atom&cformat=all&start=0&count=100&wskey=yxl6CIVEg0loBC2AzwUR7CDFvBOhqEWu5JkRvdTYnBOfoR1yGQKlE5Fj4QU7yuFWDrh50LsteuhCD1h6")
            Try
                xslSettings.EnableScript = False
                xslResultTransform.Load(Path.Combine(System.Web.HttpRuntime.AppDomainAppPath, "xslt/SRUMARC2MODS.xslt"), xslSettings, Nothing)
                xslResultTransform.Transform(xmlIn, xslTransformArgs, ms1)
                xslResultTransform.Load(Path.Combine(System.Web.HttpRuntime.AppDomainAppPath, "xslt/MODS2RQI.xslt"), xslSettings, Nothing)
                ms1.Seek(0, IO.SeekOrigin.Begin)
                xslResultTransform.Transform(New Xml.XmlTextReader(ms1), xslTransformArgs, OutXml)
            Catch ex As Exception
            End Try
        End Sub


        Public Sub New()
            CQLContextSet = New System.Collections.Specialized.StringDictionary()

            'Mapping für LoC sRU Interface
            'CQLContextSet.Add("dc.title", "Title Series")
            'CQLContextSet.Add("dc.creator", "Author")
            'CQLContextSet.Add("dc.subject", "IndexTerms Classification")
            'CQLContextSet.Add("dc.description", "Abstract")
            'CQLContextSet.Add("dc.publisher", "Publisher")
            'CQLContextSet.Add("dc.author", "Author")
            'CQLContextSet.Add("dc.date", "PublicationTime")
            'CQLContextSet.Add("dc.resourceType", "WorkType DocumentType")
            'CQLContextSet.Add("dc.resourceIdentifier", "ISDN")
            'CQLContextSet.Add("dc.source", "Source")

            'Mapping für GBV SRU Interface
            CQLContextSet.Add("pica.tit", "Title")
            CQLContextSet.Add("pica.per", "Author")
            CQLContextSet.Add("pica.slw", "IndexTerms Classification")
            'CQLContextSet.Add("dc.description", "Abstract")
            CQLContextSet.Add("pica.ser", "Series")
            CQLContextSet.Add("pica.vlg", "Publisher")
            'CQLContextSet.Add("dc.author", "Author")
            CQLContextSet.Add("pica.date", "PublicationTime")
            CQLContextSet.Add("pica.mat", "WorkType DocumentType")
            CQLContextSet.Add("pica.isb", "ISDN")
            'CQLContextSet.Add("dc.source", "Source")
            CQLContextSet.Add("pica.kor", "Institution")


            'DCContextSet.Add("dc.format", "Page")
            'DCContextSet.Add("dc.language", "Language")
            'DCContextSet.Add("dc.coverage", "AboutLocation AboutTime")
            'DCContextSet.Add("bath.any", "")
            'DCContextSet.Add("bath.author", "Author")
            'DCContextSet.Add("bath.conferenceName", "Institution")
            'DCContextSet.Add("bath.corporateAuthor", "Institution")
            'DCContextSet.Add("bath.corporateName", "About")
            'DCContextSet.Add("bath.genreForm", "")
            'DCContextSet.Add("bath.geographicName", "")
            'DCContextSet.Add("bath.isbn", "")
            'DCContextSet.Add("bath.issn", "")
            'DCContextSet.Add("bath.keyTitle", "")
            'DCContextSet.Add("bath.lcCallNumber", "")
            'DCContextSet.Add("bath.lccn", "")
            'DCContextSet.Add("bath.name", "")
            'DCContextSet.Add("bath.notes", "")
            'DCContextSet.Add("bath.personalAuthor", "")
            'DCContextSet.Add("bath.personalName", "")
            'DCContextSet.Add("bath.publisher", "")
            'DCContextSet.Add("bath.publisherNumber", "")
            'DCContextSet.Add("bath.seriesTitle", "")
            'DCContextSet.Add("bath.standardIdentifier", "")
            'DCContextSet.Add("bath.subject", "")
            'DCContextSet.Add("bath.title", "")
            'DCContextSet.Add("bath.topicalSubject", "")
            'DCContextSet.Add("bath.uniformTitle", "")
        End Sub

#End Region


    End Class

End Namespace