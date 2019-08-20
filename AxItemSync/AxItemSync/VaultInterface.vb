Imports Autodesk.Connectivity.Explorer.Extensibility
Imports Autodesk.Connectivity.WebServicesTools
Imports Autodesk.Connectivity.WebServices
Imports Autodesk.DataManagement.Client.Framework.Vault.Currency.Connections
Imports Autodesk.Connectivity.Extensibility.Framework
Imports System.Windows.Interop
Imports System.Text
Imports System.Windows.Forms.Integration





' Make sure to generate your own ID when writing your own extension. 
<Assembly: Autodesk.Connectivity.Extensibility.Framework.ExtensionId("61e8ffb2-a7c9-4e8a-bbeb-7f273ff64c4a")> 

' This number gets incremented for each Vault release.
<Assembly: Autodesk.Connectivity.Extensibility.Framework.ApiVersion("9.0")> 

Namespace AXItemsSync

    Public Class CommandExtension
        Implements IExplorerExtension

        Public vConnection As Connection
        '    Dim cred As UserIdTicketCredentials
        Public Shared _remoteHost, _knowledgeVault As String
        Public Shared isAdministrator As Boolean


#Region "IExplorerExtension Members"


        Public Function CommandSites() As IEnumerable(Of CommandSite) Implements IExplorerExtension.CommandSites
            Dim _cmdItem As New CommandItem("AxItemSync", "Import Items")
            ' _cmdItem.Image = My.Resources.logo11


            _cmdItem.Hint = "Currently automatic synchronization is unavailable, It will be included in main project"
            AddHandler _cmdItem.Execute, AddressOf _cmdItem_Execute


            Dim _toolbarBtn As New CommandSite("Ax.Toolbar", "Import Items") With { _
         .Location = CommandSiteLocation.AdvancedToolbar, _
         .DeployAsPulldownMenu = False _
        }
            _toolbarBtn.AddCommand(_cmdItem)

            Dim sites As New List(Of CommandSite)()
            sites.Add(_toolbarBtn)
            Return sites
        End Function

        Public Function CustomEntityHandlers() As IEnumerable(Of CustomEntityHandler) Implements IExplorerExtension.CustomEntityHandlers
            Return Nothing
        End Function

        Public Function DetailTabs() As IEnumerable(Of DetailPaneTab) Implements IExplorerExtension.DetailTabs
            Return Nothing
        End Function

        Public Function HiddenCommands() As IEnumerable(Of String) Implements IExplorerExtension.HiddenCommands
            Return Nothing
        End Function

        Public Sub OnLogOff(ByVal application As IApplication) Implements IExplorerExtension.OnLogOff

        End Sub

        Public Sub OnLogOn(ByVal application As IApplication) Implements IExplorerExtension.OnLogOn
            If vConnection IsNot Nothing Then
                vConnection.ClearCache(CachedObjects.All)
            End If

            ' Use the IApplication object to create a credentials object.
            'AddHandler application.CommandBegin, AddressOf application_CommandBegin
            'AddHandler application.CommandEnd, AddressOf application_CommandEnd

            ' Use the credentials to create a new WebServiceManager object.
            vConnection = application.Connection 'application.Connection.WebServiceManager()
        End Sub

        Public Sub OnShutdown(ByVal application As IApplication) Implements IExplorerExtension.OnShutdown

        End Sub

        Public Sub OnStartup(ByVal application As IApplication) Implements IExplorerExtension.OnStartup

        End Sub

#End Region



        Private Sub _cmdItem_Execute(ByVal sender As Object, ByVal e As CommandItemEventArgs)

            vConnection = e.Context.Application.Connection

            
                Dim uiObj As UpdateItemMasterProperties = New UpdateItemMasterProperties(vConnection)
                uiObj.ShowDialog()
            ' ElementHost.EnableModelessKeyboardInterop(obj)
            Try
            Catch ex As Exception

            End Try
        End Sub

        Private Sub ConvertUri(ByVal baseLink As UriBuilder, ByVal title As String, ByVal linkText As String, ByVal htmlText As String)
            'Dim hosts As New List(Of String)() With { _
            '	baseLink.Host _
            '}

            '     If settings.MultiWorkgroups Then
            'If m_sitelist Is Nothing Then
            '    m_sitelist = SiteList.Load()
            'End If

            'If m_sitelist.Sites.Count = 0 Then
            '    MessageBox.Show("Multi-site links required a configured site list.  See your Vault administrator.")
            '    linkText = [String].Empty
            '    htmlText = [String].Empty
            'End If

            'Dim linkTextBuilder As New StringBuilder()
            'Dim htmlTextBuilder As New StringBuilder()
            'htmlTextBuilder.Append(String.Format("<i>{0}</i> (", title))

            'For i As Integer = 0 To m_sitelist.Sites.Count() - 1
            '    Dim site As Site = m_sitelist.Sites(i)
            '    baseLink.Host = site.Hostname
            '    baseLink.Scheme = If(site.IsSSL, "https", "http")
            '    If site.Port <> 0 Then
            '        baseLink.Port = site.Port
            '    Else
            '        baseLink.Port = If(site.IsSSL, 443, 80)
            '    End If

            '    linkTextBuilder.AppendLine(site.Name + " - " + baseLink.ToString())

            '    If i > 0 Then
            '        htmlTextBuilder.Append(" - ")
            '    End If

            '    htmlTextBuilder.Append(String.Format("<a href=""{0}"">{1}</a>", baseLink.ToString(), site.Name))
            'Next
            'htmlTextBuilder.Append(")")
            'linkText = linkTextBuilder.ToString()
            'htmlText = htmlTextBuilder.ToString()
            'Else
            linkText = baseLink.ToString()
            htmlText = String.Format("<a href=""{0}"">{1}</a>", baseLink.ToString(), title)
            '    End If
        End Sub

        Public Shared Function EscapeHTML(ByVal value As [String]) As String
            Dim retVal As String = value
            retVal = retVal.Replace("$", "%24")
            retVal = retVal.Replace("/", "%2f")
            retVal = retVal.Replace(" ", "+")
            Return retVal
        End Function

    End Class

End Namespace

