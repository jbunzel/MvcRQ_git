Imports Microsoft.VisualBasic
Imports System.Xml

Namespace RQQueryResult.RQDescriptionElements

    Public Class RQDescriptionElement
        Implements Serialization.IXmlSerializable

        Protected _changed As Boolean
        Protected _content As String

#Region "Public Properties"

        Public Property Changed() As Boolean
            Get
                Return _changed
            End Get
            Set(ByVal value As Boolean)
                _changed = value
            End Set
        End Property


        Public Overridable Property Content() As String
            Get
                Return _content
            End Get
            Set(ByVal value As String)
                _content = value
            End Set
        End Property

#End Region


#Region "Public Constructors"

        Public Sub New()
        End Sub


        Public Sub New(ByVal Content As String)
            _content = Content
        End Sub

#End Region


#Region "Public Methods"

        Public Overridable Function SyntaxCheck(ByRef message As String) As Boolean
            Return True
        End Function


        Public Overrides Function ToString() As String
            Return Me.Content
        End Function


        'Public Function Serialize(Optional ByVal IncludeEmptyFields As Boolean = True,
        '                  Optional ByVal ListOnly As Boolean = False,
        '                  Optional ByVal IncludeNamespace As Boolean = False,
        '                  Optional ByVal IncludeSortField As Boolean = True) As XmlTextReader
        '    Dim writer As New XmlTextWriter(New System.IO.MemoryStream, Text.Encoding.UTF8)

        '    'If IncludeNamespace Then
        '    '    writer.WriteStartElement("rq:RQItem")
        '    '    writer.WriteAttributeString("xmlns:rq", "http://riquest.de/formats/rq")
        '    'Else
        '    '    writer.WriteStartElement("RQItem")
        '    'End If
        '    'If ListOnly Then
        '    '    writer.WriteStartElement("rq:ID")
        '    '    writer.WriteString(Me._fields.GetField("ID"))
        '    '    writer.WriteEndElement()
        '    '    writer.WriteStartElement("rq:DocNo")
        '    '    writer.WriteString(Me._fields.GetField("DocNo"))
        '    '    writer.WriteEndElement()
        '    '    writer.WriteStartElement("rq:Title")
        '    '    writer.WriteString(Me._fields.GetField("Title"))
        '    '    writer.WriteEndElement()
        '    '    writer.WriteStartElement("rq:Authors")
        '    '    writer.WriteString(Me._fields.GetField("Authors"))
        '    '    writer.WriteEndElement()
        '    '    writer.WriteStartElement("rq:Locality")
        '    '    writer.WriteString(Me._fields.GetField("Locality"))
        '    '    writer.WriteEndElement()
        '    '    writer.WriteStartElement("rq:PublTime")
        '    '    writer.WriteString(Me._fields.GetField("PublTime"))
        '    '    writer.WriteEndElement()
        '    '    writer.WriteStartElement("rq:DocTypeName")
        '    '    writer.WriteString(Me._fields.GetField("DocTypeName"))
        '    '    writer.WriteEndElement()
        '    'Else
        '    '    Dim pis As System.Reflection.PropertyInfo() = Me._fields.GetType().GetProperties()
        '    '    Dim pi As System.Reflection.PropertyInfo

        '    '    For Each pi In pis
        '    '        If Not String.IsNullOrEmpty(Me._fields.GetField(pi.Name).Trim) Then
        '    '            If Not IncludeNamespace Then
        '    '                writer.WriteStartElement(pi.Name)
        '    '            Else
        '    '                writer.WriteStartElement("rq:" + pi.Name)
        '    '            End If
        '    '            writer.WriteString(Me._fields.GetField(pi.Name))
        '    '            writer.WriteEndElement()
        '    '        ElseIf IncludeEmptyFields Then
        '    '            If Not IncludeNamespace Then
        '    '                writer.WriteStartElement(pi.Name)
        '    '            Else
        '    '                writer.WriteStartElement("rq:" + pi.Name)
        '    '            End If
        '    '            writer.WriteEndElement()
        '    '        End If
        '    '    Next
        '    '    If IncludeSortField Then
        '    '        writer.WriteStartElement("Feld30")
        '    '        writer.WriteString(Me._sortField)
        '    '        writer.WriteEndElement()
        '    '    End If
        '    'End If
        '    'writer.WriteEndElement()
        '    'writer.Flush()
        '    'writer.BaseStream.Seek(0, IO.SeekOrigin.Begin)
        '    'Serialize = New XmlTextReader(writer.BaseStream)
        'End Function


        Public Overridable Sub WriteXml(ByVal writer As Xml.XmlWriter) Implements Serialization.IXmlSerializable.WriteXml
        End Sub


        Public Overridable Sub ReadXml(ByVal reader As Xml.XmlReader) Implements Serialization.IXmlSerializable.ReadXml
        End Sub


        Public Overridable Function GetSchema() As Xml.Schema.XmlSchema Implements Serialization.IXmlSerializable.GetSchema
            Return Nothing
        End Function

#End Region


    End Class

End Namespace
