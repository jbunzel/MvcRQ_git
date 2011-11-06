﻿Imports Microsoft.VisualBasic


Namespace RQStateManager.RQSessionState

    Public Class RQQueryState
        Inherits sm.qs

        Public Property Query() As RQQueryForm.RQquery
            Get
                Return Me._qry
            End Get
            Set(ByVal value As RQQueryForm.RQquery)
                Me._qry = value
            End Set
        End Property


        Public Property DefaultQueryFieldSelection() As Boolean
            Get
                Return Me._dqf
            End Get
            Set(ByVal value As Boolean)
                Me._dqf = value
            End Set
        End Property


        Public Property QueryFieldBoxVisible() As Boolean
            Get
                Return Me._qfb
            End Get
            Set(ByVal value As Boolean)
                Me._qfb = value
            End Set
        End Property


        Public Property ExternalCatalogBoxVisible() As Integer
            Get
                Return Me._ecb
            End Get
            Set(ByVal value As Integer)
                Me._ecb = value
            End Set
        End Property


        Public Sub New(ByVal ID As Object)
            MyBase.New(ID)
        End Sub


        Public Shared Function GetQueryState(ByVal QueryString As String) As RQQueryForm.RQquery
            If IsNothing(QueryState) Then
                If QueryString = "" Then
                    GetQueryState = New RQQueryForm.RQquery("Recent Additions", "recent")
                Else
                    GetQueryState = New RQQueryForm.RQquery(QueryString)
                End If
                QueryState = New RQStateManager.RQSessionState.RQQueryState("qs")
            Else
                If QueryString = "" Then
                    GetQueryState = QueryState.Query
                Else
                    GetQueryState = New RQQueryForm.RQquery(QueryString)
                End If
            End If
            QueryState.Query = GetQueryState
        End Function

    End Class

End Namespace
