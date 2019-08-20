Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel

' To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line.
' <System.Web.Script.Services.ScriptService()> _
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")> _
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<ToolboxItem(False)> _
Public Class VaultAXws
    Inherits System.Web.Services.WebService

    <WebMethod()> _
    Public Function GetAccessibleAX(ByVal vUsername As String, ByVal vName As String) As DataTable
        Dim dtAxOrgs As DataTable = Nothing
        Try
            If LoadDataFromSettingsFile() Then
                dtAxOrgs = New DataTable
                dtAxOrgs.Columns.Add("IsSelected", System.Type.GetType("System.Boolean"))
                dtAxOrgs.Columns.Add("OrgName", System.Type.GetType("System.String"))
                For Each dr As DataRow In dtVaultOrg.Select("Vault = '" + vName + "'")


                Next
            End If
        Catch ex As Exception
            dtAxOrgs = Nothing
        End Try
        Return dtAxOrgs
    End Function

    <WebMethod()> _
    Public Function GetImportItemsList(ByVal vUsername As String, ByVal selAx As List(Of String)) As DataTable
        Dim dtAxOrgs As DataTable = New DataTable
        dtAxOrgs.Columns.Add("IsSelected", System.Type.GetType("System.Boolean"))
        dtAxOrgs.Columns.Add("OrgName", System.Type.GetType("System.String"))

        Return dtAxOrgs
    End Function

    <WebMethod()> _
    Public Function GetExportItemValues(ByVal vUsername As String, ByVal selItemIds As List(Of Long)) As DataTable
        Dim dtAxOrgs As DataTable = New DataTable
        dtAxOrgs.Columns.Add("IsSelected", System.Type.GetType("System.Boolean"))
        dtAxOrgs.Columns.Add("OrgName", System.Type.GetType("System.String"))

        Return dtAxOrgs
    End Function

    <WebMethod()> _
    Public Function GetExportBOMValues(ByVal vUsername As String, ByVal selItemIds As List(Of Long)) As DataSet
        Dim dsBom As DataSet = New DataSet()
        Dim dtAxOrgs As DataTable = New DataTable
        dtAxOrgs.Columns.Add("IsSelected", System.Type.GetType("System.Boolean"))
        dtAxOrgs.Columns.Add("OrgName", System.Type.GetType("System.String"))
        dsBom.Tables.Add(dtAxOrgs)
        Return dsBom
    End Function
End Class