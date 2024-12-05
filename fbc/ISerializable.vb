Imports System.IO

Namespace FlowByte
    Public Interface ISerializable
        Sub Serialize(Writer As BinaryWriter)
    End Interface

End Namespace