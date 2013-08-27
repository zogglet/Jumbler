Imports Microsoft.VisualBasic

Public Class PuzzlePieceButton
    Inherits Button

    Private _order As Integer
    Private _selected As Boolean
    Private _correct As Boolean

    Public Sub New()
        Me.BackColor = Drawing.ColorTranslator.FromHtml("#5a5a5a")
        Me.BorderColor = Drawing.ColorTranslator.FromHtml("#8a8a8a")
        Me.BorderWidth = 2
        Me.BorderStyle = BorderStyle.Solid
        Me.ForeColor = Drawing.ColorTranslator.FromHtml("#a8e03d")
        Me.Font.Bold = True
        Me.Font.Size = 25

    End Sub

    Public Property ImageUrl() As String
        Get
            Return Me.Style("background-image")
        End Get
        Set(ByVal value As String)
            Me.Style("background-image") = value
        End Set
    End Property

    Public Property Selected() As Boolean
        Get
            Return _selected
        End Get
        Set(ByVal value As Boolean)
            If value = True Then
                Me.BackColor = Drawing.ColorTranslator.FromHtml("#7065b8")
                Me.BorderColor = Drawing.ColorTranslator.FromHtml("#8a79f6")
                Me.BorderWidth = 2
                Me.BorderStyle = BorderStyle.Solid
                Me.ForeColor = Drawing.ColorTranslator.FromHtml("#8a79f6")
                Me.Font.Size = 10
                Me.Font.Bold = True
                Me.Text = "[SELECTED]"
            Else
                Me.BackColor = Drawing.ColorTranslator.FromHtml("#5a5a5a")
                Me.BorderColor = Drawing.ColorTranslator.FromHtml("#8a8a8a")
                Me.BorderWidth = 2
                Me.BorderStyle = BorderStyle.Solid
                Me.Text = ""
            End If
            _selected = value
        End Set
    End Property

    Public Property Order() As Integer
        Get
            Return _order
        End Get
        Set(ByVal value As Integer)
            _order = value
        End Set
    End Property

    Public Property Correct() As Boolean
        Get
            Return _correct
        End Get
        Set(ByVal value As Boolean)
            If value = True Then
                Me.BackColor = Drawing.ColorTranslator.FromHtml("#8ab042")
                Me.BorderColor = Drawing.ColorTranslator.FromHtml("#93bd43")
                Me.BorderWidth = 2
                Me.BorderStyle = BorderStyle.Solid
            End If
            _correct = value
        End Set
    End Property

End Class
