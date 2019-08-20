<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class UpdateItemMasterProperties
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.txtCSV = New System.Windows.Forms.TextBox()
        Me.btnBrowseCSV = New System.Windows.Forms.Button()
        Me.dgvCSV = New System.Windows.Forms.DataGridView()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.Button2 = New System.Windows.Forms.Button()
        CType(Me.dgvCSV, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(12, 33)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(80, 13)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Select CSV File"
        '
        'txtCSV
        '
        Me.txtCSV.Location = New System.Drawing.Point(101, 30)
        Me.txtCSV.Name = "txtCSV"
        Me.txtCSV.Size = New System.Drawing.Size(400, 20)
        Me.txtCSV.TabIndex = 1
        '
        'btnBrowseCSV
        '
        Me.btnBrowseCSV.Location = New System.Drawing.Point(526, 25)
        Me.btnBrowseCSV.Name = "btnBrowseCSV"
        Me.btnBrowseCSV.Size = New System.Drawing.Size(38, 29)
        Me.btnBrowseCSV.TabIndex = 2
        Me.btnBrowseCSV.Text = "..."
        Me.btnBrowseCSV.UseVisualStyleBackColor = True
        '
        'dgvCSV
        '
        Me.dgvCSV.AllowUserToAddRows = False
        Me.dgvCSV.AllowUserToDeleteRows = False
        Me.dgvCSV.AllowUserToResizeRows = False
        Me.dgvCSV.BackgroundColor = System.Drawing.SystemColors.ControlLightLight
        Me.dgvCSV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvCSV.Location = New System.Drawing.Point(17, 73)
        Me.dgvCSV.MultiSelect = False
        Me.dgvCSV.Name = "dgvCSV"
        Me.dgvCSV.ReadOnly = True
        Me.dgvCSV.Size = New System.Drawing.Size(550, 294)
        Me.dgvCSV.TabIndex = 3
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(356, 387)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(97, 29)
        Me.Button1.TabIndex = 4
        Me.Button1.Text = "&UPDATE"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'Button2
        '
        Me.Button2.Location = New System.Drawing.Point(469, 388)
        Me.Button2.Name = "Button2"
        Me.Button2.Size = New System.Drawing.Size(97, 29)
        Me.Button2.TabIndex = 5
        Me.Button2.Text = "&CANCEL"
        Me.Button2.UseVisualStyleBackColor = True
        '
        'UpdateItemMasterProperties
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.SystemColors.ControlLightLight
        Me.ClientSize = New System.Drawing.Size(584, 437)
        Me.Controls.Add(Me.Button2)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.dgvCSV)
        Me.Controls.Add(Me.btnBrowseCSV)
        Me.Controls.Add(Me.txtCSV)
        Me.Controls.Add(Me.Label1)
        Me.MaximizeBox = False
        Me.Name = "UpdateItemMasterProperties"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Update Item Master Properties from CSV"
        CType(Me.dgvCSV, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents txtCSV As System.Windows.Forms.TextBox
    Private WithEvents btnBrowseCSV As System.Windows.Forms.Button
    Friend WithEvents dgvCSV As System.Windows.Forms.DataGridView
    Private WithEvents Button1 As System.Windows.Forms.Button
    Private WithEvents Button2 As System.Windows.Forms.Button
End Class
