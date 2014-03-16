Imports Microsoft.VisualBasic
Imports System.Data
Imports System.IO
Imports System.Xml

Imports RQLib.RQQueryForm
Imports RQLib.Utilities


Namespace RQDAL

    Public Class RQBookmarkDAL

#Region "Private Variables"

        Private _bmDoc As New XmlDocument()
        Private _bmTbl As New RQBookmarkSet.BookmarksDataTable

#End Region


#Region "Public Constructors"

        Public Sub New()
            'Mapping von dir.xml auf DataSet

            'Tabele 0 (directory) Column  0 (directory_Id)
            'Tabele 0 (directory) Column  1 (maxlevel)
            'Tabele 0 (directory) Column  2 (path)
            'Tabele 0 (directory) Column  3 (name)

            'Tabele 1 (folder) Column  0 (folder_Id)
            'Tabele 1 (folder) Column  1 (DocNo)
            'Tabele 1 (folder) Column  2 (name)
            'Tabele 1 (folder) Column  3 (level)
            'Tabele 1 (folder) Column  4 (link)
            'Tabele 1 (folder) Column  5 (type)
            'Tabele 1 (folder) Column  6 (DateChanged)
            'Tabele 1 (folder) Column  7 (comment)
            'Tabele 1 (folder) Column  8 (folder_Id_0)
            'Tabele 1 (folder) Column  9 (directory_Id)

            'Tabele 2 (notation) Column  0 (notation_text)
            'Tabele 2 (notation) Column  1 (file_Id)
            'Tabele 2 (notation) Column  2 (folder_Id)

            'Tabele 3 (filelist) Column  0 (filelist_Id)
            'Tabele 3 (filelist) Column  1 (folder_Id)

            'Tabele 4 (file) Column  0 (file_Id)
            'Tabele 4 (file) Column  1 (DocNo)
            'Tabele 4 (file) Column  2 (name)
            'Tabele 4 (file) Column  3 (type)
            'Tabele 4 (file) Column  4 (link)
            'Tabele 4 (file) Column  5 (DateChanged)
            'Tabele 4 (file) Column  6 (comment)
            'Tabele 4 (file) Column  7 (filelist_Id)
            'Tabele 4 (file) Column  8 (logo)
            'Tabele 4 (file) Column  9 (icon)
            'Tabele 4 (file) Column 10 (folder_Id)

            If File.Exists(System.Web.HttpContext.Current.Server.MapPath("~/xml/dir.xml")) Then
                _bmDoc.Load(System.Web.HttpContext.Current.Server.MapPath("~/xml/dir.xml"))
            End If
        End Sub

#End Region


#Region "Private Methods"

        Private Function SetSortOrder(ByRef SortCriterion As RQquery.SortType, ByRef DirNode As System.Xml.XmlNode) As String
            Dim xmlSortAttb As System.Xml.XmlAttribute = Me._bmDoc.CreateAttribute("Feld30")

            Select Case SortCriterion
                Case RQquery.SortType.ByTitle
                    xmlSortAttb.Value = DirNode.Attributes("name").Value
                Case RQquery.SortType.ByCreationDate
                    xmlSortAttb.Value = "SORT ORDER CREATETIME NOT SUPPORTED"
                Case RQquery.SortType.BySubject
                    Dim strRVKClasses As String = ""
                    Dim notationList As System.Xml.XmlNodeList = DirNode.SelectNodes("notation")
                    Dim j As Integer

                    If notationList.Count > 0 Then
                        For j = 0 To notationList.Count - 1
                            If notationList.Item(j).InnerText.IndexOf(ClassCodePrefix) >= 0 Then
                                strRVKClasses += notationList.Item(j).InnerText.Substring(notationList.Item(j).InnerText.IndexOf(":") + 1).Trim() + ";"
                            End If
                        Next
                        If strRVKClasses <> "" Then
                            xmlSortAttb.Value = strRVKClasses
                        Else
                            xmlSortAttb.Value = "ZZ99999"
                        End If
                    Else
                        xmlSortAttb.Value = "ZZ99999"
                    End If
                Case RQquery.SortType.ByPrimarySubject
                Case RQquery.SortType.ByPublicationDate
                Case RQquery.SortType.ByShelf
                Case RQquery.SortType.undefined
            End Select
            DirNode.Attributes.Append(xmlSortAttb)
            Return xmlSortAttb.Value
        End Function


        Private Function GenerateFolderToC(ByRef theNode As XmlNode) As String
            Dim node As Xml.XmlNode

            GenerateFolderToC = ""
            For Each node In theNode.SelectNodes("filelist/file")
                GenerateFolderToC += node.SelectSingleNode("@name").Value
                GenerateFolderToC += "RQItem="
                GenerateFolderToC += node.SelectSingleNode("@link").Value + "; "
            Next
            For Each node In theNode.SelectNodes("filelist/folder")
                GenerateFolderToC += GenerateFolderToC(node)
            Next
        End Function


        Private Function WriteTableRow(ByRef DirNode As System.Xml.XmlNode, ByVal SortCriterion As RQquery.SortType) As RQBookmarkSet.BookmarksRow
            Dim bmdr As RQBookmarkSet.BookmarksRow = _bmTbl.NewRow()
            Dim notations As XmlNodeList
            Dim notation As XmlNode
            Dim notstring As String = ""

            If SortCriterion <> RQquery.SortType.undefined Then
                bmdr.Feld30 = SetSortOrder(SortCriterion, DirNode)
            Else
                bmdr.Feld30 = ""
            End If
            bmdr.DocNo = DirNode.SelectSingleNode("@DocNo").Value
            bmdr.Title = DirNode.SelectSingleNode("@name").Value
            bmdr.DocTypeCode = DirNode.SelectSingleNode("@type").Value
            bmdr.DocTypeName = DirNode.Name()
            If (DirNode.SelectSingleNode("@type").Value = "url".ToLower()) Then
                bmdr.Signature = DirNode.SelectSingleNode("@link").Value
            End If
            bmdr.CreateTime = DirNode.SelectSingleNode("@DateChanged").Value
            bmdr.Abstract = DirNode.SelectSingleNode("@comment").Value
            If bmdr.DocTypeCode = "folder" Then
                bmdr.Abstract += "$$TOC$$=" + GenerateFolderToC(DirNode)
            End If
            notations = DirNode.SelectNodes("notation")
            For Each notation In notations
                notstring += notation.InnerText + "; "
            Next
            bmdr.Classification = notstring
            Return bmdr
        End Function


        Private Function ConvertNodesToTable(ByRef DirNodes As System.Xml.XmlNodeList, ByRef OutXml As System.Xml.XmlWriter, ByVal SortCriterion As RQquery.SortType) As RQBookmarkSet.BookmarksDataTable
            Dim DirNode As System.Xml.XmlNode

            OutXml.WriteStartElement("Directory")
            OutXml.WriteAttributeString("count", CStr(DirNodes.Count))
            For Each DirNode In DirNodes
                _bmTbl.AddBookmarksRow(WriteTableRow(DirNode, SortCriterion))
                OutXml.WriteNode(New System.Xml.XmlNodeReader(DirNode), True)
            Next
            OutXml.WriteEndElement()
            OutXml.Flush()
            Return _bmTbl
        End Function


        Private Function ConvertNodesToTable(ByRef DirNodes As System.Xml.XmlNodeList, ByVal SortCriterion As RQquery.SortType) As RQBookmarkSet.BookmarksDataTable
            Dim DirNode As System.Xml.XmlNode

            For Each DirNode In DirNodes
                _bmTbl.AddBookmarksRow(WriteTableRow(DirNode, SortCriterion))
            Next
            Return _bmTbl
        End Function


        Private Function GetQueryPathFromClassCode(ByRef ClassCode As String) As String
            Return "//notation[string(.)='" + ClassCode + "']/ancestor::folder | //notation[string(.)='" + ClassCode + "']/ancestor::file"
        End Function


        Private Function GetQueryPathFromClassString(ByRef ClassString As String) As String
            Dim XPathString As String
            Dim lcLexClass As LexicalClass = New LexicalClass(ClassString)
            Dim classes() As String
            Dim i As Integer
            Dim ql As String = ""
            Dim c As String = ""

            XPathString = lcLexClass.Expand("; ", Globals.ClassCodePrefix, "")
            classes = XPathString.Split("; ")
            XPathString = ""
            c = "starts-with(., '"
            For i = 0 To classes.Length - 1
                If classes(i) <> "" Then
                    If ql <> "" Then
                        c += " or starts-with(., '"
                        ql = ""
                    End If
                    c += classes(i).Trim + "')"
                    ql = " | "
                End If
            Next
            XPathString = "//notation[" + c.Trim + "]/ancestor::folder | //notation[" + c.Trim + "]/ancestor::file"
            Return XPathString
        End Function


        Private Function GetQueryPath(ByRef QueryString As String, ByRef QueryType As String, ByRef QueryTermList As ArrayList, ByRef QueryLogic As String, ByRef QueryFields As DataTable) As String
            'Todo: Work on testoutput
            Dim XPathString As String = "/directory/folder"

            If QueryTermList.Count > 0 And QueryFields.Rows.Count > 0 Then
                Dim i As Integer
                Dim c As String = "("
                Dim ql As String = ""
                Dim testRow As DataRow

                XPathString = ""
                For i = 0 To QueryTermList.Count - 1
                    ql = ""
                    testRow = QueryFields.Rows.Find("Title")
                    If Not IsNothing(testRow.Item("Searchfield")) And testRow.Item("Searchfield") = True Then
                        c += "(starts-with(translate(@name,'ABCDEFGHIJKLMNOPQRSTUVWXYZÄÖÜ','abcdefghijklmnopqrstuvwxyzäöü'),'" + CStr(QueryTermList.Item(i)) + "')or contains(translate(@name,'ABCDEFGHIJKLMNOPQRSTUVWXYZÄÖÜ','abcdefghijklmnopqrstuvwxyzäöü'),' " + CStr(QueryTermList.Item(i)) + "'))"
                        ql = " or "
                    End If
                    testRow = QueryFields.Rows.Find("Abstract")
                    If Not IsNothing(testRow.Item("Searchfield")) And testRow.Item("Searchfield") = True Then
                        c += ql + "(starts-with(translate(@comment,'ABCDEFGHIJKLMNOPQRSTUVWXYZÄÖÜ','abcdefghijklmnopqrstuvwxyzäöü'),'" + CStr(QueryTermList.Item(i)) + "')or contains(translate(@comment,'ABCDEFGHIJKLMNOPQRSTUVWXYZÄÖÜ','abcdefghijklmnopqrstuvwxyzäöü'),' " + CStr(QueryTermList.Item(i)) + "'))"
                        ql = " or "
                    End If
                    If i < QueryTermList.Count - 1 Then
                        If QueryLogic = " OR " Then
                            c += ") or ("
                        Else
                            c += ") and ("
                        End If
                    End If
                Next
                c += ")"
                If ql <> "" Then
                    XPathString = "//folder[" + c + "] | //file[" + c + "]"
                End If
                c = ""
                testRow = QueryFields.Rows.Find("Classification")
                If Not IsNothing(testRow.Item("Searchfield")) And testRow.Item("Searchfield") = True Then
                    For i = 0 To QueryTermList.Count - 1
                        If ql <> "" Then
                            c += " | "
                            ql = ""
                        End If
                        c += "//notation[starts-with(.,translate('" + CStr(QueryTermList.Item(i)) + "','abcdefghijklmnopqrstuvwxyzäöü','ABCDEFGHIJKLMNOPQRSTUVWXYZÄÖÜ'))]/ancestor::folder | //notation[starts-with(.,translate('" + CStr(QueryTermList.Item(i)) + "','abcdefghijklmnopqrstuvwxyzäöü','ABCDEFGHIJKLMNOPQRSTUVWXYZÄÖÜ'))]/ancestor::file"
                        ql = " | "
                    Next
                    If ql <> "" Then
                        XPathString += c
                    End If
                End If
            ElseIf QueryType = "recent" Then
                XPathString = "//file[@DateChanged > '" + System.DateTime.Now.Date.AddMonths(-1).ToString("yyyyMMdd") + "']"
            ElseIf QueryType = "browse" Or QueryType = "class" Then
                XPathString = GetQueryPathFromClassCode(QueryString)
            ElseIf QueryType = "bookmarks" Then
                XPathString = "/directory/folder | /directory/folder/filelist/file"
            End If
            Return XPathString
        End Function


        Private Function ReadCommFile(ByVal strTemp As String, ByRef LastCommItems As String()) As String()
            Dim iDx As Integer
            Dim CommItems As Array = Array.CreateInstance(GetType(String), 20)
            Dim i As Integer = 1

            If File.Exists(strTemp) Then
                Dim objStream As New StreamReader(strTemp, System.Text.Encoding.Default)

                strTemp = objStream.ReadToEnd
            Else
                strTemp = ""
            End If
            iDx = InStr(strTemp, "$$CODE$$=")
            If (iDx <> 0) And InStr(strTemp, Globals.ClassCodePrefix) <> 0 Then
                iDx = iDx + Len("$$CODE$$=")
                strTemp = Mid(strTemp, iDx, Len(strTemp) - iDx + 1)
                iDx = InStr(strTemp, ";")

                Dim strClassStr As String = ""

                While (iDx <> 0)
                    strClassStr += Left(strTemp, iDx - 1) + "; "
                    strTemp = Mid(strTemp, iDx + 1, Len(strTemp) - iDx + 1)
                    iDx = InStr(strTemp, ";")
                End While

                Dim clClassStr As New ClassString(strClassStr)

                clClassStr.CompleteClassString()

                For i = 1 To clClassStr.Count
                    CommItems.SetValue(CType(clClassStr.Item(i - 1), Object), i)
                Next
            Else
                i = 1
                If Not IsNothing(LastCommItems) Then
                    While (Not IsNothing(LastCommItems(i))) And (LastCommItems(i) <> "")
                        CommItems.SetValue(LastCommItems(i), i)
                        i = i + 1
                    End While
                End If
            End If
            CommItems.SetValue(strTemp, 0)
            Return CType(CommItems, String())
        End Function


        Private Sub ExtractFileData(ByRef xmlWriter As System.Xml.XmlTextWriter, ByRef strPath As String, ByRef strFileName As String)
            Dim strTemp As String = ""
            Dim iDX As Integer

            xmlWriter.WriteAttributeString("name", Left(strFileName, Len(strFileName) - 4))
            strTemp = LCase(Right(strPath, Len(strPath) - InStrRev(strPath, ".", -1, CompareMethod.Text)))
            xmlWriter.WriteAttributeString("type", strTemp)
            Select Case strTemp
                Case "url"
                    Dim objStream As New StreamReader(strPath)

                    iDX = 0
                    While (iDX = 0 And Not objStream.EndOfStream)
                        strTemp = objStream.ReadLine
                        strTemp.Trim()
                        iDX = InStr(strTemp, "URL=")
                    End While
                    If iDX > 0 Then
                        'KORREKTURPROZEDUR
                        'Die Prozedur ist erforderlich um fehlerhafte url. Dateien zu bereinigen,
                        'die Zeilen nur mit LF und Dateiende mit #00 abschließen. 
                        Dim ch() As Char = CType(strTemp, Char())

                        While ch(strTemp.Length() - 1) = vbNullChar
                            strTemp = strTemp.Substring(0, strTemp.Length() - 1)
                        End While
                        'ENDE KORREKTURPROZEDUR
                        strTemp = Mid(strTemp, iDX + 4, Len(strTemp) - iDX - 3)
                        xmlWriter.WriteAttributeString("link", strTemp)
                    End If
                Case Else
                    xmlWriter.WriteAttributeString("link", strPath)
            End Select
        End Sub


        Private Sub TraverseFolder(ByRef xmlWriter As System.Xml.XmlTextWriter, ByRef strPath As String, ByRef strAdr As String, ByVal iLevel As Integer, ByVal iMaxLevel As Integer, Optional ByRef iItemCount As Integer = 0, Optional ByRef LastCommItems As String() = Nothing)
            Dim objFolder As New DirectoryInfo(strPath)
            Dim objFolderItem As DirectoryInfo
            Dim objFileItem As FileInfo
            Dim iIndex As Integer = 0
            'Dim strTemp, strExt As String
            Dim strTemp As String
            Dim CommItems As String()

            For Each objFileItem In objFolder.GetFiles("*.url")
                'strTemp = objFileItem.Name
                'strExt = LCase(Right(strTemp, Len(strTemp) - InStrRev(strTemp, ".", -1, CompareMethod.Text)))
                'If strExt = "url" Then
                If (iIndex = 0) Then
                    xmlWriter.WriteStartElement("filelist")
                    iIndex = 1
                End If
                xmlWriter.WriteStartElement("file")
                xmlWriter.WriteAttributeString("DocNo", "D" + CStr(iItemCount))
                iItemCount += 1
                'strTemp = LCase(Left(objFileItem.FullName, InStrRev(objFileItem.FullName, ".", -1, CompareMethod.Text) - 1)) + ".gif"
                'If File.Exists(strTemp) Then
                'xmlWriter.WriteAttributeString("logo", HttpContext.Current.Server.UrlEncode(strAdr + "/" + LCase(Left(objFileItem.Name, InStrRev(objFileItem.Name, ".", -1, CompareMethod.Text) - 1)) + ".gif"))
                'End If
                'strTemp = LCase(Left(objFileItem.FullName, InStrRev(objFileItem.FullName, ".", -1, CompareMethod.Text) - 1)) + ".ico"
                'If File.Exists(strTemp) Then
                'xmlWriter.WriteAttributeString("icon", HttpContext.Current.Server.UrlEncode(strAdr + "/" + LCase(Left(objFileItem.Name, InStrRev(objFileItem.Name, ".", -1, CompareMethod.Text) - 1)) + ".ico"))
                'End If
                ExtractFileData(xmlWriter, objFileItem.FullName, objFileItem.Name)
                xmlWriter.WriteAttributeString("DateChanged", objFileItem.LastWriteTime.Date.ToString("yyyyMMdd"))
                strTemp = LCase(Left(objFileItem.FullName, InStrRev(objFileItem.FullName, ".", -1, CompareMethod.Text) - 1)) + "$$COMM$$.txt"
                CommItems = ReadCommFile(strTemp, LastCommItems)
                If Not IsNothing(CommItems) Then
                    If CommItems.Length > 0 Then
                        xmlWriter.WriteAttributeString("comment", CommItems(0))
                    End If
                    If CommItems.Length > 1 Then
                        Dim i As Integer

                        For i = 1 To CommItems.Length - 1
                            If Not IsNothing(CommItems(i)) Then
                                If CommItems(i).Length() > 0 Then
                                    xmlWriter.WriteElementString("notation", CommItems(i))
                                End If
                            Else
                                Exit For
                            End If
                        Next
                    End If
                End If
                xmlWriter.WriteEndElement()
                'End If
            Next
            For Each objFolderItem In objFolder.GetDirectories()
                If InStr(1, objFolderItem.Name, "$COPY$", CompareMethod.Text) = 0 Then
                    xmlWriter.WriteStartElement("folder")
                    xmlWriter.WriteAttributeString("DocNo", "D" + CStr(iItemCount))
                    iItemCount += 1
                    xmlWriter.WriteAttributeString("name", objFolderItem.Name)
                    xmlWriter.WriteAttributeString("level", CStr(iLevel))
                    xmlWriter.WriteAttributeString("link", Web.HttpContext.Current.Server.UrlEncode(strAdr + "/" + objFolderItem.Name))
                    xmlWriter.WriteAttributeString("type", "folder")
                    xmlWriter.WriteAttributeString("DateChanged", objFolderItem.LastWriteTime.Date.ToString("yyyyMMdd"))
                    strTemp = LCase(objFolderItem.FullName) + "$$COMM$$.txt"
                    CommItems = ReadCommFile(strTemp, LastCommItems)
                    If Not IsNothing(CommItems) Then
                        If CommItems.Length > 0 Then
                            xmlWriter.WriteAttributeString("comment", CommItems(0))
                        End If
                        If CommItems.Length > 1 Then
                            Dim i As Integer

                            For i = 1 To CommItems.Length - 1
                                If Not IsNothing(CommItems(i)) Then
                                    If CommItems(i).Length() > 0 Then
                                        xmlWriter.WriteElementString("notation", CommItems(i))
                                    End If
                                Else
                                    Exit For
                                End If
                            Next
                        End If
                    End If
                    If iLevel < iMaxLevel Then
                        TraverseFolder(xmlWriter, strPath + "\" + objFolderItem.Name + "\", strAdr + "/" + objFolderItem.Name, iLevel + 1, iMaxLevel, iItemCount, CommItems)
                    End If
                    xmlWriter.WriteEndElement()
                Else
                    If File.Exists(objFolderItem.FullName + "/" + Left(objFolderItem.Name, InStrRev(objFolderItem.Name, "$$COPY$$", -1, CompareMethod.Text) - 1) + ".htm") Then
                        If (iIndex = 0) Then
                            xmlWriter.WriteStartElement("filelist")
                            iIndex = 1
                        End If
                        xmlWriter.WriteStartElement("file")
                        xmlWriter.WriteAttributeString("DocNo", "D" + CStr(iItemCount))
                        iItemCount += 1
                        ExtractFileData(xmlWriter, objFolderItem.FullName + "/" + Left(objFolderItem.Name, InStrRev(objFolderItem.Name, "$$COPY$$", -1, CompareMethod.Text) - 1) + ".htm", Left(objFolderItem.Name, InStrRev(objFolderItem.Name, "$$COPY$$", -1, CompareMethod.Text) - 1) + ".htm")
                        xmlWriter.WriteAttributeString("DateChanged", objFolderItem.LastWriteTime.Date.ToString("yyyyMMdd"))
                        strTemp = objFolderItem.FullName + "/" + Left(objFolderItem.Name, InStrRev(objFolderItem.Name, "$$COPY$$", -1, CompareMethod.Text) - 1) + "$$COMM$$.txt"
                        CommItems = ReadCommFile(strTemp, LastCommItems)
                        If Not IsNothing(CommItems) Then
                            If CommItems.Length > 0 Then
                                xmlWriter.WriteAttributeString("comment", CommItems(0))
                            End If
                            If CommItems.Length > 1 Then
                                Dim i As Integer

                                For i = 1 To CommItems.Length - 1
                                    If Not IsNothing(CommItems(i)) Then
                                        If CommItems(i).Length() > 0 Then
                                            xmlWriter.WriteElementString("notation", CommItems(i))
                                        End If
                                    End If
                                Next
                            End If
                        End If
                        xmlWriter.WriteEndElement()
                    End If
                End If
            Next
            If (iIndex = 1) Then
                xmlWriter.WriteEndElement()
            End If
        End Sub

        'Private Sub TraverseFolder(ByRef xmlWriter As System.Xml.XmlTextWriter, ByRef strPath As String, ByRef strAdr As String, ByVal iLevel As Integer, ByVal iMaxLevel As Integer, Optional ByRef iItemCount As Integer = 0, Optional ByRef LastCommItems As String() = Nothing)
        '    Dim objFolder As New DirectoryInfo(strPath)
        '    Dim objFolderItem As DirectoryInfo
        '    Dim objFileItem As FileInfo
        '    'Dim WorkFile As File
        '    Dim iIndex As Integer = 0
        '    Dim strTemp, strExt As String
        '    Dim CommItems As String()

        '    For Each objFileItem In objFolder.GetFiles()
        '        strTemp = objFileItem.Name
        '        strExt = LCase(Right(strTemp, Len(strTemp) - InStrRev(strTemp, ".", -1, CompareMethod.Text)))
        '        If strExt = "url" Then
        '            If (iIndex = 0) Then
        '                xmlWriter.WriteStartElement("filelist")
        '                iIndex = 1
        '            End If
        '            xmlWriter.WriteStartElement("file")
        '            xmlWriter.WriteAttributeString("DocNo", "D" + CStr(iItemCount))
        '            iItemCount += 1
        '            strTemp = LCase(Left(objFileItem.FullName, InStrRev(objFileItem.FullName, ".", -1, CompareMethod.Text) - 1)) + ".gif"
        '            If File.Exists(strTemp) Then
        '                'xmlWriter.WriteAttributeString("logo", HttpContext.Current.Server.UrlEncode(strAdr + "/" + LCase(Left(objFileItem.Name, InStrRev(objFileItem.Name, ".", -1, CompareMethod.Text) - 1)) + ".gif"))
        '            End If
        '            strTemp = LCase(Left(objFileItem.FullName, InStrRev(objFileItem.FullName, ".", -1, CompareMethod.Text) - 1)) + ".ico"
        '            If File.Exists(strTemp) Then
        '                'xmlWriter.WriteAttributeString("icon", HttpContext.Current.Server.UrlEncode(strAdr + "/" + LCase(Left(objFileItem.Name, InStrRev(objFileItem.Name, ".", -1, CompareMethod.Text) - 1)) + ".ico"))
        '            End If
        '            ExtractFileData(xmlWriter, objFileItem.FullName, objFileItem.Name)
        '            xmlWriter.WriteAttributeString("DateChanged", objFileItem.LastWriteTime.Date.ToString("yyyyMMdd"))
        '            strTemp = LCase(Left(objFileItem.FullName, InStrRev(objFileItem.FullName, ".", -1, CompareMethod.Text) - 1)) + "$$COMM$$.txt"
        '            CommItems = ReadCommFile(strTemp, LastCommItems)
        '            If Not IsNothing(CommItems) Then
        '                If CommItems.Length > 0 Then
        '                    xmlWriter.WriteAttributeString("comment", CommItems(0))
        '                End If
        '                If CommItems.Length > 1 Then
        '                    Dim i As Integer

        '                    For i = 1 To CommItems.Length - 1
        '                        If Not IsNothing(CommItems(i)) Then
        '                            If CommItems(i).Length() > 0 Then
        '                                xmlWriter.WriteElementString("notation", CommItems(i))
        '                            End If
        '                        End If
        '                    Next
        '                End If
        '            End If
        '            xmlWriter.WriteEndElement()
        '        End If
        '    Next
        '    For Each objFolderItem In objFolder.GetDirectories()
        '        If InStr(1, objFolderItem.Name, "$COPY$", CompareMethod.Text) = 0 Then
        '            xmlWriter.WriteStartElement("folder")
        '            xmlWriter.WriteAttributeString("DocNo", "D" + CStr(iItemCount))
        '            iItemCount += 1
        '            xmlWriter.WriteAttributeString("name", objFolderItem.Name)
        '            xmlWriter.WriteAttributeString("level", CStr(iLevel))
        '            'xmlWriter.WriteAttributeString("link", HttpContext.Current.Server.UrlEncode(strAdr + "/" + objFolderItem.Name))
        '            xmlWriter.WriteAttributeString("type", "folder")
        '            xmlWriter.WriteAttributeString("DateChanged", objFolderItem.LastWriteTime.Date.ToString("yyyyMMdd"))
        '            strTemp = LCase(objFolderItem.FullName) + "$$COMM$$.txt"
        '            CommItems = ReadCommFile(strTemp, LastCommItems)
        '            If Not IsNothing(CommItems) Then
        '                If CommItems.Length > 0 Then
        '                    xmlWriter.WriteAttributeString("comment", CommItems(0))
        '                End If
        '                If CommItems.Length > 1 Then
        '                    Dim i As Integer

        '                    For i = 1 To CommItems.Length - 1
        '                        If Not IsNothing(CommItems(i)) Then
        '                            If CommItems(i).Length() > 0 Then
        '                                xmlWriter.WriteElementString("notation", CommItems(i))
        '                            End If
        '                        End If
        '                    Next
        '                End If
        '            End If
        '            If iLevel < iMaxLevel Then
        '                TraverseFolder(xmlWriter, strPath + "\" + objFolderItem.Name + "\", strAdr + "/" + objFolderItem.Name, iLevel + 1, iMaxLevel, iItemCount, CommItems)
        '            End If
        '            xmlWriter.WriteEndElement()
        '        Else
        '            If File.Exists(objFolderItem.FullName + "/" + Left(objFolderItem.Name, InStrRev(objFolderItem.Name, "$$COPY$$", -1, CompareMethod.Text) - 1) + ".htm") Then
        '                If (iIndex = 0) Then
        '                    xmlWriter.WriteStartElement("filelist")
        '                    iIndex = 1
        '                End If
        '                xmlWriter.WriteStartElement("file")
        '                xmlWriter.WriteAttributeString("DocNo", "D" + CStr(iItemCount))
        '                iItemCount += 1
        '                ExtractFileData(xmlWriter, objFolderItem.FullName + "/" + Left(objFolderItem.Name, InStrRev(objFolderItem.Name, "$$COPY$$", -1, CompareMethod.Text) - 1) + ".htm", Left(objFolderItem.Name, InStrRev(objFolderItem.Name, "$$COPY$$", -1, CompareMethod.Text) - 1) + ".htm")
        '                xmlWriter.WriteAttributeString("DateChanged", objFolderItem.LastWriteTime.Date.ToString("yyyyMMdd"))
        '                strTemp = objFolderItem.FullName + "/" + Left(objFolderItem.Name, InStrRev(objFolderItem.Name, "$$COPY$$", -1, CompareMethod.Text) - 1) + "$$COMM$$.txt"
        '                CommItems = ReadCommFile(strTemp, LastCommItems)
        '                If Not IsNothing(CommItems) Then
        '                    If CommItems.Length > 0 Then
        '                        xmlWriter.WriteAttributeString("comment", CommItems(0))
        '                    End If
        '                    If CommItems.Length > 1 Then
        '                        Dim i As Integer

        '                        For i = 1 To CommItems.Length - 1
        '                            If Not IsNothing(CommItems(i)) Then
        '                                If CommItems(i).Length() > 0 Then
        '                                    xmlWriter.WriteElementString("notation", CommItems(i))
        '                                End If
        '                            End If
        '                        Next
        '                    End If
        '                End If
        '                xmlWriter.WriteEndElement()
        '            End If
        '        End If
        '    Next
        '    If (iIndex = 1) Then
        '        xmlWriter.WriteEndElement()
        '    End If
        'End Sub

#End Region


#Region "Public Methods"

        Public Function GetBookmarkSetByClassCode(ByRef classCode As String, ByRef OutXml As System.Xml.XmlWriter) As RQBookmarkSet.BookmarksDataTable
            Try
                Return ConvertNodesToTable(_bmDoc.DocumentElement.SelectNodes(GetQueryPathFromClassCode(classCode)), OutXml, RQquery.SortType.BySubject)
            Catch ex As Exception
                'Exception: Datenfeld wird von der Datenquelle nicht unterstützt  
                Return Nothing
            End Try
        End Function


        Public Function GetBookmarkSetByClassCode(ByRef classCode As String) As RQBookmarkSet.BookmarksDataTable
            Try
                Return ConvertNodesToTable(_bmDoc.DocumentElement.SelectNodes(GetQueryPathFromClassCode(classCode)), RQquery.SortType.BySubject)
            Catch ex As Exception
                'Exception: Datenfeld wird von der Datenquelle nicht unterstützt  
                Return Nothing
            End Try
        End Function


        Public Function GetBookmarkSetByClassString(ByRef classString As String, ByRef OutXml As System.Xml.XmlWriter) As RQBookmarkSet.BookmarksDataTable
            Try
                Return ConvertNodesToTable(_bmDoc.DocumentElement.SelectNodes(GetQueryPathFromClassString(classString)), OutXml, RQquery.SortType.BySubject)
            Catch ex As Exception
                'Exception: Datenfeld wird von der Datenquelle nicht unterstützt  
                Return Nothing
            End Try
        End Function


        Public Function GetBookmarkSetByClassString(ByRef classString As String) As RQBookmarkSet.BookmarksDataTable
            Try
                Return ConvertNodesToTable(_bmDoc.DocumentElement.SelectNodes(GetQueryPathFromClassString(classString)), RQquery.SortType.BySubject)
            Catch ex As Exception
                'Exception: Datenfeld wird von der Datenquelle nicht unterstützt  
                Return Nothing
            End Try
        End Function


        Public Function GetBookmarkSet(ByRef Query As RQquery, _
                                       ByRef OutXml As System.Xml.XmlWriter) As RQBookmarkSet.BookmarksDataTable
            Try
                Return ConvertNodesToTable(_bmDoc.DocumentElement.SelectNodes(GetQueryPath(Query.QueryString, _
                                                                                           Query.QueryTypeString, _
                                                                                           Query.QueryTermList, _
                                                                                           Query.QueryLogic, _
                                                                                           Query.QueryFieldList)), OutXml, Query.QuerySort)
            Catch ex As Exception
                'Exception: Datenfeld wird von der Datenquelle nicht unterstützt  
                Return Nothing
            End Try
        End Function


        Public Function GetBookmarkSet(ByRef Query As RQquery) As RQBookmarkSet.BookmarksDataTable
            Try
                Return ConvertNodesToTable(_bmDoc.DocumentElement.SelectNodes(GetQueryPath(Query.QueryString, _
                                                                                           Query.QueryTypeString, _
                                                                                           Query.QueryTermList, _
                                                                                           Query.QueryLogic, _
                                                                                           Query.QueryFieldList)), Query.QuerySort)
            Catch ex As Exception
                'Exception: Datenfeld wird von der Datenquelle nicht unterstützt  
                Return Nothing
            End Try
        End Function


        Public Sub LoadBookmarks(ByRef VLDirPath As String, ByRef VLXmlPath As String)
            Dim xmlFileStream As FileStream
            Dim xmlInStream As New System.IO.MemoryStream
            Dim xmlWriter As New System.Xml.XmlTextWriter(xmlInStream, System.Text.Encoding.Default)
            Dim strPath As String = VLDirPath
            Dim strMapPath As String = VLDirPath

            xmlWriter.WriteStartDocument()
            xmlWriter.WriteStartElement("directory")
            xmlWriter.WriteAttributeString("maxlevel", "99")
            xmlWriter.WriteAttributeString("path", strPath)
            xmlWriter.WriteAttributeString("name", Right(Left(strPath, Len(strPath) - 1), Len(strPath) - InStrRev(Left(strPath, Len(strPath) - 1), "/") - 1))
            TraverseFolder(xmlWriter, strMapPath, strPath, 0, 99)
            xmlWriter.WriteEndElement()
            xmlWriter.WriteEndDocument()
            xmlWriter.Flush()
            xmlInStream.Flush()
            Try
                xmlFileStream = New FileStream(Path.Combine(System.Web.HttpRuntime.AppDomainAppPath, VLXmlPath), FileMode.Create)
                xmlInStream.WriteTo(xmlFileStream)
                xmlFileStream.Flush()
                xmlFileStream.Close()
            Catch

            End Try
        End Sub


        Public Function Update() As Integer
            Dim bmtChanges As RQBookmarkSet.BookmarksDataTable = _bmTbl.GetChanges()

            If Not IsNothing(bmtChanges) Then
                Dim cr As RQBookmarkSet.BookmarksRow

                For Each cr In bmtChanges
                    Dim XPathString As String
                    Dim DirNode, notation As XmlNode
                    Dim notations As XmlNodeList
                    Dim classes() As String
                    Dim i As Integer

                    XPathString = "//file[@DocNo = '" + cr.DocNo + "'] | //folder[@DocNo = '" + cr.DocNo + "']"
                    DirNode = _bmDoc.SelectSingleNode(XPathString)
                    DirNode.SelectSingleNode("@name").Value = cr.Title
                    classes = cr.Classification.Split("; ")
                    notations = DirNode.SelectNodes("notation")
                    For i = 0 To classes.Length - 2
                        If Not IsNothing(notations.Item(i)) Then
                            notations.Item(i).InnerXml = classes(i).Trim
                        Else
                            notation = _bmDoc.CreateNode(XmlNodeType.Element, "", "notation", "")
                            notation.InnerXml = classes(i).Trim
                            DirNode.AppendChild(notation)
                        End If
                    Next
                Next
            End If
            Try
                _bmDoc.Save(Web.HttpContext.Current.Server.MapPath("~/xml/dir.xml"))
                Return 0
            Catch ex As Exception
                Return (ex.GetHashCode)
            End Try
            Return 0
        End Function

#End Region

    End Class

End Namespace