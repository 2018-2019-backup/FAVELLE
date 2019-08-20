<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Settings
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
        Me.components = New System.ComponentModel.Container()
        Me.TabControl1 = New System.Windows.Forms.TabControl()
        Me.TabPage1 = New System.Windows.Forms.TabPage()
        Me.txtBOMService = New System.Windows.Forms.TextBox()
        Me.Label15 = New System.Windows.Forms.Label()
        Me.Label13 = New System.Windows.Forms.Label()
        Me.txtDuration = New System.Windows.Forms.TextBox()
        Me.chkAutoImport = New System.Windows.Forms.CheckBox()
        Me.btnTab1Save = New System.Windows.Forms.Button()
        Me.txtAxService = New System.Windows.Forms.TextBox()
        Me.Label19 = New System.Windows.Forms.Label()
        Me.GroupBox5 = New System.Windows.Forms.GroupBox()
        Me.btnClear = New System.Windows.Forms.Button()
        Me.dgvVaultOrg = New System.Windows.Forms.DataGridView()
        Me.dcVault = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.dcOrganisation = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.txtTOrg = New System.Windows.Forms.TextBox()
        Me.btnVODelete = New System.Windows.Forms.Button()
        Me.Label18 = New System.Windows.Forms.Label()
        Me.btnVOEdit = New System.Windows.Forms.Button()
        Me.btnVOAdd = New System.Windows.Forms.Button()
        Me.txtTVault = New System.Windows.Forms.TextBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.txtVServer = New System.Windows.Forms.TextBox()
        Me.txtVPwd = New System.Windows.Forms.TextBox()
        Me.txtVUsername = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.TabPage2 = New System.Windows.Forms.TabPage()
        Me.btnTab2Save = New System.Windows.Forms.Button()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.dgvPropMapping = New System.Windows.Forms.DataGridView()
        Me.DataGridViewTextBoxColumn1 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.TabPage4 = New System.Windows.Forms.TabPage()
        Me.btnTab4Save = New System.Windows.Forms.Button()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.dgvBOMPropMapping = New System.Windows.Forms.DataGridView()
        Me.DataGridViewTextBoxColumn2 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.TabPage3 = New System.Windows.Forms.TabPage()
        Me.btnTab3Save = New System.Windows.Forms.Button()
        Me.txtAfterExportBOM = New System.Windows.Forms.TextBox()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.txtAfterExportItem = New System.Windows.Forms.TextBox()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.cmbxVaultsforJobs = New System.Windows.Forms.ComboBox()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.Label14 = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.cmbx_lifeCylceDef = New System.Windows.Forms.ComboBox()
        Me.lst_FileState = New System.Windows.Forms.ListBox()
        Me.lst_FileTransition = New System.Windows.Forms.ListBox()
        Me.lst_FileJobs = New System.Windows.Forms.ListBox()
        Me.cmenuItemJobs = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.AddItemJob = New System.Windows.Forms.ToolStripMenuItem()
        Me.AddBOMExportJobToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.DeleteItemJob = New System.Windows.Forms.ToolStripMenuItem()
        Me.TabPage5 = New System.Windows.Forms.TabPage()
        Me.btnvUserSave = New System.Windows.Forms.Button()
        Me.dgvUsers = New System.Windows.Forms.DataGridView()
        Me.DataGridViewTextBoxColumn3 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.dgcEmpCode = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.ImportItem = New System.Windows.Forms.DataGridViewCheckBoxColumn()
        Me.ExportItem = New System.Windows.Forms.DataGridViewCheckBoxColumn()
        Me.ExportBOM = New System.Windows.Forms.DataGridViewCheckBoxColumn()
        Me.TabPage6 = New System.Windows.Forms.TabPage()
        Me.btnCATSave = New System.Windows.Forms.Button()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.btnCATClear = New System.Windows.Forms.Button()
        Me.dgCAT = New System.Windows.Forms.DataGridView()
        Me.dcCATAx = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.dcCATVault = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.txtCATVault = New System.Windows.Forms.TextBox()
        Me.btnCATDelete = New System.Windows.Forms.Button()
        Me.Label16 = New System.Windows.Forms.Label()
        Me.btnCATEdit = New System.Windows.Forms.Button()
        Me.btnCATAdd = New System.Windows.Forms.Button()
        Me.txtCATAX = New System.Windows.Forms.TextBox()
        Me.Label17 = New System.Windows.Forms.Label()
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.TabControl1.SuspendLayout()
        Me.TabPage1.SuspendLayout()
        Me.GroupBox5.SuspendLayout()
        CType(Me.dgvVaultOrg, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupBox1.SuspendLayout()
        Me.TabPage2.SuspendLayout()
        CType(Me.dgvPropMapping, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.TabPage4.SuspendLayout()
        CType(Me.dgvBOMPropMapping, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.TabPage3.SuspendLayout()
        Me.cmenuItemJobs.SuspendLayout()
        Me.TabPage5.SuspendLayout()
        CType(Me.dgvUsers, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.TabPage6.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        CType(Me.dgCAT, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'TabControl1
        '
        Me.TabControl1.Controls.Add(Me.TabPage1)
        Me.TabControl1.Controls.Add(Me.TabPage2)
        Me.TabControl1.Controls.Add(Me.TabPage4)
        Me.TabControl1.Controls.Add(Me.TabPage3)
        Me.TabControl1.Controls.Add(Me.TabPage5)
        Me.TabControl1.Controls.Add(Me.TabPage6)
        Me.TabControl1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TabControl1.Location = New System.Drawing.Point(0, 0)
        Me.TabControl1.Name = "TabControl1"
        Me.TabControl1.SelectedIndex = 0
        Me.TabControl1.Size = New System.Drawing.Size(509, 536)
        Me.TabControl1.TabIndex = 0
        '
        'TabPage1
        '
        Me.TabPage1.Controls.Add(Me.txtBOMService)
        Me.TabPage1.Controls.Add(Me.Label15)
        Me.TabPage1.Controls.Add(Me.Label13)
        Me.TabPage1.Controls.Add(Me.txtDuration)
        Me.TabPage1.Controls.Add(Me.chkAutoImport)
        Me.TabPage1.Controls.Add(Me.btnTab1Save)
        Me.TabPage1.Controls.Add(Me.txtAxService)
        Me.TabPage1.Controls.Add(Me.Label19)
        Me.TabPage1.Controls.Add(Me.GroupBox5)
        Me.TabPage1.Controls.Add(Me.GroupBox1)
        Me.TabPage1.Location = New System.Drawing.Point(4, 22)
        Me.TabPage1.Name = "TabPage1"
        Me.TabPage1.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage1.Size = New System.Drawing.Size(501, 510)
        Me.TabPage1.TabIndex = 0
        Me.TabPage1.Text = "Basic Configurations"
        Me.TabPage1.UseVisualStyleBackColor = True
        '
        'txtBOMService
        '
        Me.txtBOMService.Location = New System.Drawing.Point(128, 413)
        Me.txtBOMService.Name = "txtBOMService"
        Me.txtBOMService.Size = New System.Drawing.Size(343, 20)
        Me.txtBOMService.TabIndex = 15
        '
        'Label15
        '
        Me.Label15.AutoSize = True
        Me.Label15.Location = New System.Drawing.Point(22, 418)
        Me.Label15.Name = "Label15"
        Me.Label15.Size = New System.Drawing.Size(87, 13)
        Me.Label15.TabIndex = 16
        Me.Label15.Text = "AX BOM Service"
        '
        'Label13
        '
        Me.Label13.AutoSize = True
        Me.Label13.Location = New System.Drawing.Point(272, 445)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(46, 13)
        Me.Label13.TabIndex = 14
        Me.Label13.Text = "minutes."
        Me.Label13.Visible = False
        '
        'txtDuration
        '
        Me.txtDuration.Enabled = False
        Me.txtDuration.Location = New System.Drawing.Point(195, 442)
        Me.txtDuration.Name = "txtDuration"
        Me.txtDuration.Size = New System.Drawing.Size(66, 20)
        Me.txtDuration.TabIndex = 14
        Me.txtDuration.Text = "0"
        Me.txtDuration.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.txtDuration.Visible = False
        '
        'chkAutoImport
        '
        Me.chkAutoImport.AutoSize = True
        Me.chkAutoImport.Location = New System.Drawing.Point(26, 444)
        Me.chkAutoImport.Name = "chkAutoImport"
        Me.chkAutoImport.Size = New System.Drawing.Size(163, 17)
        Me.chkAutoImport.TabIndex = 8
        Me.chkAutoImport.Text = "Import Items from AX in every"
        Me.chkAutoImport.UseVisualStyleBackColor = True
        Me.chkAutoImport.Visible = False
        '
        'btnTab1Save
        '
        Me.btnTab1Save.Location = New System.Drawing.Point(396, 469)
        Me.btnTab1Save.Name = "btnTab1Save"
        Me.btnTab1Save.Size = New System.Drawing.Size(75, 23)
        Me.btnTab1Save.TabIndex = 7
        Me.btnTab1Save.Text = "Save"
        Me.btnTab1Save.UseVisualStyleBackColor = True
        '
        'txtAxService
        '
        Me.txtAxService.Location = New System.Drawing.Point(129, 384)
        Me.txtAxService.Name = "txtAxService"
        Me.txtAxService.Size = New System.Drawing.Size(343, 20)
        Me.txtAxService.TabIndex = 5
        '
        'Label19
        '
        Me.Label19.AutoSize = True
        Me.Label19.Location = New System.Drawing.Point(23, 389)
        Me.Label19.Name = "Label19"
        Me.Label19.Size = New System.Drawing.Size(83, 13)
        Me.Label19.TabIndex = 6
        Me.Label19.Text = "AX Item Service"
        '
        'GroupBox5
        '
        Me.GroupBox5.Controls.Add(Me.btnClear)
        Me.GroupBox5.Controls.Add(Me.dgvVaultOrg)
        Me.GroupBox5.Controls.Add(Me.txtTOrg)
        Me.GroupBox5.Controls.Add(Me.btnVODelete)
        Me.GroupBox5.Controls.Add(Me.Label18)
        Me.GroupBox5.Controls.Add(Me.btnVOEdit)
        Me.GroupBox5.Controls.Add(Me.btnVOAdd)
        Me.GroupBox5.Controls.Add(Me.txtTVault)
        Me.GroupBox5.Controls.Add(Me.Label4)
        Me.GroupBox5.Location = New System.Drawing.Point(11, 157)
        Me.GroupBox5.Name = "GroupBox5"
        Me.GroupBox5.Size = New System.Drawing.Size(481, 212)
        Me.GroupBox5.TabIndex = 3
        Me.GroupBox5.TabStop = False
        Me.GroupBox5.Text = "Vault - Organisation Details"
        '
        'btnClear
        '
        Me.btnClear.Location = New System.Drawing.Point(415, 52)
        Me.btnClear.Name = "btnClear"
        Me.btnClear.Size = New System.Drawing.Size(46, 23)
        Me.btnClear.TabIndex = 13
        Me.btnClear.Text = "Clear"
        Me.btnClear.UseVisualStyleBackColor = True
        '
        'dgvVaultOrg
        '
        Me.dgvVaultOrg.AllowUserToAddRows = False
        Me.dgvVaultOrg.AllowUserToDeleteRows = False
        Me.dgvVaultOrg.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.dgvVaultOrg.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells
        Me.dgvVaultOrg.BackgroundColor = System.Drawing.SystemColors.ControlLightLight
        Me.dgvVaultOrg.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvVaultOrg.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.dcVault, Me.dcOrganisation})
        Me.dgvVaultOrg.Location = New System.Drawing.Point(15, 86)
        Me.dgvVaultOrg.MultiSelect = False
        Me.dgvVaultOrg.Name = "dgvVaultOrg"
        Me.dgvVaultOrg.ReadOnly = True
        Me.dgvVaultOrg.RowHeadersVisible = False
        Me.dgvVaultOrg.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.dgvVaultOrg.Size = New System.Drawing.Size(445, 110)
        Me.dgvVaultOrg.TabIndex = 12
        '
        'dcVault
        '
        Me.dcVault.HeaderText = "Vault"
        Me.dcVault.Name = "dcVault"
        Me.dcVault.ReadOnly = True
        '
        'dcOrganisation
        '
        Me.dcOrganisation.HeaderText = "AX Organisation"
        Me.dcOrganisation.Name = "dcOrganisation"
        Me.dcOrganisation.ReadOnly = True
        '
        'txtTOrg
        '
        Me.txtTOrg.Location = New System.Drawing.Point(81, 55)
        Me.txtTOrg.Name = "txtTOrg"
        Me.txtTOrg.Size = New System.Drawing.Size(152, 20)
        Me.txtTOrg.TabIndex = 9
        '
        'btnVODelete
        '
        Me.btnVODelete.Enabled = False
        Me.btnVODelete.Location = New System.Drawing.Point(358, 53)
        Me.btnVODelete.Name = "btnVODelete"
        Me.btnVODelete.Size = New System.Drawing.Size(46, 23)
        Me.btnVODelete.TabIndex = 11
        Me.btnVODelete.Text = "Delete"
        Me.btnVODelete.UseVisualStyleBackColor = True
        '
        'Label18
        '
        Me.Label18.AutoSize = True
        Me.Label18.Location = New System.Drawing.Point(11, 58)
        Me.Label18.Name = "Label18"
        Me.Label18.Size = New System.Drawing.Size(66, 13)
        Me.Label18.TabIndex = 10
        Me.Label18.Text = "Organisation"
        '
        'btnVOEdit
        '
        Me.btnVOEdit.Enabled = False
        Me.btnVOEdit.Location = New System.Drawing.Point(302, 53)
        Me.btnVOEdit.Name = "btnVOEdit"
        Me.btnVOEdit.Size = New System.Drawing.Size(46, 23)
        Me.btnVOEdit.TabIndex = 10
        Me.btnVOEdit.Text = "Edit"
        Me.btnVOEdit.UseVisualStyleBackColor = True
        '
        'btnVOAdd
        '
        Me.btnVOAdd.Location = New System.Drawing.Point(246, 53)
        Me.btnVOAdd.Name = "btnVOAdd"
        Me.btnVOAdd.Size = New System.Drawing.Size(46, 23)
        Me.btnVOAdd.TabIndex = 9
        Me.btnVOAdd.Text = "Add"
        Me.btnVOAdd.UseVisualStyleBackColor = True
        '
        'txtTVault
        '
        Me.txtTVault.Location = New System.Drawing.Point(82, 29)
        Me.txtTVault.Name = "txtTVault"
        Me.txtTVault.Size = New System.Drawing.Size(152, 20)
        Me.txtTVault.TabIndex = 7
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(12, 30)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(31, 13)
        Me.Label4.TabIndex = 8
        Me.Label4.Text = "Vault"
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.txtVServer)
        Me.GroupBox1.Controls.Add(Me.txtVPwd)
        Me.GroupBox1.Controls.Add(Me.txtVUsername)
        Me.GroupBox1.Controls.Add(Me.Label1)
        Me.GroupBox1.Controls.Add(Me.Label2)
        Me.GroupBox1.Controls.Add(Me.Label3)
        Me.GroupBox1.Location = New System.Drawing.Point(11, 15)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(481, 135)
        Me.GroupBox1.TabIndex = 0
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Vault Credentials"
        '
        'txtVServer
        '
        Me.txtVServer.Location = New System.Drawing.Point(81, 92)
        Me.txtVServer.Name = "txtVServer"
        Me.txtVServer.Size = New System.Drawing.Size(379, 20)
        Me.txtVServer.TabIndex = 2
        '
        'txtVPwd
        '
        Me.txtVPwd.Location = New System.Drawing.Point(81, 60)
        Me.txtVPwd.Name = "txtVPwd"
        Me.txtVPwd.PasswordChar = Global.Microsoft.VisualBasic.ChrW(42)
        Me.txtVPwd.Size = New System.Drawing.Size(379, 20)
        Me.txtVPwd.TabIndex = 1
        '
        'txtVUsername
        '
        Me.txtVUsername.Location = New System.Drawing.Point(81, 28)
        Me.txtVUsername.Name = "txtVUsername"
        Me.txtVUsername.Size = New System.Drawing.Size(379, 20)
        Me.txtVUsername.TabIndex = 0
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(11, 32)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(55, 13)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Username"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(11, 63)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(53, 13)
        Me.Label2.TabIndex = 1
        Me.Label2.Text = "Password"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(11, 94)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(38, 13)
        Me.Label3.TabIndex = 2
        Me.Label3.Text = "Server"
        '
        'TabPage2
        '
        Me.TabPage2.Controls.Add(Me.btnTab2Save)
        Me.TabPage2.Controls.Add(Me.Label5)
        Me.TabPage2.Controls.Add(Me.dgvPropMapping)
        Me.TabPage2.Location = New System.Drawing.Point(4, 22)
        Me.TabPage2.Name = "TabPage2"
        Me.TabPage2.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage2.Size = New System.Drawing.Size(501, 510)
        Me.TabPage2.TabIndex = 1
        Me.TabPage2.Text = "Item Property Mapping"
        Me.TabPage2.UseVisualStyleBackColor = True
        '
        'btnTab2Save
        '
        Me.btnTab2Save.Location = New System.Drawing.Point(417, 473)
        Me.btnTab2Save.Name = "btnTab2Save"
        Me.btnTab2Save.Size = New System.Drawing.Size(75, 23)
        Me.btnTab2Save.TabIndex = 15
        Me.btnTab2Save.Text = "Save"
        Me.btnTab2Save.UseVisualStyleBackColor = True
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label5.Location = New System.Drawing.Point(8, 489)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(248, 13)
        Me.Label5.TabIndex = 14
        Me.Label5.Text = "* If there is no property available in vault mention '-'."
        '
        'dgvPropMapping
        '
        Me.dgvPropMapping.AllowUserToAddRows = False
        Me.dgvPropMapping.AllowUserToDeleteRows = False
        Me.dgvPropMapping.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells
        Me.dgvPropMapping.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells
        Me.dgvPropMapping.BackgroundColor = System.Drawing.SystemColors.ControlLightLight
        Me.dgvPropMapping.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvPropMapping.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.DataGridViewTextBoxColumn1})
        Me.dgvPropMapping.Location = New System.Drawing.Point(9, 16)
        Me.dgvPropMapping.MultiSelect = False
        Me.dgvPropMapping.Name = "dgvPropMapping"
        Me.dgvPropMapping.RowHeadersVisible = False
        Me.dgvPropMapping.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect
        Me.dgvPropMapping.Size = New System.Drawing.Size(485, 448)
        Me.dgvPropMapping.TabIndex = 13
        '
        'DataGridViewTextBoxColumn1
        '
        Me.DataGridViewTextBoxColumn1.HeaderText = "AX Field"
        Me.DataGridViewTextBoxColumn1.Name = "DataGridViewTextBoxColumn1"
        Me.DataGridViewTextBoxColumn1.ReadOnly = True
        Me.DataGridViewTextBoxColumn1.Width = 71
        '
        'TabPage4
        '
        Me.TabPage4.Controls.Add(Me.btnTab4Save)
        Me.TabPage4.Controls.Add(Me.Label12)
        Me.TabPage4.Controls.Add(Me.dgvBOMPropMapping)
        Me.TabPage4.Location = New System.Drawing.Point(4, 22)
        Me.TabPage4.Name = "TabPage4"
        Me.TabPage4.Size = New System.Drawing.Size(501, 510)
        Me.TabPage4.TabIndex = 3
        Me.TabPage4.Text = "BOM Property Mapping"
        Me.TabPage4.UseVisualStyleBackColor = True
        '
        'btnTab4Save
        '
        Me.btnTab4Save.Location = New System.Drawing.Point(417, 473)
        Me.btnTab4Save.Name = "btnTab4Save"
        Me.btnTab4Save.Size = New System.Drawing.Size(75, 23)
        Me.btnTab4Save.TabIndex = 18
        Me.btnTab4Save.Text = "Save"
        Me.btnTab4Save.UseVisualStyleBackColor = True
        '
        'Label12
        '
        Me.Label12.AutoSize = True
        Me.Label12.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label12.Location = New System.Drawing.Point(7, 485)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(248, 13)
        Me.Label12.TabIndex = 17
        Me.Label12.Text = "* If there is no property available in vault mention '-'."
        '
        'dgvBOMPropMapping
        '
        Me.dgvBOMPropMapping.AllowUserToAddRows = False
        Me.dgvBOMPropMapping.AllowUserToDeleteRows = False
        Me.dgvBOMPropMapping.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells
        Me.dgvBOMPropMapping.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells
        Me.dgvBOMPropMapping.BackgroundColor = System.Drawing.SystemColors.ControlLightLight
        Me.dgvBOMPropMapping.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvBOMPropMapping.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.DataGridViewTextBoxColumn2})
        Me.dgvBOMPropMapping.Location = New System.Drawing.Point(8, 12)
        Me.dgvBOMPropMapping.MultiSelect = False
        Me.dgvBOMPropMapping.Name = "dgvBOMPropMapping"
        Me.dgvBOMPropMapping.RowHeadersVisible = False
        Me.dgvBOMPropMapping.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect
        Me.dgvBOMPropMapping.Size = New System.Drawing.Size(485, 448)
        Me.dgvBOMPropMapping.TabIndex = 16
        '
        'DataGridViewTextBoxColumn2
        '
        Me.DataGridViewTextBoxColumn2.HeaderText = "AX Field"
        Me.DataGridViewTextBoxColumn2.Name = "DataGridViewTextBoxColumn2"
        Me.DataGridViewTextBoxColumn2.ReadOnly = True
        Me.DataGridViewTextBoxColumn2.Width = 71
        '
        'TabPage3
        '
        Me.TabPage3.Controls.Add(Me.btnTab3Save)
        Me.TabPage3.Controls.Add(Me.txtAfterExportBOM)
        Me.TabPage3.Controls.Add(Me.Label11)
        Me.TabPage3.Controls.Add(Me.txtAfterExportItem)
        Me.TabPage3.Controls.Add(Me.Label10)
        Me.TabPage3.Controls.Add(Me.cmbxVaultsforJobs)
        Me.TabPage3.Controls.Add(Me.Label7)
        Me.TabPage3.Controls.Add(Me.Label8)
        Me.TabPage3.Controls.Add(Me.Label9)
        Me.TabPage3.Controls.Add(Me.Label14)
        Me.TabPage3.Controls.Add(Me.Label6)
        Me.TabPage3.Controls.Add(Me.cmbx_lifeCylceDef)
        Me.TabPage3.Controls.Add(Me.lst_FileState)
        Me.TabPage3.Controls.Add(Me.lst_FileTransition)
        Me.TabPage3.Controls.Add(Me.lst_FileJobs)
        Me.TabPage3.Location = New System.Drawing.Point(4, 22)
        Me.TabPage3.Name = "TabPage3"
        Me.TabPage3.Size = New System.Drawing.Size(501, 510)
        Me.TabPage3.TabIndex = 2
        Me.TabPage3.Text = "Export to AX Configurations"
        Me.TabPage3.UseVisualStyleBackColor = True
        '
        'btnTab3Save
        '
        Me.btnTab3Save.Location = New System.Drawing.Point(419, 461)
        Me.btnTab3Save.Name = "btnTab3Save"
        Me.btnTab3Save.Size = New System.Drawing.Size(75, 23)
        Me.btnTab3Save.TabIndex = 33
        Me.btnTab3Save.Text = "Save"
        Me.btnTab3Save.UseVisualStyleBackColor = True
        '
        'txtAfterExportBOM
        '
        Me.txtAfterExportBOM.Location = New System.Drawing.Point(196, 403)
        Me.txtAfterExportBOM.Name = "txtAfterExportBOM"
        Me.txtAfterExportBOM.Size = New System.Drawing.Size(197, 20)
        Me.txtAfterExportBOM.TabIndex = 31
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!)
        Me.Label11.Location = New System.Drawing.Point(26, 433)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(118, 26)
        Me.Label11.TabIndex = 32
        Me.Label11.Text = "Property to update after" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Item Export" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10)
        '
        'txtAfterExportItem
        '
        Me.txtAfterExportItem.Location = New System.Drawing.Point(196, 433)
        Me.txtAfterExportItem.Name = "txtAfterExportItem"
        Me.txtAfterExportItem.Size = New System.Drawing.Size(197, 20)
        Me.txtAfterExportItem.TabIndex = 7
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!)
        Me.Label10.Location = New System.Drawing.Point(26, 397)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(145, 26)
        Me.Label10.TabIndex = 30
        Me.Label10.Text = "Property to update after BOM" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & " Export"
        '
        'cmbxVaultsforJobs
        '
        Me.cmbxVaultsforJobs.FormattingEnabled = True
        Me.cmbxVaultsforJobs.Location = New System.Drawing.Point(196, 17)
        Me.cmbxVaultsforJobs.Name = "cmbxVaultsforJobs"
        Me.cmbxVaultsforJobs.Size = New System.Drawing.Size(197, 21)
        Me.cmbxVaultsforJobs.TabIndex = 29
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!)
        Me.Label7.Location = New System.Drawing.Point(26, 23)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(31, 13)
        Me.Label7.TabIndex = 28
        Me.Label7.Text = "Vault"
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!)
        Me.Label8.Location = New System.Drawing.Point(26, 293)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(83, 13)
        Me.Label8.TabIndex = 27
        Me.Label8.Text = "Configured Jobs"
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!)
        Me.Label9.Location = New System.Drawing.Point(26, 185)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(98, 13)
        Me.Label9.TabIndex = 26
        Me.Label9.Text = "Lifecycle Transition"
        '
        'Label14
        '
        Me.Label14.AutoSize = True
        Me.Label14.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!)
        Me.Label14.Location = New System.Drawing.Point(26, 79)
        Me.Label14.Name = "Label14"
        Me.Label14.Size = New System.Drawing.Size(77, 13)
        Me.Label14.TabIndex = 25
        Me.Label14.Text = "Lifecycle State"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!)
        Me.Label6.Location = New System.Drawing.Point(26, 49)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(115, 13)
        Me.Label6.TabIndex = 24
        Me.Label6.Text = "File Lifecycle Definition"
        '
        'cmbx_lifeCylceDef
        '
        Me.cmbx_lifeCylceDef.FormattingEnabled = True
        Me.cmbx_lifeCylceDef.Location = New System.Drawing.Point(196, 46)
        Me.cmbx_lifeCylceDef.Name = "cmbx_lifeCylceDef"
        Me.cmbx_lifeCylceDef.Size = New System.Drawing.Size(197, 21)
        Me.cmbx_lifeCylceDef.TabIndex = 4
        '
        'lst_FileState
        '
        Me.lst_FileState.FormattingEnabled = True
        Me.lst_FileState.HorizontalScrollbar = True
        Me.lst_FileState.Location = New System.Drawing.Point(196, 79)
        Me.lst_FileState.Name = "lst_FileState"
        Me.lst_FileState.Size = New System.Drawing.Size(197, 95)
        Me.lst_FileState.TabIndex = 5
        '
        'lst_FileTransition
        '
        Me.lst_FileTransition.FormattingEnabled = True
        Me.lst_FileTransition.HorizontalScrollbar = True
        Me.lst_FileTransition.Location = New System.Drawing.Point(196, 185)
        Me.lst_FileTransition.Name = "lst_FileTransition"
        Me.lst_FileTransition.Size = New System.Drawing.Size(197, 95)
        Me.lst_FileTransition.TabIndex = 6
        '
        'lst_FileJobs
        '
        Me.lst_FileJobs.ContextMenuStrip = Me.cmenuItemJobs
        Me.lst_FileJobs.FormattingEnabled = True
        Me.lst_FileJobs.HorizontalScrollbar = True
        Me.lst_FileJobs.Location = New System.Drawing.Point(196, 293)
        Me.lst_FileJobs.Name = "lst_FileJobs"
        Me.lst_FileJobs.Size = New System.Drawing.Size(197, 95)
        Me.lst_FileJobs.TabIndex = 7
        '
        'cmenuItemJobs
        '
        Me.cmenuItemJobs.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.AddItemJob, Me.AddBOMExportJobToolStripMenuItem, Me.DeleteItemJob})
        Me.cmenuItemJobs.Name = "cmenuItemJobs"
        Me.cmenuItemJobs.Size = New System.Drawing.Size(184, 70)
        '
        'AddItemJob
        '
        Me.AddItemJob.Name = "AddItemJob"
        Me.AddItemJob.Size = New System.Drawing.Size(183, 22)
        Me.AddItemJob.Text = "Add Item Export Job"
        '
        'AddBOMExportJobToolStripMenuItem
        '
        Me.AddBOMExportJobToolStripMenuItem.Name = "AddBOMExportJobToolStripMenuItem"
        Me.AddBOMExportJobToolStripMenuItem.Size = New System.Drawing.Size(183, 22)
        Me.AddBOMExportJobToolStripMenuItem.Text = "Add BOM Export Job"
        '
        'DeleteItemJob
        '
        Me.DeleteItemJob.Name = "DeleteItemJob"
        Me.DeleteItemJob.Size = New System.Drawing.Size(183, 22)
        Me.DeleteItemJob.Text = "Delete"
        '
        'TabPage5
        '
        Me.TabPage5.Controls.Add(Me.btnvUserSave)
        Me.TabPage5.Controls.Add(Me.dgvUsers)
        Me.TabPage5.Location = New System.Drawing.Point(4, 22)
        Me.TabPage5.Name = "TabPage5"
        Me.TabPage5.Size = New System.Drawing.Size(501, 510)
        Me.TabPage5.TabIndex = 4
        Me.TabPage5.Text = "User Mapping"
        Me.TabPage5.UseVisualStyleBackColor = True
        '
        'btnvUserSave
        '
        Me.btnvUserSave.Location = New System.Drawing.Point(417, 473)
        Me.btnvUserSave.Name = "btnvUserSave"
        Me.btnvUserSave.Size = New System.Drawing.Size(75, 23)
        Me.btnvUserSave.TabIndex = 19
        Me.btnvUserSave.Text = "Save"
        Me.btnvUserSave.UseVisualStyleBackColor = True
        '
        'dgvUsers
        '
        Me.dgvUsers.AllowUserToAddRows = False
        Me.dgvUsers.AllowUserToDeleteRows = False
        Me.dgvUsers.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells
        Me.dgvUsers.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells
        Me.dgvUsers.BackgroundColor = System.Drawing.SystemColors.ControlLightLight
        Me.dgvUsers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvUsers.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.DataGridViewTextBoxColumn3, Me.dgcEmpCode, Me.ImportItem, Me.ExportItem, Me.ExportBOM})
        Me.dgvUsers.Location = New System.Drawing.Point(8, 10)
        Me.dgvUsers.Name = "dgvUsers"
        Me.dgvUsers.RowHeadersVisible = False
        Me.dgvUsers.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect
        Me.dgvUsers.Size = New System.Drawing.Size(485, 448)
        Me.dgvUsers.TabIndex = 17
        '
        'DataGridViewTextBoxColumn3
        '
        Me.DataGridViewTextBoxColumn3.HeaderText = "Vault Users"
        Me.DataGridViewTextBoxColumn3.Name = "DataGridViewTextBoxColumn3"
        Me.DataGridViewTextBoxColumn3.ReadOnly = True
        Me.DataGridViewTextBoxColumn3.Width = 79
        '
        'dgcEmpCode
        '
        Me.dgcEmpCode.HeaderText = "Personal Number"
        Me.dgcEmpCode.Name = "dgcEmpCode"
        Me.dgcEmpCode.Width = 104
        '
        'ImportItem
        '
        Me.ImportItem.HeaderText = "Import Item from AX"
        Me.ImportItem.Name = "ImportItem"
        Me.ImportItem.Width = 82
        '
        'ExportItem
        '
        Me.ExportItem.HeaderText = "Export Item To AX"
        Me.ExportItem.Name = "ExportItem"
        Me.ExportItem.Width = 77
        '
        'ExportBOM
        '
        Me.ExportBOM.HeaderText = "Export BOM To AX"
        Me.ExportBOM.Name = "ExportBOM"
        Me.ExportBOM.Width = 80
        '
        'TabPage6
        '
        Me.TabPage6.Controls.Add(Me.btnCATSave)
        Me.TabPage6.Controls.Add(Me.GroupBox2)
        Me.TabPage6.Location = New System.Drawing.Point(4, 22)
        Me.TabPage6.Name = "TabPage6"
        Me.TabPage6.Size = New System.Drawing.Size(501, 510)
        Me.TabPage6.TabIndex = 5
        Me.TabPage6.Text = "CAT Field Value Mapping"
        Me.TabPage6.UseVisualStyleBackColor = True
        '
        'btnCATSave
        '
        Me.btnCATSave.Location = New System.Drawing.Point(414, 459)
        Me.btnCATSave.Name = "btnCATSave"
        Me.btnCATSave.Size = New System.Drawing.Size(75, 23)
        Me.btnCATSave.TabIndex = 0
        Me.btnCATSave.Text = "Save"
        Me.btnCATSave.UseVisualStyleBackColor = True
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.btnCATClear)
        Me.GroupBox2.Controls.Add(Me.dgCAT)
        Me.GroupBox2.Controls.Add(Me.txtCATVault)
        Me.GroupBox2.Controls.Add(Me.btnCATDelete)
        Me.GroupBox2.Controls.Add(Me.Label16)
        Me.GroupBox2.Controls.Add(Me.btnCATEdit)
        Me.GroupBox2.Controls.Add(Me.btnCATAdd)
        Me.GroupBox2.Controls.Add(Me.txtCATAX)
        Me.GroupBox2.Controls.Add(Me.Label17)
        Me.GroupBox2.Location = New System.Drawing.Point(8, 14)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(481, 428)
        Me.GroupBox2.TabIndex = 4
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "CAT Field Value Mapping"
        '
        'btnCATClear
        '
        Me.btnCATClear.Location = New System.Drawing.Point(415, 52)
        Me.btnCATClear.Name = "btnCATClear"
        Me.btnCATClear.Size = New System.Drawing.Size(46, 23)
        Me.btnCATClear.TabIndex = 6
        Me.btnCATClear.Text = "Clear"
        Me.btnCATClear.UseVisualStyleBackColor = True
        '
        'dgCAT
        '
        Me.dgCAT.AllowUserToAddRows = False
        Me.dgCAT.AllowUserToDeleteRows = False
        Me.dgCAT.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.dgCAT.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells
        Me.dgCAT.BackgroundColor = System.Drawing.SystemColors.ControlLightLight
        Me.dgCAT.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgCAT.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.dcCATAx, Me.dcCATVault})
        Me.dgCAT.Location = New System.Drawing.Point(15, 86)
        Me.dgCAT.MultiSelect = False
        Me.dgCAT.Name = "dgCAT"
        Me.dgCAT.ReadOnly = True
        Me.dgCAT.RowHeadersVisible = False
        Me.dgCAT.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.dgCAT.Size = New System.Drawing.Size(445, 320)
        Me.dgCAT.TabIndex = 7
        '
        'dcCATAx
        '
        Me.dcCATAx.HeaderText = "AX CAT Value"
        Me.dcCATAx.Name = "dcCATAx"
        Me.dcCATAx.ReadOnly = True
        '
        'dcCATVault
        '
        Me.dcCATVault.HeaderText = "Vault CAT Value"
        Me.dcCATVault.Name = "dcCATVault"
        Me.dcCATVault.ReadOnly = True
        '
        'txtCATVault
        '
        Me.txtCATVault.Location = New System.Drawing.Point(81, 55)
        Me.txtCATVault.Name = "txtCATVault"
        Me.txtCATVault.Size = New System.Drawing.Size(152, 20)
        Me.txtCATVault.TabIndex = 2
        '
        'btnCATDelete
        '
        Me.btnCATDelete.Enabled = False
        Me.btnCATDelete.Location = New System.Drawing.Point(358, 53)
        Me.btnCATDelete.Name = "btnCATDelete"
        Me.btnCATDelete.Size = New System.Drawing.Size(46, 23)
        Me.btnCATDelete.TabIndex = 5
        Me.btnCATDelete.Text = "Delete"
        Me.btnCATDelete.UseVisualStyleBackColor = True
        '
        'Label16
        '
        Me.Label16.AutoSize = True
        Me.Label16.Location = New System.Drawing.Point(11, 58)
        Me.Label16.Name = "Label16"
        Me.Label16.Size = New System.Drawing.Size(61, 13)
        Me.Label16.TabIndex = 10
        Me.Label16.Text = "Vault Value"
        '
        'btnCATEdit
        '
        Me.btnCATEdit.Enabled = False
        Me.btnCATEdit.Location = New System.Drawing.Point(302, 53)
        Me.btnCATEdit.Name = "btnCATEdit"
        Me.btnCATEdit.Size = New System.Drawing.Size(46, 23)
        Me.btnCATEdit.TabIndex = 4
        Me.btnCATEdit.Text = "Edit"
        Me.btnCATEdit.UseVisualStyleBackColor = True
        '
        'btnCATAdd
        '
        Me.btnCATAdd.Location = New System.Drawing.Point(246, 53)
        Me.btnCATAdd.Name = "btnCATAdd"
        Me.btnCATAdd.Size = New System.Drawing.Size(46, 23)
        Me.btnCATAdd.TabIndex = 3
        Me.btnCATAdd.Text = "Add"
        Me.btnCATAdd.UseVisualStyleBackColor = True
        '
        'txtCATAX
        '
        Me.txtCATAX.Location = New System.Drawing.Point(82, 29)
        Me.txtCATAX.Name = "txtCATAX"
        Me.txtCATAX.Size = New System.Drawing.Size(152, 20)
        Me.txtCATAX.TabIndex = 1
        '
        'Label17
        '
        Me.Label17.AutoSize = True
        Me.Label17.Location = New System.Drawing.Point(12, 30)
        Me.Label17.Name = "Label17"
        Me.Label17.Size = New System.Drawing.Size(51, 13)
        Me.Label17.TabIndex = 8
        Me.Label17.Text = "AX Value"
        '
        'Settings
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.SystemColors.ControlLightLight
        Me.ClientSize = New System.Drawing.Size(509, 536)
        Me.Controls.Add(Me.TabControl1)
        Me.MaximizeBox = False
        Me.Name = "Settings"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Favelle - Vault2AX Settings"
        Me.TabControl1.ResumeLayout(False)
        Me.TabPage1.ResumeLayout(False)
        Me.TabPage1.PerformLayout()
        Me.GroupBox5.ResumeLayout(False)
        Me.GroupBox5.PerformLayout()
        CType(Me.dgvVaultOrg, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.TabPage2.ResumeLayout(False)
        Me.TabPage2.PerformLayout()
        CType(Me.dgvPropMapping, System.ComponentModel.ISupportInitialize).EndInit()
        Me.TabPage4.ResumeLayout(False)
        Me.TabPage4.PerformLayout()
        CType(Me.dgvBOMPropMapping, System.ComponentModel.ISupportInitialize).EndInit()
        Me.TabPage3.ResumeLayout(False)
        Me.TabPage3.PerformLayout()
        Me.cmenuItemJobs.ResumeLayout(False)
        Me.TabPage5.ResumeLayout(False)
        CType(Me.dgvUsers, System.ComponentModel.ISupportInitialize).EndInit()
        Me.TabPage6.ResumeLayout(False)
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        CType(Me.dgCAT, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents TabControl1 As System.Windows.Forms.TabControl
    Friend WithEvents TabPage1 As System.Windows.Forms.TabPage
    Friend WithEvents TabPage2 As System.Windows.Forms.TabPage
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents txtVServer As System.Windows.Forms.TextBox
    Friend WithEvents txtVPwd As System.Windows.Forms.TextBox
    Friend WithEvents txtVUsername As System.Windows.Forms.TextBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents GroupBox5 As System.Windows.Forms.GroupBox
    Friend WithEvents dgvVaultOrg As System.Windows.Forms.DataGridView
    Friend WithEvents dcVault As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents dcOrganisation As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents txtTOrg As System.Windows.Forms.TextBox
    Friend WithEvents btnVODelete As System.Windows.Forms.Button
    Friend WithEvents Label18 As System.Windows.Forms.Label
    Friend WithEvents btnVOEdit As System.Windows.Forms.Button
    Friend WithEvents btnVOAdd As System.Windows.Forms.Button
    Friend WithEvents txtTVault As System.Windows.Forms.TextBox
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents txtAxService As System.Windows.Forms.TextBox
    Friend WithEvents Label19 As System.Windows.Forms.Label
    Friend WithEvents dgvPropMapping As System.Windows.Forms.DataGridView
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents btnTab1Save As System.Windows.Forms.Button
    Friend WithEvents btnTab2Save As System.Windows.Forms.Button
    Friend WithEvents TabPage3 As System.Windows.Forms.TabPage
    Friend WithEvents cmbxVaultsforJobs As System.Windows.Forms.ComboBox
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents Label14 As System.Windows.Forms.Label
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents cmbx_lifeCylceDef As System.Windows.Forms.ComboBox
    Friend WithEvents lst_FileState As System.Windows.Forms.ListBox
    Friend WithEvents lst_FileTransition As System.Windows.Forms.ListBox
    Friend WithEvents lst_FileJobs As System.Windows.Forms.ListBox
    Friend WithEvents btnTab3Save As System.Windows.Forms.Button
    Friend WithEvents txtAfterExportBOM As System.Windows.Forms.TextBox
    Friend WithEvents Label11 As System.Windows.Forms.Label
    Friend WithEvents txtAfterExportItem As System.Windows.Forms.TextBox
    Friend WithEvents Label10 As System.Windows.Forms.Label
    Friend WithEvents btnClear As System.Windows.Forms.Button
    Friend WithEvents DataGridViewTextBoxColumn1 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents ToolTip1 As System.Windows.Forms.ToolTip
    Friend WithEvents cmenuItemJobs As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents AddItemJob As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents AddBOMExportJobToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents DeleteItemJob As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents TabPage4 As System.Windows.Forms.TabPage
    Friend WithEvents btnTab4Save As System.Windows.Forms.Button
    Friend WithEvents Label12 As System.Windows.Forms.Label
    Friend WithEvents dgvBOMPropMapping As System.Windows.Forms.DataGridView
    Friend WithEvents DataGridViewTextBoxColumn2 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents TabPage5 As System.Windows.Forms.TabPage
    Friend WithEvents btnvUserSave As System.Windows.Forms.Button
    Friend WithEvents dgvUsers As System.Windows.Forms.DataGridView
    Friend WithEvents Label13 As System.Windows.Forms.Label
    Friend WithEvents txtDuration As System.Windows.Forms.TextBox
    Friend WithEvents chkAutoImport As System.Windows.Forms.CheckBox
    Friend WithEvents txtBOMService As System.Windows.Forms.TextBox
    Friend WithEvents Label15 As System.Windows.Forms.Label
    Friend WithEvents DataGridViewTextBoxColumn3 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents dgcEmpCode As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents ImportItem As System.Windows.Forms.DataGridViewCheckBoxColumn
    Friend WithEvents ExportItem As System.Windows.Forms.DataGridViewCheckBoxColumn
    Friend WithEvents ExportBOM As System.Windows.Forms.DataGridViewCheckBoxColumn
    Friend WithEvents TabPage6 As System.Windows.Forms.TabPage
    Friend WithEvents btnCATSave As System.Windows.Forms.Button
    Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
    Friend WithEvents btnCATClear As System.Windows.Forms.Button
    Friend WithEvents dgCAT As System.Windows.Forms.DataGridView
    Friend WithEvents txtCATVault As System.Windows.Forms.TextBox
    Friend WithEvents btnCATDelete As System.Windows.Forms.Button
    Friend WithEvents Label16 As System.Windows.Forms.Label
    Friend WithEvents btnCATEdit As System.Windows.Forms.Button
    Friend WithEvents btnCATAdd As System.Windows.Forms.Button
    Friend WithEvents txtCATAX As System.Windows.Forms.TextBox
    Friend WithEvents Label17 As System.Windows.Forms.Label
    Friend WithEvents dcCATAx As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents dcCATVault As System.Windows.Forms.DataGridViewTextBoxColumn
End Class
