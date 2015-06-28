Imports Microsoft.VisualBasic
Imports System.Web
Imports System.Xml

Namespace Utilities

    Public Class DatabaseManagement

        Public Shared Sub Reindex()
            Dim doc As XmlDocument = New XmlDocument()
            Dim icg As XmlNodeList
            Dim luceneAPI As RQLucene.RQLuceneAPI

            doc.Load(RQLib.glbIndexConfigPath)
            icg = doc.SelectNodes("/indexConfiguration/*[@name='" + RQLib.glbIndexProjectName + "']")

            If IsNothing(icg(0).Attributes("indexFolderPath")) Then
                Dim ifp As XmlAttribute = doc.CreateAttribute("indexFolderPath")

                ifp.Value = HttpContext.Current.Server.MapPath(icg(0).Attributes("indexFolderUrl").Value)
                icg(0).Attributes.Append(ifp)
            End If
            luceneAPI = New RQLucene.RQLuceneAPI(icg)
            luceneAPI.Reindex()
        End Sub

        Public Shared Sub Optimize()
            Dim doc As XmlDocument = New XmlDocument()
            Dim icg As XmlNodeList
            Dim luceneAPI As RQLucene.RQLuceneAPI

            doc.Load(RQLib.glbIndexConfigPath)
            icg = doc.SelectNodes("/indexConfiguration/*[@name='" + RQLib.glbIndexProjectName + "']")

            If IsNothing(icg(0).Attributes("indexFolderPath")) Then
                Dim ifp As XmlAttribute = doc.CreateAttribute("indexFolderPath")

                ifp.Value = HttpContext.Current.Server.MapPath(icg(0).Attributes("indexFolderUrl").Value)
                icg(0).Attributes.Append(ifp)
            End If
            luceneAPI = New RQLucene.RQLuceneAPI(icg)
            luceneAPI.Optimize()
        End Sub
    End Class

End Namespace

