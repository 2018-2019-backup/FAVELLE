Imports System.Xml
'Imports FavelleVaultTools.wsFavelle
Imports System.ServiceModel
Imports Autodesk.Connectivity.WebServicesTools
Imports Autodesk.DataManagement.Client.Framework.Vault.Currency.Connections
Imports System.Data

Module CommonFunctions
    Dim settingsFile = "C:\ProgramData\Autodesk\Vault 2016\Extensions\FavelleVaultTools\FavelleVaultTools.dll.config"
    ' Dim wsObj As wsMethodsSoapClient '= New wsMethodsSoapClient()
    Dim adminCredentials As UserPasswordCredentials
    Public adminVConnection As Connection
    Dim adminServiceManager As WebServiceManager
    ' Public adminObj As wsFavelle.VaultCredentials
    Public curVault As String
    Public msgCaption As String = "Favelle Vault Tools"

    Public Function wsGetVaultDetails() As Boolean
        Dim retValue As Boolean = False
        If (SetServiceReference()) Then
            ' adminObj = wsObj.GetVaultDetails()
            retValue = True
        End If
        Return retValue
    End Function

    Public Function SetServiceReference() As Boolean
        Dim retValue As Boolean = False
        Try
            Dim wsUrl = String.Empty
            ' Dim wsUsername = String.Empty
            ' Dim wsPassword = String.Empty

            If System.IO.File.Exists(settingsFile) Then
                Dim tempXml As New XmlDocument()
                tempXml.Load(settingsFile)
                wsUrl = tempXml.SelectSingleNode("configuration").SelectSingleNode("system.serviceModel").SelectSingleNode("client").SelectSingleNode("endpoint").Attributes("address").Value

                Try
                    Dim remoteAddress = New System.ServiceModel.EndpointAddress(wsUrl)
                    '  wsObj = New wsMethodsSoapClient(New System.ServiceModel.BasicHttpBinding(), remoteAddress)
                    retValue = True
                    'Using wsObj = New wsMethodsSoapClient(New System.ServiceModel.BasicHttpBinding(), remoteAddress)
                    '    'set timeout
                    '    wsObj.Endpoint.Binding.SendTimeout = New TimeSpan(0, 0, 0, 3)

                    '    'call web service method
                    '    productResponse = wsObj.GetProducts()
                    'End Using


                    'wsObj = New wsMethodsSoapClient(wsObj.Endpoint.Binding, New EndpointAddress(wsUrl))
                    'webObj.UserName = xmlUsername
                    'webObj.Password = xmlPassword
                Catch ex As Exception
                    ' WriteLog(ex)
                End Try
            End If
        Catch ex As Exception
            ' WriteLog(ex)
        End Try
        Return retValue
    End Function

    Public Function GetImportItems() As DataTable
        Dim retValue As DataTable = Nothing
        Try
            If (SetServiceReference()) Then
                ' retValue = wsObj.GetItemsforVaults()
            End If
        Catch ex As Exception

        End Try
        Return retValue
    End Function

    Public Function GetDBPropertyMapping() As DataTable
        Dim retValue As DataTable = Nothing
        Try
            If (SetServiceReference()) Then
                ' retValue = wsObj.GetPropertyColumnMapping()
            End If
        Catch ex As Exception

        End Try
        Return retValue
    End Function

    Public Function UpdateImportStatus(ByVal dt As DataTable) As Boolean
        Dim retValue As Boolean = False
        Try
            If (SetServiceReference()) Then
                ' retValue = wsObj.UpdateV2AXItemExportStatus(dt)
            End If
        Catch ex As Exception

        End Try
        Return retValue
    End Function

    Public Function GetVaultsProps() As DataTable
        Dim retValue As DataTable = Nothing
        Try
            If (SetServiceReference()) Then
                ' retValue = wsObj.GetVaultProps()
            End If
        Catch ex As Exception

        End Try
        Return retValue
    End Function
    Public Function GetAdminCredentials(ByVal vaultName As String) As Boolean
        Try
            'adminCredentials = New UserPasswordCredentials(adminObj.vServer, vaultName, adminObj.vUser, adminObj.vPwd)
            'adminServiceManager = New WebServiceManager(adminCredentials)
            'Dim userId As Long = adminCredentials.SecurityHeader.UserId
            'adminVConnection = New Connection(adminServiceManager, vaultName, userId, adminObj.vServer, AuthenticationFlags.Standard)
            Return True
        Catch
            Return False
        End Try
    End Function

End Module


