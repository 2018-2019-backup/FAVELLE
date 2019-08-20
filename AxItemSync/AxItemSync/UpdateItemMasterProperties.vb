Imports System.Windows.Forms
Imports System.Data
Imports System.Data.OleDb
Imports System.Globalization
Imports Autodesk.DataManagement.Client.Framework.Vault.Currency.Connections
Imports Autodesk.Connectivity.WebServices

Imports System.Net
Imports System.Net.Security
Imports System

Public Class UpdateItemMasterProperties

    Private vCon As Connection
    Private dtItemProp As DataTable
    Dim dtCSV As DataTable
    Dim lstProps As New List(Of String)
    Dim csvPath As String = "C:\ItemListExport.csv"
    '"C:\Users\balaji\Google Drive\KKM_Cetas\ItemList.csv"
    Public Sub New(ByVal con As Connection)
        vCon = con
        InitializeComponent()
        txtCSV.Text = csvPath
    End Sub


    Private Sub UpdateItemMasterProperties_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Dim ftpPath As String = "182.73.238.180/Axapta"
        Select Case vCon.Vault
            Case "", "AXA"
                ftpPath += "/ORG1"
                Exit Select
            Case "AXA-ORG2"
                ftpPath += "/ORG2"
                Exit Select
            Case "AXA-ORG3"
                ftpPath += "/ORG3"
                Exit Select
            Case Else
                Exit Select
        End Select

        Download(ftpPath, "ItemListExport.csv", "cetas/browsing", "browsing@cetas")
        LoadCSVValues()
    End Sub


    Public Sub Download(urlString As String, fileToDownload As String, user As String, password As String)
        Try

            Dim filename As String = System.IO.Path.GetFileName(fileToDownload)

            ' Get the object used to communicate with the server.
            Dim request As FtpWebRequest = DirectCast(WebRequest.Create(New Uri(Convert.ToString((Convert.ToString("ftp://") & urlString) + "/") & filename)), FtpWebRequest)
            request.Method = WebRequestMethods.Ftp.DownloadFile

            ' This example assumes the FTP site uses anonymous logon.
            'request.Credentials = new NetworkCredential("anonymous", "janeDoe@contoso.com");
            If Not String.IsNullOrEmpty(user) AndAlso Not String.IsNullOrEmpty(password) Then
                request.Credentials = New NetworkCredential(user, password)
            End If

            Dim response As FtpWebResponse = DirectCast(request.GetResponse(), FtpWebResponse)

            Dim responseStream As System.IO.Stream = response.GetResponseStream()
            Dim reader As New System.IO.StreamReader(responseStream)

            Dim write As New System.IO.StreamWriter(csvPath) '(Convert.ToString("C:\") & filename)
            write.Write(reader.ReadToEnd())
            'Console.WriteLine(reader.ReadToEnd());
            'Console.WriteLine("Download Complete, status {0}", response.StatusDescription);
            write.Close()
            reader.Close()
            response.Close()
        Catch
        End Try
    End Sub


    Private Sub btnBrowseCSV_Click(sender As Object, e As EventArgs) Handles btnBrowseCSV.Click
        Try

            Dim dialog = New OpenFileDialog()
            If (txtCSV.Text.Trim().Equals(String.Empty)) Then
                dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
            Else
                dialog.InitialDirectory = txtCSV.Text
            End If
            dialog.Multiselect = False
            dialog.Filter = "CSV File | *.csv"
            dialog.DefaultExt = "csv"

            Dim result As DialogResult = dialog.ShowDialog()
            If (result.Equals(System.Windows.Forms.DialogResult.OK)) Then
                txtCSV.Text = dialog.FileName
            End If
            LoadCSVValues()
        Catch
        End Try
    End Sub

    Private Sub LoadCSVValues()
        '   dtCSV = GetDataTableFromCsv()

        dgvCSV.DataSource = dtCSV
        ' GetItemProperties(dtCSV)
        ShowVariationInGrid()

    End Sub
    Private Sub ShowVariationInGrid()
        Try

            For i As Integer = 0 To dgvCSV.Rows.Count - 1
                For Each dtRow As DataRow In dtItemProp.Select("Number = '" + dgvCSV.Rows(i).Cells(0).Value.ToString().ToString() + "'")
                    For j As Integer = 2 To dgvCSV.Columns.Count - 1
                        If (Not dgvCSV.Rows(i).Cells(j).Value.ToString().Trim().Equals(dtRow(j).ToString().Trim())) And dtRow("ToBeAdded").Equals("False") Then
                            dgvCSV.Rows(i).Cells(j).Style.BackColor = System.Drawing.Color.Yellow
                            dtRow("ToBeUpdated") = True
                        End If
                    Next
                Next
            Next

        Catch ex As Exception

        End Try
    End Sub



  



    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Try

            Dim props() As PropDef = vCon.WebServiceManager.PropertyService.GetPropertyDefinitionsByEntityClassId("ITEM")

            Dim vaultProp() As PropDef = (From prop In props
                                       Where lstProps.Contains(prop.DispName, StringComparer.InvariantCultureIgnoreCase).Equals(True)
                                         Select prop).ToArray() ' prop.DispName.Equals("Revision", StringComparison.InvariantCultureIgnoreCase).Equals(True) Or
            Dim vPropIds() As Long = (From prps In vaultProp
                                     Select prps.Id).ToArray()
            Dim tobeModifiedCount = dtItemProp.Select("ToBeUpdated = " & True).Count()
            '   Dim prpParmArray(tobeModifiedCount) As PropInstParamArray
            '  Dim index As Integer
            Dim itemIndex = 0
            Dim itemIds As New List(Of Long)
            Dim propInstParams As New List(Of PropInstParam)
            Dim propInstParamsArrays(tobeModifiedCount - 1) As PropInstParamArray
            Dim newpropInstParamsArrays As New List(Of PropInstParamArray)

            For Each dr As DataRow In dtCSV.Rows
                propInstParams = New List(Of PropInstParam)


                For Each tempRow As DataRow In dtItemProp.Select("Number = '" + dr("Number").ToString() + "'")
                    If tempRow("ToBeUpdated").ToString().Equals("True", StringComparison.InvariantCultureIgnoreCase) Then
                        Dim myItem As Item = vCon.WebServiceManager.ItemService.GetLatestItemByItemNumber(tempRow("Number"))
                        If myItem.LfCycStateId <> 24 Then
                            Continue For
                        End If
                        itemIds.Add(Convert.ToDouble(tempRow("RevId").ToString()))

                        '   Dim prpInstParam As New PropInstParam
                        ' index = 0
                        For Each vProp In vaultProp
                            For Each prp As PropDef In props.Where(Function(T) T.Id.Equals(vProp.Id))
                                Dim propInst As New PropInstParam()

                                propInst.PropDefId = prp.Id
                                propInst.Val = dr.Item(prp.DispName.ToString())   ' .Replace("_", " ")).ToString()
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
            Next
            Try
                If (itemIds.Count.Equals(0)) Then
                    MessageBox.Show("Properties are already Same!", "Update Items From CSV", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                    Return
                End If

                'Dim masterList As New List(Of Long)()
                'For Each item As Item In items
                '    Dim state As Long = item.LfCycStateId
                '    If state <> 26 Then
                '        masterList.Add(item.MasterId)
                '    End If
                'Next
                '.Where(Function(t1) t1.LfCycStateId = 24)

                'Dim items As Item() = vCon.WebServiceManager.ItemService.GetItemsByIds(itemIds.ToArray())
                'Dim newItemIds = items.Where(Function(t) t.LfCycStateId = 24).Select(Function(t) t.RevId).ToArray()


                Dim newItemIds = vCon.WebServiceManager.ItemService.EditItems(itemIds.ToArray()).Select(Function(t) t.RevId).ToArray()

                Dim comItems = vCon.WebServiceManager.ItemService.UpdateItemProperties(newItemIds, newpropInstParamsArrays.ToArray())
                vCon.WebServiceManager.ItemService.UpdateAndCommitItems(comItems)
                MessageBox.Show("Properties Updated Succcessfully.", "Update Items From CSV", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                Me.Close()
                Return
            Catch ex As Exception
                vCon.WebServiceManager.ItemService.UndoEditItems(itemIds.ToArray)
            End Try

        Catch ex As Exception

        End Try
        MessageBox.Show("Unable to Update Properties! Kindly Try Again", "Update Items From CSV", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)

    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Me.Close()
    End Sub
End Class