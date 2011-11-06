Imports Microsoft.VisualBasic
Imports System.IO
Imports System.Web
'Imports RQStateManager.RQSessionState


Public Module Globals

#Region "Public Members"

    Public glbDataSrc As String = Path.Combine(HttpRuntime.AppDomainAppPath, "~db/katalog30.mdb")
    Public glbVLDirPath As String = "xml/dir.xml"
    Public glbVLDirSrc As String = Path.Combine(HttpRuntime.AppDomainAppPath, glbVLDirPath)
    Public glbMyDocsPath As String = SetMyDocsPath()
    Public glbMyMusicPath As String = SetMyAudioPath()
    Public glbMyVideoPath As String = SetMyVideoPath()

    ''EINSTELLUNGEN LOCALHOST
    ''Public glbMyDocsPath As String = "http://" & HttpContext.Current.Request.ServerVariables("HTTP_HOST") & HttpRuntime.AppDomainAppVirtualPath & "/ItemViewer.aspx?IA=MyDocs"
    ''Public glbMyDocsPath As String = "ftp://192.168.178.1/wdcwd50-00aavs-00ztb0-01/MyDocs/"
    ''Public glbMyMusicPath As String = "ftp://192.168.178.1/wdcwd50-00aavs-00ztb0-01/Audio/"
    ''EINSTELLUNGEN WWW.RIQUEST.DE
    ''Public glbMyDocsPath As String = "http://mydocs.strands.de/MyDocs"
    ''Public glbMyMusicPath As String = "http://www.jbunzel.online.de/MyMusic"

    ''General Globals
    Public ClassCodePrefix As String = CStr(Chr(167)) + CStr(Chr(167)) + "RVK" + CStr(Chr(167)) + CStr(Chr(167)) + ":"
    'Public PageSize As Integer = -1 ' -1 = undefined

    Public Enum BibliographicFormats
        mods
        oai_dc
        srw_dc
        info_ofi
        pubmed
        RQintern
        unknown
    End Enum

#End Region


    Private Function SetMyDocsPath() As String
        If HttpContext.Current.Request.ServerVariables("SERVER_NAME") = "localhost" Then
            Return "http://" & HttpContext.Current.Request.ServerVariables("HTTP_HOST") & HttpRuntime.AppDomainAppVirtualPath & "/ItemViewer/ItemViewer.aspx?IA=MyDocs"
        Else
            Return "http://" & HttpContext.Current.Request.ServerVariables("HTTP_HOST") & HttpRuntime.AppDomainAppVirtualPath & "/ItemViewer/ItemViewer.aspx?IA=MyDocs"
            'Return "http://mydocs.strands.de/MyDocs"
        End If
    End Function


    Private Function SetMyAudioPath() As String
        If HttpContext.Current.Request.ServerVariables("SERVER_NAME") = "localhost" Then
            Return "http://" & HttpContext.Current.Request.ServerVariables("HTTP_HOST") & HttpRuntime.AppDomainAppVirtualPath & "/ItemViewer/ItemViewer.aspx?IA=MyAudio"
        Else
            Return "http://" & HttpContext.Current.Request.ServerVariables("HTTP_HOST") & HttpRuntime.AppDomainAppVirtualPath & "/ItemViewer/ItemViewer.aspx?IA=MyAudio" '"https://sofs.uni-koeln.de/private/jbunzel/Eigene%20Dateien/Eigene%20Musik"
        End If
    End Function


    Private Function SetMyVideoPath() As String
        If HttpContext.Current.Request.ServerVariables("SERVER_NAME") = "localhost" Then
            Return "http://" & HttpContext.Current.Request.ServerVariables("HTTP_HOST") & HttpRuntime.AppDomainAppVirtualPath & "/ItemViewer/ItemViewer.aspx?IA=MyVideo"
        Else
            Return "http://" & HttpContext.Current.Request.ServerVariables("HTTP_HOST") & HttpRuntime.AppDomainAppVirtualPath & "/ItemViewer/ItemViewer.aspx?IA=MyVideo" '"https://sofs.uni-koeln.de/private/jbunzel/Eigene%20Dateien/Eigene%20Musik"
        End If
    End Function

End Module


Public Module EditGlobals
    Public Message As String = ""
End Module

