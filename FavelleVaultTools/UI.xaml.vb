
Imports System.Data
Imports VDF = Autodesk.DataManagement.Client.Framework
Imports Autodesk.DataManagement.Client.Framework.Vault.Currency.Connections
Imports System.Xml
Imports Autodesk.Connectivity.WebServices
Imports System.Windows
Imports Autodesk.DataManagement.Client.Framework.Vault
Imports System.Net.Mail
Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports System.Globalization
Imports System.Reflection
Imports FavelleVaultTools.FavelleVaultTools

Public Class UI
    Inherits Window
    Implements ILifeCycleEvent

    Public Shared _settingFileName As String = "Settings.xml"
    Public Shared _dirSep As String = System.IO.Path.DirectorySeparatorChar.ToString()
    Public Shared _outputPath As String = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + _dirSep + "KKMiVault" + _dirSep

    Dim vConnection As Connection
    Dim _jobType As String = String.Empty
    ' Dim dtSubFolders, dtMaterialSpec, dtNamingScheme, dtProjects, dtLibNamingScheme, dtCategory, dtVaultUsers, dtDrawingNotes, dtFilePublishOption, dtItemPublishOption As New DataTable
    Private vaultGroup As Dictionary(Of String, Long) = New Dictionary(Of String, Long)
    Public vaultFolderId As Long
    Dim selectedExportOption As String
    Dim vUsers() As User
    Dim vGroups() As Group
    Dim vGroupInfo As Dictionary(Of Long, User())
    Dim dtEmailNotification As New DataTable

    Public Sub New(ByVal _con As Connection)
        ' This call is required by the designer.
        Try
            InitializeComponent()

            If Not System.IO.Directory.Exists(_outputPath) Then
                System.IO.Directory.CreateDirectory(_outputPath)
            End If
            ' Add any initialization after the InitializeComponent() call.
            vConnection = _con
            If CommandExtension.isAdministrator Then
                AddColumns()

                PopulateUI()
                Me.DataContext = Me
                GetAllUsers()
                GetAllGroups()
            End If
        Catch ex As Exception

        End Try
    End Sub
    Private Sub AddColumns()
        dtEmailNotification.Columns.Add("TransistionId", GetType(Long))
        dtEmailNotification.Columns.Add("Users")
    End Sub

    Public Sub GetAllUsers()
        vUsers = vConnection.WebServiceManager.AdminService.GetAllUsers()
    End Sub

    Public Sub GetAllGroups()
        vGroups = vConnection.WebServiceManager.AdminService.GetAllGroups()
        vGroupInfo = New Dictionary(Of Long, User())
        For Each grp As Group In vGroups
            Dim usr = vConnection.WebServiceManager.AdminService.GetMemberUsersByGroupId(grp.Id)
            vGroupInfo.Add(grp.Id, usr)
        Next
    End Sub

#Region "Download File"

    Public Function DownloadFile(ByVal _filename As String, ByVal isCheckout As Boolean) As Boolean
        ' download individual files to a temp location
        Try

            If System.IO.File.Exists(_outputPath + _filename) Then
                Dim oFileInfo As New IO.FileInfo(_outputPath + _filename)
                If (oFileInfo.Attributes And IO.FileAttributes.ReadOnly) > 0 Then
                    oFileInfo.Attributes = oFileInfo.Attributes Xor IO.FileAttributes.ReadOnly
                    System.IO.File.Delete(_outputPath + _filename)
                Else
                    System.IO.File.Delete(_outputPath + _filename)
                End If
            End If

            Dim oFileIteration As VDF.Vault.Currency.Entities.FileIteration = getFileIteration(_filename, vConnection)
            Dim settings As New VDF.Vault.Settings.AcquireFilesSettings(vConnection)
            settings.LocalPath = New VDF.Currency.FolderPathAbsolute(_outputPath)

            ' For Each fileIter As VDF.Vault.Currency.Entities.FileIteration In fileIters
            If isCheckout Then
                settings.AddFileToAcquire(oFileIteration, VDF.Vault.Settings.AcquireFilesSettings.AcquisitionOption.Checkout Or VDF.Vault.Settings.AcquireFilesSettings.AcquisitionOption.Download)
            Else
                settings.AddFileToAcquire(oFileIteration, VDF.Vault.Settings.AcquireFilesSettings.AcquisitionOption.Download)
            End If

            ' Next
            Dim results As VDF.Vault.Results.AcquireFilesResults = vConnection.FileManager.AcquireFiles(settings)

            If results.FileResults(0).Status.Equals(VDF.Vault.Results.FileAcquisitionResult.AcquisitionStatus.Success) Then
                Return True
            Else
                Return False
            End If
        Catch ex As Exception
            Return False
        End Try
    End Function

    Public Function getFileIterationWithFolderId(ByVal nameOfFile As String, ByVal folderId As Long) _
                                                                     As VDF.Vault.Currency.Entities.FileIteration
        Dim conditions As SrchCond()

        ReDim conditions(0)

        Dim lCode As Long = 3

        Dim Defs As PropDef() = vConnection.WebServiceManager.PropertyService.GetPropertyDefinitionsByEntityClassId("FILE")

        Dim Prop As PropDef = Nothing

        For Each def As PropDef In Defs
            If def.DispName = "File Name" Then
                Prop = def
                Exit For
            End If
        Next def




        Dim searchCondition As SrchCond = New SrchCond()
        searchCondition.PropDefId = Prop.Id

        searchCondition.PropTyp = PropertySearchType.SingleProperty
        searchCondition.SrchOper = lCode

        searchCondition.SrchTxt = nameOfFile

        conditions(0) = searchCondition

        ' search for files
        Dim FileList As List(Of Autodesk.Connectivity.WebServices.File) = New List(Of Autodesk.Connectivity.WebServices.File)()
        Dim sBookmark As String = String.Empty
        Dim Status As SrchStatus = Nothing

        While (Status Is Nothing OrElse FileList.Count < Status.TotalHits)

            Dim files As Autodesk.Connectivity.WebServices.File() = vConnection.WebServiceManager.
                                                      DocumentService.FindFilesBySearchConditions(conditions,
                                                             Nothing, Nothing, True, True, sBookmark, Status)

            If (Not files Is Nothing) Then
                FileList.AddRange(files)
            End If
        End While

        Dim oFileIteration As VDF.Vault.Currency.Entities.FileIteration = Nothing
        For i As Integer = 0 To FileList.Count - 1
            If FileList(i).Name = nameOfFile And FileList(i).FolderId = folderId Then
                oFileIteration = New VDF.Vault.Currency.Entities.FileIteration(vConnection, FileList(i))
            End If
        Next

        Return oFileIteration

    End Function

    Public Function getFileIteration(ByVal nameOfFile As String, ByVal connection As VDF.Vault.Currency.Connections.Connection) _
                                                                       As VDF.Vault.Currency.Entities.FileIteration
        Dim conditions As SrchCond()

        ReDim conditions(0)

        Dim lCode As Long = 3

        Dim Defs As PropDef() = connection.WebServiceManager.PropertyService.GetPropertyDefinitionsByEntityClassId("FILE")

        Dim Prop As PropDef = Nothing

        For Each def As PropDef In Defs
            If def.DispName = "File Name" Then
                Prop = def
                Exit For
            End If
        Next def



        Dim searchCondition As SrchCond = New SrchCond()
        searchCondition.PropDefId = Prop.Id

        searchCondition.PropTyp = PropertySearchType.SingleProperty
        searchCondition.SrchOper = lCode

        searchCondition.SrchTxt = nameOfFile

        conditions(0) = searchCondition

        ' search for files
        Dim FileList As List(Of Autodesk.Connectivity.WebServices.File) = New List(Of Autodesk.Connectivity.WebServices.File)()
        Dim sBookmark As String = String.Empty
        Dim Status As SrchStatus = Nothing

        While (Status Is Nothing OrElse FileList.Count < Status.TotalHits)

            Dim files As Autodesk.Connectivity.WebServices.File() = connection.WebServiceManager.
                                                      DocumentService.FindFilesBySearchConditions(conditions,
                                                             Nothing, Nothing, True, True, sBookmark, Status)

            If (Not files Is Nothing) Then
                FileList.AddRange(files)
            End If
        End While

        Dim oFileIteration As VDF.Vault.Currency.Entities.FileIteration = Nothing
        For i As Integer = 0 To FileList.Count - 1
            If FileList(i).Name = nameOfFile Then
                oFileIteration = New VDF.Vault.Currency.Entities.FileIteration(connection, FileList(i))
            End If
        Next

        Return oFileIteration

    End Function

#End Region

    Public Function SaveSettingsInVault()
        Try
            Dim filePath As String = _outputPath + _settingFileName
            If System.IO.File.Exists(filePath) Then
                Dim vaultFolderId As Long = 0
                Try
                    Dim folderpath() As String = {"$/KKMiVault"}
                    Dim _folder() As Folder = vConnection.WebServiceManager.DocumentService.FindFoldersByPaths(folderpath)
                    If Not _folder.Count.Equals(0) AndAlso _folder(0).Id <> -1 Then
                        vaultFolderId = _folder(0).Id
                    Else
                        Dim rootFolderId As Long = vConnection.WebServiceManager.DocumentService.GetFolderRoot().Id
                        Dim settingsFolder = vConnection.WebServiceManager.DocumentService.AddFolder("KKMiVault", rootFolderId, False)
                        If (settingsFolder IsNot Nothing) Then
                            vaultFolderId = settingsFolder.Id
                        Else
                            Return False
                        End If
                    End If
                Catch ex As Exception

                End Try

                Dim str() As String = {"$/KKMiVault/Settings.xml"}
                Dim _settingsFile() As File = vConnection.WebServiceManager.DocumentService.FindLatestFilesByPaths(str)

                If Not _settingsFile.Count.Equals(0) AndAlso _settingsFile(0).Id <> -1 Then
                    ' vConnection.WebServiceManager.DocumentService.DeleteFileFromFolderUnconditional(_settingsFile(0).MasterId, vaultFolderId)

                    Dim oFileIteration As VDF.Vault.Currency.Entities.FileIteration = getFileIteration("Settings.xml", vConnection)
                    vConnection.FileManager.CheckinFile(oFileIteration, "Settings File.", False, Nothing, Nothing, True, String.Empty, oFileIteration.FileClassification, oFileIteration.IsHidden, New VDF.Currency.FilePathAbsolute(filePath))
                Else
                    Dim folderEntity As New Currency.Entities.Folder(vConnection, vConnection.WebServiceManager.DocumentService.GetFolderById(vaultFolderId))
                    Dim filePathAbs As VDF.Currency.FilePathAbsolute = New VDF.Currency.FilePathAbsolute(filePath)

                    vConnection.FileManager.AddFile(folderEntity, "Settings File.", Nothing, Nothing, FileClassification.None, False, filePathAbs)
                    'ElseIf _settingsFile(0).Id <> -1 Then
                    '    vConnection.WebServiceManager.DocumentService.DeleteFileFromFolderUnconditional(_settingsFile(0).MasterId, vaultFolderId)
                End If

            Else
                lblError.Content = "Settings file is not Configured! Kindly Try Again."
                lblError.Foreground = New SolidColorBrush(Colors.Red)
                Return False
            End If
        Catch ex As Exception
            Return False
        End Try
        Return True
    End Function

    Public Function GetFolderByPath(ByVal parFolderId As Long, ByVal folderPath As String, ByVal foldername As String) As Folder
        Dim _folder As Folder = Nothing
        Try
            _folder = vConnection.WebServiceManager.DocumentService.GetFolderByPath(folderPath + "/" + foldername)
        Catch ex As Exception

        End Try
        If _folder Is Nothing Then
            _folder = vConnection.WebServiceManager.DocumentService.AddFolder(foldername, parFolderId, False)
        End If
        Return _folder
    End Function


    Private Function GetFileFromFolderId(ByVal _folderId As Long) As Autodesk.Connectivity.WebServices.File
        Try
            Dim filename(0) As String
            Dim results As Autodesk.Connectivity.WebServices.File()

            results = vConnection.WebServiceManager.DocumentService.GetLatestFilesByFolderId(_folderId, True) ' EdmSecurity.Instance.VaultConnection.WebServiceManager.DocumentService.GetLatestFilePathsByNames(filename)

            For Each _file In results
                If _file.Name.Equals("Settings.xml", StringComparison.InvariantCultureIgnoreCase) Then
                    Return _file
                End If
            Next
        Catch ex As Exception

        End Try
        Return Nothing
    End Function







#Region "Publish PDF Settings"
    Private m_currentThreadId As Guid = Guid.Empty
    Private m_idsToStates As Dictionary(Of Long, LfCycState)
    Private m_transIdsToEvents As Dictionary(Of Long, StringArray)
    Private m_pendingChanges As Dictionary(Of Long, StringArray)
    Private m_committingChanges As Boolean = False

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

        jobArrayArray = vConnection.WebServiceManager.LifeCycleService.GetJobTypesByLifeCycleStateTransitionIds(transIds.ToArray())

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
        Try
            Dim transitionItem As LfCycTransListItem = TryCast(lst_FileTransition.SelectedItem, LfCycTransListItem)
            If transitionItem Is Nothing Then
                Return
            End If

            Dim transition As LfCycTrans = transitionItem.LfCycTrans
            If m_pendingChanges.ContainsKey(transition.Id) Then
                m_pendingChanges.Remove(transition.Id)
            End If

            Dim jobTypes As New StringArray()

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
        Catch ex As Exception

        End Try
    End Sub

#End Region

#Region "WORKER_THREADS"

    Public Sub PopulateUIWorker(ByVal o As Object)
        Try
            Dim lcDefs As LfCycDef() = Nothing
            Try
                ' get all the lifecycle information
                lcDefs = vConnection.WebServiceManager.LifeCycleService.GetAllLifeCycleDefinitions()
            Catch ex As Exception
                ' Show the error on the UI Thread

            End Try

            If lcDefs IsNot Nothing Then
                For Each lcDef As LfCycDef In lcDefs
                    cmbx_lifeCylceDef.Items.Add(New LfCycDefListItem(lcDef))
                Next
                If cmbx_lifeCylceDef.Items.Count > 0 Then
                    cmbx_lifeCylceDef.SelectedIndex = 0
                End If
            End If
        Catch ex As Exception

        End Try
    End Sub


    Private Sub UpdateStatesWorker(ByVal threadId As Guid, ByVal lcDef As LfCycDefListItem)
        Try
            Dim states As LfCycState() = Nothing
            If lcDef.LfCycDef IsNot Nothing Then
                states = lcDef.LfCycDef.StateArray
            End If
            Try
                If states IsNot Nothing Then
                    GetTransitionEvents(lcDef.LfCycDef)
                End If
            Catch ex As Exception

            End Try
            If threadId.Equals(m_currentThreadId) Then
                If states IsNot Nothing Then
                    For Each state As LfCycState In states
                        lst_FileState.Items.Add(New LfCycStateListItem(state))
                        m_idsToStates.Add(state.Id, state)
                    Next
                End If
                m_currentThreadId = Guid.Empty
            End If
        Catch ex As Exception

        End Try
    End Sub
#End Region

#Region "Interface Implementation"

    Public Sub AddJobEvent() Implements FavelleVaultTools.ILifeCycleEvent.AddJobEvent
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

    Public ReadOnly Property CanAddJobEvent As Boolean Implements FavelleVaultTools.ILifeCycleEvent.CanAddJobEvent
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

    Public Sub CommitChanges() Implements ILifeCycleEvent.CommitChanges
        If m_pendingChanges.Count = 0 OrElse m_committingChanges Then
            Return
        End If

        m_committingChanges = True

        Try
            Dim enumerator As Dictionary(Of Long, StringArray).Enumerator = m_pendingChanges.GetEnumerator()
            Dim i As Integer = 0
            While enumerator.MoveNext()
                vConnection.WebServiceManager.LifeCycleService.UpdateLifeCycleStateTransitionJobTypes(enumerator.Current.Key, enumerator.Current.Value.Items)
                i += 1
            End While

            m_pendingChanges.Clear()
        Catch ex As Exception
            ' Show the error on the UI Thread
            'Me.BeginInvoke(DirectCast(Function() Do MessageBox.Show(ex.ToString())End Function, MethodInvoker))
        End Try

        m_committingChanges = False
    End Sub

    Public Sub DeleteJobEvent() Implements ILifeCycleEvent.DeleteJobEvent
        Dim transition As LfCycTransListItem = TryCast(lst_FileTransition.SelectedItem, LfCycTransListItem)
        If transition Is Nothing OrElse lst_FileJobs.SelectedItem Is Nothing Then
            Return
        End If
        lst_FileJobs.Items.RemoveAt(lst_FileJobs.SelectedIndex)
        UpdatePendingChanges()
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
        Try
            If m_idsToStates Is Nothing Then
                m_idsToStates = New Dictionary(Of Long, LfCycState)()
                m_transIdsToEvents = New Dictionary(Of Long, StringArray)()
                m_pendingChanges = New Dictionary(Of Long, StringArray)()
                Dim o As New Object
                PopulateUIWorker(o)

                'ThreadPool.QueueUserWorkItem(New WaitCallback(AddressOf PopulateUIWorker))
            End If
        Catch ex As Exception

        End Try
    End Sub

#End Region

#End Region

    Private Sub cmbx_lifeCylceDef_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles cmbx_lifeCylceDef.SelectionChanged
        Try
            lst_FileJobs.Items.Clear()
            lst_FileTransition.Items.Clear()
            lst_FileState.Items.Clear()
            '  m_jobTypesListBox.Items.Clear()
            If (m_idsToStates IsNot Nothing) Then
                m_idsToStates.Clear()
            End If
            m_currentThreadId = Guid.NewGuid()

            UpdateStatesWorker(m_currentThreadId, cmbx_lifeCylceDef.SelectedItem)
        Catch ex As Exception

        End Try
    End Sub

    Private Sub lst_FileState_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles lst_FileState.SelectionChanged
        If lst_FileState.SelectedItem IsNot Nothing Then
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
        End If
    End Sub


    Private Sub lst_FileTransition_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles lst_FileTransition.SelectionChanged
        If lst_FileTransition.SelectedItem IsNot Nothing Then

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
        End If
    End Sub

    Private Sub AddJobClick(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles cmAdd.Click, cmAddBOM.Click
        _jobType = String.Empty
        If DirectCast(sender, System.Windows.Controls.MenuItem).Name.Equals("cmAdd", StringComparison.InvariantCultureIgnoreCase) Then
            _jobType = "FavelleVaultJobs.ExportItem.LifeCycleStateChange"
        ElseIf DirectCast(sender, System.Windows.Controls.MenuItem).Name.Equals("cmAddBOM", StringComparison.InvariantCultureIgnoreCase) Then
            _jobType = "FavelleVaultJobs.ExportBOM.LifeCycleStateChange"
        End If
        If _jobType IsNot String.Empty And Not lst_FileJobs.Items.Contains(_jobType) Then
            AddJobEvent()
            CommitChanges()
        End If
    End Sub


    Private Sub lst_FileJobs_PreviewMouseRightButtonDown(ByVal sender As System.Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles lst_FileJobs.PreviewMouseRightButtonDown
        If lst_FileTransition.SelectedItem Is Nothing Then
            cmAdd.IsEnabled = False
            cmAddBOM.IsEnabled = False
            cmDelete.IsEnabled = False
            mnuItemDeleteIco.Opacity = 0.5
            mnuItem.Opacity = 0.5
            mnuBOM.Opacity = 0.5
        Else
            If lst_FileJobs.SelectedItem Is Nothing Then
                cmDelete.IsEnabled = False
                mnuItemDeleteIco.Opacity = 0.5
            Else
                cmDelete.IsEnabled = True
                mnuItemDeleteIco.Opacity = 1
            End If
            cmAdd.IsEnabled = True
            cmAddBOM.IsEnabled = True
            mnuItem.Opacity = 1
            mnuBOM.Opacity = 1
            If lst_FileJobs.Items.Contains("FavelleVaultJobs.ExportItem.LifeCycleStateChange") Then
                cmAdd.IsEnabled = False
                mnuItem.Opacity = 0.5
            End If
            If lst_FileJobs.Items.Contains("FavelleVaultJobs.ExportBOM.LifeCycleStateChange") Then
                cmAddBOM.IsEnabled = False
                mnuBOM.Opacity = 0.5
            End If
        End If
    End Sub

    Private Sub DeleteJobClick(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles cmDelete.Click
        DeleteJobEvent()
        CommitChanges()
    End Sub

End Class


