Imports System.IO

Namespace FlowByte

    '
    ' Represent a value that has a word that must be changed to a value at compile time, like a label or a function name
    '
    Public Class NamedValue
        Inherits FlowByte.Value

        'Label name
        Private ReadOnly LabelName As String
        Private ReadOnly Location As Location
        Private Resolved As Boolean

        'Constructor
        Public Sub New(Location As Location, LabelName As String)
            MyBase.New(FlowByte.ArgumentType.ARG_NULL)
            Me.Location = Location
            Me.LabelName = LabelName
            Me.Resolved = False
        End Sub

        'Resolve label name
        Public Shared Function GenerateResolveLabelFunction(Fn As FlowByte.Function) As Action(Of FlowByte.NamedValue)
            Return Sub(Value As FlowByte.NamedValue)

                       'If label doesn't exist
                       If Not Fn.Labels.ContainsKey(Value.LabelName) Then
                           Exit Sub
                       End If

                       'Change value to label index
                       Value.Type = ArgumentType.ARG_INT
                       Value.Value = Fn.Labels(Value.LabelName)

                       'Set resolved
                       Value.Resolved = True

                   End Sub
        End Function

        'Resolve function name
        Public Shared Function GenerateResolveFunctionName(File As FlowByte.File) As Action(Of FlowByte.NamedValue)
            Return Sub(Value As FlowByte.NamedValue)

                       'Search the function
                       Dim Fn As FlowByte.Function = File.SearchFunction(Value.LabelName)
                       If Fn Is Nothing Then
                           Exit Sub
                       End If

                       'Change value to function index
                       Value.Type = ArgumentType.ARG_FUN
                       Value.Value = File.GetFunctionIndex(Fn)

                       'Set resolved
                       Value.Resolved = True

                   End Sub
        End Function

        'Can resolve
        Public Shared Function ShouldResolve(Value As FlowByte.Value) As Boolean
            Return (Value IsNot Nothing) AndAlso (TypeOf Value Is FlowByte.NamedValue) AndAlso (Not DirectCast(Value, FlowByte.NamedValue).Resolved)
        End Function

    End Class

End Namespace