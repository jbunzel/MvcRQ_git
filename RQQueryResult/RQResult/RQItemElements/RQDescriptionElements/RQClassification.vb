Imports Microsoft.VisualBasic
Imports RQLib.RQKos.Classifications


Namespace RQQueryResult.RQDescriptionElements

    '<System.Xml.Serialization.XmlInclude(GetType(ClassificationCode))> _
    Public Class RQClassification
        Inherits RQDescriptionElement
        Implements Collections.IEnumerable

#Region "Private Members"

        '<System.Xml.Serialization.XmlArrayItem(GetType(ClassificationCode), ElementName:="ClassificationCode")> _
        '<System.Xml.Serialization.XmlArray(ElementName:="MyDocumentId")> _
        Private _classCodes() As ClassificationCode = {}

#End Region


#Region "Public Properties"

        Public ReadOnly Property count() As Integer
            Get
                If Not IsNothing(Me._classCodes) Then
                    Return Me._classCodes.Length
                Else
                    Return 0
                End If
            End Get
        End Property


        Public Property items() As ClassificationCode()
            Get
                Return Me._classCodes
            End Get
            Set(ByVal value As ClassificationCode())
                Me._classCodes = value
            End Set
        End Property


        Public Overrides Property Content() As String
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
            MyBase.New(Content)

            Dim splits() As String = {"; ", ";"}
            Dim i As Integer = 0

            For Each c As String In Me._content.Split(splits, StringSplitOptions.RemoveEmptyEntries)
                Array.Resize(Me._classCodes, i + 1)
                Me._classCodes(i) = New ClassificationCode(c)
                i = i + 1
            Next
        End Sub

#End Region


#Region "Public Methods"

        Public Overrides Function SyntaxCheck(ByRef message As String) As Boolean
            Return True
        End Function


        Public Function GetEnumerator() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
            Return New RQClassificationEnum(Me)
        End Function


        Public Function GetItem(ByVal i As Integer) As ClassificationCode
            If i < count Then
                If Not IsNothing(Me._classCodes(i)) Then
                    Return Me._classCodes(i)
                Else
                    Return Nothing
                End If
            End If
            Return Nothing
        End Function


        Public Sub Add(ByVal obj As System.Object)

        End Sub


        Public Overrides Sub WriteXml(ByVal writer As Xml.XmlWriter)
            Dim cc As ClassificationCode
            Dim str As String = ""

            For Each cc In Me._classCodes
                writer.WriteStartElement("ClassificationCode")
                Select Case cc.ClassificationSystem
                    Case ClassificationCode.ClassificationSystems.rq
                        str = "rq"
                    Case ClassificationCode.ClassificationSystems.rvk
                        str = "rvk"
                    Case ClassificationCode.ClassificationSystems.jel
                        str = "jel"
                    Case ClassificationCode.ClassificationSystems.oldrq
                        str = "oldrq"
                    Case Else
                        str = ""
                End Select
                writer.WriteElementString("ClassificationSystem", str)
                writer.WriteElementString("Notation", cc.ClassNotation)
                If cc.ClassificationSystem = ClassificationCode.ClassificationSystems.rq Then
                    Dim cn As ClassificationCode = cc

                    writer.WriteElementString("ClassLabel", cc.ClassLabel)
                    writer.WriteElementString("ClassAltLabel", cc.ClassAltLabel)
                    str = ""
                    Do
                        str = str.Insert(0, "/" + cn.Id + "$" + cn.ClassNotation)
                        cn = cn.GetBroaderClass()
                    Loop Until (IsNothing(cn))
                    writer.WriteElementString("ClassificationPath", str)
                End If
                writer.WriteEndElement()
            Next
        End Sub


        Public Overrides Sub ReadXml(ByVal reader As Xml.XmlReader)

        End Sub


        Public Overrides Function GetSchema() As Xml.Schema.XmlSchema
            Return Nothing
        End Function


#End Region

    End Class


    Public Class RQClassificationEnum
        Implements IEnumerator


#Region "Private Members"

        Private _classification As RQClassification

        ' Enumerators are positioned before the first element
        '        ' until the first MoveNext() call.
        Dim position As Integer = -1

#End Region


#Region "Constructors"

        Public Sub New(ByRef theClass As RQClassification)
            Me._classification = theClass
        End Sub

#End Region


#Region "Public Methods"

        Public Function MoveNext() As Boolean Implements IEnumerator.MoveNext
            position = position + 1
            Return (position < Me._classification.count)
        End Function


        Public Sub Reset() Implements IEnumerator.Reset
            position = -1
        End Sub


        Public ReadOnly Property Current() As Object Implements IEnumerator.Current
            Get
                Try
                    Return Me._classification.GetItem(position)
                Catch ex As IndexOutOfRangeException
                    Throw New InvalidOperationException()
                End Try
            End Get
        End Property

#End Region

    End Class

End Namespace
