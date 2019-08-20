Imports System.Data

Public Class uiSelectAX
    Public dtAxOrgs As DataTable
    Public Sub New(ByVal lstOrgs As List(Of String))
        InitializeComponent()
        dtAxOrgs = New DataTable
        dtAxOrgs.Columns.Add("IsSelected", System.Type.GetType("System.Boolean"))
        dtAxOrgs.Columns.Add("OrgName", System.Type.GetType("System.String"))
        Dim dr As DataRow
        For Each _org As String In lstOrgs
            dr = dtAxOrgs.NewRow()
            dr(0) = False
            dr(1) = _org
            dtAxOrgs.Rows.Add(dr)
        Next
        lstAxOrgs.ItemsSource = dtAxOrgs.DefaultView()
    End Sub

    Private Sub btnImport_Click_1(sender As Object, e As RoutedEventArgs)
        Try
            If dtAxOrgs.Select("IsSelected = 'True'").Count() > 0 Then
                Me.DialogResult = True
            Else
                MessageBox.Show("Kindly select atleast one organisation", msgCaption, MessageBoxButton.OK, MessageBoxImage.Information)
            End If
        Catch ex As Exception

        End Try
    End Sub
End Class
