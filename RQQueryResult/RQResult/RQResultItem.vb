Imports Microsoft.VisualBasic
Imports System.IO
Imports System.Xml
Imports System.Collections.Generic
Imports System.Collections.Specialized

Imports RQLib.RQDAL
'Imports RQDigitalObject
Imports RQLib.RQQueryResult.RQDescriptionElements

Namespace RQQueryResult

    Public Class RQItemDescription

#Region "Private Members"

        Private _fields As OrderedDictionary
        Private _changed As Boolean = False

#End Region


#Region "Public Properties"

        Public Property ID() As String
            Get
                Return Me.GetField("ID")
            End Get
            Set(ByVal value As String)
                Me.SetField("ID", value)
            End Set
        End Property


        Public Property DocNo() As String
            Get
                Return Me.GetField("DocNo")
            End Get
            Set(ByVal value As String)
                Me.SetField("DocNo", value)
            End Set
        End Property


        Public Property Title() As String
            Get
                Return Me.GetField("Title")
            End Get
            Set(ByVal value As String)
                Me.SetField("Title", value)
            End Set
        End Property


        Public Property Authors() As String
            Get
                Return Me.GetField("Authors")
            End Get
            Set(ByVal value As String)
                Me.SetField("Authors", value)
            End Set
        End Property


        Public Property Source() As String
            Get
                Return Me.GetField("Source")
            End Get
            Set(ByVal value As String)
                Me.SetField("Source", value)
            End Set
        End Property


        Public Property Institutions() As String
            Get
                Return Me.GetField("Institutions")
            End Get
            Set(ByVal value As String)
                Me.SetField("Institutions", value)
            End Set
        End Property


        Public Property Series() As String
            Get
                Return Me.GetField("Series")
            End Get
            Set(ByVal value As String)
                Me.SetField("Series", value)
            End Set
        End Property


        Public Property IndexTerms() As String
            Get
                Return Me.GetField("IndexTerms")
            End Get
            Set(ByVal value As String)
                Me.SetField("IndexTerms", value)
            End Set
        End Property


        Public Property Subjects() As String
            Get
                Return Me.GetField("Subjects")
            End Get
            Set(ByVal value As String)
                Me.SetField("Subjects", value)
            End Set
        End Property


        Public Property AboutPersons() As String
            Get
                Return Me.GetField("AboutPersons")
            End Get
            Set(ByVal value As String)
                Me.SetField("AboutPersons", value)
            End Set
        End Property


        Public Property Abstract() As String
            Get
                Return Me.GetField("Abstract")
            End Get
            Set(ByVal value As String)
                Me.SetField("Abstract", value)
            End Set
        End Property


        Public Property Edition() As String
            Get
                Return Me.GetField("Edition")
            End Get
            Set(ByVal value As String)
                Me.SetField("Edition", value)
            End Set
        End Property


        Public Property ISDN() As String
            Get
                Return Me.GetField("ISDN")
            End Get
            Set(ByVal value As String)
                Me.SetField("ISDN", value)
            End Set
        End Property


        Public Property Coden() As String
            Get
                Return Me.GetField("Coden")
            End Get
            Set(ByVal value As String)
                Me.SetField("Coden", value)
            End Set
        End Property


        Public Property Locality() As String
            Get
                Return Me.GetField("Locality")
            End Get
            Set(ByVal value As String)
                Me.SetField("Locality", value)
            End Set
        End Property


        Public Property Publisher() As String
            Get
                Return Me.GetField("Publisher")
            End Get
            Set(ByVal value As String)
                Me.SetField("Publisher", value)
            End Set
        End Property


        Public Property PublTime() As String
            Get
                Return Me.GetField("PublTime")
            End Get
            Set(ByVal value As String)
                Me.SetField("PublTime", value)
            End Set
        End Property


        Public Property Volume() As String
            Get
                Return Me.GetField("Volume")
            End Get
            Set(ByVal value As String)
                Me.SetField("Volume", value)
            End Set
        End Property


        Public Property Issue() As String
            Get
                Return Me.GetField("Issue")
            End Get
            Set(ByVal value As String)
                Me.SetField("Issue", value)
            End Set
        End Property


        Public Property Pages() As String
            Get
                Return Me.GetField("Pages")
            End Get
            Set(ByVal value As String)
                Me.SetField("Pages", value)
            End Set
        End Property


        Public Property Language() As String
            Get
                Return Me.GetField("Language")
            End Get
            Set(ByVal value As String)
                Me.SetField("Language", value)
            End Set
        End Property


        Public Property Signature() As String
            Get
                Return Me.GetField("Signature")
            End Get
            Set(ByVal value As String)
                Me.SetField("Signature", value)
            End Set
        End Property


        Public Property DocTypeCode() As String
            Get
                Return Me.GetField("DocTypeCode")
            End Get
            Set(ByVal value As String)
                Me.SetField("DocTypeCode", value)
            End Set
        End Property


        Public Property DocTypeName() As String
            Get
                Return Me.GetField("DocTypeName")
            End Get
            Set(ByVal value As String)
                Me.SetField("DocTypeName", value)
            End Set
        End Property


        Public Property WorkType() As String
            Get
                Return Me.GetField("WorkType")
            End Get
            Set(ByVal value As String)
                Me.SetField("WorkType", value)
            End Set
        End Property


        Public Property AboutLocation() As String
            Get
                Return Me.GetField("AboutLocation")
            End Get
            Set(ByVal value As String)
                Me.SetField("AboutLocation", value)
            End Set
        End Property


        Public Property AboutTime() As String
            Get
                Return Me.GetField("AboutTime")
            End Get
            Set(ByVal value As String)
                Me.SetField("AboutTime", value)
            End Set
        End Property


        Public Property CreateLocation() As String
            Get
                Return Me.GetField("CreateLocation")
            End Get
            Set(ByVal value As String)
                Me.SetField("CreateLocation", value)
            End Set
        End Property


        Public Property CreateTime() As String
            Get
                Return Me.GetField("CreateTime")
            End Get
            Set(ByVal value As String)
                Me.SetField("CreateTime", value)
            End Set
        End Property


        Public Property Notes() As String
            Get
                Return Me.GetField("Notes")
            End Get
            Set(ByVal value As String)
                Me.SetField("Notes", value)
            End Set
        End Property


        Public Property Classification() As String
            Get
                Return Me.GetField("Classification")
            End Get
            Set(ByVal value As String)
                Me.SetField("Classification", value)
            End Set
        End Property

#End Region


#Region "Public Constructors"

        Public Sub New()
            _fields = New OrderedDictionary
        End Sub

#End Region


#Region "Friend Methods"

        Friend Function GetField(ByVal name As String) As String
            If Not IsNothing(Me._fields(name)) Then
                Return CType(Me._fields(name), RQDescriptionElement).Content
            Else
                Return Nothing
            End If
        End Function


        Friend Sub SetField(ByVal name As String, ByVal value As String)
            If Not IsNothing(Me._fields(name)) Then
                If value <> CType(Me._fields(name), RQDescriptionElement).Content Then
                    CType(Me._fields(name), RQDescriptionElement).Content = value
                    CType(Me._fields(name), RQDescriptionElement).Changed = True
                    Me._changed = True
                End If
            Else
                Select Case name
                    Case "Title"
                        Me._fields.Add(name, New RQDescriptionElements.RQTitle(value))
                    Case "Classification"
                        Me._fields.Add(name, New RQDescriptionElements.RQClassification(value))
                    Case Else
                        Me._fields.Add(name, New RQDescriptionElement(value))
                End Select
            End If
        End Sub


        Friend Sub SetChangedFlag()
            Me._changed = True
        End Sub

#End Region


#Region "Public Methods"

        Public Function HasChanged() As Boolean
            Return Me._changed
        End Function


        Public Function SyntaxCheck(ByRef message As String) As Boolean
            Dim pis As System.Reflection.PropertyInfo() = Me.GetType().GetProperties()
            Dim retVal As Boolean = True
            Dim pi As System.Reflection.PropertyInfo

            For Each pi In pis
                If Not IsNothing(Me._fields(pi.Name)) Then
                    retVal = retVal And CType(Me._fields(pi.Name), RQDescriptionElement).SyntaxCheck(message)
                End If
            Next
            Return retVal
        End Function

#End Region

    End Class


    Public Class RQResultItem

#Region "Private Variables"

        Private _owner As RQResultSet
        Private _type As RQItemType = RQItemType.void
        Private _itemsource As RQItemSource
        Private _fields As RQItemDescription
        Private _sortField As String

#End Region


#Region "Public Variables"

        Public Enum RQItemType
            docdesc
            bookmark
            undefined
            void
        End Enum


        Public Enum RQItemSource
            local
            external
        End Enum

#End Region


#Region "Public Properties"

        Public ReadOnly Property RQResultItemChanged() As Boolean
            Get
                Return Me._fields.HasChanged
            End Get
        End Property


        Public ReadOnly Property RQResultItemType() As RQItemType
            Get
                Return Me._type
            End Get
        End Property


        Public ReadOnly Property ItemDescription As RQItemDescription
            Get
                Return Me._fields
            End Get
        End Property


        Public Property RQResultItemOwner() As RQResultSet
            Get
                Return _owner
            End Get
            Set(ByVal value As RQResultSet)
                _owner = value
            End Set
        End Property

#End Region


#Region "Public Constructors"

        Public Sub New()
            Me._fields = New RQItemDescription()
        End Sub


        Public Sub New(ByRef type As RQItemType)
            Me.New()
            Me._type = type
        End Sub


        'Public Sub New(ByRef resultDoc As System.Xml.XmlDocument, ByRef index As Integer)
        '    Me.New()
        '    Me.Read(resultDoc, index)
        'End Sub

#End Region


#Region "Private Methods"

        Private Sub CopyDocDescription(ByRef fromItem As RQResultItem)
            Dim pis As System.Reflection.PropertyInfo() = Me._fields.GetType().GetProperties()
            Dim pi As System.Reflection.PropertyInfo

            For Each pi In pis
                If Not IsNothing(fromItem._fields.GetField(pi.Name)) Then
                    Me._fields.SetField(pi.Name, fromItem._fields.GetField(pi.Name))
                End If
            Next
        End Sub


        Private Sub ReadDocDescription(ByVal fromRow As RQDataSet.DokumenteRow)
            Dim pis As System.Reflection.PropertyInfo() = Me._fields.GetType().GetProperties()
            Dim pi As System.Reflection.PropertyInfo

            For Each pi In pis
                If fromRow.Table.Columns.Contains(pi.Name) Then
                    If IsDBNull(fromRow.Item(pi.Name)) Then
                        Me._fields.SetField(pi.Name, "")
                    Else
                        Me._fields.SetField(pi.Name, fromRow.Item(pi.Name))
                    End If
                End If
            Next
        End Sub


        Private Sub ReadDocDescription(ByVal fromResultDocList As System.Xml.XmlDocument, ByVal index As Integer)
            Dim pis As System.Reflection.PropertyInfo() = Me._fields.GetType().GetProperties()
            Dim pi As System.Reflection.PropertyInfo

            For Each pi In pis
                If Not IsNothing(fromResultDocList.SelectNodes("//Dokument").Item(index).Item(pi.Name)) Then
                    Dim field As String = fromResultDocList.SelectNodes("//Dokument").Item(index).Item(pi.Name).InnerText()

                    If Not IsNothing(field) Then
                        Me._fields.SetField(pi.Name, field)
                    End If
                End If
            Next
        End Sub


        'Private Sub ReadDocDescription(ByVal fromDigitalObject As DigitalObject)
        '    Dim pis As System.Reflection.PropertyInfo() = Me._fields.GetType().GetProperties()
        '    Dim pi As System.Reflection.PropertyInfo

        '    For Each pi In pis
        '        Dim field As String = CType(fromDigitalObject, StructuredDigitalObject).MapDescriptionElement(pi.Name)

        '        If Not IsNothing(field) Then
        '            Me._fields.SetField(pi.Name, field)
        '        End If
        '    Next

        'End Sub


        Private Sub ReadDocDescription(ByVal dr As RQBookmarkSet.BookmarksRow)
            Dim pis As System.Reflection.PropertyInfo() = Me._fields.GetType().GetProperties()
            Dim pi As System.Reflection.PropertyInfo

            For Each pi In pis
                If dr.Table.Columns.Contains(pi.Name) Then
                    If IsDBNull(dr.Item(pi.Name)) Then
                        Me._fields.SetField(pi.Name, "")
                    Else
                        Me._fields.SetField(pi.Name, dr.Item(pi.Name))
                    End If
                End If
            Next
        End Sub


        Private Sub WriteDocDescription(ByRef dr As RQDataSet.DokumenteRow)
            Dim pis As System.Reflection.PropertyInfo() = Me._fields.GetType().GetProperties()
            Dim pi As System.Reflection.PropertyInfo

            For Each pi In pis
                If pi.Name <> "ID" And dr.Table.Columns.Contains(pi.Name) Then
                    dr.Item(pi.Name) = Me._fields.GetField(pi.Name)
                End If
            Next
        End Sub


        Private Sub WriteDocDescription(ByRef dr As RQBookmarkSet.BookmarksRow)
            Dim pis As System.Reflection.PropertyInfo() = Me._fields.GetType().GetProperties()
            Dim pi As System.Reflection.PropertyInfo

            For Each pi In pis
                If dr.Table.Columns.Contains(pi.Name) Then
                    dr.Item(pi.Name) = Me._fields.GetField(pi.Name)
                End If
            Next
        End Sub


        Private Sub WriteDocDescription(ByRef strdict As StringDictionary)
            Dim pis As System.Reflection.PropertyInfo() = Me._fields.GetType().GetProperties()
            Dim pi As System.Reflection.PropertyInfo

            For Each pi In pis
                strdict.Add(pi.Name, Me._fields.GetField(pi.Name))
            Next
        End Sub


#End Region


#Region "Public Methods"

        Public Sub Read(ByRef fromRow As Data.DataRow)
            If fromRow.GetType.Name = "DokumenteRow" Then
                Dim dr As RQDataSet.DokumenteRow = CType(fromRow, RQDataSet.DokumenteRow)

                Me._type = RQItemType.docdesc
                Me.ReadDocDescription(dr)
                Me._sortField = dr.Feld30.ToString()
            End If
            If fromRow.GetType.Name = "BookmarksRow" Then
                Dim br As RQBookmarkSet.BookmarksRow = CType(fromRow, RQBookmarkSet.BookmarksRow)

                Me._type = RQItemType.bookmark
                Me.ReadDocDescription(br)
                Me._sortField = br.Feld30.ToString()
            End If
        End Sub


        Public Sub Read(ByRef fromResultDocList As System.Xml.XmlDocument, ByRef index As Integer)
            If Not IsNothing(fromResultDocList.SelectNodes("//Dokument").Item(index - 1).Item("ID")) Then
                Dim strId As String = fromResultDocList.SelectNodes("//Dokument").Item(index - 1).Item("ID").InnerText

                If Not String.IsNullOrEmpty(strId) Then
                    Me.ReadDocDescription(CType(New RQCatalogDAL().GetRecordByID(strId, "RQDataSet", "Dokumente"), RQDataSet.DokumenteRow))
                    Me._type = RQItemType.docdesc
                Else
                    Me.ReadDocDescription(fromResultDocList, index - 1)
                    Me._type = RQItemType.docdesc
                End If
            End If
        End Sub


        'Public Sub Read(ByRef fromDigitalObject As DigitalObject, ByRef index As Integer, Optional ByRef containerItem As RQResultItem = Nothing)
        '    If (index > 0) And Not IsNothing(containerItem) Then
        '        'Set default description elements from the root of the digital object structure 
        '        Dim ndo As StructuredDigitalObject = CType(fromDigitalObject, StructuredDigitalObject).SelectElement(index)

        '        Me.CopyDocDescription(containerItem)
        '        Me.ReadDocDescription(ndo)
        '    Else
        '        Me.ReadDocDescription(fromDigitalObject)
        '    End If
        'End Sub


        Public Sub Write(ByRef row As Data.DataRow)
            If row.GetType.Name = "DokumenteRow" Then
                Dim dr As RQDataSet.DokumenteRow = CType(row, RQDataSet.DokumenteRow)

                WriteDocDescription(dr)
                row = dr
            End If
            If row.GetType.Name = "BookmarksRow" Then
                Dim br As RQBookmarkSet.BookmarksRow = CType(row, RQBookmarkSet.BookmarksRow)

                WriteDocDescription(br)
                row = br
            End If
        End Sub


        Public Function Write() As StringDictionary
            Dim retval As New StringDictionary()

            Me.WriteDocDescription(retval)
            Return retval
        End Function


        Public Function ConvertTo(ByVal format As BibliographicFormats, Optional ByVal IncludeEmptyFields As Boolean = True,
                                                                        Optional ByVal ListOnly As Boolean = False,
                                                                        Optional ByRef IncludeNamespace As Boolean = False,
                                                                        Optional ByRef IncludeSortField As Boolean = True) As XmlTextReader
            Dim writer As New XmlTextWriter(New System.IO.MemoryStream, Text.Encoding.UTF8)

            Select Case format
                Case BibliographicFormats.info_ofi
                Case BibliographicFormats.mods
                Case BibliographicFormats.oai_dc
                Case BibliographicFormats.pubmed
                Case BibliographicFormats.srw_dc
                Case BibliographicFormats.RQintern
                    writer.WriteStartElement("RQItem")
                    If IncludeNamespace Then writer.WriteAttributeString("xmlns:rq", "http://www.riquest.de/formats/rq")
                    If ListOnly Then
                        writer.WriteStartElement("rq:ID")
                        writer.WriteString(Me._fields.GetField("ID"))
                        writer.WriteEndElement()
                        writer.WriteStartElement("rq:DocNo")
                        writer.WriteString(Me._fields.GetField("DocNo"))
                        writer.WriteEndElement()
                        writer.WriteStartElement("rq:Title")
                        writer.WriteString(Me._fields.GetField("Title"))
                        writer.WriteEndElement()
                        writer.WriteStartElement("rq:Authors")
                        writer.WriteString(Me._fields.GetField("Authors"))
                        writer.WriteEndElement()
                        writer.WriteStartElement("rq:Locality")
                        writer.WriteString(Me._fields.GetField("Locality"))
                        writer.WriteEndElement()
                        writer.WriteStartElement("rq:PublTime")
                        writer.WriteString(Me._fields.GetField("PublTime"))
                        writer.WriteEndElement()
                        writer.WriteStartElement("rq:DocTypeName")
                        writer.WriteString(Me._fields.GetField("DocTypeName"))
                        writer.WriteEndElement()
                    Else
                        Dim pis As System.Reflection.PropertyInfo() = Me._fields.GetType().GetProperties()
                        Dim pi As System.Reflection.PropertyInfo

                        For Each pi In pis
                            If Not String.IsNullOrEmpty(Me._fields.GetField(pi.Name).Trim) Then
                                If Not IncludeNamespace Then
                                    writer.WriteStartElement(pi.Name)
                                Else
                                    writer.WriteStartElement("rq:" + pi.Name)
                                End If
                                writer.WriteString(Me._fields.GetField(pi.Name))
                                writer.WriteEndElement()
                            ElseIf IncludeEmptyFields Then
                                If Not IncludeNamespace Then
                                    writer.WriteStartElement(pi.Name)
                                Else
                                    writer.WriteStartElement("rq:" + pi.Name)
                                End If
                                writer.WriteEndElement()
                            End If
                        Next
                        If IncludeSortField Then
                            writer.WriteStartElement("Feld30")
                            writer.WriteString(Me._sortField)
                            writer.WriteEndElement()
                        End If
                    End If
                    writer.WriteEndElement()
                Case BibliographicFormats.unknown
                Case Else
            End Select
            writer.Flush()
            writer.BaseStream.Seek(0, IO.SeekOrigin.Begin)
            ConvertTo = New XmlTextReader(writer.BaseStream)
        End Function


        Public Sub SetChanged()
            Me._fields.SetChangedFlag()
        End Sub

#End Region

    End Class

End Namespace
