Imports Microsoft.VisualBasic
Imports System.Text.RegularExpressions


Namespace RQQueryResult.RQDescriptionElements

    Public Class RQTitle
        Inherits RQDescriptionElement

#Region "Public Constructors"

        Public Sub New()
        End Sub


        Public Sub New(ByVal Content As String)
            MyBase.New(Content)
        End Sub

#End Region


#Region "Public Methods"

        Public Overrides Function SyntaxCheck(ByRef message As String) As Boolean
            Dim testStr As String = MyBase._content
            Dim regexStr As String = "([^=:;/\|]+)(\| ([0-9A-Z]+\. )?[^=:;/.\|]+)*(= ([0-9A-Z]+\. )?[^=:;/.\|]+)?((: ([0-9A-Z]+\. )?[^=:;/.\|]+)(; ([0-9A-Z]+\. )?[^=:;/.\|]+)*)?((/ ([0-9A-Z]+\. )?[^=:;/.\|]+)(; ([0-9A-Z]+\. )?[^=:;/.\|]+)*((\. ([0-9A-Z]+\. )?[^=:;/\|]+)(; ([0-9A-Z]+\. )?[^=:;/\|]+)*)?)?"
            Dim retVal As Boolean = False

            ' TitleField syntax checks & replacements
            If Char.IsWhiteSpace(testStr(testStr.Length - 1)) Then testStr = testStr.Substring(0, testStr.Length - 1)
            If Regex.IsMatch(testStr, " ?: ?") Then
                testStr = Regex.Replace(testStr, " ?: ?", " : ")
            End If
            If Regex.IsMatch(testStr, " ?= ?") Then
                testStr = Regex.Replace(testStr, " ?= ?", " = ")
            End If
            If Regex.IsMatch(testStr, " ?/ ?") Then
                testStr = Regex.Replace(testStr, " ?/ ?", " / ")
            End If
            If Regex.IsMatch(testStr, " ?; ?") Then
                testStr = Regex.Replace(testStr, " ?; ?", " ; ")
            End If
            retVal = Regex.Match(testStr, regexStr).Value = testStr
            If retVal Then MyBase._content = testStr
            Return retVal
        End Function

#End Region

    End Class

End Namespace
