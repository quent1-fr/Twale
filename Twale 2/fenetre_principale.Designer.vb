<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class fenetre_principale
    Inherits System.Windows.Forms.Form

    'Form remplace la méthode Dispose pour nettoyer la liste des composants.
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

    'Requise par le Concepteur Windows Form
    Private components As System.ComponentModel.IContainer

    'REMARQUE : la procédure suivante est requise par le Concepteur Windows Form
    'Elle peut être modifiée à l'aide du Concepteur Windows Form.  
    'Ne la modifiez pas à l'aide de l'éditeur de code.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(fenetre_principale))
        Me.TabControl1 = New System.Windows.Forms.TabControl()
        Me.TabTchat = New System.Windows.Forms.TabPage()
        Me.Split_tchat = New System.Windows.Forms.SplitContainer()
        Me.split_contenu_textbox = New System.Windows.Forms.SplitContainer()
        Me.ContenuTchat = New System.Windows.Forms.RichTextBox()
        Me.Split_textbox_bouton = New System.Windows.Forms.SplitContainer()
        Me.MessageAEnvoyer = New System.Windows.Forms.TextBox()
        Me.BoutonEnvoyer = New System.Windows.Forms.Button()
        Me.ListeConnectes = New System.Windows.Forms.ListBox()
        Me.TabApropos = New System.Windows.Forms.TabPage()
        Me.Split_a_propos = New System.Windows.Forms.SplitContainer()
        Me.Apropos = New System.Windows.Forms.Label()
        Me.IconeNotification = New System.Windows.Forms.NotifyIcon(Me.components)
        Me.MenuIconeNotif = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.TchatToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
        Me.FermerToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.TabControl1.SuspendLayout()
        Me.TabTchat.SuspendLayout()
        CType(Me.Split_tchat, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Split_tchat.Panel1.SuspendLayout()
        Me.Split_tchat.Panel2.SuspendLayout()
        Me.Split_tchat.SuspendLayout()
        CType(Me.split_contenu_textbox, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.split_contenu_textbox.Panel1.SuspendLayout()
        Me.split_contenu_textbox.Panel2.SuspendLayout()
        Me.split_contenu_textbox.SuspendLayout()
        CType(Me.Split_textbox_bouton, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Split_textbox_bouton.Panel1.SuspendLayout()
        Me.Split_textbox_bouton.Panel2.SuspendLayout()
        Me.Split_textbox_bouton.SuspendLayout()
        Me.TabApropos.SuspendLayout()
        CType(Me.Split_a_propos, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Split_a_propos.Panel2.SuspendLayout()
        Me.Split_a_propos.SuspendLayout()
        Me.MenuIconeNotif.SuspendLayout()
        Me.SuspendLayout()
        '
        'TabControl1
        '
        Me.TabControl1.Controls.Add(Me.TabTchat)
        Me.TabControl1.Controls.Add(Me.TabApropos)
        Me.TabControl1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TabControl1.Location = New System.Drawing.Point(0, 0)
        Me.TabControl1.Name = "TabControl1"
        Me.TabControl1.SelectedIndex = 0
        Me.TabControl1.Size = New System.Drawing.Size(882, 553)
        Me.TabControl1.TabIndex = 0
        '
        'TabTchat
        '
        Me.TabTchat.Controls.Add(Me.Split_tchat)
        Me.TabTchat.Location = New System.Drawing.Point(4, 25)
        Me.TabTchat.Margin = New System.Windows.Forms.Padding(3, 3, 3, 50)
        Me.TabTchat.Name = "TabTchat"
        Me.TabTchat.Padding = New System.Windows.Forms.Padding(3)
        Me.TabTchat.Size = New System.Drawing.Size(874, 524)
        Me.TabTchat.TabIndex = 0
        Me.TabTchat.Text = "T'chat"
        Me.TabTchat.UseVisualStyleBackColor = True
        '
        'Split_tchat
        '
        Me.Split_tchat.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Split_tchat.FixedPanel = System.Windows.Forms.FixedPanel.Panel2
        Me.Split_tchat.Location = New System.Drawing.Point(3, 3)
        Me.Split_tchat.Name = "Split_tchat"
        '
        'Split_tchat.Panel1
        '
        Me.Split_tchat.Panel1.Controls.Add(Me.split_contenu_textbox)
        Me.Split_tchat.Panel1MinSize = 250
        '
        'Split_tchat.Panel2
        '
        Me.Split_tchat.Panel2.Controls.Add(Me.ListeConnectes)
        Me.Split_tchat.Size = New System.Drawing.Size(868, 518)
        Me.Split_tchat.SplitterDistance = 638
        Me.Split_tchat.TabIndex = 0
        '
        'split_contenu_textbox
        '
        Me.split_contenu_textbox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.split_contenu_textbox.FixedPanel = System.Windows.Forms.FixedPanel.Panel2
        Me.split_contenu_textbox.Location = New System.Drawing.Point(0, 0)
        Me.split_contenu_textbox.Name = "split_contenu_textbox"
        Me.split_contenu_textbox.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'split_contenu_textbox.Panel1
        '
        Me.split_contenu_textbox.Panel1.Controls.Add(Me.ContenuTchat)
        '
        'split_contenu_textbox.Panel2
        '
        Me.split_contenu_textbox.Panel2.Controls.Add(Me.Split_textbox_bouton)
        Me.split_contenu_textbox.Panel2MinSize = 80
        Me.split_contenu_textbox.Size = New System.Drawing.Size(638, 518)
        Me.split_contenu_textbox.SplitterDistance = 432
        Me.split_contenu_textbox.TabIndex = 1
        '
        'ContenuTchat
        '
        Me.ContenuTchat.BackColor = System.Drawing.Color.White
        Me.ContenuTchat.Cursor = System.Windows.Forms.Cursors.Arrow
        Me.ContenuTchat.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ContenuTchat.Font = New System.Drawing.Font("Segoe UI Emoji", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ContenuTchat.ForeColor = System.Drawing.Color.Green
        Me.ContenuTchat.Location = New System.Drawing.Point(0, 0)
        Me.ContenuTchat.Name = "ContenuTchat"
        Me.ContenuTchat.ReadOnly = True
        Me.ContenuTchat.Size = New System.Drawing.Size(638, 432)
        Me.ContenuTchat.TabIndex = 0
        Me.ContenuTchat.Text = ""
        '
        'Split_textbox_bouton
        '
        Me.Split_textbox_bouton.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Split_textbox_bouton.FixedPanel = System.Windows.Forms.FixedPanel.Panel2
        Me.Split_textbox_bouton.Location = New System.Drawing.Point(0, 0)
        Me.Split_textbox_bouton.Name = "Split_textbox_bouton"
        '
        'Split_textbox_bouton.Panel1
        '
        Me.Split_textbox_bouton.Panel1.Controls.Add(Me.MessageAEnvoyer)
        '
        'Split_textbox_bouton.Panel2
        '
        Me.Split_textbox_bouton.Panel2.Controls.Add(Me.BoutonEnvoyer)
        Me.Split_textbox_bouton.Panel2MinSize = 100
        Me.Split_textbox_bouton.Size = New System.Drawing.Size(638, 82)
        Me.Split_textbox_bouton.SplitterDistance = 520
        Me.Split_textbox_bouton.TabIndex = 0
        '
        'MessageAEnvoyer
        '
        Me.MessageAEnvoyer.Dock = System.Windows.Forms.DockStyle.Fill
        Me.MessageAEnvoyer.Location = New System.Drawing.Point(0, 0)
        Me.MessageAEnvoyer.MaxLength = 500
        Me.MessageAEnvoyer.Multiline = True
        Me.MessageAEnvoyer.Name = "MessageAEnvoyer"
        Me.MessageAEnvoyer.Size = New System.Drawing.Size(520, 82)
        Me.MessageAEnvoyer.TabIndex = 0
        '
        'BoutonEnvoyer
        '
        Me.BoutonEnvoyer.Cursor = System.Windows.Forms.Cursors.Hand
        Me.BoutonEnvoyer.Dock = System.Windows.Forms.DockStyle.Fill
        Me.BoutonEnvoyer.Location = New System.Drawing.Point(0, 0)
        Me.BoutonEnvoyer.Name = "BoutonEnvoyer"
        Me.BoutonEnvoyer.Size = New System.Drawing.Size(114, 82)
        Me.BoutonEnvoyer.TabIndex = 0
        Me.BoutonEnvoyer.Text = "Envoyer"
        Me.BoutonEnvoyer.UseVisualStyleBackColor = True
        '
        'ListeConnectes
        '
        Me.ListeConnectes.BackColor = System.Drawing.SystemColors.Control
        Me.ListeConnectes.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ListeConnectes.FormattingEnabled = True
        Me.ListeConnectes.ItemHeight = 16
        Me.ListeConnectes.Location = New System.Drawing.Point(0, 0)
        Me.ListeConnectes.Name = "ListeConnectes"
        Me.ListeConnectes.Size = New System.Drawing.Size(226, 518)
        Me.ListeConnectes.Sorted = True
        Me.ListeConnectes.TabIndex = 0
        '
        'TabApropos
        '
        Me.TabApropos.Controls.Add(Me.Split_a_propos)
        Me.TabApropos.Location = New System.Drawing.Point(4, 25)
        Me.TabApropos.Name = "TabApropos"
        Me.TabApropos.Padding = New System.Windows.Forms.Padding(3)
        Me.TabApropos.Size = New System.Drawing.Size(874, 524)
        Me.TabApropos.TabIndex = 2
        Me.TabApropos.Text = "À propos"
        Me.TabApropos.UseVisualStyleBackColor = True
        '
        'Split_a_propos
        '
        Me.Split_a_propos.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Split_a_propos.FixedPanel = System.Windows.Forms.FixedPanel.Panel1
        Me.Split_a_propos.IsSplitterFixed = True
        Me.Split_a_propos.Location = New System.Drawing.Point(3, 3)
        Me.Split_a_propos.Name = "Split_a_propos"
        '
        'Split_a_propos.Panel1
        '
        Me.Split_a_propos.Panel1.BackgroundImage = Global.Twale_2.My.Resources.Resources.osi
        Me.Split_a_propos.Panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
        '
        'Split_a_propos.Panel2
        '
        Me.Split_a_propos.Panel2.Controls.Add(Me.Apropos)
        Me.Split_a_propos.Size = New System.Drawing.Size(868, 518)
        Me.Split_a_propos.SplitterDistance = 213
        Me.Split_a_propos.TabIndex = 0
        '
        'Apropos
        '
        Me.Apropos.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Apropos.Font = New System.Drawing.Font("Calibri", 10.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Apropos.Location = New System.Drawing.Point(0, 0)
        Me.Apropos.Margin = New System.Windows.Forms.Padding(0)
        Me.Apropos.Name = "Apropos"
        Me.Apropos.Padding = New System.Windows.Forms.Padding(5, 10, 5, 10)
        Me.Apropos.Size = New System.Drawing.Size(651, 518)
        Me.Apropos.TabIndex = 0
        Me.Apropos.Text = resources.GetString("Apropos.Text")
        '
        'IconeNotification
        '
        Me.IconeNotification.ContextMenuStrip = Me.MenuIconeNotif
        Me.IconeNotification.Icon = CType(resources.GetObject("IconeNotification.Icon"), System.Drawing.Icon)
        Me.IconeNotification.Text = "Twale 2"
        Me.IconeNotification.Visible = True
        '
        'MenuIconeNotif
        '
        Me.MenuIconeNotif.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.TchatToolStripMenuItem, Me.ToolStripSeparator1, Me.FermerToolStripMenuItem})
        Me.MenuIconeNotif.Name = "MenuIconeNotif"
        Me.MenuIconeNotif.Size = New System.Drawing.Size(125, 58)
        '
        'TchatToolStripMenuItem
        '
        Me.TchatToolStripMenuItem.Name = "TchatToolStripMenuItem"
        Me.TchatToolStripMenuItem.Size = New System.Drawing.Size(124, 24)
        Me.TchatToolStripMenuItem.Text = "T'chat"
        '
        'ToolStripSeparator1
        '
        Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
        Me.ToolStripSeparator1.Size = New System.Drawing.Size(121, 6)
        '
        'FermerToolStripMenuItem
        '
        Me.FermerToolStripMenuItem.Name = "FermerToolStripMenuItem"
        Me.FermerToolStripMenuItem.Size = New System.Drawing.Size(124, 24)
        Me.FermerToolStripMenuItem.Text = "Fermer"
        '
        'fenetre_principale
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(882, 553)
        Me.Controls.Add(Me.TabControl1)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MinimumSize = New System.Drawing.Size(900, 600)
        Me.Name = "fenetre_principale"
        Me.Text = "Twale 2"
        Me.TabControl1.ResumeLayout(False)
        Me.TabTchat.ResumeLayout(False)
        Me.Split_tchat.Panel1.ResumeLayout(False)
        Me.Split_tchat.Panel2.ResumeLayout(False)
        CType(Me.Split_tchat, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Split_tchat.ResumeLayout(False)
        Me.split_contenu_textbox.Panel1.ResumeLayout(False)
        Me.split_contenu_textbox.Panel2.ResumeLayout(False)
        CType(Me.split_contenu_textbox, System.ComponentModel.ISupportInitialize).EndInit()
        Me.split_contenu_textbox.ResumeLayout(False)
        Me.Split_textbox_bouton.Panel1.ResumeLayout(False)
        Me.Split_textbox_bouton.Panel1.PerformLayout()
        Me.Split_textbox_bouton.Panel2.ResumeLayout(False)
        CType(Me.Split_textbox_bouton, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Split_textbox_bouton.ResumeLayout(False)
        Me.TabApropos.ResumeLayout(False)
        Me.Split_a_propos.Panel2.ResumeLayout(False)
        CType(Me.Split_a_propos, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Split_a_propos.ResumeLayout(False)
        Me.MenuIconeNotif.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents TabControl1 As System.Windows.Forms.TabControl
    Friend WithEvents TabTchat As System.Windows.Forms.TabPage
    Friend WithEvents TabApropos As System.Windows.Forms.TabPage
    Friend WithEvents Split_tchat As System.Windows.Forms.SplitContainer
    Friend WithEvents split_contenu_textbox As System.Windows.Forms.SplitContainer
    Friend WithEvents ListeConnectes As System.Windows.Forms.ListBox
    Friend WithEvents Split_textbox_bouton As System.Windows.Forms.SplitContainer
    Friend WithEvents ContenuTchat As System.Windows.Forms.RichTextBox
    Friend WithEvents MessageAEnvoyer As System.Windows.Forms.TextBox
    Friend WithEvents BoutonEnvoyer As System.Windows.Forms.Button
    Friend WithEvents IconeNotification As System.Windows.Forms.NotifyIcon
    Friend WithEvents Split_a_propos As System.Windows.Forms.SplitContainer
    Friend WithEvents Apropos As System.Windows.Forms.Label
    Friend WithEvents MenuIconeNotif As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents TchatToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator1 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents FermerToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem

End Class
