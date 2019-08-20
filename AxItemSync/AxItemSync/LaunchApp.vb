Imports System.Threading
Imports System.ComponentModel
Imports System.Timers
Imports System.Diagnostics
Imports System.Runtime.InteropServices
Imports System.IO
Imports System.Xml.Linq
Imports System.Collections.Generic
Imports System.Text
Imports System.Net
Imports Autodesk.Connectivity.WebServices
Imports System.Data.OleDb
Imports Autodesk.DataManagement.Client.Framework.Vault.Currency.Connections
Imports Autodesk.Connectivity.WebServicesTools
Imports System.Xml

Module LaunchApp

    Public pop_up_closed = 1
    Public objSetting As Settings = New Settings()
    Public strMsgCaption As String = "Favelle - Vault2AX"

    Public CanDoListening As [Boolean] = True

    Public _dirSep As String = System.IO.Path.DirectorySeparatorChar.ToString()
    Public settingsFile As String = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + _dirSep + "Vault2AX" + _dirSep + "Settings.xml"

    Private InputFolderPath As [String] = String.Empty
    Private OutputFolderPath As [String] = String.Empty

    Private appPath As String = AppDomain.CurrentDomain.BaseDirectory
    Public columnNames() As String = My.Settings.dbColumns.Split(",") '.Select(Function(x) x.Trim())
    Public defaultProps() As String = My.Settings.vaultProps.Split(",") '.Select(Function(x) x.Trim())
    Public columnNamesBOM() As String = My.Settings.dbBomColumns.Split(",")
    Public defaultPropsBOM() As String = My.Settings.vaultBomProps.Split(",")
    Public vCon As Connection
    Private vUsername, vPwd, vServer, vName As String
    Private autoImport As Boolean = False
    Private waitTime As Integer
    Private dtItemProp As DataTable
    Dim dtCSV As DataTable
    Public Const importJob As String = "FavelleVaultJobs.ImportItem.AutoSync"
    Dim lstProps As New List(Of String)
    Dim _timer As System.Timers.Timer

    Public Sub Main()
        'Turn visual styles back on
        Application.EnableVisualStyles()

        startTimer()

        'Run the application using AppContext
        Application.Run(New AppContext)

        ''You can also run the application using a main form
        'Application.Run(New MainForm)

        ''Or in a default context with no user interface at all
        'Application.Run()
    End Sub

    Public Sub startTimer()
        InitialLoad()
        If autoImport Then
            _timer = New System.Timers.Timer(waitTime * 60000)
            AddHandler _timer.Elapsed, AddressOf timer_Elapsed
            _timer.Start()
        End If
    End Sub

    Public Sub RWDeleteFile(ByVal file As String)
        Try
            If IO.File.Exists(file) Then
                Dim oFileInfo As New IO.FileInfo(file)
                If (oFileInfo.Attributes And IO.FileAttributes.ReadOnly) > 0 Then
                    oFileInfo.Attributes = oFileInfo.Attributes Xor IO.FileAttributes.ReadOnly
                End If
                System.IO.File.Delete(file)
            End If
        Catch ex As Exception
            WriteLog(ex.Message.ToString)
        End Try
    End Sub

    Public Sub timer_Elapsed(sender As Object, e As System.Timers.ElapsedEventArgs)
        If CanDoListening = True Then
            CanDoListening = False
            Try
                If SetConnection(vServer, vName, vUsername, vPwd) Then
                    Dim paramList As JobParam() = New JobParam(0) {}
                    Dim vUser As New JobParam()
                    vUser.Name = "vUser"
                    vUser.Val = vUsername ' vaultObj.Id.ToString()
                    paramList(0) = vUser
                    vCon.WebServiceManager.JobService.AddJob(importJob, "Import Items from AX - Automatic Sync", paramList, 1)
                End If
            Catch ex As Exception
                WriteLog(ex.Message.ToString)
            End Try
            CanDoListening = True
        End If
    End Sub
    Public Function SetConnection(ByVal vaultServer As String, ByVal vaultName As String, ByVal vaultUsername As String, ByVal vaultPassword As String) As Boolean
        Try
            Dim adminCred As UserPasswordCredentials = New UserPasswordCredentials(vaultServer, vaultName, vaultUsername, vaultPassword)
            Dim adminServiceManager As WebServiceManager = New WebServiceManager(adminCred)
            Dim userId As Long = adminCred.SecurityHeader.UserId
            vCon = New Connection(adminServiceManager, vaultUsername, vaultPassword, vaultName, userId, vaultServer, _
                AuthenticationFlags.Standard)
            Return True
        Catch ex As Exception
            WriteLog(ex.Message.ToString)
            Return False
        End Try
    End Function

    Private Sub InitialLoad()
        Try
            If System.IO.File.Exists(settingsFile) Then
                LoadBasicSettings()
            End If
        Catch ex As Exception
            WriteLog(ex.Message.ToString)
        End Try
    End Sub

    Private Sub LoadBasicSettings()
        Try
            Dim settingsXml As New XmlDocument
            Dim rootNode, xnBasicSettings As XmlNode
            settingsXml.Load(settingsFile)
            rootNode = settingsXml.SelectSingleNode("Settings")
            xnBasicSettings = rootNode.SelectSingleNode("BasicSettings")
            vUsername = xnBasicSettings.SelectSingleNode("vUSername").InnerText
            vPwd = xnBasicSettings.SelectSingleNode("vPassword").InnerText
            vServer = xnBasicSettings.SelectSingleNode("vServer").InnerText
            ' txtAxService.Text = xnBasicSettings.SelectSingleNode("AXService").InnerText
            
            For Each node As XmlNode In xnBasicSettings.SelectNodes("VO")
                vName = node.SelectSingleNode("Vault").InnerText
            Next
            Boolean.TryParse(xnBasicSettings.SelectSingleNode("AutoImport").InnerText, autoImport)
            Integer.TryParse(xnBasicSettings.SelectSingleNode("WaitTime").InnerText, waitTime)
        Catch ex As Exception

        End Try
    End Sub

    Public Sub WriteLog(ByVal str As String)
        Try
            Dim log As StreamWriter
            If Not System.IO.File.Exists(appPath & "logfile.txt") Then
                log = New StreamWriter(appPath & "logfile.txt")
            Else
                log = System.IO.File.AppendText(appPath & "logfile.txt")
            End If
            log.WriteLine()
            log.WriteLine(Now.ToString() & Environment.NewLine & str)
            log.Close()
        Catch ex As Exception

        End Try
    End Sub

#Region "Vault"

    Private Function UpdateItemProperties() As Boolean
        Try
            Dim props() As PropDef = vCon.WebServiceManager.PropertyService.GetPropertyDefinitionsByEntityClassId("ITEM")

            Dim vaultProp() As PropDef = (From prop In props
                                       Where lstProps.Contains(prop.DispName, StringComparer.InvariantCultureIgnoreCase).Equals(True)
                                         Select prop).ToArray() ' prop.DispName.Equals("Revision", StringComparison.InvariantCultureIgnoreCase).Equals(True) Or
            Dim vPropIds() As Long = (From prps In vaultProp
                                     Select prps.Id).ToArray()
            Dim tobeModifiedCount = dtItemProp.Select("ToBeAdded = " & False).Count()
            '   Dim prpParmArray(tobeModifiedCount) As PropInstParamArray
            '  Dim index As Integer
            Dim itemIndex = 0
            Dim itemIds As New List(Of Long)
            Dim propInstParams As New List(Of PropInstParam)
            Dim propInstParamsArrays(tobeModifiedCount - 1) As PropInstParamArray
            Dim newpropInstParamsArrays As New List(Of PropInstParamArray)

            For Each dr As DataRow In dtCSV.Rows
                If dr("Number").ToString() <> String.Empty Then
                    propInstParams = New List(Of PropInstParam)
                    For Each tempRow As DataRow In dtItemProp.Select("Number = '" + dr("Number").ToString() + "'")
                        If tempRow("ToBeAdded").ToString().Equals("False", StringComparison.InvariantCultureIgnoreCase) Then
                            Dim myItem As Item = vCon.WebServiceManager.ItemService.GetLatestItemByItemNumber(tempRow("Number"))
                            'If myItem.LfCycStateId <> 24 Then
                            '    Continue For
                            'End If
                            itemIds.Add(Convert.ToDouble(tempRow("RevId").ToString()))
                            '   Dim prpInstParam As New PropInstParam
                            ' index = 0
                            For Each vProp In vaultProp
                                For Each prp As PropDef In props.Where(Function(T) T.Id.Equals(vProp.Id))
                                    Dim propInst As New PropInstParam()

                                    propInst.PropDefId = prp.Id
                                    propInst.Val = dr.Item(prp.DispName.ToString()).ToString()   ' .Replace("_", " ")).ToString()
                                    If (propInst.Val Is Nothing) Then
                                        propInst.Val = String.Empty
                                    End If
                                    propInstParams.Add(propInst)
                                    'index = index + 1
                                Next
                            Next
                            Dim propInstParamsArray As New PropInstParamArray()
                            propInstParamsArray.Items = propInstParams.ToArray()
                            propInstParamsArrays(itemIndex) = propInstParamsArray
                            newpropInstParamsArrays.Add(propInstParamsArray)
                            'prpParmArray(itemIndex) = prpInstParam
                            itemIndex = itemIndex + 1
                        End If
                    Next
                End If
            Next
            Try
                If itemIds.Count <> 0 Then
                    vCon.WebServiceManager.ItemService.DeleteUncommittedItems(True)
                    Dim newItemIds = vCon.WebServiceManager.ItemService.EditItems(itemIds.ToArray()).Select(Function(t) t.RevId).ToArray()
                    Dim comItems = vCon.WebServiceManager.ItemService.UpdateItemProperties(newItemIds, newpropInstParamsArrays.ToArray())
                    vCon.WebServiceManager.ItemService.UpdateAndCommitItems(comItems)
                    Return True
                End If
            Catch ex As Exception
                vCon.WebServiceManager.ItemService.UndoEditItems(itemIds.ToArray)
                CreateNotification("Unable to Set Properties for Items." & ex.Message)
                WriteLog(ex.Message.ToString)
            End Try
        Catch ex As Exception
            CreateNotification("Unable to Set Properties for Items." & ex.Message)
            WriteLog(ex.Message.ToString)
        End Try
        Return False
        '    MessageBox.Show("Unable to Update Properties! Kindly Try Again", "Update Items From CSV", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)

    End Function

    Private Function GetDataTableFromCsv(ByVal csvFile As String) As DataTable
        Dim dataTable As New DataTable()
        'string header = isFirstRowHeader ? "Yes" : "No";
        Try
            '  Dim pathOnly As String = System.IO.Path.GetDirectoryName(txtCSV.Text)
            Dim fileName As String = System.IO.Path.GetFileName(csvFile)

            Dim enumerator As New OleDbEnumerator()
            Dim table As DataTable = enumerator.GetElements()

            Dim sql As String = (Convert.ToString("SELECT * FROM [") & fileName) + "]"

            Using connection As New OleDbConnection((Convert.ToString("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=") & appPath) + ";Extended Properties=""Text;IMEX=1;HDR=Yes""")
                Using command As New OleDbCommand(sql, connection)
                    Using adapter As New OleDbDataAdapter(command)
                        '  dataTable.Locale = CultureInfo.CurrentCulture
                        'return dataTable;
                        adapter.Fill(dataTable)
                    End Using
                End Using
            End Using
        Catch ex As Exception
            WriteLog(ex.Message.ToString)
            CreateNotification("Error in reading CSV file! " & ex.Message.ToString())
        End Try
        Return dataTable
    End Function

    Private Sub GetItemProperties(ByVal dtCSV As DataTable)
        lstProps = New List(Of String)
        For i As Int16 = 2 To dtCSV.Columns.Count - 1
            lstProps.Add(dtCSV.Columns(i).ColumnName.Trim()) '.Replace(" ", "_"))
        Next
        dtItemProp = New DataTable()
        dtItemProp = dtCSV.Clone()
        dtItemProp.Columns.Add("ToBeAdded")
        ' dtItemProp.Columns.Add("ToBeUpdated")
        dtItemProp.Columns.Add("Id")
        dtItemProp.Columns.Add("RevId")
        Try
            For Each dr As DataRow In dtCSV.Rows
                If dr(0).ToString() <> String.Empty Then
                    Dim newRow As DataRow = dtItemProp.NewRow() '
                    Try
                        Dim itm As Item = vCon.WebServiceManager.ItemService.GetLatestItemByItemNumber(dr(0))
                        newRow(0) = dr(0)
                        newRow("Id") = itm.Id
                        newRow("RevId") = itm.RevId
                        newRow("ToBeAdded") = False
                    Catch ex As Exception
                        If AddItem(dr(0).ToString()) Then
                            Dim itm As Item = vCon.WebServiceManager.ItemService.GetLatestItemByItemNumber(dr(0))
                            newRow(0) = dr(0)
                            newRow("ToBeAdded") = False
                            newRow("Id") = itm.Id
                            newRow("RevId") = itm.RevId
                        Else
                            newRow(0) = dr(0)
                            newRow("ToBeAdded") = True
                            newRow("Id") = -1
                            newRow("RevId") = -1
                        End If
                    End Try
                    dtItemProp.Rows.Add(newRow)
                End If
            Next
        Catch ex As Exception
            WriteLog(ex.Message.ToString)
        End Try

    End Sub

    Private Function AddItem(ByVal itemNumber As String) As Boolean
        Dim retValue As Boolean = True
        Try
            Dim itemSvc As ItemService = vCon.WebServiceManager.ItemService

            Dim categories As Cat() = vCon.WebServiceManager.CategoryService.GetCategoriesByEntityClassId("ITEM", True)

            Dim catId As Long = -1
            For Each category As Cat In categories
                If (category.Name = "Part") Then
                    catId = category.Id
                End If
            Next

            Dim editableItem As Item = vCon.WebServiceManager.ItemService.AddItemRevision(catId)

            Dim numSchms() As NumSchm

            numSchms = itemSvc.GetNumberingSchemesByType(NumSchmType.Activated)



            Dim numArr As ArrayList

            numArr = New ArrayList

            Dim _numSchm As NumSchm



            For Each _numSchm In numSchms

                If (_numSchm.Name = "Mapped") Then numArr.Add(_numSchm.SchmID)

            Next



            Dim numSchmIds() As Long = CType(numArr.ToArray(GetType(System.Int64)), Long())



            Dim masterIds(0) As Long

            masterIds(0) = editableItem.MasterId


            Dim newItem() As String

            newItem = New String() {itemNumber}



            Dim fieldInputs(0) As StringArray

            Dim tempArr As New StringArray

            tempArr.Items = newItem



            fieldInputs(0) = tempArr

            Dim restric() As ProductRestric = Nothing

            Dim numbers As ItemNum() = itemSvc.AddItemNumbers(masterIds, numSchmIds, fieldInputs, restric)

            editableItem.ItemNum = numbers(0).ItemNum1



            ' commit the item, which finalizes the object

            Dim items(0) As Item

            items(0) = editableItem

            itemSvc.UpdateAndCommitItems(items)

        Catch ex As Exception
            retValue = False
            WriteLog(ex.Message.ToString)
            CreateNotification("Unable to Add Item " & itemNumber & ". " & ex.Message.ToString())
        End Try
        Return retValue
    End Function

    Private Sub CreateNotification(ByVal errDesc As String)
        Try
            Dim custDefs As CustEntDef() = vCon.WebServiceManager.CustomEntityService.GetAllCustomEntityDefinitions()

            Dim coDefId As Long = (From c In custDefs Where c.DispName.Equals("Notification")
                            Select c.Id).First()

            Dim newTimeSheet As CustEnt = vCon.WebServiceManager.CustomEntityService.AddCustomEntity(coDefId, "Notification")
            'custEnts.Add(newTimeSheet);
            '  CustEnt custEnt = connection.WebServiceManager.CustomEntityService.GetCustomEntitiesByIds(new long[] { timesheetDefId }).First();

            Dim propInstances As PropDefInfo() = vCon.WebServiceManager.PropertyService.GetPropertyDefinitionInfosByEntityClassId("CUSTENT", New Long() {newTimeSheet.Id})

            Dim propisntances As PropDef() = vCon.WebServiceManager.PropertyService.GetPropertyDefinitionsByEntityClassId("CUSTENT")


            Dim propId = (From p In propisntances
                       Where p.DispName.Equals("Error Description", StringComparison.InvariantCultureIgnoreCase)
                       Select p.Id).FirstOrDefault()

            ' Dim propValues As Dictionary(Of Long(), Object()) = OrderValuesAndIds(dic)

            If propId <> -1 Then

                Dim newpropInstParamsArrays As New List(Of PropInstParamArray)

                Dim propInstParamsArray As New PropInstParamArray()
                Dim propInstParams = New List(Of PropInstParam)
                Dim propInst As New PropInstParam()

                propInst.PropDefId = propId
                propInst.Val = errDesc
                propInstParams.Add(propInst)

                propInstParamsArray.Items = propInstParams.ToArray()
                '  propInstParamsArrays(itemIndex) = propInstParamsArray
                newpropInstParamsArrays.Add(propInstParamsArray)
                vCon.WebServiceManager.CustomEntityService.UpdateCustomEntityProperties(New Long() {newTimeSheet.Id}, newpropInstParamsArrays.ToArray())
            End If
        Catch ex As Exception
            WriteLog(ex.Message.ToString())
        End Try
    End Sub


#End Region

#Region "FTP"

    Private Function CheckIfFileExistsOnServer(remotePath As String, fileName As String, user As String, password As String) As Boolean
        Dim request = DirectCast(WebRequest.Create(remotePath & fileName), FtpWebRequest)
        request.Credentials = New NetworkCredential(user, password)
        request.Method = WebRequestMethods.Ftp.GetFileSize
        Try
            Dim response As FtpWebResponse = DirectCast(request.GetResponse(), FtpWebResponse)
            Return True
        Catch ex As WebException
            WriteLog(ex.Message.ToString)
            Dim response As FtpWebResponse = DirectCast(ex.Response, FtpWebResponse)
            If response.StatusCode = FtpStatusCode.ActionNotTakenFileUnavailable Then
                Return False
            End If
            CreateNotification(ex.Message)
        End Try
        Return False
    End Function

    Public Function Delete_PreviousFile(FTPServer As String, fileToUpload As String, user As String, password As String) As Boolean
        'Dim filename As String = Path.GetFileName(fileToUpload)
        'filename = filename.Substring(0, filename.LastIndexOf("."))
        Try
            Dim delete_request As FtpWebRequest = TryCast(FtpWebRequest.Create(New Uri(FTPServer & fileToUpload)), FtpWebRequest)
            delete_request.Proxy = Nothing
            delete_request.UseBinary = True
            delete_request.KeepAlive = False
            If Not String.IsNullOrEmpty(user) AndAlso Not String.IsNullOrEmpty(password) Then
                delete_request.Credentials = New NetworkCredential(user, password)
            End If
            delete_request.Method = WebRequestMethods.Ftp.DeleteFile
            Dim delete_response As FtpWebResponse = DirectCast(delete_request.GetResponse(), FtpWebResponse)
            delete_response.Close()
            Return True
        Catch e As Exception
            WriteLog(e.Message.ToString)
            CreateNotification(e.Message)
            Return False
        End Try
    End Function

    Public Function Download(remotePath As String, fileNameToDownload As String, saveToLocalPath As String, user As String, password As String) As Boolean
        Try
            RWDeleteFile(saveToLocalPath)
            Dim request As FtpWebRequest = TryCast(FtpWebRequest.Create(New Uri(remotePath & fileNameToDownload)), FtpWebRequest)
            request.Proxy = Nothing
            request.UseBinary = True
            request.KeepAlive = False
            request.Method = WebRequestMethods.Ftp.DownloadFile
            If Not String.IsNullOrEmpty(user) AndAlso Not String.IsNullOrEmpty(password) Then
                request.Credentials = New NetworkCredential(user, password)
            End If

            Dim response As FtpWebResponse = Nothing
            ' = request.GetResponse() as FtpWebResponse;
            Dim responseStream As Stream = Nothing
            Dim outputStream As FileStream = Nothing
            Try
                response = TryCast(request.GetResponse(), FtpWebResponse)
                responseStream = response.GetResponseStream()
                '   File.Delete(saveToLocalPath);
                outputStream = New FileStream(saveToLocalPath, FileMode.Create)

                Dim bufferSize As Integer = 1024
                Dim readCount As Integer
                Dim buffer As Byte() = New Byte(bufferSize - 1) {}

                readCount = responseStream.Read(buffer, 0, bufferSize)
                While readCount > 0
                    outputStream.Write(buffer, 0, readCount)
                    readCount = responseStream.Read(buffer, 0, bufferSize)
                End While
                Dim statusDescription As String = response.StatusDescription

                While Not System.IO.File.Exists(saveToLocalPath)
                End While
            Catch ex As Exception
                WriteLog(ex.Message.ToString)
            End Try
            responseStream.Close()
            outputStream.Close()
            response.Close()
            Return True
        Catch e As Exception
            WriteLog(e.Message.ToString)
            CreateNotification(e.Message)
            'throw new Exception("Error downloading from URL " + "ftp://" + FTPServer + @"/" + remotePath + fileNameToDownload, e);
            Return False
        End Try
    End Function

#End Region






End Module
