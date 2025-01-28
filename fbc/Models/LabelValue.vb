Imports System.IO

Namespace FlowByte
    Public Class LabelValue
        Inherits FlowByte.Value

        'Label name
        Private ReadOnly LabelName As String
        Private ReadOnly Location As Location

        'Constructor
        Public Sub New(Location As Location, LabelName As String)
            MyBase.New(FlowByte.ArgumentType.ARG_NULL)
            Me.Location = Location
            Me.LabelName = LabelName
        End Sub

        'Resolve
        Public Overrides Sub ResolveLabels(Fn As [Function])

            'If label doesn't exist
            If Not Fn.Labels.ContainsKey(LabelName) Then
                Throw New SyntaxError(Location, "Label not found: " & LabelName)
            End If

            'Change by value of the offset
            Type = ArgumentType.ARG_INT
            Value = Fn.Labels(LabelName)

        End Sub

    End Class

End Namespace