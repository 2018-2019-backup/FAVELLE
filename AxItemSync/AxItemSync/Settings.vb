Imports Autodesk.DataManagement.Client.Framework.Vault.Currency.Connections
Imports Autodesk.DataManagement.Client.Framework
Imports Autodesk.Connectivity.WebServices
Imports Autodesk.Connectivity.WebServicesTools
Imports System.Data
Imports System.Xml

Public Class Settings
    Implements ILifeCycleEvent

    '  Private vConnection As Connection


    Private m_idsToStates As Dictionary(Of Long, LfCycState)
    Private m_transIdsToEvents As Dictionary(Of Long, StringArray)
    Private m_pendingChanges As Dictionary(Of Long, StringArray)
    Private m_currentThreadId As Guid = Guid.Empty
    Private m_committingChanges As Boolean = False
    Private lstVault As List(Of String) = New List(Of String)
    Dim _jobType As String = String.Empty

    Private Sub btnSave_Click(sender As Object, e As EventArgs)
        Try
            My.Settings.Save()
            MessageBox.Show("Settings value saved succcessfully", strMsgCaption, MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            WriteLog(ex.Message.ToString)
            MessageBox.Show("Unable to save the Settings!", strMsgCaption, MessageBoxButtons.OK, MessageBoxIcon.Information)
        End Try
    End Sub

    Private Sub Settings_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        InitialLoad()
        ToolTip1.SetToolTip(lst_FileJobs, "Right Click to Add/Delete Jobs")
    End Sub

    Private Sub InitialLoad()
        Try
            If System.IO.File.Exists(settingsFile) Then
                LoadDataFromSettingsFile()
            End If
        Catch ex As Exception
            WriteLog(ex.Message.ToString)
        End Try
    End Sub

    Private Sub LoadDataFromSettingsFile()
        Try
            Dim settingsXml As New XmlDocument
            Dim rootNode, xnBasicSettings, xnPropMapping, xnBOMPropMapping, xnVaultJobs, xnUserMapping, xnCATMapping As XmlNode
            settingsXml.Load(settingsFile)
            rootNode = settingsXml.SelectSingleNode("Settings")
            xnBasicSettings = rootNode.SelectSingleNode("BasicSettings")
            xnPropMapping = rootNode.SelectSingleNode("ItemPropertyMapping")
            xnBOMPropMapping = rootNode.SelectSingleNode("BomPropertyMapping")
            xnVaultJobs = rootNode.SelectSingleNode("VaultJobs")
            xnUserMapping = rootNode.SelectSingleNode("VaultUsers")
            xnCATMapping = rootNode.SelectSingleNode("CATValueMapping")
            LoadBasicSettings(xnBasicSettings)
            LoadPropMapping(xnPropMapping)
            LoadBOMPropMapping(xnBOMPropMapping)
            LoadVaultJobs(xnVaultJobs)
            LoadUserMapping(xnUserMapping)
            LoadCATValueMapping(xnCATMapping)
        Catch ex As Exception

        End Try
    End Sub
    Private Sub LoadCATValueMapping(ByVal xn As XmlNode)
        Try
            dgCAT.Rows.Clear()
            For Each node As XmlNode In xn.SelectNodes("CAT")
                dgCAT.Rows.Add(node.SelectSingleNode(dcCATAx.Name).InnerText, node.SelectSingleNode(dcCATVault.Name).InnerText)
            Next
        Catch ex As Exception

        End Try
    End Sub
    Private Sub LoadUserMapping(ByVal xn As XmlNode)
        If SetConnection(txtVServer.Text, cmbxVaultsforJobs.SelectedItem, txtVUsername.Text, txtVPwd.Text) Then
            Dim vUSers() As User = vCon.WebServiceManager.AdminService.GetAllUsers()

            For Each _user As User In vUSers
                dgvUsers.Rows.Add(New Object() {_user.Name, "", True, True, True})
            Next

            Dim tempNode1, tempNode2 As XmlNode
            Dim axFieldHeader As String = dgvUsers.Columns(0).HeaderText.Replace(" ", "_")
            Dim axField As String
            Dim tempValue As Boolean = False
            For row = 0 To dgvUsers.Rows.Count - 1
                axField = dgvUsers.Rows(row).Cells(0).Value
                tempNode1 = (From childNode As XmlNode In xn.SelectNodes("UserDetails")
                                        Where childNode.SelectSingleNode(axFieldHeader).InnerText.Equals(axField)
                           Select childNode).FirstOrDefault()

                If tempNode1 IsNot Nothing Then
                    For i = 1 To dgvUsers.Columns.Count - 1
                        tempNode2 = tempNode1.SelectSingleNode(dgvUsers.Columns(i).HeaderText.Replace(" ", "_"))
                        If tempNode2 IsNot Nothing Then
                            If i = 1 Then
                                dgvUsers.Rows(row).Cells(i).Value = tempNode2.InnerText
                            Else
                                Boolean.TryParse(tempNode2.InnerText, tempValue)
                                dgvUsers.Rows(row).Cells(i).Value = tempValue
                            End If
                        End If
                    Next
                End If
            Next
        End If
    End Sub
    Private Sub LoadVaultJobs(ByVal xn As XmlNode)
        Try
            For Each vaults In lstVault
                cmbxVaultsforJobs.Items.Add(vaults)
            Next
            If xn IsNot Nothing Then
                txtAfterExportItem.Text = xn.SelectSingleNode("ItemState").InnerText
                txtAfterExportBOM.Text = xn.SelectSingleNode("BOMProp").InnerText
            End If
        Catch ex As Exception

        End Try
    End Sub

    Private Sub LoadBOMPropMapping(ByVal xn As XmlNode)
        Try
            dgvBOMPropMapping.Rows.Clear()
            If lstVault.Count > 0 Then
                Dim i = 1
                For Each vaults In lstVault
                    dgvBOMPropMapping.Columns.Add(vaults.Trim(), vaults.Trim())
                    i = i + 1
                Next
                For Each column In columnNamesBOM
                    dgvBOMPropMapping.Rows.Add(column)
                Next
                If xn Is Nothing Then
                    For row = 0 To dgvBOMPropMapping.Rows.Count - 1
                        For i = 1 To dgvBOMPropMapping.Columns.Count - 1
                            dgvBOMPropMapping.Rows(row).Cells(i).Value = defaultPropsBOM(row).Trim
                        Next
                    Next
                Else
                    Dim tempNode1, tempNode2 As XmlNode
                    Dim axFieldHeader As String = dgvBOMPropMapping.Columns(0).HeaderText.Replace(" ", "_")
                    Dim axField As String
                    For row = 0 To dgvBOMPropMapping.Rows.Count - 1
                        axField = dgvBOMPropMapping.Rows(row).Cells(0).Value
                        tempNode1 = (From childNode As XmlNode In xn.SelectNodes("AXVaultBOM")
                                                Where childNode.SelectSingleNode(axFieldHeader).InnerText.Equals(axField)
                                   Select childNode).FirstOrDefault()
                        If tempNode1 IsNot Nothing Then
                            For i = 1 To dgvBOMPropMapping.Columns.Count - 1
                                tempNode2 = tempNode1.SelectSingleNode(dgvBOMPropMapping.Columns(i).Name.Replace(" ", "_"))
                                If tempNode2 IsNot Nothing Then
                                    dgvBOMPropMapping.Rows(row).Cells(i).Value = tempNode2.InnerText
                                Else
                                    dgvBOMPropMapping.Rows(row).Cells(i).Value = defaultPropsBOM(row).Trim
                                End If
                            Next
                        Else
                            For i = 1 To dgvBOMPropMapping.Columns.Count - 1
                                dgvBOMPropMapping.Rows(row).Cells(i).Value = defaultPropsBOM(row).Trim
                            Next
                        End If
                    Next
                End If
            End If
        Catch ex As Exception

        End Try
    End Sub


    Private Sub LoadPropMapping(ByVal xn As XmlNode)
        Try
            dgvPropMapping.Rows.Clear()
            If lstVault.Count > 0 Then
                Dim i = 1
                For Each vaults In lstVault
                    dgvPropMapping.Columns.Add(vaults.Trim(), vaults.Trim())
                    i = i + 1
                Next
                For Each column In columnNames
                    dgvPropMapping.Rows.Add(column.Trim())
                Next
                If xn Is Nothing Then
                    For row = 0 To dgvPropMapping.Rows.Count - 1
                        For i = 1 To dgvPropMapping.Columns.Count - 1
                            dgvPropMapping.Rows(row).Cells(i).Value = defaultProps(row).Trim
                        Next
                    Next
                Else
                    Dim tempNode1, tempNode2 As XmlNode
                    Dim axFieldHeader As String = dgvPropMapping.Columns(0).HeaderText.Replace(" ", "_")
                    Dim axField As String
                    For row = 0 To dgvPropMapping.Rows.Count - 1
                        axField = dgvPropMapping.Rows(row).Cells(0).Value.Trim()
                        tempNode1 = (From childNode As XmlNode In xn.ChildNodes
                                   Where childNode.SelectSingleNode(axFieldHeader).InnerText.Equals(axField)
                                   Select childNode).FirstOrDefault()
                        If tempNode1 IsNot Nothing Then
                            For i = 1 To dgvPropMapping.Columns.Count - 1
                                tempNode2 = tempNode1.SelectSingleNode(dgvPropMapping.Columns(i).Name.Replace(" ", "_"))
                                If tempNode2 IsNot Nothing Then
                                    dgvPropMapping.Rows(row).Cells(i).Value = tempNode2.InnerText.Trim()
                                Else
                                    dgvPropMapping.Rows(row).Cells(i).Value = defaultProps(row).Trim()
                                End If
                            Next
                        Else
                            For i = 1 To dgvPropMapping.Columns.Count - 1
                                dgvPropMapping.Rows(row).Cells(i).Value = defaultProps(row).Trim
                            Next
                        End If
                    Next
                End If
            End If
        Catch ex As Exception

        End Try
    End Sub

    Private Sub LoadBasicSettings(xn As XmlNode)
        Try
            txtVUsername.Text = xn.SelectSingleNode("vUSername").InnerText
            txtVPwd.Text = xn.SelectSingleNode("vPassword").InnerText
            txtVServer.Text = xn.SelectSingleNode("vServer").InnerText
            txtAxService.Text = xn.SelectSingleNode("AXService").InnerText
            Try
                txtBOMService.Text = xn.SelectSingleNode("AXBOMService").InnerText
            Catch ex As Exception

            End Try
            dgvVaultOrg.Rows.Clear()
            lstVault.Clear()
            For Each node As XmlNode In xn.SelectNodes("VO")
                dgvVaultOrg.Rows.Add(node.SelectSingleNode("Vault").InnerText, node.SelectSingleNode("Organisation").InnerText)
                If Not lstVault.Contains(node.SelectSingleNode("Vault").InnerText) Then
                    lstVault.Add(node.SelectSingleNode("Vault").InnerText)
                End If
            Next
            Boolean.TryParse(xn.SelectSingleNode("AutoImport").InnerText, chkAutoImport.Checked)
            txtDuration.Text = xn.SelectSingleNode("WaitTime").InnerText
        Catch ex As Exception

        End Try
    End Sub

    Private Sub cmbx_lifeCylceDef_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbx_lifeCylceDef.SelectedIndexChanged
        lst_FileState.Items.Clear()
        lst_FileTransition.Items.Clear()
        lst_FileJobs.Items.Clear()
        '  m_jobTypesListBox.Items.Clear()
        If (m_idsToStates IsNot Nothing) Then
            m_idsToStates.Clear()
        End If
        m_currentThreadId = Guid.NewGuid()
        UpdateStatesWorker(m_currentThreadId, cmbx_lifeCylceDef.SelectedItem)
    End Sub

    Private Sub lst_FileTransition_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles lst_FileTransition.SelectedIndexChanged
        Dim transition As LfCycTransListItem = TryCast(lst_FileTransition.SelectedItem, LfCycTransListItem)
        lst_FileJobs.Items.Clear()
        If transition Is Nothing OrElse transition.LfCycTrans Is Nothing Then
            Return
        End If

        If m_transIdsToEvents.ContainsKey(transition.LfCycTrans.Id) Then
            Dim jobArray As StringArray = m_transIdsToEvents(transition.LfCycTrans.Id)

            If jobArray IsNot Nothing AndAlso jobArray.Items IsNot Nothing Then
                For Each job As String In jobArray.Items
                    lst_FileJobs.Items.Add(job)
                Next
            End If
        End If
    End Sub

    Private Sub lst_FileState_SelectedIndexChanged(sender As Object, e As EventArgs) Handles lst_FileState.SelectedIndexChanged
        lst_FileTransition.Items.Clear()
        lst_FileJobs.Items.Clear()
        '  m_jobTypesListBox.Items.Clear()

        Dim lcDefItem As LfCycDefListItem = TryCast(cmbx_lifeCylceDef.SelectedItem, LfCycDefListItem)
        Dim stateItem As LfCycStateListItem = TryCast(lst_FileState.SelectedItem, LfCycStateListItem)

        Dim lcDef As LfCycDef = lcDefItem.LfCycDef
        Dim state As LfCycState = stateItem.LfCycState
        If lcDef IsNot Nothing AndAlso state IsNot Nothing Then
            Dim transitions As LfCycTrans() = lcDef.TransArray
            If transitions Is Nothing OrElse transitions.Length = 0 Then
                Return
            End If

            ' pass 1: going from the selected state
            For Each transition As LfCycTrans In transitions
                If transition.FromId = state.Id Then
                    Dim transItem As New LfCycTransListItem(transition)
                    transItem.DispName = state.DispName + " -> " + m_idsToStates(transition.ToId).DispName
                    lst_FileTransition.Items.Add(transItem)
                End If
            Next

            ' pass 2: going to the selected state
            For Each transition As LfCycTrans In transitions
                If transition.ToId = state.Id Then
                    Dim transItem As New LfCycTransListItem(transition)
                    transItem.DispName = state.DispName + " <- " + m_idsToStates(transition.FromId).DispName
                    lst_FileTransition.Items.Add(transItem)
                End If
            Next
        End If
    End Sub


#Region "Private methods"

    Private Sub GetTransitionEvents(ByVal lcDef As LfCycDef)
        Dim transitions As LfCycTrans() = lcDef.TransArray
        If transitions Is Nothing OrElse transitions.Length = 0 Then
            Return
        End If

        ' check to see if we have the information in the cache already
        If m_transIdsToEvents.ContainsKey(transitions(0).Id) Then
            Return
        End If

        Dim transIds As New List(Of Long)()
        For Each transition As LfCycTrans In transitions
            transIds.Add(transition.Id)
        Next

        Dim jobArrayArray As StringArray() = Nothing

        jobArrayArray = vCon.WebServiceManager.LifeCycleService.GetJobTypesByLifeCycleStateTransitionIds(transIds.ToArray())

        Dim i As Integer = 0
        For Each id As Long In transIds
            If jobArrayArray IsNot Nothing Then
                m_transIdsToEvents.Add(id, jobArrayArray(i))
            Else
                m_transIdsToEvents.Add(id, Nothing)
            End If

            i += 1
        Next
    End Sub

    Private Sub UpdatePendingChanges()
        Dim transitionItem As LfCycTransListItem = TryCast(lst_FileTransition.SelectedItem, LfCycTransListItem)
        If transitionItem Is Nothing Then
            Return
        End If

        Dim transition As LfCycTrans = transitionItem.LfCycTrans
        If m_pendingChanges.ContainsKey(transition.Id) Then
            m_pendingChanges.Remove(transition.Id)
        End If

        Dim jobTypes As New StringArray()
        'jobTypes.Items = New String(0) {"Cadline.PublishJob.LifeCycleStateChange"}

        If lst_FileJobs.Items.Count = 0 Then
            jobTypes.Items = Nothing
        Else
            jobTypes.Items = New String(lst_FileJobs.Items.Count - 1) {}
            Dim i As Integer = 0
            For Each job As String In lst_FileJobs.Items
                jobTypes.Items(i) = job
                i += 1
            Next
        End If

        m_pendingChanges.Add(transition.Id, jobTypes)

        If m_transIdsToEvents.ContainsKey(transition.Id) Then
            m_transIdsToEvents(transition.Id) = jobTypes
        Else
            m_transIdsToEvents.Add(transition.Id, jobTypes)
        End If

    End Sub

   
#End Region

#Region "WORKER_THREADS"

    Public Sub PopulateUIWorker(ByVal o As Object)
        Dim lcDefs As LfCycDef() = Nothing
        Try
            ' get all the lifecycle information
            lcDefs = vCon.WebServiceManager.LifeCycleService.GetAllLifeCycleDefinitions()
        Catch ex As Exception
            ' Show the error on the UI Thread

        End Try

        ' Process the results on the UI Thread
        'Me.BeginInvoke(DirectCast(Function() Do
        '	If lcDefs IsNot Nothing Then
        If lcDefs IsNot Nothing Then
            cmbx_lifeCylceDef.Items.Clear()
            For Each lcDef As LfCycDef In lcDefs
                cmbx_lifeCylceDef.Items.Add(New LfCycDefListItem(lcDef))
            Next
            If cmbx_lifeCylceDef.Items.Count > 0 Then
                cmbx_lifeCylceDef.SelectedIndex = 0
            End If
        End If

    End Sub

    'Private Sub UpdateStatesWorker(o As Object)
    Private Sub UpdateStatesWorker(ByVal threadId As Guid, ByVal lcDef As LfCycDefListItem)
        'Dim inputs As Object() = TryCast(o, Object())
        'Dim threadId As Guid = DirectCast(inputs(0), Guid)
        'Dim lcDef As LfCycDefListItem = TryCast(inputs(1), LfCycDefListItem)
        Dim states As LfCycState() = Nothing

        If lcDef.LfCycDef IsNot Nothing Then
            states = lcDef.LfCycDef.StateArray
        End If

        Try
            If states IsNot Nothing Then
                GetTransitionEvents(lcDef.LfCycDef)
            End If
        Catch ex As Exception
            ' Show the error on the UI Thread
            'Me.BeginInvoke(DirectCast(Function() Do
            '            MessageBox.Show(ex.ToString())
            'End Function, MethodInvoker))
        End Try

        ' Process the results on the UI Thread
        'Me.BeginInvoke(DirectCast(Function() Do
        ' only update the UI if we are the current thread
        If threadId.Equals(m_currentThreadId) Then
            If states IsNot Nothing Then
                For Each state As LfCycState In states
                    lst_FileState.Items.Add(New LfCycStateListItem(state))
                    m_idsToStates.Add(state.Id, state)
                Next
            End If

            m_currentThreadId = Guid.Empty
        End If
        'End Function, MethodInvoker))
    End Sub

#End Region

    Private Sub lstVaults_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbxVaultsforJobs.SelectedIndexChanged
        Try
            Me.Cursor = Cursors.AppStarting
            If SetConnection(txtVServer.Text, cmbxVaultsforJobs.SelectedItem, txtVUsername.Text, txtVPwd.Text) Then
                ClearJobs()
                PopulateUI()
            Else
                MessageBox.Show("Unable to connect to selected vault with the given credentials", strMsgCaption, MessageBoxButtons.OK, MessageBoxIcon.Information)
                ClearJobs()
                cmbx_lifeCylceDef.Items.Clear()
                lst_FileState.Items.Clear()
                lst_FileTransition.Items.Clear()
                lst_FileJobs.Items.Clear()
            End If
        Catch ex As Exception

        Finally
            Me.Cursor = Cursors.Arrow
        End Try
    End Sub
    Private Sub ClearJobs()
        m_idsToStates = Nothing
        m_transIdsToEvents = Nothing
        m_pendingChanges = Nothing
        m_currentThreadId = Guid.Empty
        m_committingChanges = False
    End Sub

    Private Sub btnVOAdd_Click(sender As Object, e As EventArgs) Handles btnVOAdd.Click
        If txtTOrg.Text.Trim() <> String.Empty And txtTVault.Text <> String.Empty Then
            dgvVaultOrg.Rows.Add(txtTVault.Text, txtTOrg.Text)
            Clear()
        End If
    End Sub

    Private Sub btnVOEdit_Click(sender As Object, e As EventArgs) Handles btnVOEdit.Click
        If txtTOrg.Text.Trim() <> String.Empty And txtTVault.Text <> String.Empty Then
            Dim selIndex = dgvVaultOrg.SelectedRows.Item(0).Index
            dgvVaultOrg.Rows(selIndex).Cells(0).Value = txtTVault.Text
            dgvVaultOrg.Rows(selIndex).Cells(1).Value = txtTOrg.Text
            Clear()
        End If
    End Sub
    Private Sub btnVODelete_Click(sender As Object, e As EventArgs) Handles btnVODelete.Click
        If dgvVaultOrg.SelectedRows IsNot Nothing Then
            Dim selIndex = dgvVaultOrg.SelectedRows.Item(0).Index
            dgvVaultOrg.Rows.RemoveAt(selIndex)
            Clear()
        End If
    End Sub

    Private Sub btnClear_Click(sender As Object, e As EventArgs) Handles btnClear.Click
        Clear()
    End Sub

    Private Sub Clear()
        txtTOrg.Clear()
        txtTVault.Clear()
        btnVOAdd.Enabled = True
        btnVOEdit.Enabled = False
        btnVODelete.Enabled = False
        txtTVault.Focus()
    End Sub

    Private Sub dgvVaultOrg_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvVaultOrg.CellClick
        If dgvVaultOrg.SelectedRows IsNot Nothing Then
            Dim selIndex = dgvVaultOrg.SelectedRows.Item(0).Index
            btnVOAdd.Enabled = False
            btnVOEdit.Enabled = True
            btnVODelete.Enabled = True
            txtTVault.Text = dgvVaultOrg.Rows(selIndex).Cells(0).Value
            txtTOrg.Text = dgvVaultOrg.Rows(selIndex).Cells(1).Value
        End If
    End Sub
    Private Sub btnTab1Save_Click(sender As Object, e As EventArgs) Handles btnTab1Save.Click
        Try
            Dim settingsXml As New XmlDocument
            Dim rootNode, parentNode, childNode, oldParentNode, innerChildNode As XmlNode
            oldParentNode = Nothing
            If System.IO.File.Exists(settingsFile) Then
                settingsXml.Load(settingsFile)
                rootNode = settingsXml.SelectSingleNode("Settings")
                oldParentNode = rootNode.SelectSingleNode("BasicSettings")
            Else
                Dim xmlNode As XmlNode = settingsXml.CreateXmlDeclaration("1.0", "UTF-8", Nothing)
                settingsXml.AppendChild(xmlNode)
                rootNode = settingsXml.CreateElement("Settings")
                settingsXml.AppendChild(rootNode)
            End If

            parentNode = settingsXml.CreateElement("BasicSettings")
            childNode = settingsXml.CreateElement("vUSername")
            childNode.InnerText = txtVUsername.Text
            parentNode.AppendChild(childNode)

            childNode = settingsXml.CreateElement("vPassword")
            childNode.InnerText = txtVPwd.Text
            parentNode.AppendChild(childNode)

            childNode = settingsXml.CreateElement("vServer")
            childNode.InnerText = txtVServer.Text
            parentNode.AppendChild(childNode)

            childNode = settingsXml.CreateElement("AXService")
            childNode.InnerText = txtAxService.Text
            parentNode.AppendChild(childNode)

            childNode = settingsXml.CreateElement("AXBOMService")
            childNode.InnerText = txtBOMService.Text
            parentNode.AppendChild(childNode)

            childNode = settingsXml.CreateElement("AutoImport")
            childNode.InnerText = chkAutoImport.Checked
            parentNode.AppendChild(childNode)

            childNode = settingsXml.CreateElement("WaitTime")
            childNode.InnerText = txtDuration.Text
            parentNode.AppendChild(childNode)

            For Each row As DataGridViewRow In dgvVaultOrg.Rows
                childNode = settingsXml.CreateElement("VO")
                innerChildNode = settingsXml.CreateElement("Vault")
                innerChildNode.InnerText = row.Cells(0).Value
                childNode.AppendChild(innerChildNode)
                innerChildNode = settingsXml.CreateElement("Organisation")
                innerChildNode.InnerText = row.Cells(1).Value
                childNode.AppendChild(innerChildNode)
                parentNode.AppendChild(childNode)
            Next
            If oldParentNode IsNot Nothing Then
                rootNode.ReplaceChild(parentNode, oldParentNode)
            Else
                rootNode.AppendChild(parentNode)
            End If
            Dim dir As String = IO.Path.GetDirectoryName(settingsFile)
            If Not System.IO.Directory.Exists(dir) Then
                System.IO.Directory.CreateDirectory(dir)
            End If
            settingsXml.Save(settingsFile)
            MessageBox.Show("Basic settings saved successfully!" + Environment.NewLine + settingsFile, strMsgCaption, MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            MessageBox.Show("Unable to save Basic settings! Kindly try again", strMsgCaption, MessageBoxButtons.OK, MessageBoxIcon.Information)
        End Try
    End Sub

    Private Sub btnTab2Save_Click(sender As Object, e As EventArgs) Handles btnTab2Save.Click
        Try
            Dim settingsXml As New XmlDocument
            Dim rootNode, parentNode, childNode, oldParentNode, innerChildNode As XmlNode
            oldParentNode = Nothing
            If System.IO.File.Exists(settingsFile) Then
                settingsXml.Load(settingsFile)
                rootNode = settingsXml.SelectSingleNode("Settings")
                oldParentNode = rootNode.SelectSingleNode("ItemPropertyMapping")
            Else
                Dim xmlNode As XmlNode = settingsXml.CreateXmlDeclaration("1.0", "UTF-8", Nothing)
                settingsXml.AppendChild(xmlNode)
                rootNode = settingsXml.CreateElement("Settings")
                settingsXml.AppendChild(rootNode)
            End If
            parentNode = settingsXml.CreateElement("ItemPropertyMapping")
            For Each row As DataGridViewRow In dgvPropMapping.Rows
                childNode = settingsXml.CreateElement("AXVaultItem")
                For Each column As DataGridViewColumn In dgvPropMapping.Columns
                    innerChildNode = settingsXml.CreateElement(column.HeaderText.Replace(" ", "_"))
                    innerChildNode.InnerText = row.Cells(column.Name).Value
                    childNode.AppendChild(innerChildNode)
                Next
                parentNode.AppendChild(childNode)
            Next
            If oldParentNode IsNot Nothing Then
                rootNode.ReplaceChild(parentNode, oldParentNode)
            Else
                rootNode.AppendChild(parentNode)
            End If
            Dim dir As String = IO.Path.GetDirectoryName(settingsFile)
            If Not System.IO.Directory.Exists(dir) Then
                System.IO.Directory.CreateDirectory(dir)
            End If
            settingsXml.Save(settingsFile)
            MessageBox.Show("AX - Vault Item Property Mapping saved successfully!", strMsgCaption, MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            MessageBox.Show("Unable to save AX - Vault Item Property Mapping! Kindly try again", strMsgCaption, MessageBoxButtons.OK, MessageBoxIcon.Information)
        End Try
    End Sub

    Private Sub btnTab3Save_Click(sender As Object, e As EventArgs) Handles btnTab3Save.Click
        Try
            Dim settingsXml As New XmlDocument
            Dim rootNode, parentNode, childNode, oldParentNode, innerChildNode As XmlNode
            oldParentNode = Nothing
            If System.IO.File.Exists(settingsFile) Then
                settingsXml.Load(settingsFile)
                rootNode = settingsXml.SelectSingleNode("Settings")
                oldParentNode = rootNode.SelectSingleNode("VaultJobs")
            Else
                Dim xmlNode As XmlNode = settingsXml.CreateXmlDeclaration("1.0", "UTF-8", Nothing)
                settingsXml.AppendChild(xmlNode)
                rootNode = settingsXml.CreateElement("Settings")
                settingsXml.AppendChild(rootNode)
            End If
            parentNode = settingsXml.CreateElement("VaultJobs")
            childNode = settingsXml.CreateElement("ItemState")
            childNode.InnerText = txtAfterExportItem.Text
            parentNode.AppendChild(childNode)

            childNode = settingsXml.CreateElement("BOMProp")
            childNode.InnerText = txtAfterExportBOM.Text
            parentNode.AppendChild(childNode)
            If oldParentNode IsNot Nothing Then
                rootNode.ReplaceChild(parentNode, oldParentNode)
            Else
                rootNode.AppendChild(parentNode)
            End If
            Dim dir As String = IO.Path.GetDirectoryName(settingsFile)
            If Not System.IO.Directory.Exists(dir) Then
                System.IO.Directory.CreateDirectory(dir)
            End If
            settingsXml.Save(settingsFile)
            MessageBox.Show("AX - Vault job settings saved successfully!", strMsgCaption, MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            MessageBox.Show("Unable to save AX - Vault job settings! Kindly try again", strMsgCaption, MessageBoxButtons.OK, MessageBoxIcon.Information)
        End Try
    End Sub

    Private Sub AddItemJob_Click(sender As Object, e As EventArgs) Handles AddItemJob.Click
        _jobType = "FavelleVaultJobs.ExportItem.LifeCycleStateChange"
        If _jobType IsNot String.Empty And Not lst_FileJobs.Items.Contains(_jobType) Then
            AddJobEvent()
            CommitChanges()
        End If
    End Sub

    Private Sub AddBOMExportJobToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AddBOMExportJobToolStripMenuItem.Click
        _jobType = "FavelleVaultJobs.ExportBOM.LifeCycleStateChange"
        If _jobType IsNot String.Empty And Not lst_FileJobs.Items.Contains(_jobType) Then
            AddJobEvent()
            CommitChanges()
        End If
    End Sub

    Private Sub DeleteItemJob_Click(sender As Object, e As EventArgs) Handles DeleteItemJob.Click
        DeleteJobEvent()
        CommitChanges()

        Dim transition As LfCycTransListItem = TryCast(lst_FileTransition.SelectedItem, LfCycTransListItem)
        If transition Is Nothing OrElse lst_FileJobs.SelectedItem Is Nothing Then
            Return
        End If
        lst_FileJobs.Items.RemoveAt(lst_FileJobs.SelectedIndex)
        UpdatePendingChanges()
        CommitChanges()
    End Sub

    Public Sub CommitChanges() Implements ILifeCycleEvent.CommitChanges
        If m_pendingChanges.Count = 0 OrElse m_committingChanges Then
            Return
        End If
        m_committingChanges = True
        Try
            Dim enumerator As Dictionary(Of Long, StringArray).Enumerator = m_pendingChanges.GetEnumerator()
            Dim i As Integer = 0
            While enumerator.MoveNext()
                vCon.WebServiceManager.LifeCycleService.UpdateLifeCycleStateTransitionJobTypes(enumerator.Current.Key, enumerator.Current.Value.Items)
                i += 1
            End While
            m_pendingChanges.Clear()
        Catch ex As Exception
            MessageBox.Show("Error Code:" & ex.Message.ToString(), "Vault Exception", MessageBoxButtons.OK, MessageBoxIcon.Information)
            lst_FileJobs.Items.Remove(_jobType)
        End Try
        m_committingChanges = False
    End Sub


    Public Sub AddJobEvent() Implements ILifeCycleEvent.AddJobEvent
        If m_committingChanges Then
            Return
        End If

        Dim transition As LfCycTransListItem = TryCast(lst_FileTransition.SelectedItem, LfCycTransListItem)
        If transition Is Nothing Then
            Return
        End If
        lst_FileJobs.Items.Add(_jobType)
        UpdatePendingChanges()
    End Sub

    Public ReadOnly Property CanAddJobEvent As Boolean Implements ILifeCycleEvent.CanAddJobEvent
        Get
            Dim transition As LfCycTransListItem = TryCast(lst_FileTransition.SelectedItem, LfCycTransListItem)
            If transition Is Nothing Then
                Return False
            Else
                Return True
            End If
        End Get
    End Property

    Public ReadOnly Property CanDeleteJobEvent As Boolean Implements ILifeCycleEvent.CanDeleteJobEvent
        Get
            Dim transition As LfCycTransListItem = TryCast(lst_FileTransition.SelectedItem, LfCycTransListItem)
            If transition Is Nothing Then 'OrElse m_jobTypesListBox.SelectedItem Is Nothing Then
                Return False
            Else
                Return True
            End If
        End Get
    End Property

    Public Sub DeleteJobEvent() Implements ILifeCycleEvent.DeleteJobEvent
        If m_pendingChanges.Count = 0 OrElse m_committingChanges Then
            Return
        End If

        m_committingChanges = True

        Try
            Dim enumerator As Dictionary(Of Long, StringArray).Enumerator = m_pendingChanges.GetEnumerator()
            Dim i As Integer = 0
            While enumerator.MoveNext()
                vCon.WebServiceManager.LifeCycleService.UpdateLifeCycleStateTransitionJobTypes(enumerator.Current.Key, enumerator.Current.Value.Items)
                i += 1
            End While

            m_pendingChanges.Clear()
        Catch ex As Exception
            ' Show the error on the UI Thread
            'Me.BeginInvoke(DirectCast(Function() Do MessageBox.Show(ex.ToString())End Function, MethodInvoker))
        End Try

        m_committingChanges = False
    End Sub

    Public ReadOnly Property HasPendingChanges As Boolean Implements ILifeCycleEvent.HasPendingChanges
        Get
            Return (m_pendingChanges IsNot Nothing AndAlso m_pendingChanges.Count > 0)
        End Get
    End Property

    Public ReadOnly Property IsRunning As Boolean Implements ILifeCycleEvent.IsRunning
        Get
            Return Not m_currentThreadId.Equals(Guid.Empty)
        End Get
    End Property

    Public Sub PopulateUI() Implements ILifeCycleEvent.PopulateUI
        If m_idsToStates Is Nothing Then
            m_idsToStates = New Dictionary(Of Long, LfCycState)()
            m_transIdsToEvents = New Dictionary(Of Long, StringArray)()
            m_pendingChanges = New Dictionary(Of Long, StringArray)()
            Dim o As New Object
            PopulateUIWorker(o)
            'ThreadPool.QueueUserWorkItem(New WaitCallback(AddressOf PopulateUIWorker))
        End If
    End Sub

    Private Sub btnTab4Save_Click(sender As Object, e As EventArgs) Handles btnTab4Save.Click
        Try
            Dim settingsXml As New XmlDocument
            Dim rootNode, parentNode, childNode, oldParentNode, innerChildNode As XmlNode
            oldParentNode = Nothing
            If System.IO.File.Exists(settingsFile) Then
                settingsXml.Load(settingsFile)
                rootNode = settingsXml.SelectSingleNode("Settings")
                oldParentNode = rootNode.SelectSingleNode("BomPropertyMapping")
            Else
                Dim xmlNode As XmlNode = settingsXml.CreateXmlDeclaration("1.0", "UTF-8", Nothing)
                settingsXml.AppendChild(xmlNode)
                rootNode = settingsXml.CreateElement("Settings")
                settingsXml.AppendChild(rootNode)
            End If
            parentNode = settingsXml.CreateElement("BomPropertyMapping")
            For Each row As DataGridViewRow In dgvBOMPropMapping.Rows
                childNode = settingsXml.CreateElement("AXVaultBOM")
                For Each column As DataGridViewColumn In dgvBOMPropMapping.Columns
                    innerChildNode = settingsXml.CreateElement(column.HeaderText.Replace(" ", "_"))
                    innerChildNode.InnerText = row.Cells(column.Name).Value
                    childNode.AppendChild(innerChildNode)
                Next
                parentNode.AppendChild(childNode)
            Next
            If oldParentNode IsNot Nothing Then
                rootNode.ReplaceChild(parentNode, oldParentNode)
            Else
                rootNode.AppendChild(parentNode)
            End If
            Dim dir As String = IO.Path.GetDirectoryName(settingsFile)
            If Not System.IO.Directory.Exists(dir) Then
                System.IO.Directory.CreateDirectory(dir)
            End If
            settingsXml.Save(settingsFile)
            MessageBox.Show("AX - Vault BOM Property Mapping saved successfully!", strMsgCaption, MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            MessageBox.Show("Unable to save AX - Vault BOM Property Mapping! Kindly try again", strMsgCaption, MessageBoxButtons.OK, MessageBoxIcon.Information)
        End Try
    End Sub

    Private Sub btnvUserSave_Click(sender As Object, e As EventArgs) Handles btnvUserSave.Click
        Try
            Dim settingsXml As New XmlDocument
            Dim rootNode, parentNode, childNode, oldParentNode, innerChildNode As XmlNode
            oldParentNode = Nothing
            If System.IO.File.Exists(settingsFile) Then
                settingsXml.Load(settingsFile)
                rootNode = settingsXml.SelectSingleNode("Settings")
                oldParentNode = rootNode.SelectSingleNode("VaultUsers")
            Else
                Dim xmlNode As XmlNode = settingsXml.CreateXmlDeclaration("1.0", "UTF-8", Nothing)
                settingsXml.AppendChild(xmlNode)
                rootNode = settingsXml.CreateElement("Settings")
                settingsXml.AppendChild(rootNode)
            End If
            parentNode = settingsXml.CreateElement("VaultUsers")
            For Each row As DataGridViewRow In dgvUsers.Rows
                childNode = settingsXml.CreateElement("UserDetails")
                For Each column As DataGridViewColumn In dgvUsers.Columns
                    innerChildNode = settingsXml.CreateElement(column.HeaderText.Replace(" ", "_"))
                    innerChildNode.InnerText = row.Cells(column.Name).Value
                    childNode.AppendChild(innerChildNode)
                Next
                parentNode.AppendChild(childNode)
            Next
            If oldParentNode IsNot Nothing Then
                rootNode.ReplaceChild(parentNode, oldParentNode)
            Else
                rootNode.AppendChild(parentNode)
            End If
            Dim dir As String = IO.Path.GetDirectoryName(settingsFile)
            If Not System.IO.Directory.Exists(dir) Then
                System.IO.Directory.CreateDirectory(dir)
            End If
            settingsXml.Save(settingsFile)
            MessageBox.Show("AX - Vault User Mapping saved successfully!", strMsgCaption, MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            MessageBox.Show("Unable to save AX - Vault User Mapping! Kindly try again", strMsgCaption, MessageBoxButtons.OK, MessageBoxIcon.Information)
        End Try
    End Sub

    Private Sub chkAutoImport_CheckedChanged(sender As Object, e As EventArgs) Handles chkAutoImport.CheckedChanged
            txtDuration.Enabled = chkAutoImport.Checked
    End Sub

    Private Sub lst_FileJobs_SelectedIndexChanged(sender As Object, e As EventArgs) Handles lst_FileJobs.SelectedIndexChanged

    End Sub

    Private Sub dgvUsers_DataError(sender As Object, e As DataGridViewDataErrorEventArgs) Handles dgvUsers.DataError
        Try

        Catch ex As Exception

        End Try
    End Sub

    Private Sub btnCATAdd_Click(sender As Object, e As EventArgs) Handles btnCATAdd.Click
        If txtCATAX.Text.Trim() <> String.Empty And txtCATVault.Text <> String.Empty Then
            dgCAT.Rows.Add(txtCATAX.Text, txtCATVault.Text)
            ClearCat()
        End If
    End Sub

    Private Sub btnCATEdit_Click(sender As Object, e As EventArgs) Handles btnCATEdit.Click
        If txtCATAX.Text.Trim() <> String.Empty And txtCATVault.Text <> String.Empty Then
            Dim selIndex = dgCAT.SelectedRows.Item(0).Index
            dgCAT.Rows(selIndex).Cells(0).Value = txtCATAX.Text
            dgCAT.Rows(selIndex).Cells(1).Value = txtCATVault.Text
            ClearCat()
        End If
    End Sub

    Private Sub btnCATDelete_Click(sender As Object, e As EventArgs) Handles btnCATDelete.Click
        If dgCAT.SelectedRows IsNot Nothing Then
            Dim selIndex = dgCAT.SelectedRows.Item(0).Index
            dgCAT.Rows.RemoveAt(selIndex)
            ClearCat()
        End If
    End Sub

    Private Sub btnCATClear_Click(sender As Object, e As EventArgs) Handles btnCATClear.Click
        ClearCat()
    End Sub

    Private Sub btnCATSave_Click(sender As Object, e As EventArgs) Handles btnCATSave.Click
        Try
            Dim settingsXml As New XmlDocument
            Dim rootNode, parentNode, childNode, oldParentNode, innerChildNode As XmlNode
            oldParentNode = Nothing
            If System.IO.File.Exists(settingsFile) Then
                settingsXml.Load(settingsFile)
                rootNode = settingsXml.SelectSingleNode("Settings")
                oldParentNode = rootNode.SelectSingleNode("CATValueMapping")
            Else
                Dim xmlNode As XmlNode = settingsXml.CreateXmlDeclaration("1.0", "UTF-8", Nothing)
                settingsXml.AppendChild(xmlNode)
                rootNode = settingsXml.CreateElement("Settings")
                settingsXml.AppendChild(rootNode)
            End If
            parentNode = settingsXml.CreateElement("CATValueMapping")
            For Each row As DataGridViewRow In dgCAT.Rows
                childNode = settingsXml.CreateElement("CAT")
                For Each column As DataGridViewColumn In dgCAT.Columns
                    innerChildNode = settingsXml.CreateElement(column.Name)
                    innerChildNode.InnerText = row.Cells(column.Name).Value
                    childNode.AppendChild(innerChildNode)
                Next
                parentNode.AppendChild(childNode)
            Next
            If oldParentNode IsNot Nothing Then
                rootNode.ReplaceChild(parentNode, oldParentNode)
            Else
                rootNode.AppendChild(parentNode)
            End If
            Dim dir As String = IO.Path.GetDirectoryName(settingsFile)
            If Not System.IO.Directory.Exists(dir) Then
                System.IO.Directory.CreateDirectory(dir)
            End If
            settingsXml.Save(settingsFile)
            MessageBox.Show("AX - Vault CAT value mapping saved successfully!", strMsgCaption, MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            MessageBox.Show("Unable to save AX - Vault CAT value mapping! Kindly try again", strMsgCaption, MessageBoxButtons.OK, MessageBoxIcon.Information)
        End Try
    End Sub

    Private Sub ClearCat()
        txtCATAX.Clear()
        txtCATVault.Clear()
        btnCATAdd.Enabled = True
        btnCATEdit.Enabled = False
        btnCATDelete.Enabled = False
        txtCATAX.Focus()
    End Sub

    Private Sub dgCAT_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgCAT.CellClick
        If dgCAT.SelectedRows IsNot Nothing Then
            Dim selIndex = dgCAT.SelectedRows.Item(0).Index
            btnCATAdd.Enabled = False
            btnCATEdit.Enabled = True
            btnCATDelete.Enabled = True
            txtCATAX.Text = dgCAT.Rows(selIndex).Cells(0).Value
            txtCATVault.Text = dgCAT.Rows(selIndex).Cells(1).Value
        End If
    End Sub
End Class