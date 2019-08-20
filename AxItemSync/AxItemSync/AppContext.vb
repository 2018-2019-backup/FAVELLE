Public Class AppContext
    Inherits ApplicationContext

#Region " Storage "

    Private WithEvents Tray As NotifyIcon
    Private WithEvents MainMenu As ContextMenuStrip
    Private WithEvents mnuDisplayForm As ToolStripMenuItem
    Private WithEvents mnuSep1 As ToolStripSeparator
    Private WithEvents mnuExit As ToolStripMenuItem

#End Region

#Region " Constructor "

    Public Sub New()
        'Initialize the menus

        mnuDisplayForm = New ToolStripMenuItem("Settings")
        mnuExit = New ToolStripMenuItem("Exit")
        MainMenu = New ContextMenuStrip
        mnuSep1 = New ToolStripSeparator()
        MainMenu.Items.AddRange(New ToolStripItem() {mnuDisplayForm, mnuSep1, mnuExit})

        'Initialize the tray
        Tray = New NotifyIcon
        Tray.Icon = My.Resources.vault
        Tray.ContextMenuStrip = MainMenu
        Tray.Text = "AX-Vault Item Sync"

        'Display
        Tray.Visible = True
    End Sub

#End Region

#Region " Event handlers "

    Private Sub AppContext_ThreadExit(ByVal sender As Object, ByVal e As System.EventArgs) _
    Handles Me.ThreadExit
        'Guarantees that the icon will not linger.
        Tray.Visible = False
    End Sub

    Private Sub mnuExit_Click(ByVal sender As Object, ByVal e As System.EventArgs) _
    Handles mnuExit.Click
        ExitApplication()
    End Sub

    Private Sub mnuDisplayForm_Click(ByVal sender As Object, ByVal e As System.EventArgs) _
    Handles mnuDisplayForm.Click
        Try
            CanDoListening = False

            If objSetting.IsDisposed.Equals(False) Then
                objSetting = New Settings()
            End If
            objSetting.ShowDialog()
            startTimer()
        Catch ex As Exception
            WriteLog(ex.Message.ToString)
        End Try
        CanDoListening = True
    End Sub

    Public Sub ExitApplication()
        'Perform any clean-up here
        'Then exit the application
        Application.Exit()
    End Sub

#End Region

End Class
