Imports System.IO

Namespace FlowByte
    Public Class Value
        Implements FlowByte.ISerializable

        'Values
        Private Type As FlowByte.ArgumentType
        Private Value As UInt32

        'Constructor
        Public Sub New(Type As FlowByte.ArgumentType, Optional Value As UInt32 = 0)
            Me.Type = Type
            Me.Value = Value
        End Sub

        'Serialize

        Public Sub Serialize(Writer As BinaryWriter) Implements ISerializable.Serialize

            'Convert and writer type
            Dim TypeInteger As UInt32 = Type
            Writer.Write(TypeInteger)

            'Write value
            Writer.Write(Value)

        End Sub

        'Deserialize
        Friend Shared Function Deserialize(reader As BinaryReader) As FlowByte.Value

            'Read type
            Dim Type As FlowByte.ArgumentType = reader.ReadUInt32()

            'Read value
            Dim Value As UInt32 = reader.ReadUInt32()

            'Return value
            Return New FlowByte.Value(Type, Value)

        End Function

        'To string
        Public Overrides Function ToString() As String

            Select Case Type

                Case ArgumentType.ARG_INT
                    Return Value.ToString()

                Case ArgumentType.ARG_REG
                    Return "$" & Value.ToString()

                Case ArgumentType.ARG_REG
                    Return "?ret"

                Case Else
                    Throw New NotImplementedException()

            End Select

        End Function

    End Class

    Public Enum ArgumentType
        ARG_INT ' 42
        ARG_REG ' $42
        ARG_RET ' ?ret
    End Enum

End Namespace