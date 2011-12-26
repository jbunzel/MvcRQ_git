Imports Microsoft.VisualBasic
Imports System.Xml


Namespace RQKos.Classifications

    Public Class ClassificationCode

        Public Enum ClassificationSystems
            rvk
            jel
            rq
            oldrq
            unkown
        End Enum


        Public Enum ClassificationPredicates
            preferred_label
            alternative_label
            broader_term
            narrower_term
            class_notation
        End Enum


#Region "Private Members"

        Private _classSystem As ClassificationSystems = ClassificationSystems.unkown
        Private _localName As String = ""
        Private _localNameSpace As String = ""
        Private _class As SubjClass
        Private _broaderClass As ClassificationCode = Nothing
        Private _narrowerClasses() As ClassificationCode

#End Region


#Region "Public Properties"

        Public Property Id() As String
            Get
                Return Me._class.ClassID
            End Get
            Set(ByVal value As String)
                Me._class.ClassID = value
            End Set
        End Property


        Public Property LocalName() As String
            Get
                Return Me._localName
            End Get
            Set(ByVal value As String)
                Me._localName = value
                If value.IndexOf("§§") >= 0 And value.IndexOf(":") < 0 Then
                    ' Old RQ Classification of type "§§4"
                    LocalNameSpace = "§§"
                    If Me._class.ClassCode = "" Then
                        ClassNotation = value.Remove(0, 2).Trim()
                    End If
                Else
                    LocalNameSpace = value.Substring(0, value.IndexOf(":") - value.IndexOf("§§"))
                    If Me._class.ClassCode = "" Then
                        If (value.IndexOf(":") - value.IndexOf("§§")) > 0 Then
                            ClassNotation = value.Remove(0, value.IndexOf(":") - value.IndexOf("§§") + 1)
                        Else
                            ClassNotation = Me._localName
                        End If
                    End If
                End If
            End Set
        End Property


        Public Property LocalNameSpace() As String
            Get
                Return Me._localNameSpace
            End Get
            Set(ByVal value As String)
                Me._localNameSpace = value
                Select Case value
                    Case "§§RVK§§"
                        Me.ClassificationSystem = ClassificationSystems.rvk
                    Case "§§JEL§§"
                        Me.ClassificationSystem = ClassificationSystems.jel
                    Case "§§"
                        Me.ClassificationSystem = ClassificationSystems.oldrq
                    Case ""
                        Me.ClassificationSystem = ClassificationSystems.rq
                    Case Else
                        Me.ClassificationSystem = ClassificationSystems.unkown
                End Select
            End Set
        End Property


        Public Property ClassificationSystem() As ClassificationSystems
            Get
                Return Me._classSystem
            End Get
            Set(ByVal value As ClassificationSystems)
                Me._classSystem = value
            End Set
        End Property


        Public Property ClassNotation() As String
            Get
                Return Me._class.ClassCode
            End Get
            Set(ByVal value As String)
                Me._class.ClassCode = value
                If Me.LocalName = "" Then
                    Select Case Me._classSystem
                        Case ClassificationSystems.rvk
                            Me.LocalNameSpace = "§§RVK§§"
                            Me.LocalName = Me.LocalNameSpace + ":" + Me._class.ClassCode.Substring(Me._class.ClassCode.LastIndexOf("/") + 1).Replace("_", "")
                        Case ClassificationSystems.jel
                            'NOP
                        Case ClassificationSystems.rq
                            'NOP
                        Case ClassificationSystems.oldrq
                            'NOP
                        Case Else
                            'NOP
                    End Select
                End If
            End Set
        End Property


        Public Property ClassLabel() As String
            Get
                Return Me._class.ClassShortTitle
            End Get
            Set(ByVal value As String)
                Me._class.ClassShortTitle = value
            End Set
        End Property


        Public Property ClassAltLabel() As String
            Get
                Return Me._class.ClassLongTitle
            End Get
            Set(ByVal value As String)
                Me._class.ClassLongTitle = value
            End Set
        End Property


        Public Property ParentClassID() As String
            Get
                Return Me._class.ParentClassID
            End Get
            Set(ByVal value As String)
                Me._class.ParentClassID = value
            End Set
        End Property

#End Region


#Region "Private Methods"

        Private Sub GetClassData()
            'If Me.ClassLabel = "" Then
            '    If IsNothing(Me._classDataClient) Then Me._classDataClient = New ClassificationDataClient()
            '    Me.ClassLabel = Me._classDataClient.GetPrefLabel(Me.ClassificationSystem, Me.ClassNotation)
            '    Me.ClassAltLabel = Me._classDataClient.GetAltLabel(Me.ClassificationSystem, Me.ClassNotation)
            'End If
        End Sub


        Private Sub GetClassHierarchy()
            Me._broaderClass = GetBroaderClass()

            If Not IsNothing(Me._broaderClass) Then
                Me._broaderClass.GetClassHierarchy()
            End If
        End Sub


        Private Sub GetBroaderClassInformation(ByRef theClass As ClassificationCode, ByRef writer As XmlTextWriter)
            If (Not IsNothing(theClass._broaderClass)) Then
                writer.WriteStartElement("BroaderClass")
                writer.WriteStartElement("class")
                AppendClassInformation(theClass._broaderClass, writer)
                theClass._broaderClass.GetBroaderClassInformation(theClass._broaderClass, writer)
                writer.WriteEndElement()
                writer.WriteEndElement()
            End If
        End Sub


        Private Sub AppendClassInformation(ByRef theClass As ClassificationCode, ByRef writer As XmlTextWriter)
            writer.WriteElementString("LocalName", theClass._localName.Replace(theClass._localNameSpace + ":", ""))
            writer.WriteElementString("Label", theClass._class.ClassShortTitle)
        End Sub

#End Region


#Region "Public Constructors"

        Public Sub New()
            Me._class = New SubjClass()
        End Sub


        Public Sub New(ByVal localName As String)
            Me.New()
            Me.LocalName = localName
        End Sub


        Sub New(ByVal classificationSystems As ClassificationSystems)
            Me.New()
            _classSystem = classificationSystems
        End Sub

#End Region


#Region "Public Methods"

        Public Sub Augment()
            'If IsNothing(Me._classDataClient) Then Me._classDataClient = New ClassificationDataClient()
            'Me.GetClassData()
            'Me.GetClassHierarchy()
        End Sub


        Public Function GetBroaderClass() As ClassificationCode
            If IsNothing(Me._broaderClass) Then
                If Me.ParentClassID <> "-1" And Me.ParentClassID <> "0" Then
                    Me._broaderClass = New ClassificationCode()
                    Me._broaderClass.Id = Me.ParentClassID
                Else
                    Return Nothing
                End If
            End If
            Return Me._broaderClass
        End Function


        Public Function GetCompleteClassInformation() As XmlTextWriter
            Dim stream As New System.IO.MemoryStream()
            Dim writer As New XmlTextWriter(stream, System.Text.Encoding.Default)

            Me.Augment()
            writer.WriteStartDocument()
            writer.WriteStartElement("Classes")
            writer.WriteStartElement("class")
            Me.AppendClassInformation(Me, writer)
            Me.GetBroaderClassInformation(Me, writer)
            writer.WriteEndElement()
            writer.WriteEndElement()
            writer.WriteEndDocument()
            writer.Flush()
            Return writer
        End Function

#End Region

    End Class

End Namespace
