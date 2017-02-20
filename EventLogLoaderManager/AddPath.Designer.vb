<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class AddPath
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
        Me.IBName = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Description = New System.Windows.Forms.TextBox()
        Me.IBGUID = New System.Windows.Forms.TextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Path = New System.Windows.Forms.TextBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.Button2 = New System.Windows.Forms.Button()
        Me.FolderBrowserDialog = New System.Windows.Forms.FolderBrowserDialog()
        Me.ButtonChoosePath = New System.Windows.Forms.Button()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.ESServerNameTextBox = New System.Windows.Forms.TextBox()
        Me.FilterByDateDateTimePicker = New System.Windows.Forms.DateTimePicker()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.UseFilterByDateCheckBox = New System.Windows.Forms.CheckBox()
        Me.SuspendLayout()
        '
        'IBName
        '
        Me.IBName.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.IBName.Location = New System.Drawing.Point(145, 12)
        Me.IBName.Name = "IBName"
        Me.IBName.Size = New System.Drawing.Size(268, 20)
        Me.IBName.TabIndex = 0
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(12, 15)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(126, 13)
        Me.Label1.TabIndex = 1
        Me.Label1.Text = "Краткое наименование"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(12, 67)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(57, 13)
        Me.Label2.TabIndex = 1
        Me.Label2.Text = "Описание"
        '
        'Description
        '
        Me.Description.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Description.Location = New System.Drawing.Point(145, 64)
        Me.Description.Name = "Description"
        Me.Description.Size = New System.Drawing.Size(268, 20)
        Me.Description.TabIndex = 0
        '
        'IBGUID
        '
        Me.IBGUID.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.IBGUID.Location = New System.Drawing.Point(145, 89)
        Me.IBGUID.Name = "IBGUID"
        Me.IBGUID.ReadOnly = True
        Me.IBGUID.Size = New System.Drawing.Size(268, 20)
        Me.IBGUID.TabIndex = 0
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(12, 92)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(34, 13)
        Me.Label3.TabIndex = 1
        Me.Label3.Text = "GUID"
        '
        'Path
        '
        Me.Path.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Path.Location = New System.Drawing.Point(145, 115)
        Me.Path.Name = "Path"
        Me.Path.Size = New System.Drawing.Size(244, 20)
        Me.Path.TabIndex = 0
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(12, 118)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(119, 13)
        Me.Label4.TabIndex = 1
        Me.Label4.Text = "Каталог хранения ЖР"
        '
        'Button1
        '
        Me.Button1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(204, Byte))
        Me.Button1.Location = New System.Drawing.Point(9, 195)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(200, 43)
        Me.Button1.TabIndex = 2
        Me.Button1.Text = "ОК"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'Button2
        '
        Me.Button2.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(204, Byte))
        Me.Button2.Location = New System.Drawing.Point(215, 195)
        Me.Button2.Name = "Button2"
        Me.Button2.Size = New System.Drawing.Size(200, 43)
        Me.Button2.TabIndex = 2
        Me.Button2.Text = "Отмена"
        Me.Button2.UseVisualStyleBackColor = True
        '
        'FolderBrowserDialog
        '
        Me.FolderBrowserDialog.ShowNewFolderButton = False
        '
        'ButtonChoosePath
        '
        Me.ButtonChoosePath.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ButtonChoosePath.Location = New System.Drawing.Point(391, 115)
        Me.ButtonChoosePath.Name = "ButtonChoosePath"
        Me.ButtonChoosePath.Size = New System.Drawing.Size(24, 20)
        Me.ButtonChoosePath.TabIndex = 3
        Me.ButtonChoosePath.Text = "..."
        Me.ButtonChoosePath.UseVisualStyleBackColor = True
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(12, 41)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(112, 13)
        Me.Label5.TabIndex = 5
        Me.Label5.Text = "Имя сервера для ES"
        '
        'ESServerNameTextBox
        '
        Me.ESServerNameTextBox.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ESServerNameTextBox.Location = New System.Drawing.Point(145, 38)
        Me.ESServerNameTextBox.Name = "ESServerNameTextBox"
        Me.ESServerNameTextBox.Size = New System.Drawing.Size(268, 20)
        Me.ESServerNameTextBox.TabIndex = 4
        '
        'FilterByDateDateTimePicker
        '
        Me.FilterByDateDateTimePicker.Location = New System.Drawing.Point(213, 166)
        Me.FilterByDateDateTimePicker.Name = "FilterByDateDateTimePicker"
        Me.FilterByDateDateTimePicker.Size = New System.Drawing.Size(200, 20)
        Me.FilterByDateDateTimePicker.TabIndex = 6
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(12, 144)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(88, 13)
        Me.Label6.TabIndex = 7
        Me.Label6.Text = "Начальная дата"
        '
        'UseFilterByDateCheckBox
        '
        Me.UseFilterByDateCheckBox.AutoSize = True
        Me.UseFilterByDateCheckBox.Location = New System.Drawing.Point(145, 144)
        Me.UseFilterByDateCheckBox.Name = "UseFilterByDateCheckBox"
        Me.UseFilterByDateCheckBox.Size = New System.Drawing.Size(263, 17)
        Me.UseFilterByDateCheckBox.TabIndex = 8
        Me.UseFilterByDateCheckBox.Text = "Загружать события начиная с указанной даты"
        Me.UseFilterByDateCheckBox.UseVisualStyleBackColor = True
        '
        'AddPath
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(425, 244)
        Me.Controls.Add(Me.UseFilterByDateCheckBox)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.FilterByDateDateTimePicker)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.ESServerNameTextBox)
        Me.Controls.Add(Me.ButtonChoosePath)
        Me.Controls.Add(Me.Button2)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Path)
        Me.Controls.Add(Me.IBGUID)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.Description)
        Me.Controls.Add(Me.IBName)
        Me.Name = "AddPath"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Добавление\изменение каталога ЖР вручную"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents IBName As System.Windows.Forms.TextBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Description As System.Windows.Forms.TextBox
    Friend WithEvents IBGUID As System.Windows.Forms.TextBox
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Path As System.Windows.Forms.TextBox
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents Button1 As System.Windows.Forms.Button
    Friend WithEvents Button2 As System.Windows.Forms.Button
    Friend WithEvents FolderBrowserDialog As System.Windows.Forms.FolderBrowserDialog
    Friend WithEvents ButtonChoosePath As System.Windows.Forms.Button
    Friend WithEvents Label5 As Label
    Friend WithEvents ESServerNameTextBox As TextBox
    Friend WithEvents FilterByDateDateTimePicker As DateTimePicker
    Friend WithEvents Label6 As Label
    Friend WithEvents UseFilterByDateCheckBox As CheckBox
End Class
