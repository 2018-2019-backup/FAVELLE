Imports Autodesk.Connectivity.Explorer.Extensibility
Imports Autodesk.Connectivity.WebServicesTools
Imports Autodesk.Connectivity.WebServices
Imports Autodesk.DataManagement.Client.Framework.Vault.Currency.Connections
Imports Autodesk.Connectivity.Extensibility.Framework
Imports System.Windows.Interop
Imports System.Text
Imports System.Windows.Forms.Integration
Imports System.Windows.Forms
Imports System.Xml
Imports System.Data


' Make sure to generate your own ID when writing your own extension. 
<Assembly: Autodesk.Connectivity.Extensibility.Framework.ExtensionId("61e8ffb2-a7c9-4e8a-bbeb-7f273ff64c4a")> 

' This number gets incremented for each Vault release.
<Assembly: Autodesk.Connectivity.Extensibility.Framework.ApiVersion("9.0")> 

Namespace FavelleVaultTools

    Public Class CommandExtension
        Implements IExplorerExtension

        Public vConnection As Connection
        '    Dim cred As UserIdTicketCredentials
        Public Shared _remoteHost, _knowledgeVault As String
        ' Public Shared isAdministrator As Boolean
        Public configFileName As String = "C:\ProgramData\Autodesk\Vault 2016\Extensions\FavelleVaultTools\FavelleVaultTools.dll.config"
        Public lstOrgs As List(Of String)


#Region "IExplorerExtension Members"


        Public Function CommandSites() As IEnumerable(Of CommandSite) Implements IExplorerExtension.CommandSites
            '    Dim _cmdItem As New CommandItem("FavelleVaultTools", "Favelle Vault Tools")
            '    _cmdItem.Image = My.Resources.KKMlogo
            '    _cmdItem.Hint = "Custom Tools For Vault"
            '    AddHandler _cmdItem.Execute, AddressOf _cmdItem_Execute

            '    Dim _toolbarBtn As New CommandSite("FavelleVaultTools.Toolbar", "Favelle Vault Tools") With {
            ' .Location = CommandSiteLocation.AdvancedToolbar,
            ' .DeployAsPulldownMenu = False
            '}
            '    _toolbarBtn.AddCommand(_cmdItem)

            Dim importItem As New CommandItem("ImportAXItems", "Import AX Items")
            '  importItem.Image = My.Resources.KKMlogo
            importItem.Hint = "Import Items from AX for this Vault"
            AddHandler importItem.Execute, AddressOf importItem_Execute

            Dim importBtn As New CommandSite("ImportAXItems.Toolbar", "Import AX Items") With {
         .Location = CommandSiteLocation.AdvancedToolbar,
         .DeployAsPulldownMenu = False
        }
            importBtn.AddCommand(importItem)


       


            Dim itemExport As New CommandItem("ExportToAX", "Export Items to AX") With {
    .NavigationTypes = New SelectionTypeId() {SelectionTypeId.Item},
    .MultiSelectEnabled = True
   }

            'itemExport.Image = My.Resources.pdf
            itemExport.Hint = "Export Items to AX"
            AddHandler itemExport.Execute, AddressOf itemExport_CmdHandler

            Dim itemExport_CmdSite As New CommandSite("ExportToAX.ItemContextMenu", "Export Items to AX") With {
      .Location = CommandSiteLocation.ItemContextMenu,
      .DeployAsPulldownMenu = False
     }
            itemExport_CmdSite.AddCommand(itemExport)


            Dim bomExport As New CommandItem("BOMToAX", "Export BOM to AX") With {
  .NavigationTypes = New SelectionTypeId() {SelectionTypeId.Item},
  .MultiSelectEnabled = True
 }

            'itemExport.Image = My.Resources.pdf
            bomExport.Hint = "Export BOM to AX"
            AddHandler bomExport.Execute, AddressOf bomExport_CmdHandler

            Dim bomExport_CmdSite As New CommandSite("BOMToAX.ItemContextMenu", "Export BOM to AX") With {
      .Location = CommandSiteLocation.ItemContextMenu,
      .DeployAsPulldownMenu = False
     }
            bomExport_CmdSite.AddCommand(bomExport)

            Dim sites As New List(Of CommandSite)()
            ' sites.Add(_toolbarBtn)
            sites.Add(importBtn)
            sites.Add(bomExport_CmdSite)
            sites.Add(itemExport_CmdSite)
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
            AddHandler application.CommandBegin, AddressOf application_CommandBegin
            AddHandler application.CommandEnd, AddressOf application_CommandEnd

            ' Use the credentials to create a new WebServiceManager object.
            vConnection = application.Connection 'application.Connection.WebServiceManager()
        End Sub

        Private Sub application_CommandEnd(ByVal sender As Object, ByVal e As CommandEndEventArgs)
            If e.CommandId = "File.ChangeRevision" Then

            End If
        End Sub

        Private Sub application_CommandBegin(ByVal sender As Object, ByVal e As CommandBeginEventArgs)
            If e.CommandId = "File.ChangeRevision" Then

            End If
        End Sub
        Public Sub OnShutdown(ByVal application As IApplication) Implements IExplorerExtension.OnShutdown

        End Sub

        Public Sub OnStartup(ByVal application As IApplication) Implements IExplorerExtension.OnStartup

        End Sub

#End Region


        Private Sub itemExport_CmdHandler(ByVal _obj As Object, ByVal e As CommandItemEventArgs)
            Try
                If e.Context.CurrentSelectionSet.Count > 0 Then
                    vConnection = e.Context.Application.Connection
                    Dim vUsername = vConnection.UserName
                    Dim itemCodes As String = String.Empty

                    '  curVault = vConnection.Vault
                    If IsAdministrator() Then
                        GetAXOrgs()
                        Dim obj As uiSelectAX = New uiSelectAX(lstOrgs)
                        obj.Title = "Select Organisation for Exporting Items"
                        obj.btnImport.Content = "Export"
                        obj.lblnote.Visibility = Visibility.Visible
                        If obj.ShowDialog().Equals(True) Then
                            Dim result As Integer = System.Windows.MessageBox.Show("Are you sure to Export Item to AX?", msgCaption, MessageBoxButton.YesNo, MessageBoxImage.Question)
                            If result = DialogResult.Yes Then
                                Dim axStr As String = String.Empty
                                For Each dr As DataRow In obj.dtAxOrgs.Select("IsSelected = 'True'")
                                    If axStr.Equals(String.Empty) Then
                                        axStr = dr("OrgName").ToString()
                                    Else
                                        axStr += "," + dr("OrgName").ToString()
                                    End If
                                Next
                                Dim lstIds As String = String.Empty

                                For Each iSel As ISelection In e.Context.CurrentSelectionSet
                                    If lstIds.Equals(String.Empty) Then
                                        lstIds = iSel.Id.ToString()
                                        itemCodes = GetItemCode(iSel.Id)
                                    Else
                                        lstIds += "," + iSel.Id.ToString()
                                        itemCodes += ", " + GetItemCode(iSel.Id)
                                    End If
                                Next
                                Dim paramList As JobParam() = New JobParam(2) {}
                                Dim axOrgs As New JobParam()
                                axOrgs.Name = "AxOrgs"
                                axOrgs.Val = axStr ' vaultObj.Id.ToString()
                                paramList(0) = axOrgs
                                Dim selIds As New JobParam()
                                selIds.Name = "ItemIds"
                                selIds.Val = lstIds ' vaultObj.Id.ToString()
                                paramList(1) = selIds
                                Dim vUser As New JobParam()
                                vUser.Name = "vUser"
                                vUser.Val = vUsername ' vaultObj.Id.ToString()
                                paramList(2) = vUser
                                AddJob(paramList, "FavelleVaultJobs.ExportItem.ManualSync", "Export Items to AX - Manual Sync. " + Environment.NewLine + "Selected Items:" + itemCodes + Environment.NewLine + "; Seleted AX: " + axStr)
                            End If
                        End If
                    Else
                        Dim result As Integer = System.Windows.MessageBox.Show("Are you sure to Export Item to AX?", msgCaption, MessageBoxButton.YesNo, MessageBoxImage.Question)
                        If result = DialogResult.Yes Then
                            Dim lstIds As String = String.Empty
                            For Each iSel As ISelection In e.Context.CurrentSelectionSet
                                If lstIds.Equals(String.Empty) Then
                                    lstIds = iSel.Id.ToString()
                                    itemCodes = GetItemCode(iSel.Id)
                                Else
                                    lstIds += "," + iSel.Id.ToString()
                                    itemCodes += ", " + GetItemCode(iSel.Id)
                                End If
                            Next
                            Dim paramList As JobParam() = New JobParam(2) {}
                            Dim axOrgs As New JobParam()
                            axOrgs.Name = "AxOrgs"
                            axOrgs.Val = "" ' vaultObj.Id.ToString()
                            paramList(0) = axOrgs
                            Dim selIds As New JobParam()
                            selIds.Name = "ItemIds"
                            selIds.Val = lstIds ' vaultObj.Id.ToString()
                            paramList(1) = selIds
                            Dim vUser As New JobParam()
                            vUser.Name = "vUser"
                            vUser.Val = vUsername ' vaultObj.Id.ToString()
                            paramList(2) = vUser
                            AddJob(paramList, "FavelleVaultJobs.ExportItem.ManualSync", "Export Items to AX - Manual Sync. " + "Selected Items:" + itemCodes + Environment.NewLine)
                        End If
                    End If
                End If
            Catch ex As Exception
                'If ex.Message.Equals("237", StringComparison.InvariantCultureIgnoreCase) Then
                '    System.Windows.MessageBox.Show("The Selected Item has been already added to Export Job Queue!", msgCaption, Forms.MessageBoxButtons.OK, Forms.MessageBoxIcon.Information)
                'End If
            End Try
        End Sub
        Public Function IsAdministrator() As Boolean
            Dim isAdmin = False
            Try


                Dim _roles As Autodesk.Connectivity.WebServices.Role() = Nothing
                Try
                    _roles = vConnection.WebServiceManager.AdminService.GetRolesByUserId(vConnection.UserID)
                Catch ex As Exception

                End Try
                If _roles IsNot Nothing Then
                    For Each _role In _roles
                        If _role.Name.Equals("Administrator", StringComparison.InvariantCultureIgnoreCase) Then
                            isAdmin = True
                            Exit For
                        End If
                    Next
                End If
            Catch ex As Exception

            End Try
            Return isAdmin
        End Function
        'Private Sub _cmdItem_Execute(ByVal sender As Object, ByVal e As CommandItemEventArgs)
        '    vConnection = e.Context.Application.Connection
        '    Try


        '        isAdministrator = False
        '        Dim _roles As Autodesk.Connectivity.WebServices.Role() = Nothing
        '        Try
        '            _roles = vConnection.WebServiceManager.AdminService.GetRolesByUserId(vConnection.UserID)
        '        Catch ex As Exception

        '        End Try
        '        If _roles IsNot Nothing Then
        '            For Each _role In _roles
        '                If _role.Name.Equals("Administrator", StringComparison.InvariantCultureIgnoreCase) Then
        '                    isAdministrator = True
        '                    Exit For
        '                End If
        '            Next
        '        End If


        '        If isAdministrator Then
        '            Dim obj As New UI(vConnection)
        '            obj.ShowDialog()
        '        Else
        '            System.Windows.MessageBox.Show("Only users with Administrator rights can access Settings!", msgCaption, Forms.MessageBoxButtons.OK, Forms.MessageBoxIcon.Information)
        '        End If

        '    Catch ex As Exception

        '    End Try
        'End Sub

        Private Function GetItemCode(ByVal id As Long) As String
            Dim retValue As String = String.Empty
            Try
                Dim tItem As Item
                Try
                    tItem = vConnection.WebServiceManager.ItemService.GetItemsByIds(New Long() {id})(0)
                Catch ex As Exception
                    tItem = vConnection.WebServiceManager.ItemService.GetLatestItemByItemMasterId(id)
                End Try
                If tItem IsNot Nothing Then
                    retValue = tItem.ItemNum
                End If
            Catch ex As Exception

            End Try
            Return retValue
        End Function

        Private Sub importItem_Execute(sender As Object, e As CommandItemEventArgs)
            Try
                vConnection = e.Context.Application.Connection
                Dim vUsername = vConnection.UserName
                '  curVault = vConnection.Vault
                If IsAdministrator() Then
                    GetAXOrgs()
                    Dim obj As uiSelectAX = New uiSelectAX(lstOrgs)
                    If obj.ShowDialog().Equals(True) Then
                        Dim result As Integer = System.Windows.MessageBox.Show("Are you sure to Import Items from AX?", msgCaption, MessageBoxButton.YesNo, MessageBoxImage.Question)
                        If result = DialogResult.Yes Then
                            Dim axStr As String = String.Empty
                            For Each dr As DataRow In obj.dtAxOrgs.Select("IsSelected = 'True'")
                                If axStr.Equals(String.Empty) Then
                                    axStr = dr("OrgName").ToString()
                                Else
                                    axStr += "," + dr("OrgName").ToString()
                                End If
                            Next
                            Dim paramList As JobParam() = New JobParam(1) {}
                            Dim axOrgs As New JobParam()
                            axOrgs.Name = "AxOrgs"
                            axOrgs.Val = axStr ' vaultObj.Id.ToString()
                            paramList(0) = axOrgs
                            Dim vUser As New JobParam()
                            vUser.Name = "vUser"
                            vUser.Val = vUsername ' vaultObj.Id.ToString()
                            paramList(1) = vUser
                            AddJob(paramList, "FavelleVaultJobs.ImportItem.ManualSync", "Import items from AX - Manual Sync. " + Environment.NewLine + "Selected AX: " + axStr)
                        End If
                    End If
                Else
                    Dim result As Integer = System.Windows.MessageBox.Show("Are you sure to Import Items from AX?", msgCaption, MessageBoxButton.YesNo, MessageBoxImage.Question)
                    If result = DialogResult.Yes Then
                        Dim paramList As JobParam() = New JobParam(1) {}
                        Dim axOrgs As New JobParam()
                        axOrgs.Name = "AxOrgs"
                        axOrgs.Val = "" ' vaultObj.Id.ToString()
                        paramList(0) = axOrgs
                        Dim vUser As New JobParam()
                        vUser.Name = "vUser"
                        vUser.Val = vUsername ' vaultObj.Id.ToString()
                        paramList(1) = vUser
                        AddJob(paramList, "FavelleVaultJobs.ImportItem.ManualSync", "Import items from AX - Manual Sync.")
                    End If
                End If
            Catch ex As Exception

            End Try
            'Dim obj As uiImportItems = New uiImportItems()
            'obj.ShowDialog()
        End Sub

        Public Function GetAXOrgs() As Boolean
            Dim retValue As Boolean = False
            Try
                Dim settingsXml As New XmlDocument
                Dim userSettingsNode As XmlNode
                settingsXml.Load(configFileName)
                userSettingsNode = settingsXml.SelectSingleNode("configuration").SelectSingleNode("userSettings").SelectSingleNode("FavelleVaultTools.MySettings")
                Dim axOrgs As String = (From xn As XmlNode In userSettingsNode.ChildNodes
                                       Where xn.Attributes("name").Value.Equals("AxOrgs", StringComparison.CurrentCultureIgnoreCase)
                                       Select xn.SelectSingleNode("value").InnerText).FirstOrDefault()
                lstOrgs = axOrgs.Split(",").ToList()
                retValue = True
            Catch ex As Exception

            End Try
            Return retValue
        End Function

        Private Sub bomExport_CmdHandler(sender As Object, e As CommandItemEventArgs)
            Try
                If e.Context.CurrentSelectionSet.Count > 0 Then
                    vConnection = e.Context.Application.Connection
                    Dim vUsername = vConnection.UserName
                    Dim itemCodes As String = String.Empty
                    If IsAdministrator() Then
                        '  curVault = vConnection.Vault
                        GetAXOrgs()
                        Dim obj As uiSelectAX = New uiSelectAX(lstOrgs)
                        obj.Title = "Select Organisation for Exporting BOM"
                        obj.btnImport.Content = "Export"
                        obj.lblnote.Visibility = Visibility.Visible
                        If obj.ShowDialog().Equals(True) Then
                            Dim result As Integer = System.Windows.MessageBox.Show("Are you sure to Export BOM to AX?", msgCaption, MessageBoxButton.YesNo, MessageBoxImage.Question)
                            If result = DialogResult.Yes Then
                                Dim axStr As String = String.Empty
                                For Each dr As DataRow In obj.dtAxOrgs.Select("IsSelected = 'True'")
                                    If axStr.Equals(String.Empty) Then
                                        axStr = dr("OrgName").ToString()
                                    Else
                                        axStr += "," + dr("OrgName").ToString()
                                    End If
                                Next
                                Dim lstIds As String = String.Empty
                                For Each iSel As ISelection In e.Context.CurrentSelectionSet
                                    If lstIds.Equals(String.Empty) Then
                                        lstIds = iSel.Id.ToString()
                                        itemCodes = GetItemCode(iSel.Id)
                                    Else
                                        lstIds += "," + iSel.Id.ToString()
                                        itemCodes += ", " + GetItemCode(iSel.Id)
                                    End If
                                Next
                                Dim paramList As JobParam() = New JobParam(2) {}
                                Dim axOrgs As New JobParam()
                                axOrgs.Name = "AxOrgs"
                                axOrgs.Val = axStr ' vaultObj.Id.ToString()
                                paramList(0) = axOrgs
                                Dim selIds As New JobParam()
                                selIds.Name = "ItemIds"
                                selIds.Val = lstIds ' vaultObj.Id.ToString()
                                paramList(1) = selIds
                                Dim vUser As New JobParam()
                                vUser.Name = "vUser"
                                vUser.Val = vUsername ' vaultObj.Id.ToString()
                                paramList(2) = vUser
                                AddJob(paramList, "FavelleVaultJobs.ExportBOM.ManualSync", "Export BOM to AX - Manual Sync. " + Environment.NewLine + "Selected Items:" + itemCodes + Environment.NewLine + "; Selected AX: " + axStr)
                            End If
                        End If
                    Else
                        Dim result As Integer = System.Windows.MessageBox.Show("Are you sure to Export BOM to AX?", msgCaption, MessageBoxButton.YesNo, MessageBoxImage.Question)
                        If result = DialogResult.Yes Then
                            Dim axStr As String = String.Empty
                            Dim lstIds As String = String.Empty
                            For Each iSel As ISelection In e.Context.CurrentSelectionSet
                                If lstIds.Equals(String.Empty) Then
                                    lstIds = iSel.Id.ToString()
                                    itemCodes = GetItemCode(iSel.Id)
                                Else
                                    lstIds += "," + iSel.Id.ToString()
                                    itemCodes += ", " + GetItemCode(iSel.Id)
                                End If
                            Next
                            Dim paramList As JobParam() = New JobParam(1) {}
                            Dim axOrgs As New JobParam()
                            axOrgs.Name = "AxOrgs"
                            axOrgs.Val = axStr ' vaultObj.Id.ToString()
                            paramList(0) = axOrgs
                            Dim selIds As New JobParam()
                            selIds.Name = "ItemIds"
                            selIds.Val = lstIds ' vaultObj.Id.ToString()
                            paramList(1) = selIds
                            Dim vUser As New JobParam()
                            vUser.Name = "vUser"
                            vUser.Val = vUsername ' vaultObj.Id.ToString()
                            paramList(2) = vUser
                            AddJob(paramList, "FavelleVaultJobs.ExportBOM.ManualSync", "Export BOM to AX - Manual Sync. " + Environment.NewLine + "Selected Items:" + itemCodes)
                        End If
                    End If
                End If
            Catch ex As Exception
                'If ex.Message.Equals("237", StringComparison.InvariantCultureIgnoreCase) Then
                '    System.Windows.MessageBox.Show("The Selected Item has been already added to Export Job Queue!", msgCaption, Forms.MessageBoxButtons.OK, Forms.MessageBoxIcon.Information)
                'End If
            End Try
        End Sub

        Public Sub AddJob(ByVal paramList As JobParam(), ByVal publishJobTypeName As String, ByVal desc As String)
            Try
                vConnection.WebServiceManager.JobService.AddJob(publishJobTypeName, desc, paramList, 1)
                MessageBox.Show(publishJobTypeName + " job added successfully!", msgCaption, MessageBoxButtons.OK, MessageBoxIcon.Information)
            Catch ex As Exception
                If ex.Message.Equals("237", StringComparison.InvariantCultureIgnoreCase) Then
                    System.Windows.MessageBox.Show("The job already exists in Job Queue!", msgCaption, Forms.MessageBoxButtons.OK, Forms.MessageBoxIcon.Information)
                Else
                    MessageBox.Show("Unable to add " + desc + "!" + vbNewLine + "Details: " + ex.ToString(), msgCaption, MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If
            End Try
        End Sub





    End Class







End Namespace

