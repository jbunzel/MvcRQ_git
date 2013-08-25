Imports RQLib.RQLD

Namespace RQQueryResult.RQDescriptionElements

    Public MustInherit Class ComponentDataClient

        'Protected _classSystem As SubjClass.ClassificationSystems = SubjClass.ClassificationSystems.unknown
        Protected _LDGraph As RQLDGraph = Nothing
        Protected _LDEnabled As Boolean = False


        Public Property LDGraph() As RQLDGraph
            Get
                Return Me._LDGraph
            End Get
            Set(ByVal value As RQLDGraph)
                Me._LDGraph = value
            End Set
        End Property


        'Public Property ClassSystem() As SubjClass.ClassificationSystems
        '    Get
        '        Return Me._classSystem
        '    End Get
        '    Set(ByVal value As SubjClass.ClassificationSystems)
        '        Me._classSystem = value
        '    End Set
        'End Property


        Public Property IsLinkedDataEnabled() As Boolean
            Get
                Return Me._LDEnabled
            End Get
            Set(ByVal value As Boolean)
                Me._LDEnabled = value
            End Set
        End Property


        'Public MustOverride Function GetId(ByVal ComponentName As String) As String


        'Public MustOverride Sub GetData(ByRef Component As RQDescriptionComponent)


        'Public MustOverride Sub PutData(ByRef Component As RQDescriptionComponent)


        'Public MustOverride Sub GetNarrowerClassData(ByVal majClassId As Integer, ByRef classBranch As SubjClassBranch)


        'Public MustOverride Sub GetNarrowerClassData(ByVal majClassId As String, ByRef classBranch As SubjClassBranch)


        'Public MustOverride Function UpdateDocRefs(ByRef classBranch As SubjClassBranch, Optional ByVal iSuperClassDocCount As Integer = 0) As Boolean

    End Class

End Namespace
